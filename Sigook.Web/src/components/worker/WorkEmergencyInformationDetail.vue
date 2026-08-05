<template>
  <section>
    <div class="is-flex is-align-items-center is-justify-content-space-between">
      <h3 class="section-title">{{ "Emergency information" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalEmergencyInformation = true"></b-button>
    </div>
    <div class="worker-documents">
      <div>
        <span>{{ 'Do you have any health problems / allergies?' }}</span>
        <span>
          <p class="has-text-weight-light m-0">{{ props.worker.haveAnyHealthProblem ? "Yes" : "No" }}</p>
        </span>
      </div>
      <div v-if="props.worker.haveAnyHealthProblem">
        <span>{{ 'Which' }} ?</span>
        <span>
          <p class="has-text-weight-light m-0">{{ props.worker.healthProblem }}</p>
        </span>
      </div>
      <div v-if="props.worker.haveAnyHealthProblem">
        <span>{{ 'Other allergies' }}</span>
        <span>
          <p class="has-text-weight-light m-0">{{ props.worker.otherHealthProblem }}</p>
        </span>
      </div>

      <p class="has-text-weight-normal fz-0 is-italic mb-3">{{ 'In case of emergency notify' }}: </p>
      <div>
        <span>{{ 'Name' }}</span>
        <span>
          <p class="has-text-weight-light m-0">{{ props.worker.contactEmergencyName }} {{ props.worker.contactEmergencyLastName }}</p>
        </span>
      </div>
      <div>
        <span>{{ 'Phone' }}</span>
        <span>
          <p class="has-text-weight-light m-0">{{ props.worker.contactEmergencyPhone }}</p>
        </span>
      </div>
    </div>
    <b-modal custom-content-class="card" v-model="modalEmergencyInformation" width="800px">
      <emergency-information-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import EmergencyInformationEdit from './WorkEmergencyInformationForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalEmergencyInformation = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalEmergencyInformation.value = false;
}
</script>
