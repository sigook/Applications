<template>
  <b-modal custom-content-class="card" :model-value="modelValue" width="500px" @update:model-value="emit('update:modelValue', $event)">
    <div class="p-4">
      <h2 class="has-text-centered fz1 mb-4">New client</h2>
      <client-form ref="formRef" />
      <div class="mt-4">
        <b-button @click="close">Cancel</b-button>
        <b-button type="is-primary" class="ml-2" :loading="isSaving" @click="onSave">Save</b-button>
      </div>
    </div>
  </b-modal>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import ClientForm from './ClientForm.vue';

defineProps<{ modelValue: boolean }>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'saved'): void;
}>();

const formRef = ref<InstanceType<typeof ClientForm> | null>(null);
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
