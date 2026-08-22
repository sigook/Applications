<template>
  <div>
    <b-modal has-modal-card v-model="statusOpen" width="520px" :destroy-on-hide="true">
      <runner-status-modal v-if="target" :request-id="target.requestId" :runner-id="target.runnerId"
        :current-status="target.status" @updated="emit('updated')" @close="statusOpen = false" />
    </b-modal>

    <b-modal has-modal-card v-model="interviewOpen" width="540px" :destroy-on-hide="true">
      <runner-interview-modal v-if="target" :request-id="target.requestId" :runner-id="target.runnerId"
        @updated="emit('updated')" @close="interviewOpen = false" />
    </b-modal>

    <b-modal has-modal-card v-model="historyOpen" width="760px" :destroy-on-hide="true">
      <runner-history-modal v-if="target" :request-id="target.requestId" :runner-id="target.runnerId"
        @updated="emit('updated')" @close="historyOpen = false" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import RunnerStatusModal from '@/components/runner/RunnerStatusModal.vue';
import RunnerInterviewModal from '@/components/runner/RunnerInterviewModal.vue';
import RunnerHistoryModal from '@/components/runner/RunnerHistoryModal.vue';
import type { RunnerActionTarget } from '@/composables/useRunnerActions';

defineProps<{ target: RunnerActionTarget | null }>();
const emit = defineEmits<{ (e: 'updated'): void }>();

const statusOpen = defineModel<boolean>('statusOpen', { required: true });
const interviewOpen = defineModel<boolean>('interviewOpen', { required: true });
const historyOpen = defineModel<boolean>('historyOpen', { required: true });
</script>
