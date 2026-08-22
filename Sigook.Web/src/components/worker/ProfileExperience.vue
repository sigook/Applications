<template>
  <form>
    <b-loading v-model="isLoading"></b-loading>
    <div class="tab-actions">
      <b-button type="is-primary" icon-right="plus" @click="modalWorkExperience = true">
        {{ "Add Experience" }}
      </b-button>
    </div>
    <div>
      <work-experience-detail
        v-for="(item, index) in props.worker.jobExperiences"
        :key="'workExp' + index"
        :item="item"
        :workerId="props.worker.id"
        @getWorker="() => updateExperience()" />

      <b-modal custom-content-class="card" v-model="modalWorkExperience" width="800px">
        <work-experience :workerId="props.worker.id" @updateExperience="() => updateExperience()" />
      </b-modal>
    </div>

  </form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useAppStore } from '@/stores/app';
import WorkExperience from './WorkExperienceForm.vue';
import WorkExperienceDetail from '../../components/worker/WorkExperienceDetail.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile'): void }>();

const appStore = useAppStore();

const disableStartDate = ref<any>(null);
const modalWorkExperience = ref(false);
const isLoading = ref(false);

function updateExperience() {
  modalWorkExperience.value = false;
  emit('updateProfile');
}

appStore.getCurrentDate().then(response => {
  disableStartDate.value = response;
});
</script>

<style scoped>
.tab-actions {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 12px;
}
</style>
