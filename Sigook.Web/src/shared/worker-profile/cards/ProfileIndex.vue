<template>
  <nav class="worker-card worker-profile-index" aria-label="Profile sections">
    <span class="worker-card-eyebrow">On this page</span>
    <a v-for="section in props.sections" :key="section.id" :href="`#${workerSectionAnchor(section.id)}`"
      class="worker-profile-index-link" :class="{ 'is-active': section.id === props.activeId }"
      @click.prevent="emit('select', section.id)">
      <span>{{ section.label }}</span>
      <span v-if="section.pendingCount > 0" class="worker-count is-warning">{{ section.pendingCount }}</span>
      <b-icon v-else-if="section.isComplete" icon="check" size="is-small" class="worker-profile-index-check" />
      <b-icon v-else-if="section.isComplete === false" icon="circle-outline" size="is-small" class="worker-profile-index-missing" />
    </a>
    <div class="worker-profile-completeness">
      <span class="worker-profile-completeness-label">Profile completeness</span>
      <progress class="worker-progress" :value="props.completeness" max="100">{{ props.completeness }}%</progress>
      <span class="worker-profile-completeness-value">
        {{ props.completeness }}%<template v-if="props.missingLabels.length"> · missing {{ props.missingLabels.join(', ').toLowerCase() }}</template>
      </span>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { workerSectionAnchor } from '@/shared/worker-profile/useWorkerProfileStatus';
import type { WorkerProfileSection, WorkerProfileSectionId } from '@/shared/worker-profile/types';

const props = defineProps<{
  sections: WorkerProfileSection[];
  completeness: number;
  missingLabels: string[];
  activeId: WorkerProfileSectionId | null;
}>();
const emit = defineEmits<{ (e: 'select', id: WorkerProfileSectionId): void }>();
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.worker-profile-index {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.worker-card-eyebrow {
  display: block;
  padding: 4px 10px 8px;
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  color: $grey-font;
}

.worker-profile-index-link {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  padding: 8px 10px;
  border-radius: 6px;
  color: $navy;
  font-size: 0.9rem;

  &:hover {
    background: $gray-bg;
  }

  &.is-active {
    background: rgba($blue, 0.08);
    color: $blue;
    font-weight: 600;
  }
}

.worker-count {
  min-width: 22px;
  padding: 0 7px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 700;
  text-align: center;
  background: rgba($accent, 0.16);
  color: $accent-text;
}

.worker-profile-index-check {
  color: $green-text;
}

.worker-profile-index-missing {
  color: $grey-light;
}

.worker-profile-completeness {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-top: 10px;
  padding: 12px 10px 4px;
  border-top: 1px solid $gray-border;
  font-size: 0.75rem;
}

.worker-profile-completeness-label {
  color: $grey-font;
}

.worker-profile-completeness-value {
  font-weight: 600;
}

.worker-progress {
  appearance: none;
  width: 100%;
  height: 6px;
  border: 0;
  border-radius: 3px;
  background: $gray-border;
  overflow: hidden;

  &::-webkit-progress-bar {
    background: $gray-border;
  }
  &::-webkit-progress-value {
    background: $primary;
    border-radius: 3px;
  }
  &::-moz-progress-bar {
    background: $primary;
    border-radius: 3px;
  }
}
</style>
