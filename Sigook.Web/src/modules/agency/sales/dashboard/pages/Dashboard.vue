<template>
  <div class="sales-dashboard">
    <b-loading v-model="isLoading"></b-loading>

    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Sales Dashboard
        <span v-if="agentName" class="fw-light fz-1">· {{ agentName }}</span>
      </h2>
      <span v-if="summary" class="sd-period">{{ summary.quarter.label }} · {{ shortDate(summary.asOf) }}</span>
    </div>

    <div class="sd-grid-top">
      <dashboard-card
        title="Log Interactions"
        subtitle="Recent activity"
        icon="message-text-outline"
        tone="primary"
        action-icon="plus"
        action-label="Log interaction"
        @action="startCreateInteraction"
      >
        <interaction-list :items="interactions" :as-of="nowIso" :loading="isRefreshing" @edit="startEditInteraction" />
      </dashboard-card>

      <dashboard-card
        title="Clients"
        title-to="/sales/companies"
        subtitle="Last 10 contacted"
        icon="domain"
        tone="primary"
        action-icon="plus"
        action-label="Create client"
        @action="isNewClientModalOpen = true"
      >
        <client-list :items="clients" :as-of="nowIso" :loading="isRefreshing" @select="openClientInteractions" />
      </dashboard-card>

      <dashboard-card
        title="Deals"
        subtitle=""
        icon="handshake-outline"
        tone="primary"
        action-icon="plus"
        action-label="Create deal"
        @action="startCreateDeal"
      >
        <deal-list :items="deals" :loading="isRefreshing" @edit="startEditDeal" />
      </dashboard-card>
    </div>

    <div class="sd-grid-bottom">
      <dashboard-card title="Deals by status">
        <template #subtitle>
          Total <span class="sd-total">{{ dealsByStatus?.totalCount ?? 0 }}</span>
          <span v-if="dealsByStatus"> · {{ compactMoney(dealsByStatus.totalValue) }}</span>
        </template>
        <template #actions>
          <b-datepicker
            v-model="dateRange"
            class="sd-range"
            size="is-small"
            range
            :mobile-native="false"
            icon="calendar-today"
            position="is-bottom-left"
            append-to-body
            @update:modelValue="loadDealsByStatus"
          />
        </template>
        <div class="sd-by-status">
          <b-taginput
            size="is-small"
            v-model="statusesSelected"
            autocomplete
            :data="statusOptions"
            open-on-focus
            field="value"
            icon="label"
            placeholder="All statuses"
            append-to-body
            @update:modelValue="loadDealsByStatus"
          />
          <p v-if="dealsByStatus && dealsByStatus.totalCount === 0" class="sd-by-status__empty">
            No deals dated in this period
          </p>
          <bar-chart v-else title="Deals by status" :points="dealPoints" />
        </div>
      </dashboard-card>

      <dashboard-card title="This quarter">
        <template #subtitle>
          <span v-if="summary">{{ summary.quarter.label }} · {{ shortDate(summary.quarter.from) }} – {{ shortDate(summary.quarter.to) }}</span>
        </template>
        <div class="sd-quarter">
          <meter-list title="Pipeline by status" :items="pipelineMeters" />
          <meter-list :title="activityTitle" :items="activityMeters" />
        </div>
      </dashboard-card>
    </div>

    <interaction-modal v-model="isInteractionModalOpen" :interaction="editingInteraction" @saved="onSaved" />
    <deal-modal v-model="isDealModalOpen" :deal="editingDeal" @saved="onSaved" />
    <client-modal v-model="isNewClientModalOpen" @saved="onSaved" />
    <client-interactions-modal v-model="isClientModalOpen" :client="selectedClient" @saved="onSaved" />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import DashboardCard from '@/modules/agency/sales/dashboard/components/DashboardCard.vue';
