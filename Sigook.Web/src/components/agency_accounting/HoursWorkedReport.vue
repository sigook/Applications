<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12 col-md-6 col-lg-4 col-padding">
        <b-field label="Dates (From - To)" :type="errors.has('dates') ? 'is-danger' : ''"
          :message="errors.has('dates') ? errors.first('dates') : ''">
          <b-datepicker v-model="datesSelected" v-validate="'required'" name="dates" range
            @update:modelValue="onDatesSelected" />
        </b-field>
      </div>
      <div class="col-12 col-md-6 col-lg-4 col-padding">
        <b-field label="Company" :type="errors.has('company') ? 'is-danger' : ''"
          :message="errors.has('company') ? errors.first('company') : ''">
          <b-autocomplete v-model="companySelected" :data="filteredCompanies" open-on-focus
            field="fullName" name="company" placeholder="Company" @select="selectCompany">
          </b-autocomplete>
        </b-field>
      </div>
      <div class="col-12 col-md-6 col-lg-4 col-padding">
        <b-field label="Job Position">
          <b-autocomplete v-model="jobPositionSelected" :data="filteredJobPositions" open-on-focus
            field="jobPosition.value" :loading="isLoadingJobPositions" name="jobPosition" placeholder="Job Position"
            @select="selectJobPosition">
            <template v-slot:empty>
              <p class="container text-center">No records available</p>
            </template>
          </b-autocomplete>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="getReport" :loading="isLoadingReport">Generate</b-button>
      </div>
      <div v-if="reportGenerated" class="col-12 col-padding">
        <export :url="'/api/agency/accounting/reports/hours-worked/file'" :params="serverParams"
          :fileName="'Hours Worked Report'" @onDataLoading="(value) => isLoading = value">
        </export>
        <b-table :data="report.rows" :mobile-cards="false" :loading="isLoadingReport" paginated :per-page="pageSize"
          v-model:current-page="pageIndex" pagination-rounded>
          <template v-slot:empty>
            <p class="container text-center">No records available</p>
          </template>
          <template>
            <b-table-column field="workerName" label="Worker Name" v-slot="props">
              {{ props.row.workerName }}
            </b-table-column>
            <b-table-column field="jobPosition" label="Job Position" v-slot="props">
              {{ props.row.jobPosition }}
            </b-table-column>
            <b-table-column field="billRate" label="Bill Rate" v-slot="props">
              {{ currency(props.row.billRate) }}
            </b-table-column>
            <b-table-column field="regularHoursWorked" label="Regular Hours" v-slot="props">
              {{ props.row.regularHoursWorked }}
            </b-table-column>
            <b-table-column field="totalPayRegularRate" label="Total Pay Regular Rate" v-slot="props">
              {{ currency(props.row.totalPayRegularRate) }}
            </b-table-column>
            <b-table-column field="overtimeHoursWorked" label="Overtime Hours" v-slot="props">
              {{ props.row.overtimeHoursWorked }}
            </b-table-column>
            <b-table-column field="totalPayOvertimeRate" label="Total Pay Overtime Rate" v-slot="props">
              {{ currency(props.row.totalPayOvertimeRate) }}
            </b-table-column>
            <b-table-column field="holidayHoursWorked" label="Holiday Hours" v-slot="props">
              {{ props.row.holidayHoursWorked }}
            </b-table-column>
            <b-table-column field="totalPayHolidayRate" label="Total Pay Holiday Rate" v-slot="props">
              {{ currency(props.row.totalPayHolidayRate) }}
            </b-table-column>
            <b-table-column field="totalHoursWorked" label="Total Hours" v-slot="props">
              {{ props.row.totalHoursWorked }}
            </b-table-column>
            <b-table-column field="totalPayRate" label="Total Pay Rate" v-slot="props">
              {{ currency(props.row.totalPayRate) }}
            </b-table-column>
          </template>
          <template v-slot:footer>
            <template v-if="report.rows.length > 0">
              <th></th>
              <th></th>
              <th></th>
              <th>{{ report.totalRegularHours }}</th>
              <th>{{ currency(report.totalPayRegular) }}</th>
              <th>{{ report.totalOvertimeHours }}</th>
              <th>{{ currency(report.totalPayOvertime) }}</th>
              <th>{{ report.totalHolidayHours }}</th>
              <th>{{ currency(report.totalPayHoliday) }}</th>
              <th>{{ report.totalHours }}</th>
              <th>{{ currency(report.totalPay) }}</th>
            </template>
          </template>
        </b-table>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { showAlertError } from "@/utils/toast";
import dayjs from 'dayjs';
import { currency } from '@/utils/filters';
import { getAgencyCompanyProfileWithRequests } from "@/api/agencyCompanyApi";
import { getJobPositionsHoursWorked, getHoursWorkedReport } from "@/api/agencyReportApi";

export default {
  components: {
    Export: defineAsyncComponent(() => import("@/components/Export.vue"))
  },
  data() {
    return {
      isLoading: false,
      isLoadingJobPositions: false,
      isLoadingReport: false,
      datesSelected: [],
      companies: [],
      companySelected: '',
      jobPositions: [],
      jobPositionSelected: '',
      pageIndex: 1,
      pageSize: 30,
      serverParams: {},
      reportGenerated: false,
      report: {
        rows: []
      }
    }
  },
  async created() {
    await this.loadCompanies();
  },
  methods: {
    currency,
    async loadCompanies() {
      this.isLoading = true;
      this.companies = await getAgencyCompanyProfileWithRequests();
      this.isLoading = false;
    },
    async onDatesSelected() {
      this.serverParams.startDate = dayjs(this.datesSelected[0]).format('YYYY-MM-DD');
      this.serverParams.endDate = dayjs(this.datesSelected[1]).format('YYYY-MM-DD');
      await this.loadJobPositions();
    },
    async selectCompany(company) {
      if (company) {
        this.serverParams.companyId = company.companyId;
        await this.loadJobPositions();
      } else {
        this.serverParams.companyId = null;
        this.jobPositions = [];
      }
    },
    async loadJobPositions() {
      if (this.serverParams.companyId && this.datesSelected.length === 2) {
        this.isLoadingJobPositions = true;
        this.jobPositions = await getJobPositionsHoursWorked(this.serverParams);
        this.isLoadingJobPositions = false;
      }
    },
    selectJobPosition(jobPosition) {
      if (jobPosition) {
        this.serverParams.jobPositionRateId = jobPosition.id;
      } else {
        this.serverParams.jobPositionRateId = null;
      }
    },
    async getReport() {
      const result = await this.$validator.validateAll();
      if (result) {
        this.isLoadingReport = true;
        getHoursWorkedReport(this.serverParams)
          .then((response) => {
            this.isLoadingReport = false;
            this.report = {
              ...response,
              rows: response.detail
            }
            this.reportGenerated = true;
          }).catch(error => {
            this.isLoadingReport = false;
            showAlertError(error);
          });
      }
    }
  },
  computed: {
    filteredCompanies() {
      return this.companies.filter(company => company.fullName.toLowerCase().includes(this.companySelected.toLowerCase()));
    },
    filteredJobPositions() {
      return this.jobPositions.filter(jp => jp.jobPosition.value.toLowerCase().includes(this.jobPositionSelected.toLowerCase()));
    }
  }
}
</script>