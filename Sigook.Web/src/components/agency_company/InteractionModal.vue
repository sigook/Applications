<template>
  <b-modal custom-content-class="card" :model-value="modelValue" width="500px" @update:model-value="emit('update:modelValue', $event)">
    <div class="p-4">
      <h2 class="has-text-centered fz1 mb-4">{{ interaction ? 'Edit interaction' : 'Log interaction' }}</h2>
      <interaction-form ref="formRef" :interaction="interaction" :initial-client="client" />
      <div class="mt-4">
        <b-button @click="close">Cancel</b-button>
        <b-button type="is-primary" class="ml-2" :loading="isSaving" @click="onSave">Save</b-button>
      </div>
    </div>
  </b-modal>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { CompanyInteraction } from '@/types/company';
import type { SalesClientReference } from '@/types/sales';
import InteractionForm from './InteractionForm.vue';

defineProps<{
  modelValue: boolean;
  interaction?: CompanyInteraction | null;
  client?: SalesClientReference | null;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'saved'): void;
}>();

const formRef = ref<InstanceType<typeof InteractionForm> | null>(null);
const isSaving = ref(false);

const close = (): void => emit('update:modelValue', false);

async function onSave(): Promise<void> {
  if (!formRef.value) return;
  isSaving.value = true;
  const saved = await formRef.value.submit();
  isSaving.value = false;
  if (!saved) return;
  emit('saved');
  close();
}
</script>
