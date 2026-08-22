<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Requests" :count="totalItems" :crumbs="moduleCrumbs" />
    <div>
      <table-requests @onDataLoading="(value) => isLoading = value" v-model:total-items="totalItems" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import TableRequests from '@/components/agency_request/TableRequests.vue';
import PageHeader from '@/components/PageHeader.vue';
import { useModuleBase } from '@/composables/useModuleBase';

const { moduleCrumbs } = useModuleBase();
const isLoading = ref(true);
const totalItems = ref(0);
</script>
