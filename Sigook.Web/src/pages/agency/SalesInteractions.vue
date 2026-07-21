<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Interactions
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
        <b-table-column field="company" label="Company" sortable v-slot="props">
          {{ props.row.companyName }}
        </b-table-column>
        <b-table-column field="description" label="Description" v-slot="props">
          {{ props.row.description }}
        </b-table-column>
        <b-table-column field="type" label="Type" v-slot="props">
          {{ INTERACTION_TYPE_LABELS[props.row.interactionType] }}
        </b-table-column>
        <b-table-column field="purpose" label="Purpose" v-slot="props">
          {{ INTERACTION_PURPOSE_LABELS[props.row.interactionPurpose] }}
        </b-table-column>
        <b-table-column field="status" label="Status" sortable v-slot="props">
          {{ INTERACTION_STATUS_LABELS[props.row.interactionStatus] }}
        </b-table-column>
        <b-table-column field="owner" label="Owner" v-slot="props">
          {{ props.row.ownerName }}
        </b-table-column>
        <b-table-column field="date" label="Date" sortable v-slot="props">
          {{ date(props.row.createdAt) }}
        </b-table-column>
      </b-table>
    </div>

    <sales-create-modal v-model="isModalOpen" kind="interaction" :interaction="editing" @saved="load" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { getCompanyInteractions } from '@/api/companyInteractionApi';
import {
  CompanyInteractionSortBy,
  INTERACTION_TYPE_LABELS,
  INTERACTION_PURPOSE_LABELS,
  INTERACTION_STATUS_LABELS,
} from '@/types/companyInteraction';
import type { CompanyInteraction } from '@/types/companyInteraction';
import { date } from '@/utils/filters';
import { showAlertError } from '@/utils/toast';
import SalesCreateModal from '@/components/sales_dashboard/SalesCreateModal.vue';

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<CompanyInteraction[]>([]);
const isModalOpen = ref(false);
const editing = ref<CompanyInteraction | null>(null);
const serverParams = ref({
  sortBy: CompanyInteractionSortBy.CreatedAt,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
});

load();

function load(): void {
  isLoading.value = true;
  getCompanyInteractions(serverParams.value)
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
      serverParams.value.sortBy = CompanyInteractionSortBy.Company;
      break;
    case 'status':
      serverParams.value.sortBy = CompanyInteractionSortBy.Status;
      break;
    default:
      serverParams.value.sortBy = CompanyInteractionSortBy.CreatedAt;
      break;
  }
  serverParams.value.isDescending = order !== 'asc';
  load();
}

function onCellClick(row: CompanyInteraction): void {
  editing.value = row;
  isModalOpen.value = true;
}

function openCreate(): void {
  editing.value = null;
  isModalOpen.value = true;
}
</script>
