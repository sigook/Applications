<template>
  <b-modal
    custom-content-class="card"
    :model-value="modelValue"
    width="560px"
    :can-cancel="isFormOpen ? false : ['escape', 'x', 'outside']"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <div v-if="client" class="p-4">
      <b-loading v-model="isLoading"></b-loading>
      <h2 class="has-text-centered fz1 mb-4">{{ client.fullName }}</h2>
      <p class="has-text-centered mb-4">{{ subtitle }}</p>
      <interaction-list :items="interactions" :as-of="asOf" @edit="startEdit" />
      <div class="mt-4">
        <b-button @click="close">Close</b-button>
        <b-button type="is-primary" class="ml-2" icon-left="plus" @click="startCreate">Log interaction</b-button>
      </div>
    </div>
  </b-modal>

  <interaction-modal v-model="isFormOpen" :interaction="editingInteraction" :client="client" @saved="onSaved" />
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import InteractionList from './InteractionList.vue';
import InteractionModal from '@/components/agency_company/InteractionModal.vue';
import { getCompanyInteractions } from '@/api/agencyCompanyApi';
import { CompanyInteractionSortBy } from '@/types/company';
import type { CompanyInteraction } from '@/types/company';
import type { SalesRecentClient } from '@/types/sales';
import { showAlertError } from '@/utils/toast';

const PAGE_SIZE = 50;

const props = defineProps<{
  modelValue: boolean;
  client: SalesRecentClient | null;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'saved'): void;
}>();

const interactions = ref<CompanyInteraction[]>([]);
const totalItems = ref(0);
const isLoading = ref(false);
const asOf = ref(new Date().toISOString());
const isFormOpen = ref(false);
const editingInteraction = ref<CompanyInteraction | null>(null);

const subtitle = computed(() => {
  if (!props.client) return '';
  const count = totalItems.value === 1 ? '1 interaction' : `${totalItems.value} interactions`;
  return props.client.email ? `${props.client.email} · ${count}` : count;
});

const close = (): void => emit('update:modelValue', false);

function load(): void {
  if (!props.client) return;
  isLoading.value = true;
  getCompanyInteractions(props.client.id, {
    pageSize: PAGE_SIZE,
    isDescending: true,
    sortBy: CompanyInteractionSortBy.CreatedAt,
  })
    .then((result) => {
      interactions.value = result.items;
      totalItems.value = result.totalItems;
      asOf.value = new Date().toISOString();
    })
    .catch((error) => showAlertError(error))
    .finally(() => {
      isLoading.value = false;
    });
}

function startCreate(): void {
  editingInteraction.value = null;
  isFormOpen.value = true;
}

function startEdit(interaction: CompanyInteraction): void {
  editingInteraction.value = interaction;
  isFormOpen.value = true;
}

function onSaved(): void {
  emit('saved');
  load();
}

watch(
  () => props.modelValue,
  (open) => {
    if (!open) return;
    interactions.value = [];
    totalItems.value = 0;
    load();
  }
);
</script>
