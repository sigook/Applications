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
import LocationAddress from '@/components/Address.vue';
import PhoneInput from '@/components/PhoneInput.vue';
import { showAlertError } from "@/utils/toast";
import { createWorkerContactInformation } from '@/api/workerApi';

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const worker = ref<any>({});
const addressComponent = ref<any>(null);
const mobileComponent = ref<any>(null);
const phoneComponent = ref<any>(null);

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
  createWorkerContactInformation(worker.value.id, worker.value)
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
