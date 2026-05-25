<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12 col-md-6 col-lg-4 col-padding">
        <b-field label="Dates (From - To)" :type="formErrors.dates ? 'is-danger' : ''"
          :message="formErrors.dates">
          <b-datepicker v-model="dates" name="dates" range
            @update:modelValue="onDatesSelected" />
        </b-field>
      </div>
      <div class="col-12 col-md-6 col-lg-4 col-padding">
        <b-field label="Company">
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
        <Export :url="'/api/agency/accounting/reports/hours-worked/file'" :params="serverParams"
          :fileName="'Hours Worked Report'" @onDataLoading="(value) => isLoading = value">
        </Export>
        <b-table sticky-header height="var(--grid-height)" :data="report.rows" :mobile-cards="false" :loading="isLoadingReport" paginated :per-page="pageSize"
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
<script setup lang="ts">
import { ref, computed } from 'vue';
import * as yup from 'yup';
import { showAlertError } from "@/utils/toast";
import dayjs from 'dayjs';
import { currency } from '@/utils/filters';
import { getAgencyCompanyProfileWithRequests } from "@/api/agencyCompanyApi";
import { getJobPositionsHoursWorked, getHoursWorkedReport } from "@/api/agencyReportApi";
import { useStickyForm } from '@/composables/useStickyForm';
import Export from "@/components/Export.vue";

const schema = yup.object({
  dates: yup
    .array()
    .of(yup.date())
    .min(2, 'Dates are required')
    .required('Dates are required'),
});

const form = useStickyForm<{ dates: Date[] }>({
  schema,
  initialValues: { dates: [] },
});
const { dates } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const isLoadingJobPositions = ref(false);
const isLoadingReport = ref(false);
const companies = ref<any[]>([]);
const companySelected = ref('');
const jobPositions = ref<any[]>([]);
const jobPositionSelected = ref('');
const pageIndex = ref(1);
const pageSize = ref(30);
const serverParams = ref<any>({});
const reportGenerated = ref(false);
const report = ref<any>({ rows: [] });

async function loadCompanies() {
  isLoading.value = true;
  companies.value = await getAgencyCompanyProfileWithRequests();
  isLoading.value = false;
}

async function onDatesSelected() {
  if (dates.value && dates.value.length === 2) {
    serverParams.value.startDate = dayjs(dates.value[0]).format('YYYY-MM-DD');
    serverParams.value.endDate = dayjs(dates.value[1]).format('YYYY-MM-DD');
    await loadJobPositions();
  }
}

async function selectCompany(company: any) {
  if (company) {
    serverParams.value.companyId = company.companyId;
    await loadJobPositions();
  } else {
    serverParams.value.companyId = null;
    jobPositions.value = [];
  }
}

async function loadJobPositions() {
  if (serverParams.value.companyId && dates.value && dates.value.length === 2) {
    isLoadingJobPositions.value = true;
    jobPositions.value = await getJobPositionsHoursWorked(serverParams.value);
    isLoadingJobPositions.value = false;
  }
}

function selectJobPosition(jobPosition: any) {
  if (jobPosition) {
    serverParams.value.jobPositionRateId = jobPosition.id;
  } else {
    serverParams.value.jobPositionRateId = null;
  }
}

async function getReport() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) return;
  isLoadingReport.value = true;
  getHoursWorkedReport(serverParams.value)
    .then((response) => {
      isLoadingReport.value = false;
      report.value = {
        ...response,
        rows: response.detail
      };
      reportGenerated.value = true;
    }).catch(error => {
      isLoadingReport.value = false;
      showAlertError(error);
    });
}

const filteredCompanies = computed(() =>
  companies.value.filter((company: any) => company.fullName.toLowerCase().includes(companySelected.value.toLowerCase()))
);
const filteredJobPositions = computed(() =>
  jobPositions.value.filter((jp: any) => jp.jobPosition.value.toLowerCase().includes(jobPositionSelected.value.toLowerCase()))
);

(async () => {
  await loadCompanies();
})();
</script>
