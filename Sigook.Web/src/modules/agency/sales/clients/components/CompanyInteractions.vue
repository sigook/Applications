<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid ref="grid" :fetch="loadInteractions" v-model:params="serverParams" :sort-map="sortMap"
      @update:loading="(value) => isLoading = value">
      <template #actions>
        <b-button icon-left="plus" @click="openCreate">Add</b-button>
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
    </SigookGrid>

    <interaction-modal v-model="isModalOpen" :interaction="editing" :client="company" @saved="reload" />
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { deleteCompanyInteraction, getCompanyInteractions } from '@/modules/agency/sales/clients/api';
import { CompanyInteractionSortBy, InteractionStatus, INTERACTION_TYPES, INTERACTION_PURPOSES, INTERACTION_STATUSES, INTERACTION_TYPE_LABELS, INTERACTION_PURPOSE_LABELS, INTERACTION_STATUS_LABELS } from '@/modules/agency/sales/clients/types';
import type { CompanyInteraction, CompanyInteractionFilter } from '@/modules/agency/sales/clients/types';
import type { SalesClientReference } from '@/modules/agency/sales/dashboard/types';
import type { CatalogItem, GridHandle, PaginatedList } from '@/shared/types/common';
import { useSalesOwners } from '@/modules/agency/shared/useSalesOwners';
import { date } from '@/shared/format';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import InteractionModal from '@/modules/agency/sales/clients/components/InteractionModal.vue';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const props = defineProps<{ company: SalesClientReference }>();

const statusOptions: CatalogItem<InteractionStatus>[] = INTERACTION_STATUSES.map((s) => ({
  id: s,
  value: INTERACTION_STATUS_LABELS[s],
}));

const isLoading = ref(true);
const isModalOpen = ref(false);
const editing = ref<CompanyInteraction | null>(null);
const statusesSelected = ref<CatalogItem<InteractionStatus>[]>([]);
const createdAtDatesSelected = ref<Date[]>([]);
const { isAdmin, owners, loadOwners } = useSalesOwners();
const grid = useTemplateRef<GridHandle>('grid');
const sortMap = {
  date: CompanyInteractionSortBy.CreatedAt,
  status: CompanyInteractionSortBy.Status,
};
const serverParams = ref<CompanyInteractionFilter>({
  sortBy: CompanyInteractionSortBy.CreatedAt,
  isDescending: true,
  ownerId: null,
  interactionType: null,
  interactionPurpose: null,
  statuses: undefined,
  createdAtFrom: null,
  createdAtTo: null,
});

loadOwners();

function loadInteractions(params: CompanyInteractionFilter): Promise<PaginatedList<CompanyInteraction>> {
  return getCompanyInteractions(props.company.id, params);
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

function onCreatedAtSelected(): void {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1]?.toISOString() ?? null;
  applyFilter();
}

function onCreatedAtCleared(): void {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
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
    reload();
  } catch (error) {
    isLoading.value = false;
    await showAlertError(error);
  }
}
</script>
