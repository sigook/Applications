<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="openCreate">Add</b-button>
    </b-field>
    <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false"
      paginated pagination-size="is-small" backend-pagination backend-sorting pagination-rounded
      :total="totalItems" :per-page="serverParams.pageSize" :default-sort="['date', 'desc']"
      v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange">
      <template v-slot:empty>
        <p class="container has-text-centered">No records available</p>
      </template>
      <b-table-column field="title" label="Title" v-slot="props">
        {{ props.row.title }}
      </b-table-column>
      <b-table-column field="value" label="Value" sortable v-slot="props">
        {{ currency(props.row.value) }}
      </b-table-column>
      <b-table-column field="type" label="Type" searchable>
        <template #searchable>
          <b-select v-model="serverParams.type" size="is-small" expanded @update:modelValue="applyFilter">
            <option :value="null">All</option>
            <option v-for="t in DEAL_TYPES" :key="t" :value="t">{{ DEAL_TYPE_LABELS[t] }}</option>
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
      <b-table-column field="actions" v-slot="props">
        <b-button type="is-info" outlined rounded icon-right="pencil" class="mr-2" @click="openEdit(props.row)"></b-button>
        <b-button type="is-danger" outlined rounded icon-right="delete" @click="onDelete(props.row)"></b-button>
      </b-table-column>
    </b-table>

    <deal-modal v-model="isModalOpen" :deal="editing" :client="company" @saved="load" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { deleteDeal, getDeals } from '@/api/agencyCompanyApi';
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
import type { SalesClientReference } from '@/types/sales';
import type { CatalogItem } from '@/types/common';
import { useSalesOwners } from '@/composables/useSalesOwners';
import { currency, date } from '@/utils/filters';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/utils/toast';
import DealModal from './DealModal.vue';

const props = defineProps<{ company: SalesClientReference }>();

const statusOptions: CatalogItem<DealStatus>[] = DEAL_STATUSES.map((s) => ({
  id: s,
  value: DEAL_STATUS_LABELS[s],
}));

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<Deal[]>([]);
const isModalOpen = ref(false);
const editing = ref<Deal | null>(null);
const statusesSelected = ref<CatalogItem<DealStatus>[]>([]);
const dateSelected = ref<Date[]>([]);
const { isAdmin, owners, loadOwners } = useSalesOwners();
const serverParams = ref({
  sortBy: DealSortBy.Date,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
  ownerId: null as string | null,
  type: null as DealType | null,
  statuses: undefined as DealStatus[] | undefined,
  dateFrom: null as string | null,
  dateTo: null as string | null,
});

load();
loadOwners();

function load(): void {
  isLoading.value = true;
  getDeals(props.company.id, serverParams.value)
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

function openCreate(): void {
  editing.value = null;
  isModalOpen.value = true;
}

function openEdit(deal: Deal): void {
  editing.value = deal;
  isModalOpen.value = true;
}

async function onDelete(deal: Deal): Promise<void> {
  const confirmed = await showAlertConfirm('Delete deal', 'This action cannot be undone.', 'Delete');
  if (!confirmed) return;
  isLoading.value = true;
  try {
    await deleteDeal(props.company.id, deal.id);
    showAlertSuccess('Deal deleted');
    load();
  } catch (error) {
    isLoading.value = false;
    await showAlertError(error);
  }
}
</script>
