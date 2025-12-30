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
<script>
export default {
  props: ['agency'],
  data() {
    return {
      isLoading: true,
      tableConfig: {
        showMyOrdersCheckbox: false,
        showQuickActions: false,
        enableCheckable: false,
        showRecruiterModal: false,
        showSalesRepColumn: false,
        showNotesColumn: false,
      }
    }
  },
  components: {
    TableRequests: () => import("../../components/agency_request/TableRequests"),
  }
}
</script>
