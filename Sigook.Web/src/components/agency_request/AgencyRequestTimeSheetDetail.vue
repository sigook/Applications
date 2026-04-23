<template>
  <div class="p-3">
    <h2 class="text-center main-title"> {{ dateMonth(editableDay.day) }} </h2>

    <div class="text-center">
      <div class="container-worker-report">
        <h3 class="fz-0">Worker Report</h3>
        <div class="d-flex space-between">
          <div class="pl-2 pr-2">
            <span class="fz-2 fw-700 d-block">Clock In</span>
            <span v-if="editableDay.clockIn">{{ dateHHmm(editableDay.clockIn) }}</span>
            <span v-else class="fz-1">No reported</span>

          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 fw-700 d-block">Clock Out</span>
            <span v-if="editableDay.clockOut">{{ dateHHmm(editableDay.clockOut) }}</span>
            <span v-else class="fz-1">No reported</span>
          </div>
          <div class="pl-2 pr-2">
            <span class="fz-2 fw-700 d-block">Hours</span>
            <span v-if="editableDay.clockOut && editableDay.totalHours">
              {{ hour(editableDay.totalHours) }}
            </span>
            <span v-else class="fz-1">No reported</span>
          </div>
        </div>
      </div>
    </div>

    <div class="form-100">
      <label>{{ "Hours Approved" }}</label>
      <span>{{ editableDay.totalHoursApproved }}</span>
    </div>

    <div class="form-50">
      <label>{{ "Missing Hours" }}</label>
      <span>{{ editableDay.missingHours }}</span>
    </div>

    <div class="form-50">
      <label>{{ "Missing Hours" }} {{ "Over time" }}</label>
      <span>{{ editableDay.missingHoursOvertime }}</span>
    </div>

    <div class="form-50">
      <label>Missing Worker Rate</label>
      <span>{{ editableDay.missingRateWorker }}</span>
    </div>

    <div class="form-50">
      <label>Missing Agency Rate</label>
      <span>{{ editableDay.missingRateAgency }}</span>
    </div>

    <div class="form-50">
      <label>Deductions</label>
      <span>{{ editableDay.deductionsOthers }}</span>
    </div>

    <div class="form-50">
      <label>Description</label>
      <span>{{ editableDay.deductionsOthersDescription }}</span>
    </div>

    <div class="form-50">
      <label>Bonus or others</label>
      <span>{{ editableDay.bonusOrOthers }}</span>
    </div>

    <div class="form-50">
      <label>Description</label>
      <span>{{ editableDay.bonusOrOthersDescription }}</span>
    </div>

    <div class="form-50">
      <label>Reimbursements</label>
      <span>{{ editableDay.reimbursements }}</span>
    </div>

    <div class="form-50">
      <label>Description</label>
      <span>{{ editableDay.reimbursementsDescription }}</span>
    </div>

    <div class="form-100">
      <label>{{ "Comments" }}</label>
      <span>{{ editableDay.comment }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { dateMonth, dateHHmm, hour } from '@/utils/filters';

defineProps<{ editableDay: any }>();
</script>