import InteractionList from '@/modules/agency/sales/dashboard/components/InteractionList.vue';
import ClientList from '@/modules/agency/sales/dashboard/components/ClientList.vue';
import DealList from '@/modules/agency/sales/dashboard/components/DealList.vue';
import BarChart from '@/modules/agency/sales/dashboard/components/BarChart.vue';
import MeterList from '@/modules/agency/sales/dashboard/components/MeterList.vue';
import InteractionModal from '@/modules/agency/sales/clients/components/InteractionModal.vue';
import DealModal from '@/modules/agency/sales/clients/components/DealModal.vue';
import ClientModal from '@/modules/agency/sales/dashboard/components/ClientModal.vue';
import ClientInteractionsModal from '@/modules/agency/sales/dashboard/components/ClientInteractionsModal.vue';
import { getDealsByStatus, getSalesDashboard } from '@/modules/agency/sales/dashboard/api';
import { useCurrentAgent } from '@/modules/agency/sales/dashboard/useCurrentAgent';
import { calendarDate, compactMoney, currentMonthRange, shortDate } from '@/modules/agency/sales/dashboard/format';
import { showAlertError } from '@/shared/utils/toast';
import { DEAL_STATUSES, DEAL_STATUS_COLORS, DEAL_STATUS_LABELS, INTERACTION_TYPES, INTERACTION_TYPE_COLORS, INTERACTION_TYPE_LABELS } from '@/modules/agency/sales/clients/types';
import type { Deal, CompanyInteraction, DealStatus } from '@/modules/agency/sales/clients/types';
import type { CatalogItem } from '@/shared/types/common';
import type {
  DealsByStatusFilter,
  DealsByStatusModel,
  SalesBarPoint,
  SalesDashboardSummary,
  SalesMeter,
  SalesRecentClient,
} from '@/modules/agency/sales/dashboard/types';

const { agentName, loadAgentName } = useCurrentAgent();

const isLoading = ref(false);
const isRefreshing = ref(false);
const summary = ref<SalesDashboardSummary | null>(null);
const dealsByStatus = ref<DealsByStatusModel | null>(null);
const interactions = ref<CompanyInteraction[]>([]);
const editingInteraction = ref<CompanyInteraction | null>(null);
const clients = ref<SalesRecentClient[]>([]);
const selectedClient = ref<SalesRecentClient | null>(null);
const isClientModalOpen = ref(false);
const deals = ref<Deal[]>([]);
const editingDeal = ref<Deal | null>(null);
const nowIso = new Date().toISOString();
const dateRange = ref<Date[]>(currentMonthRange());
const statusesSelected = ref<CatalogItem<DealStatus>[]>([]);
const isInteractionModalOpen = ref(false);
const isDealModalOpen = ref(false);
const isNewClientModalOpen = ref(false);

// Drops out-of-order responses when the date range or the status filter changes fast.
let dealsByStatusRequest = 0;

const statusOptions: CatalogItem<DealStatus>[] = DEAL_STATUSES.map((status) => ({
  id: status,
  value: DEAL_STATUS_LABELS[status],
}));

const activityTitle = computed(() =>
  summary.value ? `Activity this week · ${summary.value.week.label}` : 'Activity this week'
);

const dealPoints = computed<SalesBarPoint[]>(() => {
  const items = dealsByStatus.value?.items ?? [];
  return DEAL_STATUSES.filter((status) => items.some((item) => item.status === status)).map((status) => {
    const item = items.find((entry) => entry.status === status);
    return {
      key: String(status),
      label: DEAL_STATUS_LABELS[status],
      value: item?.count ?? 0,
      color: DEAL_STATUS_COLORS[status],
      caption: compactMoney(item?.totalValue ?? 0),
    };
  });
});

const pipelineMeters = computed<SalesMeter[]>(() => {
  const pipeline = summary.value?.pipeline ?? [];
  return DEAL_STATUSES.filter((status) => pipeline.some((entry) => entry.status === status)).map((status) => ({
    label: DEAL_STATUS_LABELS[status],
    count: pipeline.find((entry) => entry.status === status)?.count ?? 0,
    color: DEAL_STATUS_COLORS[status],
  }));
});

