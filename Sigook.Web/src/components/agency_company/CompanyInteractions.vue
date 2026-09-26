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
      <b-table-column field="description" label="Description" v-slot="props">
        {{ props.row.description }}
      </b-table-column>
      <b-table-column field="type" label="Type" searchable>
        <template #searchable>
          <b-select v-model="serverParams.interactionType" size="is-small" expanded @update:modelValue="applyFilter">
            <option :value="null">All</option>
            <option v-for="t in INTERACTION_TYPES" :key="t" :value="t">{{ INTERACTION_TYPE_LABELS[t] }}</option>
          </b-select>
        </template>
        <template v-slot="props">
          {{ INTERACTION_TYPE_LABELS[props.row.interactionType] }}
        </template>
      </b-table-column>
      <b-table-column field="purpose" label="Purpose" searchable>
        <template #searchable>
          <b-select v-model="serverParams.interactionPurpose" size="is-small" expanded @update:modelValue="applyFilter">
            <option :value="null">All</option>
            <option v-for="p in INTERACTION_PURPOSES" :key="p" :value="p">{{ INTERACTION_PURPOSE_LABELS[p] }}</option>
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
      <b-table-column field="actions" v-slot="props">
        <b-button type="is-info" outlined rounded icon-right="pencil" class="mr-2" @click="openEdit(props.row)"></b-button>
        <b-button type="is-danger" outlined rounded icon-right="delete" @click="onDelete(props.row)"></b-button>
      </b-table-column>
    </b-table>

    <interaction-modal v-model="isModalOpen" :interaction="editing" :client="company" @saved="load" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { deleteCompanyInteraction, getCompanyInteractions } from '@/api/agencyCompanyApi';
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
import type { SalesClientReference } from '@/types/sales';
import type { CatalogItem } from '@/types/common';
import { useSalesOwners } from '@/composables/useSalesOwners';
import { date } from '@/utils/filters';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/utils/toast';
import InteractionModal from './InteractionModal.vue';

const props = defineProps<{ company: SalesClientReference }>();

const statusOptions: CatalogItem<InteractionStatus>[] = INTERACTION_STATUSES.map((s) => ({
  id: s,
  value: INTERACTION_STATUS_LABELS[s],
}));

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<CompanyInteraction[]>([]);
const isModalOpen = ref(false);
const editing = ref<CompanyInteraction | null>(null);
const statusesSelected = ref<CatalogItem<InteractionStatus>[]>([]);
const createdAtDatesSelected = ref<Date[]>([]);
const { isAdmin, owners, loadOwners } = useSalesOwners();
const serverParams = ref({
  sortBy: CompanyInteractionSortBy.CreatedAt,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
  ownerId: null as string | null,
  interactionType: null as InteractionType | null,
  interactionPurpose: null as InteractionPurpose | null,
  statuses: undefined as InteractionStatus[] | undefined,
  createdAtFrom: null as string | null,
  createdAtTo: null as string | null,
});

load();
loadOwners();

function load(): void {
  isLoading.value = true;
  getCompanyInteractions(props.company.id, serverParams.value)
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
  serverParams.value.sortBy = field === 'status' ? CompanyInteractionSortBy.Status : CompanyInteractionSortBy.CreatedAt;
  serverParams.value.isDescending = order !== 'asc';
  load();
}

function openCreate(): void {
  editing.value = null;
  isModalOpen.value = true;
}

function openEdit(interaction: CompanyInteraction): void {
  editing.value = interaction;
  isModalOpen.value = true;
}

async function onDelete(interaction: CompanyInteraction): Promise<void> {
  const confirmed = await showAlertConfirm('Delete interaction', 'This action cannot be undone.', 'Delete');
  if (!confirmed) return;
  isLoading.value = true;
  try {
    await deleteCompanyInteraction(props.company.id, interaction.id);
    showAlertSuccess('Interaction deleted');
    load();
  } catch (error) {
    isLoading.value = false;
    await showAlertError(error);
  }
}
</script>
