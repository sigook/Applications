<template>
  <profile-card title="Skills">
    <template #actions>
      <button type="button" class="profile-link" @click="isModalOpen = true">Edit</button>
    </template>

    <div v-if="props.worker.skills.length" class="profile-tags">
      <span v-for="(item, index) in props.worker.skills" :key="item.id ?? index" class="profile-tag is-outlined">
        {{ item.skill }}
      </span>
    </div>
    <p v-else class="profile-empty">No skills added</p>

    <b-modal custom-content-class="card" v-model="isModalOpen" width="500px">
      <skills-form :data="props.worker" @closeModal="onSaved" />
    </b-modal>
  </profile-card>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { WorkerProfileDetail } from '@/types/worker';
import ProfileCard from './ProfileCard.vue';
import SkillsForm from '@/components/worker/WorkSkillsForm.vue';

const props = defineProps<{ worker: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'updateProfile'): void }>();

const isModalOpen = ref(false);

function onSaved() {
  isModalOpen.value = false;
  emit('updateProfile');
}
</script>

<style lang="scss" scoped>
@import '../../assets/scss/agency-worker-profile';

.profile-empty {
  margin: 0;
}
</style>
