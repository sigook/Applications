<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <LocationAddress ref="addressComponent" v-model:model="worker.location"
          @isLoading="(value) => isLoading = value" />
      </div>
      <div class="column is-6">
        <PhoneInput ref="mobileComponent" :required="true" model="Mobile Number" :defaultValue="worker.mobileNumber"
          @formattedPhone="(phone) => worker.mobileNumber = phone" />
      </div>
      <div class="column is-6">
        <PhoneInput ref="phoneComponent" :required="false" model="Phone" :defaultValue="worker.phone"
          @formattedPhone="(phone) => worker.phone = phone" />
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="validateAll()">{{ "Save" }}</b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import LocationAddress from '@/shared/ui/Address.vue';
import PhoneInput from '@/shared/ui/PhoneInput.vue';
import { showAlertError } from "@/shared/utils/toast";
import { createWorkerContactInformation } from '@/shared/worker-profile/api';
import type { WorkerProfileDetail, WorkerContactInformationModel } from '@/shared/worker-profile/types';

const props = defineProps<{ data?: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const worker = ref<Partial<WorkerProfileDetail>>({});
const addressComponent = ref<InstanceType<typeof LocationAddress> | null>(null);
const mobileComponent = ref<InstanceType<typeof PhoneInput> | null>(null);
const phoneComponent = ref<InstanceType<typeof PhoneInput> | null>(null);

async function validateAll() {
  const addressValid = await addressComponent.value?.validateAddress();
  const mobileValid = await mobileComponent.value?.validatePhone();
  const phoneValid = await phoneComponent.value?.validatePhone();
  if (addressValid && mobileValid && phoneValid) {
    saveContactInformation();
  } else {
    showAlertError('Please make sure all required fields are filled out correctly');
  }
}

function saveContactInformation() {
  isLoading.value = true;
  const payload: WorkerContactInformationModel = {
    mobileNumber: worker.value.mobileNumber,
    phone: worker.value.phone,
    phoneExt: worker.value.phoneExt,
    location: worker.value.location,
  };
  createWorkerContactInformation(worker.value.id, payload)
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

if (props.data != null) {
  worker.value = Object.assign({}, props.data);
  worker.value.location = Object.assign({}, props.data.location);
}
</script>
