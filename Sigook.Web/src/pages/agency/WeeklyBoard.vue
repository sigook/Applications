<template>
  <div>
    <admin-weekly-board v-if="isPayrollManager" />
    <recruiter-weekly-board v-else />
  </div>
</template>

<script setup lang="ts">
import { defineAsyncComponent } from 'vue';
import { useBillingAdmin } from '@/composables/useBillingAdmin';

const AdminWeeklyBoard = defineAsyncComponent(() => import('@/components/weekly_board/AdminWeeklyBoard.vue'));
const RecruiterWeeklyBoard = defineAsyncComponent(() => import('@/components/weekly_board/RecruiterWeeklyBoard.vue'));

const { isPayrollManager } = useBillingAdmin();
</script>
