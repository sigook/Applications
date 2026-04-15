<template>
  <div class="white-container-mobile">
    <invoices></invoices>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
export default {
  components: {
    Invoices: defineAsyncComponent(() => import("../../components/company/CompanyInvoices.vue"))
  },
  data() {
    return {
      
    }
  },
  methods: {
  },
  created() {
    // no-op
  }
}
</script>
