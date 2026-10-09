<template>
  <div>
    <admin-weekly-board v-if="isAdmin" />
    <recruiter-weekly-board v-else />
  </div>
</template>

<script setup lang="ts">
import { defineAsyncComponent } from 'vue';
import { useAdmin } from '@/shared/composables/useAdmin';

const AdminWeeklyBoard = defineAsyncComponent(() => import('@/modules/agency/recruiting/weekly-board/components/AdminWeeklyBoard.vue'));
const RecruiterWeeklyBoard = defineAsyncComponent(() => import('@/modules/agency/recruiting/weekly-board/components/RecruiterWeeklyBoard.vue'));

const { isAdmin } = useAdmin();
</script>
