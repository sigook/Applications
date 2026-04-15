<template>
  <div>
    <b-tabs v-model="tab" :destroy-on-hide="true" type="is-toggle">
      <b-tab-item value="0" label="Subcontractors" :visible="!isUsaAgency">
        <subcontractors-report></subcontractors-report>
      </b-tab-item>
      <b-tab-item value="1" label="Payment" :visible="!isUsaAgency">
        <payment-report></payment-report>
      </b-tab-item>
      <b-tab-item value="2" label="T4" :visible="!isUsaAgency">
        <t4-report></t4-report>
      </b-tab-item>
      <b-tab-item value="3" label="CRA Payroll" :visible="!isUsaAgency">
        <cra-payroll-report></cra-payroll-report>
      </b-tab-item>
      <b-tab-item value="4" label="Hours Worked">
        <hours-worked-report></hours-worked-report>
      </b-tab-item>
    </b-tabs>
  </div>
</template>
<script lang="ts">
import { mapStores } from 'pinia';
import { useAgencyStore } from '@/stores/agency';
export default {
  data() {
    return {
      tab: null
    };
  },
  components: {
    SubcontractorsReport: () => import("@/components/agency_accounting/SubcontractorsReport.vue"),
    HoursWorkedReport: () => import("@/components/agency_accounting/HoursWorkedReport.vue"),
    T4Report: () => import("@/components/agency_accounting/T4.vue"),
    CraPayrollReport: () => import("@/components/agency_accounting/CRAPayroll.vue"),
    PaymentReport: () => import("@/components/agency_accounting/PaymentReport.vue")
  },
  created() {
    if (this.isUsaAgency) {
      this.tab = '4';
    } else {
      this.tab = this.$route.query.tab || '0';
    }
  },
  computed: {
    ...mapStores(useAgencyStore),
    isUsaAgency() {
      return this.agencyStore.usaAgency;
    }
  },
  watch: {
    tab(tab) {
      this.$router.push({
        path: '/accounting/reports',
        query: {
          tab: tab
        }
      });
    }
  }
}
</script>