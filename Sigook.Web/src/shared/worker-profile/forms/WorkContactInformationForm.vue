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
import { ref, useTemplateRef } from 'vue';
import LocationAddress from '@/shared/ui/Address.vue';
import PhoneInput from '@/shared/ui/PhoneInput.vue';
import { showAlertError } from "@/shared/utils/toast";
import { createWorkerContactInformation } from '@/shared/worker-profile/api';
import type { WorkerProfileDetail, WorkerContactInformationModel } from '@/shared/worker-profile/types';

const { data } = defineProps<{ data: WorkerProfileDetail }>();
const emit = defineEmits<(e: 'closeModal', value: boolean) => void>();

const isLoading = ref(false);
const worker = ref<WorkerProfileDetail>({ ...data });
const addressComponent = useTemplateRef<InstanceType<typeof LocationAddress>>('addressComponent');
const mobileComponent = useTemplateRef<InstanceType<typeof PhoneInput>>('mobileComponent');
const phoneComponent = useTemplateRef<InstanceType<typeof PhoneInput>>('phoneComponent');

async function validateAll() {
  const addressValid = await addressComponent.value?.validateAddress();
  const mobileValid = await mobileComponent.value?.validatePhone();
  const phoneValid = await phoneComponent.value?.validatePhone();
  if (addressValid && mobileValid && phoneValid) {
    await saveContactInformation();
  } else {
    showAlertError('Please make sure all required fields are filled out correctly');
  }
}

async function saveContactInformation() {
  isLoading.value = true;
  const payload: WorkerContactInformationModel = {
    mobileNumber: worker.value.mobileNumber,
    phone: worker.value.phone,
    phoneExt: worker.value.phoneExt,
    location: worker.value.location,
  };
  try {
    await createWorkerContactInformation(worker.value.id, payload);
    isLoading.value = false;
    emit('closeModal', true);
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

worker.value = Object.assign({}, data);
worker.value.location = Object.assign({}, data.location);
</script>
