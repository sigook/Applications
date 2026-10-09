<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>

    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field label="Availability Times">
          <b-checkbox v-for="item in availabilityTimes" :key="item.id" v-model="worker.availabilityTimes"
              :native-value="item" class="mb-2">
              {{ item.value }}
            </b-checkbox>
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="saveWorkerAvailabilityTimes()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { getAvailabilityTimes } from "@/shared/api/catalogApi";
import { createWorkerAvailabilityTimes } from '@/shared/worker-profile/api';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import type { CatalogItem } from '@/shared/types/common';

const props = defineProps<{ data?: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const availabilityTimes = ref<CatalogItem[]>([]);
const worker = reactive<{ availabilityTimes: CatalogItem[] }>({ availabilityTimes: [] });

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
