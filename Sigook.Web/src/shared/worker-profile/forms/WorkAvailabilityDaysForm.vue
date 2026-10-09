<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field :label="'Available days'">
          <div class="columns is-multiline">
            <div class="column is-6 is-3-desktop">
              <b-checkbox v-model="allDaysSelected" @update:modelValue="changeDaysSelected">
                All Days
              </b-checkbox>
            </div>
            <div class="column is-6 is-3-desktop" v-for="day in days" v-bind:key="day.id">
              <b-checkbox v-model="worker.availabilityDays" :native-value="day" @update:modelValue="changeAllDays">
                {{ day.value }}
              </b-checkbox>
            </div>
          </div>
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="saveWorkerAvailabilityDays()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { getDays } from "@/shared/api/catalogApi";
import { createWorkerAvailabilityDays } from '@/shared/worker-profile/api';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import type { CatalogItem } from '@/shared/types/common';

const props = defineProps<{ data?: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const allDaysSelected = ref(false);
const days = ref<CatalogItem[]>([]);
const worker = reactive<{ availabilityDays: CatalogItem[] }>({ availabilityDays: [] });

function saveWorkerAvailabilityDays() {
  isLoading.value = true;
  createWorkerAvailabilityDays(props.data.id, worker.availabilityDays)
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function changeDaysSelected() {
  worker.availabilityDays = [];
  if (allDaysSelected.value) {
    for (let i = 0; i < days.value.length; i++) {
      worker.availabilityDays.push(days.value[i]);
    }
  }
}

function changeAllDays() {
  for (let i = 0; i < worker.availabilityDays.length; i++) {
    allDaysSelected.value = worker.availabilityDays.length === days.value.length;
  }
}

(async () => {
  days.value = await getDays();
  if (props.data != null) {
    worker.availabilityDays = props.data.availabilityDays;
    changeAllDays();
  }
})();
</script>
