<template>
  <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
    <template #trigger>
      <slot name="trigger">
        <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
      </slot>
    </template>
    <b-dropdown-item v-if="canEdit && status !== RunnerStatus.Hired" aria-role="listitem"
      @click="emit('open', 'status')">
      Change status
    </b-dropdown-item>
    <b-dropdown-item v-if="canEdit && canAddInterview(status)" aria-role="listitem" @click="emit('open', 'interview')">
      Add interview
    </b-dropdown-item>
    <b-dropdown-item aria-role="listitem" @click="emit('open', 'history')">
      View history
    </b-dropdown-item>
    <b-dropdown-item v-if="canEdit" aria-role="listitem" class="has-text-danger" @click="emit('delete')">
      Delete
    </b-dropdown-item>
  </b-dropdown>
</template>

<script setup lang="ts">
import { RunnerStatus, canAddInterview } from '@/types/runner';
import type { RunnerAction } from '@/composables/useRunnerActions';

withDefaults(defineProps<{ status: RunnerStatus; canEdit?: boolean }>(), { canEdit: true });
const emit = defineEmits<{ (e: 'open', action: RunnerAction): void; (e: 'delete'): void }>();
</script>
