<template>
  <div class="white-container-mobile">
    <invoices></invoices>
  </div>
</template>

<script>
export default {
  components: {
    Invoices: () => import("../../components/company/CompanyInvoices")
  },
  data() {
    return {
      
    }
  },
  methods: {
  },
  created() {
    
  }
}
</script>
