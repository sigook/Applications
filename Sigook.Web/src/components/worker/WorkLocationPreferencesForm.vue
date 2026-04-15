<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field label="Location Preferences">
          <b-taginput v-model="worker.locationPreferences" autocomplete :data="filteredCitiesLocations"
            open-on-focus field="value" icon="label" placeholder="Select Locations" 
            @typing="getFilteredCities" append-to-body>
          </b-taginput>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="createWorkerLocationPreferences()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { showAlertError } from "@/utils/toast";
import { getCities } from "@/api/locationApi";
import { createWorkerLocationPreferences } from '@/api/workerApi';
export default {
  props: ['data'],
  data() {
    return {
      isLoading: false,
      citiesLocations: [],
      filteredCitiesLocations: [],
      worker: {
        locationPreferences: []
      }
    }
  },
  methods: {
    createWorkerLocationPreferences() {
      this.isLoading = true;
      createWorkerLocationPreferences(this.data.id, this.worker.locationPreferences)
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        })
    },
    getCities() {
      this.isLoading = true;
      getCities(this.data.location.city.province.id)
        .then(result => {
          this.isLoading = false;
          this.citiesLocations = result;
          this.filteredCitiesLocations = result;
        }).catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    getFilteredCities(text) {
      this.filteredCitiesLocations = this.citiesLocations.filter((option) => 
        option.value.toLowerCase().includes(text.toLowerCase())
      );
    }
  },
  created() {
    if (this.data != null) {
      this.worker.locationPreferences = this.data.locationPreferences;
      this.getCities();
    }
  }
}
</script>