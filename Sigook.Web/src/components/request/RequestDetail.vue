<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <!-- Highlight -->
    <div class="col-12 col-padding highlight-content" v-if="request">
      <div class="item">
        <span class="fw-700">Rate / Salary</span>
        <p>{{ (request.workerRate || request.workerSalary) | currency }}</p>
      </div>
      <div class="item">
        <span class="fw-700">Term</span>
        <p>{{ request.durationTerm | splitCapital }}</p>
      </div>
      <div class="item">
        <span class="fw-700">Employment Type</span>
        <p>{{ request.employmentType | splitCapital }}</p>
      </div>
      <div class="item">
        <span class="fw-700">Start
          <span
            v-if="((request.status === $statusFinalized || request.status === $statusCancelled) && request.durationTerm === $longTerm) || request.durationTerm === $shortTerm">
            / Finish</span>
        </span>
        <p>
          {{ request.startAt | dateMonth }}
          <span class="fz-0" v-if="request.durationTerm !== $longTerm">
            / {{ request.finishAt | dateMonth }}</span>
          <span class="fz-0"
            v-if="(request.status === $statusFinalized || request.status === $statusCancelled) && request.durationTerm === $longTerm">
            / {{ request.finishAt | dateMonth }}
          </span>
        </p>
      </div>
      <div class="item worker-options">
        <span class="fw-700">Workers</span>
        <p class="hover-actions">
          <span class="mr-1 fz-0">{{ request.workersQuantityWorking }} /
            {{ request.workersQuantity }}</span>
          <button v-if="request.canEdit" @click="increaseWorkersQuantityByOne()"
            class="btn-icon-sm btn-icon-circle-plus bg-transparent relative actions">
            add
          </button>
          <button @click="reduceWorkersQuantityByOne"
            class="btn-icon-sm btn-icon-circle-minus bg-transparent relative actions"
            v-if="request.canEdit && request.workersQuantityWorking < request.workersQuantity && request.workersQuantity !== 1">
            reduce
          </button>
        </p>
      </div>
      <div class="item">
        <span class="fw-700">Is Asap</span>
        <p>
          <b-checkbox v-model="request.isAsap" @input="updateAgencyRequestIsAsap()"></b-checkbox>
        </p>
      </div>
      <div class="item">
        <span class="fw-700">Visible Punch Card</span>
        <p class="w-50">
          <b-checkbox v-model="request.punchCardOptionEnabled" @input="updatePunchCardIsVisibleInApp()"></b-checkbox>
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
      <span class="fw-400 ml-2"> {{ request.incentive | currency }}</span>
      <pre class="long-description">{{ request.incentiveDescription }} </pre>
    </section>

    <!-- Break -->
    <section class="col-12 col-padding">
      <span class="fw-700 mr-2">Break</span>
      <span class="fw-400">{{ request.durationBreak }}</span>
      <span v-if="request.breakIsPaid" class="fw-400">
        | {{ $t("RequestBreakPaid") }}</span>
    </section>
  </div>
</template>
<script>
import toastMixin from "@/mixins/toastMixin";
export default {
  props: ["request"],
  data() {
    return {
      isLoading: false,
    };
  },
  mixins: [toastMixin],
  components: {
    Skills: () => import("../agency_request/AgencyRequestSkills"),
    AgencyShift: () => import("../agency_request/AgencyShiftDetail"),
  },
  methods: {
    increaseWorkersQuantityByOne() {
      this.isLoading = true;
      this.$store
        .dispatch("agency/increaseWorkersQuantityByOne", this.request.id)
        .then(() => {
          this.isLoading = false;
          this.request.workersQuantity = this.request.workersQuantity + 1;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    reduceWorkersQuantityByOne() {
      this.isLoading = true;
      this.$store
        .dispatch("agency/reduceWorkersQuantityByOne", this.request.id)
        .then(() => {
          this.isLoading = false;
          this.request.workersQuantity = this.request.workersQuantity - 1;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    updateAgencyRequestIsAsap() {
      this.isLoading = true;
      this.$store
        .dispatch("agency/updateAgencyRequestIsAsap", this.request.id)
        .then(() => {
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    updatePunchCardIsVisibleInApp() {
      this.isLoading = true;
      this.$store
        .dispatch(
          "agency/updateAgencyPunchCardVisibilityStatusInApp",
          this.request.id
        )
        .then(() => {
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
  },
};
</script>
<style lang="scss">
.bullet-list {
  &>ul {
    list-style: inside;
  }
}
</style>
