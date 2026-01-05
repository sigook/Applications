<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field :label="$t('WorkerAvailableDays')">
          <div class="container-flex">
            <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
              <b-checkbox v-model="allDaysSelected" @input="changeDaysSelected">
                All Days
              </b-checkbox>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-3 col-padding" v-for="day in days" v-bind:key="day.id">
              <b-checkbox v-model="worker.availabilityDays" :native-value="day" @input="changeAllDays">
                {{ day.value }}
              </b-checkbox>
            </div>
          </div>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="createWorkerAvailabilityDays()">
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
      allDaysSelected: false,
      days: [],
      worker: {
        availabilityDays: []
      }
    }
  },
  methods: {
    createWorkerAvailabilityDays() {
      this.isLoading = true;
      this.$store.dispatch('worker/createWorkerAvailabilityDays', { profileId: this.data.id, model: this.worker.availabilityDays })
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    changeDaysSelected() {
      this.worker.availabilityDays = [];
      if (this.allDaysSelected) {
        for (let i = 0; i < this.days.length; i++) {
          this.worker.availabilityDays.push(this.days[i]);
        }
      }
    },
    changeAllDays() {
      for (let i = 0; i < this.worker.availabilityDays.length; i++) {
        this.allDaysSelected = this.worker.availabilityDays.length === this.days.length;
      }
    }
  },
  async created() {
    this.days = await this.$store.dispatch('getDays');
    if (this.data != null) {
      this.worker.availabilityDays = this.data.availabilityDays;
      this.changeAllDays()
    }
  }
}
</script>