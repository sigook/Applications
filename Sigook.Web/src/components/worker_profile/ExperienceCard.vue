<template>
  <profile-card title="Experience">
    <template #actions>
      <b-button type="is-ghost" size="is-small" class="profile-link" icon-left="plus" @click="open(null)">Add</b-button>
    </template>

    <ul v-if="props.worker.jobExperiences.length" class="experience-list">
      <li v-for="item in props.worker.jobExperiences" :key="item.id" class="experience-row">
        <span class="experience-period">
          {{ monthYear(item.startDate) }} — {{ item.isCurrentJobPosition ? 'Present' : monthYear(item.endDate) }}
        </span>
        <div class="experience-body">
          <span class="experience-title">
            {{ item.company }}<template v-if="item.supervisor"> · <span class="experience-supervisor">{{ item.supervisor }}</span></template>
          </span>
          <span v-if="item.duties" class="experience-duties">{{ item.duties }}</span>
        </div>
        <div class="experience-actions">
          <b-button type="is-ghost" size="is-small" class="profile-icon-button" icon-left="pencil" aria-label="Edit experience" @click="open(item)" />
          <b-button type="is-ghost" size="is-small" class="profile-icon-button is-destructive" icon-left="delete-outline"
            aria-label="Delete experience" @click="confirmDelete(item)" />
        </div>
      </li>
    </ul>
    <p v-else class="profile-empty">No experience added</p>

    <b-modal custom-content-class="card" v-model="isModalOpen" :width="editing ? '800px' : '500px'">
      <work-experience-form :workerId="props.worker.id" :data="editing ?? undefined" @updateExperience="onSaved" />
    </b-modal>
  </profile-card>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import dayjs from 'dayjs';
import { showAlertConfirm, showAlertError } from '@/utils/toast';
import { deleteWorkerWorkExperience } from '@/api/workerApi';
import type { WorkerProfileDetail, WorkerProfileJobExperienceDetail } from '@/types/worker';
import ProfileCard from './ProfileCard.vue';
import WorkExperienceForm from '@/components/worker/WorkExperienceForm.vue';

const props = defineProps<{ worker: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'updateProfile'): void; (e: 'loading', value: boolean): void }>();

const isModalOpen = ref(false);
const editing = ref<WorkerProfileJobExperienceDetail | null>(null);

function monthYear(value: string | null): string {
  return value ? dayjs(value).format('MMM YYYY') : '—';
}

function open(item: WorkerProfileJobExperienceDetail | null) {
  editing.value = item;
  isModalOpen.value = true;
}

function onSaved() {
  isModalOpen.value = false;
  emit('updateProfile');
}

function confirmDelete(item: WorkerProfileJobExperienceDetail) {
  showAlertConfirm('Are you sure?', 'You want to delete this experience').then((response) => {
    if (!response) {
      return;
    }
    emit('loading', true);
    deleteWorkerWorkExperience(props.worker.id, item.id)
      .then(() => emit('updateProfile'))
      .catch((error) => {
        emit('loading', false);
        showAlertError(error);
      });
  });
}
</script>

<style lang="scss" scoped>
@import '../../assets/scss/worker-profile';

.experience-list {
  display: flex;
  flex-direction: column;
  margin: 0;
  padding: 0;
  list-style: none;
}

.experience-row {
  display: grid;
  grid-template-columns: 170px minmax(0, 1fr) auto;
  gap: 16px;
  align-items: start;
  padding: 12px 0;
  border-top: 1px solid $gray-border;

  &:first-child {
    border-top: 0;
    padding-top: 0;
  }

  @include mobile {
    grid-template-columns: minmax(0, 1fr) auto;

    .experience-period {
      grid-column: 1 / -1;
    }
  }
}

.experience-period {
  font-size: 0.85rem;
  color: $grey-font;
}

.experience-body {
  display: flex;
  flex-direction: column;
  gap: 2px;
  font-size: 0.9rem;
  color: $navy;
}

.experience-title {
  font-weight: 600;
}

.experience-supervisor {
  font-weight: 400;
  color: $grey-font;
}

.experience-duties {
  color: $grey-font;
  white-space: pre-line;
}

.experience-actions {
  display: flex;
  gap: 2px;
}

.profile-empty {
  margin: 0;
}
</style>
