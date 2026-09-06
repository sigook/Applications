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
        <b-table-column field="description" label="Description" v-slot="props">
          {{ props.row.description }}
        </b-table-column>
        <b-table-column field="type" label="Type" searchable>
          <template #searchable>
            <b-select v-model="serverParams.interactionType" size="is-small" expanded @update:modelValue="applyFilter">
              <option :value="null">All</option>
              <option v-for="t in interactionTypes" :key="t" :value="t">{{ INTERACTION_TYPE_LABELS[t] }}</option>
            </b-select>
          </template>
          <template v-slot="props">
            {{ INTERACTION_TYPE_LABELS[props.row.interactionType] }}
          </template>
        </b-table-column>
        <b-table-column field="purpose" label="Purpose" searchable>
          <template #searchable>
            <b-select v-model="serverParams.interactionPurpose" size="is-small" expanded
              @update:modelValue="applyFilter">
              <option :value="null">All</option>
              <option v-for="p in interactionPurposes" :key="p" :value="p">{{ INTERACTION_PURPOSE_LABELS[p] }}</option>
            </b-select>
          </template>
          <template v-slot="props">
            {{ INTERACTION_PURPOSE_LABELS[props.row.interactionPurpose] }}
          </template>
        </b-table-column>
        <b-table-column field="status" label="Status" sortable searchable>
          <template #searchable>
            <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statusOptions" open-on-focus
              field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusChange"
              append-to-body />
          </template>
          <template v-slot="props">
            {{ INTERACTION_STATUS_LABELS[props.row.interactionStatus] }}
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
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Created At" range
              v-model="createdAtDatesSelected" :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''"
              icon-right-clickable @icon-right-click="onCreatedAtCleared" @update:modelValue="onCreatedAtSelected"
              append-to-body />
          </template>
          <template v-slot="props">
            {{ date(props.row.createdAt) }}
          </template>
        </b-table-column>
      </b-table>
    </div>

    <sales-create-modal v-model="isModalOpen" kind="interaction" :interaction="editing" @saved="load" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { getCompanyInteractions } from '@/api/companyApi';
import { getAgencyCompaniesList } from '@/api/agencyCompanyApi';
import {
  CompanyInteractionSortBy,
  InteractionPurpose,
  InteractionStatus,
  InteractionType,
  INTERACTION_TYPES,
  INTERACTION_PURPOSES,
  INTERACTION_STATUSES,
  INTERACTION_TYPE_LABELS,
  INTERACTION_PURPOSE_LABELS,
  INTERACTION_STATUS_LABELS,
} from '@/types/company';
import type { CompanyInteraction } from '@/types/company';
import type { CatalogItem } from '@/types/common';
import { useSalesOwners } from '@/composables/useSalesOwners';
import { date } from '@/utils/filters';
import { showAlertError } from '@/utils/toast';
import SalesCreateModal from '@/components/sales_dashboard/SalesCreateModal.vue';
import SearchSelect from '@/components/sales_dashboard/SearchSelect.vue';

const MINIMUM_SEARCH_LENGTH = 3;

const interactionTypes = INTERACTION_TYPES;
const interactionPurposes = INTERACTION_PURPOSES;
const statusOptions: CatalogItem<InteractionStatus>[] = INTERACTION_STATUSES.map((s) => ({
  id: s,
  value: INTERACTION_STATUS_LABELS[s],
}));

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<CompanyInteraction[]>([]);
const isModalOpen = ref(false);
const editing = ref<CompanyInteraction | null>(null);
const clients = ref<CatalogItem[]>([]);
const isLoadingClients = ref(false);
const statusesSelected = ref<CatalogItem<InteractionStatus>[]>([]);
const createdAtDatesSelected = ref<Date[]>([]);
const { isAdmin, owners, loadOwners } = useSalesOwners();
const serverParams = ref({
  sortBy: CompanyInteractionSortBy.CreatedAt,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
  ownerId: null as string | null,
  companyProfileId: null as string | null,
  interactionType: null as InteractionType | null,
  interactionPurpose: null as InteractionPurpose | null,
  statuses: undefined as InteractionStatus[] | undefined,
  createdAtFrom: null as string | null,
  createdAtTo: null as string | null,
});

const clientOptions = computed(() => clients.value.map((c) => ({ value: c.id, label: c.value })));

load();
loadOwners();

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

function onCreatedAtSelected(): void {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1]?.toISOString() ?? null;
  applyFilter();
}

function onCreatedAtCleared(): void {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
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
