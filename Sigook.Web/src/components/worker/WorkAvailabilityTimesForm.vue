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
        <b-button type="is-primary" @click="createWorkerAvailabilityTimes()">
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
      availabilityTimes: [],
      worker: {
        availabilityTimes: []
      }
    }
  },
  methods: {
    createWorkerAvailabilityTimes() {
      this.isLoading = true;
      this.$store.dispatch('worker/createWorkerAvailabilityTimes', { profileId: this.data.id, model: this.worker.availabilityTimes })
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    }
  },
  async created() {
    this.availabilityTimes = await this.$store.dispatch('getAvailabilityTimes');
    if (this.data != null) {
      this.worker.availabilityTimes = this.data.availabilityTimes;
    }
  }
}
</script>