<template>
  <div class="columns is-multiline detail-split" v-if="request">
    <section class="column is-9 section-left detail-split-main mt-2">
      <!-- Highlight -->
      <div class="highlight-content is-justify-content-space-between">
        <div class="item">
          <span class="has-text-weight-bold">Created</span>
          <p>{{ dateFromNow(request.createdAt) }}</p>
        </div>
        <div class="item">
          <span class="has-text-weight-bold">Rate / Salary</span>
          <p>{{ currency(request.agencyRate || request.workerSalary) }}</p>
        </div>
        <div class="item">
          <span class="has-text-weight-bold">Term</span>
          <p>{{ DurationTermLabels[request.durationTerm] }}</p>
        </div>
        <div class="item worker-options">
          <span class="has-text-weight-bold">Workers</span>
          <p class="hover-actions">
            <span class="mr-1 fz-0">
              {{ request.workersQuantityWorking }} / {{ request.workersQuantity }}
            </span>
          </p>
        </div>
      </div>

      <!-- Role -->
      <section class="mt-3">
        <span class="has-text-weight-bold mr-2">Role</span>
        <span class="has-text-weight-normal" v-if="request.jobPositionRate">{{ request.jobPositionRate.value }}</span>
        <span v-if="request.displayShift" class="request-shift-container"><b class="has-text-weight-bold ">Shift</b>
          <agency-shift class="ml-3" :requestId="request.id" :displayShift="request.displayShift"
            :fetchShift="getRequestShift" />
        </span>
      </section>

      <!-- Detail -->
      <section class="mt-5">
        <span class="has-text-weight-bold is-inline-block mb-2">Description</span>
        <pre class="long-description" v-html="request.description"></pre>
      </section>

      <section class="mt-5">
        <span class="has-text-weight-bold is-inline-block mb-2">Responsibilities</span>
        <pre class="long-description" v-html="request.responsibilities"></pre>
      </section>

      <!-- Requirements -->
      <section class="mt-5">
        <span class="has-text-weight-bold is-inline-block mb-2">Requirements</span>
        <pre class="long-description" v-html="request.requirements"></pre>
      </section>

      <!-- Incentive -->
      <section class="mt-5" v-if="request.incentive">
        <span class="has-text-weight-bold is-inline-block mb-2">Plus </span>
        <span class="has-text-weight-normal ml-2"> {{ currency(request.incentive) }}</span>
        <pre class="long-description">{{ request.incentiveDescription }} </pre>
      </section>

      <!-- Break -->
      <section class="mt-5">
        <span class="has-text-weight-bold mr-2">Break</span>
        <span class="has-text-weight-normal">{{ request.durationBreak }}</span>
        <span v-if="request.breakIsPaid" class="has-text-weight-normal">
          | {{ "Break paid" }}</span>
      </section>
    </section>
    <aside class="column is-3 section-right detail-split-aside">
      <location :jobLocation="request.jobLocation" />
    </aside>
  </div>
</template>

<script setup lang="ts">
import { dateFromNow, currency } from '@/utils/filters';
import { DurationTermLabels } from "@/constants/enums";
import Location from "../request/RequestLocation.vue";
import AgencyShift from "../agency_request/AgencyShiftDetail.vue";
import { getRequestShift } from "@/api/companyApi";

defineProps<{ request: any }>();
</script>
