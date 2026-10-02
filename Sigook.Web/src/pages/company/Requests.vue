<template>
  <div class="company-requests">
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Staff Requests" :count="totalItems" />
    <div>
      <SigookGrid ref="grid" :fetch="loadRequests" v-model:params="serverParams" :sort-map="sortMap"
        @update:loading="(value) => isLoading = value" @loaded="(total) => totalItems = total" @cellclick="onCellClick">
        <template #actions>
          <template v-if="isTouch">
            <b-input v-model="serverParams.jobTitle" placeholder="Search position..." icon="magnify"
              @keyup.enter="onSearch"></b-input>
            <div class="filter-trigger">
              <b-button icon-left="filter-variant" @click="showFilters = true" />
              <span v-if="activeFilterCount > 0" class="filter-count-badge">{{ activeFilterCount }}</span>
            </div>
          </template>
          <b-button tag="router-link" to="/company-requests/create" icon-left="plus">Create Request</b-button>
        </template>
        <template #mobile-card="{ row }">
          <div class="rcard is-clickable" @click="goToRequest(row)">
            <div class="rcard__head">
              <div>
                <span class="rcard__title">{{ row.numberId }}</span>
                <span v-if="row.isAsap" class="asap ml-2">Asap</span>
                <span v-if="row.isDirectHiring" class="asap ml-2">DH</span>
              </div>
              <div class="rcard-status">
                <div class="status-dot-container">
                  <img v-if="row.requestStatus === RequestStatus.Filled" src="../../assets/images/check_white.png"
                    alt="check" class="request-check" />
                  <div class="dot-status" :class="getStatusClass(row)"></div>
                </div>
                <span>{{ RequestStatusLabels[row.requestStatus] }}</span>
              </div>
            </div>
            <p class="rcard__title">{{ row.jobTitle }}</p>
            <p class="rcard__sub">{{ dateFromNow(row.createdAt) }}</p>
            <div class="rcard__rows">
              <div class="rcard__row">
                <span class="rcard__label">Location</span>
                <span>{{ row.location }}<span v-if="row.entrance"> - {{ row.entrance }}</span></span>
              </div>
              <div class="rcard__row" @click.stop>
                <span class="rcard__label">Shift</span>
                <AgencyShift :requestId="row.id" :displayShift="row.displayShift" :fetchShift="getRequestShift">
                </AgencyShift>
              </div>
              <div class="rcard__row" @click.stop="goToWorkers(row)">
                <span class="rcard__label">Workers</span>
                <span>{{ row.workersQuantityWorking }} / {{ row.workersQuantity }}</span>
              </div>
            </div>
          </div>
        </template>
        <b-table-column field="numberId" label="Request ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <router-link :to="{ path: '/company-requests/' + props.row.id }">
              <p>{{ props.row.numberId }}</p>
            </router-link>
            <p v-if="props.row.isAsap" class="asap">{{ "Asap" }}</p>
            <p v-if="props.row.isDirectHiring" class="asap">DH</p>
          </template>
        </b-table-column>
        <b-table-column field="jobTitle" label="Position" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.jobTitle" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.jobTitle }}
            <i class="fz-2 block">{{ dateFromNow(props.row.createdAt) }}</i>
          </template>
        </b-table-column>
        <b-table-column field="location" label="Location" searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.location" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.location }}
            <span v-if="props.row.entrance"> - {{ props.row.entrance }}</span>
          </template>
        </b-table-column>
        <b-table-column field="displayShift" label="Shift" v-slot="props">
          <AgencyShift class="is-block" :requestId="props.row.id"
            :displayShift="props.row.displayShift" :fetchShift="getRequestShift"></AgencyShift>
        </b-table-column>
        <b-table-column field="workersQuantityWorking" sortable>
          <template v-slot:header>
            <p class="has-text-weight-semibold">Workers</p>
            <p class="has-text-weight-semibold">({{ totalQuantityWorking }} / {{ totalQuantity }})</p>
          </template>
          <template v-slot="props">
            {{ props.row.workersQuantityWorking }} / {{ props.row.workersQuantity }}
          </template>
        </b-table-column>
        <b-table-column field="requestStatus" label="Status" v-slot="props">
          <div class="has-text-centered">
            <b-tooltip :label="RequestStatusLabels[props.row.requestStatus]" type="is-dark" append-to-body>
              <div class="status-dot-container">
                <img v-if="props.row.requestStatus === RequestStatus.Filled"
                  src="../../assets/images/check_white.png" alt="check" class="request-check" />
                <div class="dot-status" :class="getStatusClass(props.row)"></div>
              </div>
            </b-tooltip>
          </div>
        </b-table-column>
      </SigookGrid>
      <MobileFiltersPanel v-if="isTouch" v-model="showFilters" :active-count="activeFilterCount" @apply="onSearch"
        @clear="clearFilters">
        <b-field label="Request ID">
          <b-input v-model="serverParams.numberId"></b-input>
        </b-field>
        <b-field label="Position">
          <b-input v-model="serverParams.jobTitle"></b-input>
        </b-field>
        <b-field label="Location">
          <b-input v-model="serverParams.location"></b-input>
        </b-field>
      </MobileFiltersPanel>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, useTemplateRef } from 'vue';
