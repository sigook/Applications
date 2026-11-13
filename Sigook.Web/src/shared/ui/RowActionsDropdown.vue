<template>
  <b-dropdown v-if="isMounted" ref="dropdown" aria-role="list" :position="position" append-to-body>
    <template #trigger>
      <slot name="trigger">
        <b-button icon-right="dots-vertical" size="is-medium" type="is-text" :aria-label="label" />
      </slot>
    </template>
    <slot></slot>
  </b-dropdown>
  <div v-else class="dropdown" @click="mount">
    <div class="dropdown-trigger">
      <slot name="trigger">
        <b-button icon-right="dots-vertical" size="is-medium" type="is-text" :aria-label="label" />
      </slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { nextTick, ref, useTemplateRef } from 'vue';

const { position = 'is-bottom-left', label = 'Row actions' } = defineProps<{
  position?: string;
  label?: string;
}>();

const dropdown = useTemplateRef<{ toggle: () => void }>('dropdown');
const isMounted = ref(false);

async function mount() {
  isMounted.value = true;
  await nextTick();
  dropdown.value?.toggle();
}
</script>
