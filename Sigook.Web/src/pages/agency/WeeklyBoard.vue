<template>
  <div>
    <admin-weekly-board v-if="isAccountingManager" />
    <recruiter-weekly-board v-else />
  </div>
</template>

<script setup lang="ts">
import { defineAsyncComponent } from 'vue';
import { useAccountingAdmin } from '@/composables/useAccountingAdmin';

const AdminWeeklyBoard = defineAsyncComponent(() => import('@/components/weekly_board/AdminWeeklyBoard.vue'));
const RecruiterWeeklyBoard = defineAsyncComponent(() => import('@/components/weekly_board/RecruiterWeeklyBoard.vue'));

const { isAccountingManager } = useAccountingAdmin();
</script>
