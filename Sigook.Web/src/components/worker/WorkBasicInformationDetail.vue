<template>
  <section>
    <div class="d-flex align-items-center justify-content-between">
      <h3 class="section-title mt-0">{{ "Basic Information" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil"
        @click="modalBasicInformation = true"></b-button>
    </div>
    <div class="worker-documents">
      <div>
        <span>{{ 'Full Name' }}</span>
        <span>
          <p class="fw-light m-0">
            {{ props.worker.firstName }} {{ props.worker.middleName }} {{ props.worker.lastName }} {{ props.worker.secondLastName }}
          </p>
        </span>
      </div>
      <div>
        <span>{{ 'Date of birth' }}</span>
        <span>
          <p class="fw-light m-0">{{ dateMonth(props.worker.birthDay) }}</p>
        </span>
      </div>
      <div>
        <span>{{ 'Gender' }}</span>
        <span>
          <p class="fw-light m-0">{{ props.worker.gender ? props.worker.gender.value : null }}</p>
        </span>
      </div>
      <div>
        <span>{{ 'Do you have your own vehicle?' }}</span>
        <span>
          <p class="fw-light m-0">{{ props.worker.hasVehicle ? 'Yes' : 'No' }}</p>
        </span>
      </div>
    </div>
    <!-- custom modal -->
    <b-modal v-model="modalBasicInformation" @close="modalBasicInformation = false" width="500px">
      <basic-information-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>

    <!-- end custom modal -->
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { dateMonth } from '@/utils/filters';
import BasicInformationEdit from './WorkBasicInformationForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalBasicInformation = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalBasicInformation.value = false;
}
</script>
