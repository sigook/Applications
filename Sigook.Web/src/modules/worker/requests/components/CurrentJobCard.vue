<template>
  <div class="current-job" @click="router.push(`/worker-requests/applied/${assignment.id}`)">
    <div class="current-job__head">
      <div class="current-job__info">
        <p class="current-job__title">{{ assignment.jobTitle }}</p>
        <p class="current-job__sub">{{ subtitle }}</p>
        <p class="current-job__sub">Since {{ dateMonth(assignment.startWorking ?? assignment.startAt) }} · #{{ assignment.numberId }}</p>
      </div>
      <b-tag :class="statusTagType(assignment.status)">{{ assignment.status }}</b-tag>
    </div>
    <OpenPunchCardButton :request-id="assignment.id" expanded />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { dateMonth } from '@/shared/format';
import { statusTagType } from '@/modules/worker/activity/activityFormat';
import type { WorkerAssignmentListItem } from '@/modules/worker/activity/types';
import OpenPunchCardButton from '@/modules/worker/requests/components/OpenPunchCardButton.vue';

const props = defineProps<{ assignment: WorkerAssignmentListItem }>();
const router = useRouter();

const subtitle = computed(() => [props.assignment.clientName, props.assignment.location].filter(Boolean).join(' · '));
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.current-job {
  display: flex;
  flex-direction: column;
  gap: 16px;
  height: 100%;
  padding: 16px;
  background: $white;
  border: 2px solid $green;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  cursor: pointer;
}

.current-job__head {
  display: flex;
  align-items: flex-start;
  flex: 1;
  justify-content: space-between;
  gap: 12px;
}

.current-job__info {
  min-width: 0;
}

.current-job__title {
  font-size: 1.1rem;
  font-weight: 700;
  color: $navy;
  overflow-wrap: anywhere;
}

.current-job__sub {
  font-size: 0.9rem;
  color: $grey-font;
  overflow-wrap: anywhere;
}
</style>
