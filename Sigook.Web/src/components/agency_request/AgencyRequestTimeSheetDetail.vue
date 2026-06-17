<template>
  <div class="p-3">
    <h2 class="text-center main-title"> {{ dateMonth(editableDay.day) }} </h2>

    <div class="text-center">
      <div class="container-worker-report">
        <h3 class="fz-0">Worker Report</h3>
        <div class="d-flex justify-content-between">
          <div class="ps-2 pe-2">
            <span class="fz-2 fw-bold d-block">Clock In</span>
            <span v-if="editableDay.clockIn">{{ dateHHmm(editableDay.clockIn) }}</span>
            <span v-else class="fz-1">No reported</span>

          </div>
          <div class="ps-2 pe-2">
            <span class="fz-2 fw-bold d-block">Clock Out</span>
            <span v-if="editableDay.clockOut">{{ dateHHmm(editableDay.clockOut) }}</span>
            <span v-else class="fz-1">No reported</span>
          </div>
          <div class="ps-2 pe-2">
            <span class="fz-2 fw-bold d-block">Hours</span>
            <span v-if="editableDay.clockOut && editableDay.totalHours">
              {{ hour(editableDay.totalHours) }}
            </span>
            <span v-else class="fz-1">No reported</span>
          </div>
        </div>
      </div>
    </div>

    <div class="container-flex">
      <div class="col-12 col-padding">
        <label class="d-block">{{ "Hours Approved" }}</label>
        <span>{{ editableDay.totalHoursApproved }}</span>
      </div>

      <div class="col-6 col-padding">
        <label class="d-block">{{ "Missing Hours" }}</label>
        <span>{{ editableDay.missingHours }}</span>
      </div>

      <div class="col-6 col-padding">
        <label class="d-block">{{ "Missing Hours" }} {{ "Over time" }}</label>
        <span>{{ editableDay.missingHoursOvertime }}</span>
      </div>

      <div class="col-6 col-padding">
        <label class="d-block">Missing Worker Rate</label>
        <span>{{ editableDay.missingRateWorker }}</span>
      </div>

      <div class="col-6 col-padding">
        <label class="d-block">Missing Agency Rate</label>
        <span>{{ editableDay.missingRateAgency }}</span>
      </div>

      <div class="col-6 col-padding">
        <label class="d-block">Deductions</label>
        <span>{{ editableDay.deductionsOthers }}</span>
      </div>

      <div class="col-6 col-padding">
        <label class="d-block">Description</label>
        <span>{{ editableDay.deductionsOthersDescription }}</span>
      </div>

      <div class="col-6 col-padding">
        <label class="d-block">Bonus or others</label>
        <span>{{ editableDay.bonusOrOthers }}</span>
      </div>

      <div class="col-6 col-padding">
        <label class="d-block">Description</label>
        <span>{{ editableDay.bonusOrOthersDescription }}</span>
      </div>

      <div class="col-6 col-padding">
        <label class="d-block">Reimbursements</label>
        <span>{{ editableDay.reimbursements }}</span>
      </div>

      <div class="col-6 col-padding">
        <label class="d-block">Description</label>
        <span>{{ editableDay.reimbursementsDescription }}</span>
      </div>

      <div class="col-12 col-padding">
        <label class="d-block">{{ "Comments" }}</label>
        <span>{{ editableDay.comment }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { dateMonth, dateHHmm, hour } from '@/utils/filters';

defineProps<{ editableDay: any }>();
</script>