import { useRouter } from 'vue-router';
import { useCompanyStore } from '@/stores/company';
import { getRequests } from '@/api/companyApi';
import type { CompanyRequestFilter, CompanyRequestListItem } from '@/types/company';
import type { GridHandle, TableColumnRef } from '@/types/common';
import { RequestStatus, RequestStatusLabels } from '@/constants/enums';
import { dateFromNow } from '@/utils/filters';
import AgencyShift from '@/components/agency_request/AgencyShiftDetail.vue';
import { getRequestShift } from '@/api/companyApi';
import PageHeader from '@/components/PageHeader.vue';
import SigookGrid from '@/components/SigookGrid.vue';
import MobileFiltersPanel from '@/components/responsive/MobileFiltersPanel.vue';
import { useBreakpoint } from '@/composables/useBreakpoint';

const router = useRouter();
const companyStore = useCompanyStore();
const { isTouch } = useBreakpoint();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  numberId: 0,
  jobTitle: 2,
  workersQuantityWorking: 6,
};

const showFilters = ref(false);
const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<CompanyRequestListItem[]>([]);
const serverParams = ref<CompanyRequestFilter>(companyStore.companyRequestFilter ?? {
  sortBy: 0,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
});

const totalQuantityWorking = computed(() => {
  if (rows.value.length > 0) {
    return rows.value.map((r) => r.workersQuantityWorking).reduce((a, b) => a + b);
  }
  return 0;
});

const totalQuantity = computed(() => {
  if (rows.value.length > 0) {
    return rows.value.map((r) => r.workersQuantity).reduce((a, b) => a + b);
  }
  return 0;
});

function loadRequests(params: CompanyRequestFilter) {
  companyStore.setCompanyRequestFilter(params);
  return getRequests(params)
    .then((response) => {
      rows.value = response.items;
      return response;
    });
}

function onSearch() {
  grid.value?.search();
}

function onCellClick(row: CompanyRequestListItem, column: TableColumnRef) {
  switch (column.field) {
    case 'displayShift':
      break;
    case 'workersQuantityWorking':
      router.push({
        path: `/company-requests/${row.id}`,
        query: { tab: 'Workers' },
      });
      break;
    default:
      router.push(`/company-requests/${row.id}`);
  }
}

const activeFilterCount = computed(() =>
  [serverParams.value.numberId, serverParams.value.jobTitle, serverParams.value.location].filter((v) => !!v).length,
);

function clearFilters() {
  serverParams.value.numberId = undefined;
  serverParams.value.jobTitle = undefined;
  serverParams.value.location = undefined;
  onSearch();
}

function goToRequest(row: CompanyRequestListItem) {
  router.push(`/company-requests/${row.id}`);
}

function goToWorkers(row: CompanyRequestListItem) {
  router.push({
    path: `/company-requests/${row.id}`,
    query: { tab: 'Workers' },
  });
}

function getStatusClass(row: CompanyRequestListItem) {
  if (row.requestStatus === RequestStatus.Open &&
    row.workersQuantityWorking > 0 &&
    row.workersQuantityWorking < row.workersQuantity) {
    return 'status-inprogress';
  }
  return 'status-' + RequestStatusLabels[row.requestStatus].toLowerCase();
}
</script>

<style lang="scss" scoped>
.rcard-status {
  display: flex;
  align-items: center;
  gap: 6px;
  flex-shrink: 0;
  font-size: 0.85rem;
}
</style>
