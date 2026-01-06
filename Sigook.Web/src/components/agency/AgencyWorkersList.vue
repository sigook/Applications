<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-field label="Worker" :type="errors.has('worker') ? 'is-danger' : ''"
          :message="errors.has('worker') ? errors.first('worker') : 'Type at least 3 characters to search'">
          <b-autocomplete v-model="workerSelected" :data="workers" placeholder="Worker" name="worker" append-to-body
            :loading="isLoadingList" @typing="onWorkerInput" v-validate="'required'" @select="selectWorker"
            :custom-formatter="(option) => `${option.fullName} | ${option.approvedToWork ? 'Approved' : 'Not Approved'}`">
          </b-autocomplete>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="bookWorker" :disabled="!workerId">Book Worker</b-button>
      </div>
    </div>
  </div>
</template>

<script>

export default {
  data() {
    return {
      isLoading: false,
      requestId: this.$route.params.id,
      isLoadingList: false,
      workers: [],
      workerSelected: null,
      workerId: null
    }
  },
  methods: {
    onWorkerInput(text) {
      if (text.length >= 3) {
        this.getAllWorkers(text);
      } else {
        this.workers = [];
      }
    },
    getAllWorkers(text) {
      this.isLoadingList = true;
      this.$store.dispatch("agency/getAllWorkers", { searchTerm: text })
        .then(response => {
          this.isLoadingList = false;
          this.workers = response;
        })
        .catch(error => {
          this.isLoadingList = false;
          this.showAlertError(error);
        });
    },
    selectWorker(worker) {
      if (worker) {
        this.workerId = worker.id;
      } else {
        this.workerId = null;
      }
    },
    async bookWorker() {
      this.$buefy.dialog.prompt({
        message: "Starting Date",
        inputAttrs: {
          type: 'date',
          placeholder: 'Date',
          required: true
        },
        confirmText: 'Book',
        onConfirm: async (value, dialog) => {
          this.isLoading = true;
          await this.$store.dispatch("agency/bookAgencyRequestWorker", {
            requestId: this.requestId,
            workerId: this.workerId,
            model: { startDate: value }
          }).then(() => {
            this.isLoading = false;
            this.showAlertSuccess(this.$t('Booked'));
            this.$emit('workerBooked');
            dialog.close();
          }).catch(error => {
            this.isLoading = false;
            this.showAlertError(error);
          });
        }
      });
    },
  },
}
</script>