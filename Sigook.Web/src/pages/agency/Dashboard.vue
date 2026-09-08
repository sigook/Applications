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
      <sales-card
        title="Log Interactions"
        title-to="/sales/interactions"
        subtitle="Recent activity"
        icon="message-text-outline"
        tone="primary"
        action-icon="plus"
        action-label="Log interaction"
        @action="startCreateInteraction"
      >
        <sales-interaction-list :items="interactions" :as-of="nowIso" @edit="startEditInteraction" />
      </sales-card>

      <sales-card
        title="Clients"
        title-to="/sales/companies"
        :subtitle="clientsSubtitle"
        icon="domain"
        tone="primary"
        action-icon="plus"
        action-label="Create client"
        @action="openDrawer('client')"
      >
        <sales-client-list :items="clients" @select="startInteractionForClient" />
      </sales-card>

      <sales-card
        title="Deals"
        title-to="/sales/deals"
        subtitle=""
        icon="handshake-outline"
        tone="primary"
        action-icon="plus"
        action-label="Create deal"
        @action="startCreateDeal"
      >
        <sales-deal-list :items="deals" @edit="startEditDeal" />
      </sales-card>
    </div>

    <div class="sd-grid-bottom">
      <sales-card title="Deals by status">
        <template #subtitle>
          Total <span class="sd-total">{{ dealsByStatus?.totalCount ?? 0 }}</span>
          <span v-if="dealsByStatus"> · {{ compactMoney(dealsByStatus.totalValue) }} · {{ dealsByStatus.period.label }}</span>
        </template>
        <template #actions>
          <sales-range-tabs v-model="period" />
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
          <sales-bar-chart v-else title="Deals by status" :points="dealPoints" />
        </div>
      </sales-card>

      <sales-card title="This quarter">
        <template #subtitle>
          <span v-if="summary">{{ summary.quarter.label }} · {{ shortDate(summary.quarter.from) }} – {{ shortDate(summary.quarter.to) }}</span>
        </template>
        <div class="sd-quarter">
          <sales-meter-list title="Pipeline by status" :items="pipelineMeters" />
          <sales-meter-list :title="activityTitle" :items="activityMeters" />
        </div>
      </sales-card>
    </div>

    <sales-create-modal
      v-model="isModalOpen"
      :kind="modalKind"
      :interaction="editingInteraction"
      :deal="editingDeal"
      :interaction-client="interactionClient"
      @saved="onSaved"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import SalesCard from '@/components/sales_dashboard/SalesCard.vue';
import SalesInteractionList from '@/components/sales_dashboard/SalesInteractionList.vue';
import SalesClientList from '@/components/sales_dashboard/SalesClientList.vue';
import SalesDealList from '@/components/sales_dashboard/SalesDealList.vue';
import SalesRangeTabs from '@/components/sales_dashboard/SalesRangeTabs.vue';
import SalesBarChart from '@/components/sales_dashboard/SalesBarChart.vue';
import SalesMeterList from '@/components/sales_dashboard/SalesMeterList.vue';
import SalesCreateModal from '@/components/sales_dashboard/SalesCreateModal.vue';
import { getDealsByStatus, getSalesDashboardSummary } from '@/api/salesDashboardApi';
import { getCompanyInteractions, getDeals } from '@/api/companyApi';
import { getSalesCompanies } from '@/api/salesApi';
import { useCurrentAgent } from '@/composables/useCurrentAgent';
import { compactMoney, shortDate } from '@/utils/salesDashboardFormat';
import { showAlertError } from '@/utils/toast';
import {
  DealSortBy,
  DEAL_STATUSES,
  DEAL_STATUS_COLORS,
  DEAL_STATUS_LABELS,
  CompanyInteractionSortBy,
  INTERACTION_TYPES,
  INTERACTION_TYPE_COLORS,
  INTERACTION_TYPE_LABELS,
} from '@/types/company';
import type { Deal, CompanyInteraction, DealStatus } from '@/types/company';
import type { AgencyCompanyListItem } from '@/types/agency';
import type { CatalogItem } from '@/types/common';
import {
  SalesPeriod,
  type DealsByStatusModel,
  type SalesBarPoint,
  type SalesCreateKind,
  type SalesDashboardSummary,
  type SalesMeter,
} from '@/types/sales';

