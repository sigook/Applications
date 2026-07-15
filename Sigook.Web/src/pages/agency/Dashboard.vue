<template>
  <div class="sales-dashboard">
    <b-loading v-model="isLoading"></b-loading>

    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Sales Dashboard
        <span v-if="data" class="fw-light fz-1">· {{ data.agent.fullName }}</span>
      </h2>
      <span v-if="data" class="sd-period">{{ data.period.label }} · {{ shortDate(data.period.asOf) }}</span>
    </div>

    <template v-if="data">
      <div class="sd-grid-top">
        <sales-card
          title="Log Interactions"
          subtitle="Recent activity"
          icon="message-text-outline"
          tone="primary"
          action-icon="plus"
          action-label="Log interaction"
          @action="openDrawer('interaction')"
        >
          <sales-interaction-list :items="data.interactions.items" :as-of="data.period.asOf" />
        </sales-card>

        <sales-card
          title="Clients"
          :subtitle="clientsSubtitle"
          icon="domain"
          tone="green"
          action-icon="plus"
          action-label="Create client"
          @action="openDrawer('client')"
        >
          <sales-client-list :items="data.clients.items" />
        </sales-card>

        <sales-card
          title="Deals"
          :subtitle="dealsSubtitle"
          icon="handshake-outline"
          tone="accent"
          action-icon="plus"
          action-label="Create deal"
          @action="openDrawer('deal')"
        >
          <sales-deal-list :items="data.deals.items" />
        </sales-card>
      </div>

      <div class="sd-grid-bottom">
        <sales-card title="Deals closed">
          <template #subtitle>
            Total <span class="sd-total">{{ closedTotal }}</span>
          </template>
          <template #actions>
            <sales-range-tabs v-model="range" />
          </template>
          <sales-bar-chart :points="closedPoints" />
        </sales-card>

        <sales-card title="This quarter">
          <div class="sd-quarter">
            <sales-goal-donut :goal="data.goal" />
            <sales-meter-list title="Pipeline by stage" :items="pipelineMeters" />
            <sales-meter-list title="Activity this week" :items="activityMeters" />
          </div>
        </sales-card>
      </div>

      <sales-create-drawer v-model="isDrawerOpen" :kind="drawerKind" :clients="data.clients.items" />
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import SalesCard from '@/components/sales_dashboard/SalesCard.vue';
import SalesInteractionList from '@/components/sales_dashboard/SalesInteractionList.vue';
import SalesClientList from '@/components/sales_dashboard/SalesClientList.vue';
import SalesDealList from '@/components/sales_dashboard/SalesDealList.vue';
import SalesRangeTabs from '@/components/sales_dashboard/SalesRangeTabs.vue';
import SalesBarChart from '@/components/sales_dashboard/SalesBarChart.vue';
import SalesGoalDonut from '@/components/sales_dashboard/SalesGoalDonut.vue';
import SalesMeterList from '@/components/sales_dashboard/SalesMeterList.vue';
import SalesCreateDrawer from '@/components/sales_dashboard/SalesCreateDrawer.vue';
import { getSalesDashboard } from '@/api/salesDashboardApi';
import { compactMoney, shortDate } from '@/utils/salesDashboardFormat';
import {
  SALES_ACTIVITY_COLORS,
  SALES_ACTIVITY_LABELS,
  SALES_STAGE_COLORS,
} from '@/types/salesDashboard';
import type {
  SalesCreateKind,
  SalesDashboardModel,
  SalesMeter,
  SalesRangeKey,
} from '@/types/salesDashboard';

const isLoading = ref(false);
const data = ref<SalesDashboardModel | null>(null);
const range = ref<SalesRangeKey>('week');
const isDrawerOpen = ref(false);
const drawerKind = ref<SalesCreateKind | null>(null);

const clientsSubtitle = computed(() => {
  if (!data.value) {
    return '';
  }
  const { activeCount, newThisMonth } = data.value.clients;
  return `${activeCount} active · ${newThisMonth} new this month`;
});

const dealsSubtitle = computed(() =>
  data.value ? `${compactMoney(data.value.deals.pipelineValue)} in pipeline` : ''
);

const closedPoints = computed(() => (data.value ? data.value.dealsClosed[range.value] : []));

const closedTotal = computed(() =>
  compactMoney(closedPoints.value.reduce((sum, point) => sum + point.value, 0))
);

const pipelineMeters = computed<SalesMeter[]>(() =>
  (data.value?.pipeline ?? []).map((entry) => ({
    label: entry.stage,
    count: entry.count,
    color: SALES_STAGE_COLORS[entry.stage],
  }))
);

const activityMeters = computed<SalesMeter[]>(() =>
  (data.value?.activity ?? []).map((entry) => ({
    label: SALES_ACTIVITY_LABELS[entry.type],
    count: entry.count,
    color: SALES_ACTIVITY_COLORS[entry.type],
  }))
);

function openDrawer(kind: SalesCreateKind): void {
  drawerKind.value = kind;
  isDrawerOpen.value = true;
}

onMounted(() => {
  isLoading.value = true;
  getSalesDashboard()
    .then((result) => {
      data.value = result;
    })
    .finally(() => {
      isLoading.value = false;
    });
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
    color: $green-vivid;
    font-weight: 600;
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
