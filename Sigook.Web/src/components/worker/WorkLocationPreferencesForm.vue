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
          {{ $t("Save") }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script>
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
      this.$store.dispatch('worker/createWorkerLocationPreferences', { profileId: this.data.id, model: this.worker.locationPreferences })
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    getCities() {
      this.isLoading = true;
      this.$store.dispatch('getCities', this.data.location.city.province.id)
        .then(result => {
          this.isLoading = false;
          this.citiesLocations = result;
          this.filteredCitiesLocations = result;
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
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