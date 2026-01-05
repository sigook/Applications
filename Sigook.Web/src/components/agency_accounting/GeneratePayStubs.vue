<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-table :data="rows" narrowed hoverable :mobile-cards="false" :checked-rows.sync="selectedWorkers" checkable>
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="firstName" label="Worker" v-slot="props">
          {{ props.row.firstName }} {{ props.row.middleName }} {{ props.row.lastName }} {{ props.row.secondLastName }}
        </b-table-column>
        <b-table-column field="businessName" label="Company" v-slot="props">
          {{ props.row.businessName }}
        </b-table-column>
      </template>
    </b-table>
    <b-button type="is-primary" :disabled="selectedWorkers.length === 0" @click="generatePayStubs">Generate</b-button>
  </div>
</template>
<script>
export default {
  data() {
    return {
      isLoading: false,
      rows: [],
      selectedWorkers: []
    }
  },
  created() {
    this.getWorkersReadyForPayStub();
  },
  methods: {
    getWorkersReadyForPayStub() {
      this.isLoading = true;
      this.$store.dispatch("agency/getWorkersReadyForPayStub")
        .then((response) => {
          this.rows = response;
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    generatePayStubs() {
      this.isLoading = true;
      const workerIds = this.selectedWorkers.map(worker => worker.workerId);
      this.$store.dispatch("agency/generatePayStubs", workerIds)
        .then((response) => {
          this.isLoading = false;
          this.$emit("pay-stubs-generated");
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
          this.getWorkersReadyForPayStub();
        });
    }
  }
}
</script>