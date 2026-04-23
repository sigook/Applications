<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="mt-3">
      <table-requests @onDataLoading="(value) => isLoading = value" :companyId="company.companyId" />
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import TableRequests from '../../components/agency_request/TableRequests.vue';

defineProps<{ company: any }>();

const isLoading = ref(true);
</script>
