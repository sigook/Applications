<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <SigookGrid :fetch="getCompanyInvoice" v-model:params="serverParams"
        @update:loading="(value) => isLoading = value">
        <b-table-column label="Invoice Number" field="invoiceNumberId" v-slot="props">
          <span>{{ props.row.invoiceNumber }}</span>
        </b-table-column>
        <b-table-column label="Created At" field="createdAt" v-slot="props">
          <span>{{ datetime(props.row.createdAt) }}</span>
        </b-table-column>
        <b-table-column label="Week Ending" field="weekEnding" v-slot="props">
          <span v-if="props.row.weekEnding">{{ date(props.row.weekEnding) }}</span>
          <span v-else>N/A</span>
        </b-table-column>
        <b-table-column label="Total" field="totalNet" v-slot="props">
          <span>{{ currency(props.row.totalNet) }}</span>
        </b-table-column>
      </SigookGrid>
    </div>
  </div>
</template>


<script setup lang="ts">
import { ref } from 'vue';
import { datetime, date, currency } from '@/shared/format';
import { getCompanyInvoice } from '@/modules/company/invoices/api';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import type { CompanyInvoiceFilter } from '@/modules/company/invoices/types';

const isLoading = ref(true);
const serverParams = ref<CompanyInvoiceFilter>({
  sortBy: 0,
  isDescending: false,
  pageIndex: 1,
  pageSize: 30,
});
</script>
