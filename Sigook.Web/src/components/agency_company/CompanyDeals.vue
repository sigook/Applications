<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid ref="grid" :fetch="loadDeals" v-model:params="serverParams" :sort-map="sortMap"
      @update:loading="(value) => isLoading = value">
      <template #actions>
        <b-button icon-left="plus" @click="openCreate">Add</b-button>
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
    </SigookGrid>

    <deal-modal v-model="isModalOpen" :deal="editing" :client="company" @saved="reload" />
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { deleteDeal, getDeals } from '@/api/agencyCompanyApi';
import {
  DealSortBy,
  DealStatus,
  DEAL_TYPES,
  DEAL_STATUSES,
  DEAL_TYPE_LABELS,
  DEAL_STATUS_LABELS,
} from '@/types/company';
import type { Deal, DealFilter } from '@/types/company';
import type { SalesClientReference } from '@/types/sales';
import type { CatalogItem, GridHandle, PaginatedList } from '@/types/common';
import { useSalesOwners } from '@/composables/useSalesOwners';
import { currency, date } from '@/utils/filters';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/utils/toast';
import DealModal from './DealModal.vue';
import SigookGrid from '@/components/SigookGrid.vue';

const props = defineProps<{ company: SalesClientReference }>();

const statusOptions: CatalogItem<DealStatus>[] = DEAL_STATUSES.map((s) => ({
  id: s,
  value: DEAL_STATUS_LABELS[s],
}));

const isLoading = ref(true);
const isModalOpen = ref(false);
const editing = ref<Deal | null>(null);
const statusesSelected = ref<CatalogItem<DealStatus>[]>([]);
const dateSelected = ref<Date[]>([]);
const { isAdmin, owners, loadOwners } = useSalesOwners();
const grid = useTemplateRef<GridHandle>('grid');
const sortMap = {
  date: DealSortBy.Date,
  value: DealSortBy.Value,
  status: DealSortBy.Status,
};
const serverParams = ref<DealFilter>({
  sortBy: DealSortBy.Date,
  isDescending: true,
  ownerId: null,
  type: null,
  statuses: undefined,
  dateFrom: null,
  dateTo: null,
});

loadOwners();

function loadDeals(params: DealFilter): Promise<PaginatedList<Deal>> {
  return getDeals(props.company.id, params);
}

function applyFilter(): void {
  grid.value?.search();
}

function reload(): void {
  grid.value?.reload();
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
    reload();
  } catch (error) {
    isLoading.value = false;
    await showAlertError(error);
  }
}
</script>
