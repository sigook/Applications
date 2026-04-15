<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-table :data="rows" narrowed hoverable :mobile-cards="false" v-model:checked-rows="selectedWorkers" checkable>
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
    <b-button type="is-primary" :disabled="selectedWorkers.length === 0" @click="submitGeneratePayStubs">Generate</b-button>
  </div>
</template>
<script lang="ts">
import { showAlertError } from "@/utils/toast";
import { getWorkersReadyForPayStub, generatePayStubs } from "@/api/agencyPayStubApi";

export default {
  data() {
    return {
      isLoading: false,
      rows: [],
      selectedWorkers: []
    }
  },
  created() {
    this.loadWorkers();
  },
  methods: {
    loadWorkers() {
      this.isLoading = true;
      getWorkersReadyForPayStub()
        .then((response) => {
          this.rows = response;
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    submitGeneratePayStubs() {
      this.isLoading = true;
      const workerIds = this.selectedWorkers.map(worker => worker.workerId);
      generatePayStubs(workerIds)
        .then(() => {
          this.isLoading = false;
          this.$emit("pay-stubs-generated");
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
          this.loadWorkers();
        });
    }
  }
}
</script>