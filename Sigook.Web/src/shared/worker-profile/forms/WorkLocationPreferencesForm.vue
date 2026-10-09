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

const props = defineProps<{ data?: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const citiesLocations = ref<City[]>([]);
const filteredCitiesLocations = ref<City[]>([]);
const worker = reactive<{ locationPreferences: CatalogItem[] }>({ locationPreferences: [] });

function saveWorkerLocationPreferences() {
  isLoading.value = true;
  createWorkerLocationPreferences(props.data.id, worker.locationPreferences)
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function loadCities() {
  isLoading.value = true;
  getCities(props.data.location.city.province.id)
    .then(result => {
      isLoading.value = false;
      citiesLocations.value = result;
      filteredCitiesLocations.value = result;
    }).catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function getFilteredCities(text: string) {
  filteredCitiesLocations.value = citiesLocations.value.filter((option) =>
    option.value.toLowerCase().includes(text.toLowerCase())
  );
}

if (props.data != null) {
  worker.locationPreferences = props.data.locationPreferences;
  loadCities();
}
</script>
