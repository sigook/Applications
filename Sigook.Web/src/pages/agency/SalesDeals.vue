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
        <b-table-column field="company" label="Company" sortable v-slot="props">
          {{ props.row.companyName }}
        </b-table-column>
        <b-table-column field="value" label="Value" sortable v-slot="props">
          {{ currency(props.row.value) }}
        </b-table-column>
        <b-table-column field="type" label="Type" v-slot="props">
          {{ DEAL_TYPE_LABELS[props.row.type] }}
        </b-table-column>
        <b-table-column field="status" label="Status" sortable v-slot="props">
          {{ DEAL_STATUS_LABELS[props.row.status] }}
        </b-table-column>
        <b-table-column field="owner" label="Owner" v-slot="props">
          {{ props.row.owner }}
        </b-table-column>
        <b-table-column field="date" label="Date" sortable v-slot="props">
          {{ date(props.row.date) }}
        </b-table-column>
      </b-table>
    </div>

    <sales-create-modal v-model="isModalOpen" kind="deal" :deal="editing" @saved="load" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { getDeals } from '@/api/companyApi';
import { DealSortBy, DEAL_TYPE_LABELS, DEAL_STATUS_LABELS } from '@/types/company';
import type { Deal } from '@/types/company';
import { currency, date } from '@/utils/filters';
import { showAlertError } from '@/utils/toast';
import SalesCreateModal from '@/components/sales_dashboard/SalesCreateModal.vue';

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<Deal[]>([]);
const isModalOpen = ref(false);
const editing = ref<Deal | null>(null);
const serverParams = ref({
  sortBy: DealSortBy.Date,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
});

load();

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

function onPageChange(page: number): void {
  serverParams.value.pageIndex = page;
  load();
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
