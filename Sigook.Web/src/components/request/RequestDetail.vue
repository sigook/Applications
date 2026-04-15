<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <!-- Highlight -->
    <div class="col-12 col-padding highlight-content" v-if="request">
      <div class="item">
        <span class="fw-700">Rate / Salary</span>
        <p>{{ currency(request.workerRate || request.workerSalary) }}</p>
      </div>
      <div class="item">
        <span class="fw-700">Term</span>
        <p>{{ DurationTermLabels[request.durationTerm] }}</p>
      </div>
      <div class="item">
        <span class="fw-700">Employment Type</span>
        <p>{{ EmploymentTypeLabels[request.employmentType] }}</p>
      </div>
      <div class="item">
        <span class="fw-700">Start
          <span
            v-if="((request.status === RequestStatus.Filled || request.status === RequestStatus.Cancelled) && request.durationTerm === DurationTerm.LongTerm) || request.durationTerm === DurationTerm.ShortTerm">
            / Finish</span>
        </span>
        <p>
          {{ dateMonth(request.startAt) }}
          <span class="fz-0" v-if="request.durationTerm !== DurationTerm.LongTerm">
            / {{ dateMonth(request.finishAt) }}</span>
          <span class="fz-0"
            v-if="(request.status === RequestStatus.Filled || request.status === RequestStatus.Cancelled) && request.durationTerm === DurationTerm.LongTerm">
            / {{ dateMonth(request.finishAt) }}
          </span>
        </p>
      </div>
      <div class="item worker-options">
        <span class="fw-700">Workers</span>
        <p class="hover-actions">
          <span class="mr-1 fz-0">{{ request.workersQuantityWorking }} /
            {{ request.workersQuantity }}</span>
          <button v-if="request.canEdit" @click="onIncreaseWorkersQuantity()"
            class="btn-icon-sm btn-icon-circle-plus bg-transparent relative actions">
            add
          </button>
          <button @click="onReduceWorkersQuantity"
            class="btn-icon-sm btn-icon-circle-minus bg-transparent relative actions"
            v-if="request.canEdit && request.workersQuantityWorking < request.workersQuantity && request.workersQuantity !== 1">
            reduce
          </button>
        </p>
      </div>
      <div class="item">
        <span class="fw-700">Is Asap</span>
        <p>
          <b-checkbox v-model="localRequest.isAsap" @input="onToggleIsAsap()"></b-checkbox>
        </p>
      </div>
      <div class="item">
        <span class="fw-700">Visible Punch Card</span>
        <p class="w-50">
          <b-checkbox v-model="localRequest.punchCardOptionEnabled" @input="onTogglePunchCardVisibility()"></b-checkbox>
        </p>
      </div>
      <div class="item">
        <span class="fw-700">Vaccination</span>
        <router-link :to="'/agency-companies/company/' + this.request.companyProfileId">
          <p>
            {{ request.vaccinationRequired ? "yes" : "No" }}
            <b-icon icon="needle" class="ml-2"></b-icon>
          </p>
        </router-link>
      </div>
    </div>

    <!-- Role -->
    <div class="col-12 col-padding">
      <span class="fw-700 mr-2">Role</span>
      <span class="fw-400">{{ request.jobPosition }}</span>
      <span v-if="request.displayShift" class="request-shift-container"><b class="fw-700 ">Shift</b>
        <agency-shift class="ml-3" :requestId="request.id" :displayShift="request.displayShift" />
      </span>
    </div>

    <!-- Skills -->
    <skills :request="request"></skills>

    <!-- Detail -->
    <section class="col-12 col-padding">
      <span class="fw-700 is-inline-block mb-2">Description</span>
      <pre class="long-description bullet-list" v-html="request.description"></pre>
    </section>

    <section class="col-12 col-padding">
      <span class="fw-700 is-inline-block mb-2">Responsibilities</span>
      <pre class="long-description bullet-list" v-html="request.responsibilities"></pre>
    </section>

    <!-- Requirements -->
    <section class="col-12 col-padding">
      <span class="fw-700 is-inline-block mb-2">Requirements</span>
      <pre class="long-description bullet-list" v-html="request.requirements"></pre>
    </section>

    <section class="col-12 col-padding">
      <span class="fw-700 is-inline-block mb-2">Internal Requirements</span>
      <pre class="long-description bullet-list" v-html="request.internalRequirements"></pre>
    </section>

    <!-- Incentive -->
    <section class="col-12 col-padding" v-if="request.incentive">
      <span class="fw-700 is-inline-block mb-2">Plus </span>
      <span class="fw-400 ml-2"> {{ currency(request.incentive) }}</span>
      <pre class="long-description">{{ request.incentiveDescription }} </pre>
    </section>

    <!-- Break -->
    <section class="col-12 col-padding">
      <span class="fw-700 mr-2">Break</span>
      <span class="fw-400">{{ request.durationBreak }}</span>
      <span v-if="request.breakIsPaid" class="fw-400">
        | {{ "Break paid" }}</span>
    </section>
  </div>
</template>
<script lang="ts">
import { showAlertError } from "@/utils/toast";
import { currency, dateMonth } from '@/utils/filters';
import {
  increaseWorkersQuantityByOne,
  reduceWorkersQuantityByOne,
  updateAgencyRequestIsAsap,
  updateAgencyPunchCardVisibilityStatusInApp
} from "@/api/agencyRequestApi";
import {
  DurationTerm,
  DurationTermLabels,
  EmploymentTypeLabels,
  RequestStatus
} from "@/constants/enums";
export default {
  props: ["request"],
  computed: {
    DurationTerm: () => DurationTerm,
    DurationTermLabels: () => DurationTermLabels,
    EmploymentTypeLabels: () => EmploymentTypeLabels,
    RequestStatus: () => RequestStatus
  },
  data() {
    return {
      isLoading: false,
      localRequest: JSON.parse(JSON.stringify(this.request))
    };
  },
  watch: {
    request: {
      handler(newVal) {
        this.localRequest = JSON.parse(JSON.stringify(newVal));
      },
      deep: true
    }
  },
  components: {
    Skills: () => import("../agency_request/AgencyRequestSkills.vue"),
    AgencyShift: () => import("../agency_request/AgencyShiftDetail.vue")
  },
  methods: {
    currency,
    dateMonth,
    onIncreaseWorkersQuantity() {
      this.isLoading = true;
      increaseWorkersQuantityByOne(this.request.id)
        .then(() => {
          this.isLoading = false;
          // Emit event to refresh request data from API to get updated status
          this.$emit('refreshRequest');
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    onReduceWorkersQuantity() {
      this.isLoading = true;
      reduceWorkersQuantityByOne(this.request.id)
        .then(() => {
          this.isLoading = false;
          // Emit event to refresh request data from API to get updated status
          this.$emit('refreshRequest');
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    onToggleIsAsap() {
      this.isLoading = true;
      updateAgencyRequestIsAsap(this.request.id)
        .then(() => {
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    onTogglePunchCardVisibility() {
      this.isLoading = true;
      updateAgencyPunchCardVisibilityStatusInApp(this.request.id)
        .then(() => {
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    }
  }
};
</script>
<style lang="scss">
.bullet-list {
  &>ul {
    list-style: inside;
  }
}
</style>
