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
<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAgencyStore } from '@/stores/agency';
import SubcontractorsReport from '@/components/agency_accounting/SubcontractorsReport.vue';
import HoursWorkedReport from '@/components/agency_accounting/HoursWorkedReport.vue';
import T4Report from '@/components/agency_accounting/T4.vue';
import CraPayrollReport from '@/components/agency_accounting/CRAPayroll.vue';
import PaymentReport from '@/components/agency_accounting/PaymentReport.vue';

const route = useRoute();
const router = useRouter();
const agencyStore = useAgencyStore();

const tab = ref<string | null>(null);
const isUsaAgency = computed(() => agencyStore.agency.usaAgency);

if (isUsaAgency.value) {
  tab.value = '4';
} else {
  tab.value = (route.query.tab as string) || '0';
}

watch(tab, (value) => {
  router.push({
    path: '/accounting/reports',
    query: {
      tab: value,
    },
  });
});
</script>
