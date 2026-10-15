<template>
  <div class="modal-card" style="width: auto">
    <header class="modal-card-head">
      <div>
        <p class="modal-card-title">{{ detail?.name || 'History' }}</p>
        <i v-if="detail?.email" class="fz-2 is-lowercase">{{ detail.email }}</i>
      </div>
    </header>
    <section class="modal-card-body" style="min-width: 720px; position: relative">
      <RunnerDetailPanel :request-id="requestId" :runner-id="runnerId" @updated="emit('updated')"
        @loaded="(value) => detail = value" />
    </section>
    <footer class="modal-card-foot">
      <b-button @click="emit('close')">Close</b-button>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { RunnerDetail } from '@/modules/agency/recruiting/requests/runners/types';
import RunnerDetailPanel from '@/modules/agency/recruiting/runners/components/RunnerDetailPanel.vue';

defineProps<{ requestId: string; runnerId: string }>();
const emit = defineEmits<{ (e: 'updated'): void; (e: 'close'): void }>();

const detail = ref<RunnerDetail | null>(null);
</script>