const { agentName, loadAgentName } = useCurrentAgent();

const isLoading = ref(false);
const summary = ref<SalesDashboardSummary | null>(null);
const dealsByStatus = ref<DealsByStatusModel | null>(null);
const interactions = ref<CompanyInteraction[]>([]);
const editingInteraction = ref<CompanyInteraction | null>(null);
const clients = ref<AgencyCompanyListItem[]>([]);
const clientsTotal = ref<number | null>(null);
const interactionClient = ref<AgencyCompanyListItem | null>(null);
const deals = ref<Deal[]>([]);
const editingDeal = ref<Deal | null>(null);
const nowIso = new Date().toISOString();
const period = ref<SalesPeriod>(SalesPeriod.Week);
const statusesSelected = ref<CatalogItem<DealStatus>[]>([]);
const isModalOpen = ref(false);
const modalKind = ref<SalesCreateKind | null>(null);

// Drops out-of-order responses when the period or the status filter changes fast.
let dealsByStatusRequest = 0;

const statusOptions: CatalogItem<DealStatus>[] = DEAL_STATUSES.map((status) => ({
  id: status,
  value: DEAL_STATUS_LABELS[status],
}));

const clientsSubtitle = computed(() =>
  clientsTotal.value === null ? '' : `${clientsTotal.value} in your book`
);

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

function openDrawer(kind: SalesCreateKind): void {
  modalKind.value = kind;
  isModalOpen.value = true;
}

function startCreateInteraction(): void {
  editingInteraction.value = null;
  interactionClient.value = null;
  modalKind.value = 'interaction';
  isModalOpen.value = true;
}

function startEditInteraction(interaction: CompanyInteraction): void {
  editingInteraction.value = interaction;
  interactionClient.value = null;
  modalKind.value = 'interaction';
  isModalOpen.value = true;
}

function startInteractionForClient(client: AgencyCompanyListItem): void {
  editingInteraction.value = null;
  interactionClient.value = client;
  modalKind.value = 'interaction';
  isModalOpen.value = true;
}

function startCreateDeal(): void {
  editingDeal.value = null;
  modalKind.value = 'deal';
  isModalOpen.value = true;
}

function startEditDeal(deal: Deal): void {
  editingDeal.value = deal;
  modalKind.value = 'deal';
  isModalOpen.value = true;
}

function loadDealsByStatus(): void {
  const request = ++dealsByStatusRequest;
  getDealsByStatus({
    period: period.value,
    statuses: statusesSelected.value.length ? statusesSelected.value.map((status) => status.id) : undefined,
  })
    .then((result) => {
      if (request === dealsByStatusRequest) dealsByStatus.value = result;
    })
    .catch((error) => showAlertError(error));
}

function loadSummary(): void {
  getSalesDashboardSummary()
    .then((result) => {
      summary.value = result;
    })
    .catch((error) => showAlertError(error))
    .finally(() => {
      isLoading.value = false;
    });
}

function loadInteractions(): void {
  getCompanyInteractions({ pageSize: 6, isDescending: true, sortBy: CompanyInteractionSortBy.CreatedAt })
    .then((result) => {
      interactions.value = result.items;
    })
    .catch((error) => showAlertError(error));
}

function loadClients(): void {
  getSalesCompanies({ pageIndex: 1, pageSize: 6, sortBy: 3, isDescending: true })
    .then((result) => {
      clients.value = result.items;
      clientsTotal.value = result.totalItems;
    })
    .catch((error) => showAlertError(error));
}

function loadDeals(): void {
  getDeals({ pageSize: 6, isDescending: true, sortBy: DealSortBy.Date })
    .then((result) => {
      deals.value = result.items;
    })
    .catch((error) => showAlertError(error));
}

function onSaved(): void {
  loadInteractions();
  loadClients();
  loadDeals();
  loadDealsByStatus();
  loadSummary();
}

watch(period, () => loadDealsByStatus());

onMounted(() => {
  isLoading.value = true;
  loadSummary();
  loadDealsByStatus();
  loadInteractions();
  loadClients();
  loadDeals();
  loadAgentName();
});
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

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
