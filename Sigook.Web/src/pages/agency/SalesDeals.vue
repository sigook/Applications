<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Deals
        <span class="fw-light fz-1">({{ totalItems }})</span>
      </h2>
    </div>
    <div>
      <b-field grouped position="is-right">
        <b-button icon-left="plus" @click="openCreate">
          {{ 'Create' }}
        </b-button>
      </b-field>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false"
        paginated pagination-size="is-small" backend-pagination backend-sorting pagination-rounded
        :total="totalItems" :per-page="serverParams.pageSize" :default-sort="['date', 'desc']"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <b-table-column field="title" label="Title" v-slot="props">
          {{ props.row.title }}
        </b-table-column>
        <b-table-column field="company" label="Company" sortable searchable>
          <template #searchable>
            <search-select v-model="serverParams.companyProfileId" :options="clientOptions" :loading="isLoadingClients"
              :min-search-length="MINIMUM_SEARCH_LENGTH" size="is-small" remote clearable placeholder="Client..."
              @search="onClientSearch" @update:modelValue="applyFilter" />
          </template>
          <template v-slot="props">
            {{ props.row.companyName }}
          </template>
        </b-table-column>
        <b-table-column field="value" label="Value" sortable v-slot="props">
          {{ currency(props.row.value) }}
        </b-table-column>
        <b-table-column field="type" label="Type" searchable>
          <template #searchable>
            <b-select v-model="serverParams.type" size="is-small" expanded @update:modelValue="applyFilter">
              <option :value="null">All</option>
              <option v-for="t in dealTypes" :key="t" :value="t">{{ DEAL_TYPE_LABELS[t] }}</option>
            </b-select>
          </template>
          <template v-slot="props">
            {{ DEAL_TYPE_LABELS[props.row.type] }}
          </template>
        </b-table-column>
        <b-table-column field="status" label="Status" sortable searchable>
          <template #searchable>
            <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statusOptions" open-on-focus
              field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusChange"
              append-to-body />
          </template>
          <template v-slot="props">
            {{ DEAL_STATUS_LABELS[props.row.status] }}
          </template>
        </b-table-column>
        <b-table-column field="owner" label="Owner" :searchable="isAdmin">
          <template #searchable>
            <b-select v-model="serverParams.ownerId" size="is-small" expanded @update:modelValue="applyFilter">
              <option :value="null">All owners</option>
              <option v-for="o in owners" :key="o.userId" :value="o.userId">{{ o.name || o.email }}</option>
            </b-select>
          </template>
          <template v-slot="props">
            {{ props.row.owner }}
          </template>
        </b-table-column>
        <b-table-column field="date" label="Date" sortable searchable>
          <template #searchable>
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Date" range v-model="dateSelected"
              :icon-right="dateSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onDateCleared" @update:modelValue="onDateSelected" append-to-body />
          </template>
          <template v-slot="props">
            {{ date(props.row.date) }}
          </template>
        </b-table-column>
      </b-table>
    </div>

    <sales-create-modal v-model="isModalOpen" kind="deal" :deal="editing" @saved="load" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { getDeals } from '@/api/companyApi';
import { getAgencyCompaniesList } from '@/api/agencyCompanyApi';
import {
  DealSortBy,
  DealStatus,
  DealType,
  DEAL_TYPES,
  DEAL_STATUSES,
  DEAL_TYPE_LABELS,
  DEAL_STATUS_LABELS,
} from '@/types/company';
import type { Deal } from '@/types/company';
import type { CatalogItem } from '@/types/common';
import { useSalesOwners } from '@/composables/useSalesOwners';
import { currency, date } from '@/utils/filters';
import { showAlertError } from '@/utils/toast';
import SalesCreateModal from '@/components/sales_dashboard/SalesCreateModal.vue';
import SearchSelect from '@/components/sales_dashboard/SearchSelect.vue';

const MINIMUM_SEARCH_LENGTH = 3;

const dealTypes = DEAL_TYPES;
const statusOptions: CatalogItem<DealStatus>[] = DEAL_STATUSES.map((s) => ({
  id: s,
  value: DEAL_STATUS_LABELS[s],
}));

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<Deal[]>([]);
const isModalOpen = ref(false);
const editing = ref<Deal | null>(null);
const clients = ref<CatalogItem[]>([]);
const isLoadingClients = ref(false);
const statusesSelected = ref<CatalogItem<DealStatus>[]>([]);
const dateSelected = ref<Date[]>([]);
const { isAdmin, owners, loadOwners } = useSalesOwners();
const serverParams = ref({
  sortBy: DealSortBy.Date,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
  ownerId: null as string | null,
  companyProfileId: null as string | null,
  type: null as DealType | null,
  statuses: undefined as DealStatus[] | undefined,
  dateFrom: null as string | null,
  dateTo: null as string | null,
});

const clientOptions = computed(() => clients.value.map((c) => ({ value: c.id, label: c.value })));

load();
loadOwners();

function load(): void {
  isLoading.value = true;
  getDeals(serverParams.value)
    .then((result) => {
      rows.value = result.items;
      totalItems.value = result.totalItems;
    })
    .catch((error) => showAlertError(error))
    .finally(() => {
      isLoading.value = false;
    });
}

function applyFilter(): void {
  serverParams.value.pageIndex = 1;
  load();
}

function onPageChange(page: number): void {
  serverParams.value.pageIndex = page;
  load();
}

function onClientSearch(term: string): void {
  const normalized = term.trim();
  if (normalized.length < MINIMUM_SEARCH_LENGTH) {
    clients.value = [];
    return;
  }
  isLoadingClients.value = true;
  getAgencyCompaniesList(normalized)
    .then((result) => {
      clients.value = result;
    })
    .catch((error) => showAlertError(error))
    .finally(() => {
      isLoadingClients.value = false;
    });
}

function onStatusChange(): void {
  serverParams.value.statuses = statusesSelected.value.length
    ? statusesSelected.value.map((s) => s.id)
    : undefined;
  applyFilter();
}

function onDateSelected(): void {
  serverParams.value.dateFrom = dateSelected.value[0]?.toISOString() ?? null;
  serverParams.value.dateTo = dateSelected.value[1]?.toISOString() ?? null;
  applyFilter();
}

function onDateCleared(): void {
  dateSelected.value = [];
  onDateSelected();
}

function onSortChange(field: string, order: string): void {
  switch (field) {
    case 'company':
      serverParams.value.sortBy = DealSortBy.Company;
      break;
    case 'value':
      serverParams.value.sortBy = DealSortBy.Value;
      break;
    case 'status':
      serverParams.value.sortBy = DealSortBy.Status;
      break;
    default:
      serverParams.value.sortBy = DealSortBy.Date;
      break;
  }
  serverParams.value.isDescending = order !== 'asc';
  load();
}

function onCellClick(row: Deal): void {
  editing.value = row;
  isModalOpen.value = true;
}

function openCreate(): void {
  editing.value = null;
  isModalOpen.value = true;
}
</script>
