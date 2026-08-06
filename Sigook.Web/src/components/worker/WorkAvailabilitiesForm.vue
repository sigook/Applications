<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field :label="'Availabilities'" class="has-text-weight-normal">
          <b-checkbox v-for="item in availabilities" :key="item.id" v-model="worker.availabilities" :native-value="item"
            class="mb-2">
            {{ item.value }}
          </b-checkbox>
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="saveWorkerAvailabilities()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getAvailability } from "@/api/catalogApi";
import { createWorkerAvailabilities } from '@/api/workerApi';

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const availabilities = ref<any[]>([]);
const worker = reactive<{ availabilities: any[] }>({ availabilities: [] });

function saveWorkerAvailabilities() {
  isLoading.value = true;
  createWorkerAvailabilities(props.data.id, worker.availabilities)
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
  availabilities.value = await getAvailability();
  if (props.data != null) {
    worker.availabilities = props.data.availabilities;
  }
})();
</script>
