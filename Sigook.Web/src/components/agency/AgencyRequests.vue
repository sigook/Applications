<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="mt-3">
      <table-requests
        @onDataLoading="(value) => isLoading = value"
        :agencyId="agency.id"
        :config="tableConfig" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import TableRequests from "../../components/agency_request/TableRequests.vue";

defineProps<{ agency: any }>();

const isLoading = ref(true);
const tableConfig = ref({
  showMyRequestsCheckbox: false,
  showQuickActions: false,
  enableCheckable: false,
  showRecruiterModal: false,
  showSalesRepColumn: false,
  showNotesColumn: false,
});
</script>