const activityMeters = computed<SalesMeter[]>(() => {
  const activity = summary.value?.activity ?? [];
  return INTERACTION_TYPES.filter((type) => activity.some((entry) => entry.type === type)).map((type) => ({
    label: INTERACTION_TYPE_LABELS[type],
    count: activity.find((entry) => entry.type === type)?.count ?? 0,
    color: INTERACTION_TYPE_COLORS[type],
  }));
});

function startCreateInteraction(): void {
  editingInteraction.value = null;
  isInteractionModalOpen.value = true;
}

function startEditInteraction(interaction: CompanyInteraction): void {
  editingInteraction.value = interaction;
  isInteractionModalOpen.value = true;
}

function openClientInteractions(client: SalesRecentClient): void {
  selectedClient.value = client;
  isClientModalOpen.value = true;
}

function startCreateDeal(): void {
  editingDeal.value = null;
  isDealModalOpen.value = true;
}

function startEditDeal(deal: Deal): void {
  editingDeal.value = deal;
  isDealModalOpen.value = true;
}

function dealsByStatusFilter(): DealsByStatusFilter {
  return {
    dateFrom: calendarDate(dateRange.value[0]),
    dateTo: calendarDate(dateRange.value[1]),
    statuses: statusesSelected.value.length ? statusesSelected.value.map((status) => status.id) : undefined,
  };
}

function loadDealsByStatus(): void {
  const request = ++dealsByStatusRequest;
  getDealsByStatus(dealsByStatusFilter())
    .then((result) => {
      if (request === dealsByStatusRequest) dealsByStatus.value = result;
    })
    .catch((error) => showAlertError(error));
}

function loadDashboard(): Promise<void> {
  const request = ++dealsByStatusRequest;
  return getSalesDashboard(dealsByStatusFilter())
    .then((result) => {
      summary.value = result.summary;
      interactions.value = [...result.recentInteractions];
      clients.value = [...result.recentClients];
      deals.value = [...result.recentDeals];
      if (request === dealsByStatusRequest) dealsByStatus.value = result.dealsByStatus;
    })
    .catch((error) => showAlertError(error));
}

function onSaved(): void {
  isRefreshing.value = true;
  loadDashboard().finally(() => {
    isRefreshing.value = false;
  });
}

onMounted(() => {
  isLoading.value = true;
  Promise.allSettled([loadDashboard(), loadAgentName()]).finally(() => {
    isLoading.value = false;
  });
});
</script>

<style scoped lang="scss">
@import "@/assets/scss/variables";

.sales-dashboard {
  .section-top-title {
    align-items: baseline;
  }

  .sd-period {
    margin-left: auto;
    font-size: 0.8125rem;
    color: $grey-light;
    white-space: nowrap;
  }

  .sd-grid-top {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1rem;
    margin-bottom: 1rem;

    > * {
      height: 342px;
    }
  }

  .sd-grid-bottom {
    display: grid;
    grid-template-columns: 1.72fr 1fr;
    gap: 1rem;

    > * {
      height: 308px;
    }
  }

  .sd-total {
    color: $green;
    font-weight: 600;
  }

  .sd-range {
    width: 13.5rem;
  }

  .sd-by-status {
    flex: 1;
    min-height: 0;
    display: flex;
    flex-direction: column;
    padding: 0.625rem 0.95rem 0;
  }

  .sd-by-status__empty {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0;
    font-size: 0.75rem;
    color: #9a9a9a;
  }

  .sd-quarter {
    flex: 1;
    min-height: 0;
    overflow-y: auto;
    padding: 0.875rem 1rem;
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  @media screen and (max-width: 1215px) {
    .sd-grid-top {
      grid-template-columns: 1fr 1fr;
    }

    .sd-grid-bottom {
      grid-template-columns: 1fr;

      > * {
        height: 340px;
      }
    }
  }

  @media screen and (max-width: 768px) {
    .sd-grid-top {
      grid-template-columns: 1fr;
    }

    .sd-period {
      display: none;
    }
  }
}
</style>
