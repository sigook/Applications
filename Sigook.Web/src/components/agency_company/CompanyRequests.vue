<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="mt-3">
      <table-requests @onDataLoading="(value) => isLoading = value" :companyId="company.companyId" />
    </div>
  </div>
</template>
<script lang="ts">
import toastMixin from "@/mixins/toastMixin";
export default {
  props: ['company'],
  mixins: [toastMixin],
  data() {
    return {
      isLoading: true,
    }
  },
  components: {
    TableRequests: () => import("../../components/agency_request/TableRequests.vue"),
  }
}
</script>