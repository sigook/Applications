<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <!-- Highlight -->
    <div class="highlight-content" v-if="props.request">
      <div class="item">
        <span class="fw-bold">Created</span>
        <p>{{ dateFromNow(props.request.createdAt) }}</p>
      </div>
      <div class="item">
        <span class="fw-bold">Rate / Salary</span>
        <p>{{ currency(props.request.workerRate || props.request.workerSalary) }}</p>
      </div>
      <div class="item">
        <span class="fw-bold">Term</span>
        <p>{{ splitCapital(props.request.durationTerm) }}</p>
      </div>
      <div class="item">
        <span class="fw-bold">Start
          <span
            v-if="((props.request.status === $statusFilled || props.request.status === $statusCancelled) && props.request.durationTerm === $longTerm) || props.request.durationTerm === $shortTerm">
            / Finish</span>
        </span>
        <p>
          {{ dateMonth(props.request.startAt) }}
          <span class="fz-0" v-if="props.request.durationTerm !== $longTerm">
            / {{ dateMonth(props.request.finishAt) }}</span>
          <span class="fz-0"
            v-if="(props.request.status === $statusFilled || props.request.status === $statusCancelled) && props.request.durationTerm === $longTerm">
            / {{ dateMonth(props.request.finishAt) }}
          </span>
        </p>
      </div>
      <div class="item worker-options">
        <span class="fw-bold">Spots</span>
        <p class="hover-actions">
          <span class="me-1 fz-0">{{ props.request.workersQuantity }}</span>
        </p>
      </div>
    </div>

    <!-- Role -->
    <section class="mt-3">
      <span class="fw-bold me-2">Role</span>
      <span class="fw-normal">{{ props.request.jobPosition }}</span>
    </section>

    <!-- Detail -->
    <section class="mt-5">
      <span class="fw-bold is-inline-block mb-2">Description</span>
      <pre class="long-description" v-html="props.request.description"></pre>
    </section>

    <!-- Requirements -->
    <section class="mt-5">
      <span class="fw-bold is-inline-block mb-2">Requirements</span>
      <pre class="long-description" v-html="props.request.requirements"></pre>
    </section>

    <!-- Incentive -->
    <section class="mt-5" v-if="props.request.incentive">
      <span class="fw-bold is-inline-block mb-2">Plus </span>
      <span class="fw-normal ms-2"> {{ currency(props.request.incentive) }}</span>
      <pre class="long-description">{{ props.request.incentiveDescription }} </pre>
    </section>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { currency, dateFromNow, splitCapital, dateMonth } from '@/utils/filters';

const props = defineProps<{ request?: any }>();

const isLoading = ref(false);
</script>
