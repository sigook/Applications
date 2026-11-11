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
import { showAlertError } from "@/shared/utils/toast";
import { getAvailability } from "@/shared/api/catalogApi";
import { createWorkerAvailabilities } from '@/shared/worker-profile/api';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import type { CatalogItem } from '@/shared/types/common';

const { data } = defineProps<{ data: WorkerProfileDetail }>();
const emit = defineEmits<(e: 'closeModal', value: boolean) => void>();

const isLoading = ref(false);
const availabilities = ref<CatalogItem[]>([]);
const worker = reactive<{ availabilities: CatalogItem[] }>({ availabilities: [] });

async function saveWorkerAvailabilities() {
  isLoading.value = true;
  try {
    await createWorkerAvailabilities(data.id, worker.availabilities);
    isLoading.value = false;
    emit('closeModal', true);
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

void (async () => {
  availabilities.value = await getAvailability();
  worker.availabilities = data.availabilities;
})();
</script>
