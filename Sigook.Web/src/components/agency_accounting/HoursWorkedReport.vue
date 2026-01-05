<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12 col-md-6 col-lg-4 col-padding">
        <b-field label="Dates (From - To)" :type="errors.has('dates') ? 'is-danger' : ''"
          :message="errors.has('dates') ? errors.first('dates') : ''">
          <b-datepicker v-model="datesSelected" v-validate="'required'" name="dates" range
            @input="onDatesSelected" />
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
        <b-button type="is-primary" @click="getReport">Generate</b-button>
      </div>
      <div class="col-12 col-padding">
        <export :url="'/api/agency/accounting/reports/hours-worked/file'" :params="serverParams"
          :fileName="'Hours Worked Report'" @onDataLoading="(value) => isLoading = value">
        </export>
        <b-table :data="report.rows" :mobile-cards="false" :loading="isLoadingReport" paginated :per-page="pageSize"
          :current-page.sync="pageIndex" pagination-rounded>
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
              {{ props.row.billRate | currency }}
            </b-table-column>
            <b-table-column field="regularHoursWorked" label="Regular Hours" v-slot="props">
              {{ props.row.regularHoursWorked }}
            </b-table-column>
            <b-table-column field="totalPayRegularRate" label="Total Pay Regular Rate" v-slot="props">
              {{ props.row.totalPayRegularRate | currency }}
            </b-table-column>
            <b-table-column field="overtimeHoursWorked" label="Overtime Hours" v-slot="props">
              {{ props.row.overtimeHoursWorked }}
            </b-table-column>
            <b-table-column field="totalPayOvertimeRate" label="Total Pay Overtime Rate" v-slot="props">
              {{ props.row.totalPayOvertimeRate | currency }}
            </b-table-column>
            <b-table-column field="holidayHoursWorked" label="Holiday Hours" v-slot="props">
              {{ props.row.holidayHoursWorked }}
            </b-table-column>
            <b-table-column field="totalPayHolidayRate" label="Total Pay Holiday Rate" v-slot="props">
              {{ props.row.totalPayHolidayRate | currency }}
            </b-table-column>
            <b-table-column field="totalHoursWorked" label="Total Hours" v-slot="props">
              {{ props.row.totalHoursWorked }}
            </b-table-column>
            <b-table-column field="totalPayRate" label="Total Pay Rate" v-slot="props">
              {{ props.row.totalPayRate | currency }}
            </b-table-column>
          </template>
          <template v-slot:footer>
            <template v-if="report.rows.length > 0">
              <th></th>
              <th></th>
              <th></th>
              <th>{{ report.totalRegularHours }}</th>
              <th>{{ report.totalPayRegular | currency }}</th>
              <th>{{ report.totalOvertimeHours }}</th>
              <th>{{ report.totalPayOvertime | currency }}</th>
              <th>{{ report.totalHolidayHours }}</th>
              <th>{{ report.totalPayHoliday | currency }}</th>
              <th>{{ report.totalHours }}</th>
              <th>{{ report.totalPay | currency }}</th>
            </template>
          </template>
        </b-table>
      </div>
    </div>
  </div>
</template>
<script>
import dayjs from 'dayjs';

export default {
  components: {
    Export: () => import("@/components/Export")
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
      report: {
        rows: []
      }
    }
  },
  async created() {
    await this.getCompanies();
  },
  methods: {
    async getCompanies() {
      this.isLoading = true;
      this.companies = await this.$store.dispatch('agency/getAgencyCompanyProfileWithRequests');
      this.isLoading = false;
    },
    async onDatesSelected() {
      this.serverParams.startDate = dayjs(this.datesSelected[0]).format('YYYY-MM-DD');
      this.serverParams.endDate = dayjs(this.datesSelected[1]).format('YYYY-MM-DD');
      await this.getJobPositionsHoursWorked();
    },
    async selectCompany(company) {
      if (company) {
        this.serverParams.companyId = company.companyId;
        await this.getJobPositionsHoursWorked();
      } else {
        this.serverParams.companyId = null;
        this.jobPositions = [];
      }
    },
    async getJobPositionsHoursWorked() {
      if (this.serverParams.companyId && this.datesSelected.length === 2) {
        this.isLoadingJobPositions = true;
        this.jobPositions = await this.$store.dispatch('agency/getJobPositionsHoursWorked', this.serverParams);
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
        this.$store.dispatch('agency/getHoursWorkedReport', this.serverParams)
          .then(response => {
            this.isLoadingReport = false;
            this.report = {
              ...response,
              rows: response.detail,
            }
          }).catch(error => {
            this.isLoadingReport = false;
            this.showAlertError(error);
          });
      }
    },
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