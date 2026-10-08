<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="mt-3">
      <table-requests @onDataLoading="(value) => isLoading = value" :companyProfileId="company.id" />
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import TableRequests from '@/modules/agency/recruiting/requests/components/TableRequests.vue';
import type { AgencyCompanyProfileDetail } from '@/modules/agency/recruiting/clients/types';

defineProps<{ company: AgencyCompanyProfileDetail }>();

const isLoading = ref(true);
</script>
