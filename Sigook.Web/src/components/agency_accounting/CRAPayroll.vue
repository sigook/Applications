<template>
  <div>
    <div class="container-flex">
      <div class="col-3 col-padding">
        <b-field label="Dates (From - To)" :type="errors.has('dates') ? 'is-danger' : ''"
          :message="errors.has('dates') ? errors.first('dates') : ''">
          <b-datepicker v-model="createdAtDatesSelected" v-validate="'required'" name="dates" range
            @update:modelValue="onDatesSelected" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="getReport" :loading="isLoading">Generate</b-button>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { showAlertError } from "@/utils/toast";
import dayjs from "dayjs";
import { downloadFile } from "@/utils/downloadFile";
import { getCraPayrollReport } from "@/api/agencyReportApi";

export default {
  data() {
    return {
      isLoading: false,
      createdAtDatesSelected: [],
      serverParams: {}
    }
  },
  methods: {
    downloadFile,
    onDatesSelected() {
      this.serverParams.startDate = dayjs(this.createdAtDatesSelected[0]).format('YYYY-MM-DD');
      this.serverParams.endDate = dayjs(this.createdAtDatesSelected[1]).format('YYYY-MM-DD');
    },
    async getReport() {
      const result = await this.$validator.validateAll();
      if (result) {
        this.isLoading = true;
        getCraPayrollReport(this.serverParams)
          .then(response => {
            this.isLoading = false;
            this.downloadFile(response, `CRA_Payroll_${this.serverParams.startDate}_${this.serverParams.endDate}`);
          })
          .catch(error => {
            this.isLoading = false;
            showAlertError(error);
          });
      }
    }
  }
}
</script>
