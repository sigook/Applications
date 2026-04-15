<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <!-- Highlight -->
    <div class="highlight-content" v-if="request">
      <div class="item">
        <span class="fw-700">Created</span>
        <p>{{ dateFromNow(request.createdAt) }}</p>
      </div>
      <div class="item">
        <span class="fw-700">Rate / Salary</span>
        <p>{{ currency(request.workerRate || request.workerSalary) }}</p>
      </div>
      <div class="item">
        <span class="fw-700">Term</span>
        <p>{{ splitCapital(request.durationTerm) }}</p>
      </div>
      <div class="item">
        <span class="fw-700">Start
          <span
            v-if="((request.status === $statusFilled || request.status === $statusCancelled) && request.durationTerm === $longTerm) || request.durationTerm === $shortTerm">
            / Finish</span>
        </span>
        <p>
          {{ dateMonth(request.startAt) }}
          <span class="fz-0" v-if="request.durationTerm !== $longTerm">
            / {{ dateMonth(request.finishAt) }}</span>
          <span class="fz-0"
            v-if="(request.status === $statusFilled || request.status === $statusCancelled) && request.durationTerm === $longTerm">
            / {{ dateMonth(request.finishAt) }}
          </span>
        </p>
      </div>
      <div class="item worker-options">
        <span class="fw-700">Spots</span>
        <p class="hover-actions">
          <span class="mr-1 fz-0">{{ request.workersQuantity }}</span>
        </p>
      </div>
    </div>

    <!-- Role -->
    <section class="mt-3">
      <span class="fw-700 mr-2">Role</span>
      <span class="fw-400">{{ request.jobPosition }}</span>
    </section>

    <!-- Detail -->
    <section class="mt-5">
      <span class="fw-700 is-inline-block mb-2">Description</span>
      <pre class="long-description" v-html="request.description"></pre>
    </section>

    <!-- Requirements -->
    <section class="mt-5">
      <span class="fw-700 is-inline-block mb-2">Requirements</span>
      <pre class="long-description" v-html="request.requirements"></pre>
    </section>

    <!-- Incentive -->
    <section class="mt-5" v-if="request.incentive">
      <span class="fw-700 is-inline-block mb-2">Plus </span>
      <span class="fw-400 ml-2"> {{ currency(request.incentive) }}</span>
      <pre class="long-description">{{ request.incentiveDescription }} </pre>
    </section>
  </div>
</template>
<script lang="ts">
import { currency, dateFromNow, splitCapital, dateMonth } from '@/utils/filters';
export default {
  props: ["request"],
  data() {
    return {
      isLoading: false
    };
  },
  methods: {
    currency,
    dateFromNow,
    splitCapital,
    dateMonth
  }
};
</script>
