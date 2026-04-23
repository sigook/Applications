<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>

    <div class="container-flex">
      <div class="col-12">
        <b-field label="Availability Times">
          <b-checkbox v-for="item in availabilityTimes" :key="item.id" v-model="worker.availabilityTimes"
              :native-value="item" class="mb-2">
              {{ item.value }}
            </b-checkbox>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="saveWorkerAvailabilityTimes()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getAvailabilityTimes } from "@/api/catalogApi";
import { createWorkerAvailabilityTimes } from '@/api/workerApi';

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const availabilityTimes = ref<any[]>([]);
const worker = reactive<{ availabilityTimes: any[] }>({ availabilityTimes: [] });

function saveWorkerAvailabilityTimes() {
  isLoading.value = true;
  createWorkerAvailabilityTimes(props.data.id, worker.availabilityTimes)
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

(async () => {
  availabilityTimes.value = await getAvailabilityTimes();
  if (props.data != null) {
    worker.availabilityTimes = props.data.availabilityTimes;
  }
})();
</script>
