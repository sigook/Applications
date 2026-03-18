<template>
  <div class="white-container-mobile">
    <invoices></invoices>
  </div>
</template>

<script lang="ts">
export default {
  components: {
    Invoices: () => import("../../components/company/CompanyInvoices.vue")
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
