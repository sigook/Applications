<template>
  <div class="container-flex" v-if="request">
    <section class="col-md-9 col-sm-12 section-left mt-2">
      <!-- Highlight -->
      <div class="highlight-content space-between">
        <div class="item">
          <span class="fw-700">Created</span>
          <p>{{ request.createdAt | dateFromNow }}</p>
        </div>
        <div class="item">
          <span class="fw-700">Rate / Salary</span>
          <p>{{ (request.agencyRate || request.workerSalary) | currency }}</p>
        </div>
        <div class="item">
          <span class="fw-700">Term</span>
          <p>{{ request.durationTerm | splitCapital }}</p>
        </div>
        <div class="item worker-options">
          <span class="fw-700">Workers</span>
          <p class="hover-actions">
            <span class="mr-1 fz-0">
              {{ request.workersQuantityWorking }} / {{ request.workersQuantity }}
            </span>
          </p>
        </div>
      </div>

      <!-- Role -->
      <section class="mt-3">
        <span class="fw-700 mr-2">Role</span>
        <span class="fw-400" v-if="request.jobPositionRate">{{ request.jobPositionRate.value }}</span>
        <span v-if="request.displayShift" class="request-shift-container"><b class="fw-700 ">Shift</b>
          <agency-shift class="ml-3" :requestId="request.id" :displayShift="request.displayShift" />
        </span>
      </section>

      <!-- Detail -->
      <section class="mt-5">
        <span class="fw-700 is-inline-block mb-2">Description</span>
        <pre class="long-description" v-html="request.description"></pre>
      </section>

      <section class="mt-5">
        <span class="fw-700 is-inline-block mb-2">Responsibilities</span>
        <pre class="long-description" v-html="request.responsibilities"></pre>
      </section>

      <!-- Requirements -->
      <section class="mt-5">
        <span class="fw-700 is-inline-block mb-2">Requirements</span>
        <pre class="long-description" v-html="request.requirements"></pre>
      </section>

      <!-- Incentive -->
      <section class="mt-5" v-if="request.incentive">
        <span class="fw-700 is-inline-block mb-2">Plus </span>
        <span class="fw-400 ml-2"> {{ request.incentive | currency }}</span>
        <pre class="long-description">{{ request.incentiveDescription }} </pre>
      </section>

      <!-- Break -->
      <section class="mt-5">
        <span class="fw-700 mr-2">Break</span>
        <span class="fw-400">{{ request.durationBreak }}</span>
        <span v-if="request.breakIsPaid" class="fw-400">
          | {{ $t("RequestBreakPaid") }}</span>
      </section>
    </section>
    <aside class="col-md-3 col-sm-12 section-right">
      <location :jobLocation="request.jobLocation" />
    </aside>
  </div>
</template>
<script>
export default {
  props: ["request"],
  components: {
    Location: () => import("../request/RequestLocation"),
    AgencyShift: () => import("../agency_request/AgencyShiftDetail"),
  },
};
</script>
