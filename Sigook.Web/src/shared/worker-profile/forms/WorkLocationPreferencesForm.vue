<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field label="Location Preferences">
          <b-taginput v-model="worker.locationPreferences" autocomplete :data="filteredCitiesLocations"
            open-on-focus field="value" icon="label" placeholder="Select Locations"
            @typing="getFilteredCities" append-to-body>
          </b-taginput>
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="saveWorkerLocationPreferences()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { getCities } from "@/shared/api/locationApi";
import { createWorkerLocationPreferences } from '@/shared/worker-profile/api';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import type { CatalogItem, City } from '@/shared/types/common';

const { data } = defineProps<{ data: WorkerProfileDetail }>();
const emit = defineEmits<(e: 'closeModal', value: boolean) => void>();

const isLoading = ref(false);
const citiesLocations = ref<City[]>([]);
const filteredCitiesLocations = ref<City[]>([]);
const worker = reactive<{ locationPreferences: CatalogItem[] }>({ locationPreferences: [] });

async function saveWorkerLocationPreferences() {
  isLoading.value = true;
  try {
    await createWorkerLocationPreferences(data.id, worker.locationPreferences);
    isLoading.value = false;
    emit('closeModal', true);
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

async function loadCities() {
  const provinceId = data.location?.city?.province?.id;
  if (!provinceId) return;
  isLoading.value = true;
  try {
    const result = await getCities(provinceId);
    isLoading.value = false;
    citiesLocations.value = result;
    filteredCitiesLocations.value = result;
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

function getFilteredCities(text: string) {
  filteredCitiesLocations.value = citiesLocations.value.filter((option) =>
    option.value.toLowerCase().includes(text.toLowerCase())
  );
}

worker.locationPreferences = data.locationPreferences;
void loadCities();
</script>
