<template>
  <div class="p-3">
    <h2 class="has-text-centered main-title"> {{ dateMonth(editableDay.day) }} </h2>

    <div class="has-text-centered">
      <div class="container-worker-report">
        <h3 class="fz-0">Worker Report</h3>
        <div class="is-flex is-justify-content-space-between">
          <div class="pl-2 pr-2">
            <span class="fz-2 has-text-weight-bold is-block">Clock In</span>
            <span v-if="editableDay.clockIn">{{ dateHHmm(editableDay.clockIn) }}</span>
            <span v-else class="fz-1">No reported</span>

          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 has-text-weight-bold is-block">Clock Out</span>
            <span v-if="editableDay.clockOut">{{ dateHHmm(editableDay.clockOut) }}</span>
            <span v-else class="fz-1">No reported</span>
          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 has-text-weight-bold is-block">Hours</span>
            <span v-if="editableDay.clockOut && editableDay.totalHours">
              {{ hour(editableDay.totalHours) }}
            </span>
            <span v-else class="fz-1">No reported</span>
          </div>
        </div>
      </div>
    </div>

    <div class="columns is-multiline">
      <div class="column is-12">
        <label class="is-block">{{ "Hours Approved" }}</label>
        <span>{{ editableDay.totalHoursApproved }}</span>
      </div>

      <div class="column is-6-mobile is-6">
        <label class="is-block">{{ "Missing Hours" }}</label>
        <span>{{ editableDay.missingHours }}</span>
      </div>

      <div class="column is-6-mobile is-6">
        <label class="is-block">{{ "Missing Hours" }} {{ "Over time" }}</label>
        <span>{{ editableDay.missingHoursOvertime }}</span>
      </div>

      <div class="column is-12">
        <label class="is-block">{{ "Comments" }}</label>
        <span>{{ editableDay.comment }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { dateMonth, dateHHmm, hour } from '@/utils/filters';

defineProps<{ editableDay: any }>();
</script>
