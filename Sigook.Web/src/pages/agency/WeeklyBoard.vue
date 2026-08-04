<template>
  <div>
    <admin-weekly-board v-if="isAdmin" />
    <recruiter-weekly-board v-else />
  </div>
</template>

<script setup lang="ts">
import { defineAsyncComponent } from 'vue';
import { useAdmin } from '@/composables/useAdmin';

const AdminWeeklyBoard = defineAsyncComponent(() => import('@/components/weekly_board/AdminWeeklyBoard.vue'));
const RecruiterWeeklyBoard = defineAsyncComponent(() => import('@/components/weekly_board/RecruiterWeeklyBoard.vue'));

const { isAdmin } = useAdmin();
</script>
