<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable paginated pagination-size="is-small" backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="invoiceNumberId"
        v-model:current-page="serverParams.pageIndex">
        <template v-slot:empty>
          <p class="container has-text-centered">No records available</p>
        </template>
        <template>
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
        </template>
      </b-table>
    </div>
  </div>
</template>


<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from "@/utils/toast";
import { datetime, date, currency } from '@/utils/filters';
import { getCompanyInvoice } from "@/api/companyApi";

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const serverParams = reactive<any>({
  sortBy: 0,
  isDescending: false,
  pageIndex: 1,
  pageSize: 30,
});

function onGetCompanyInvoice() {
  isLoading.value = true;
  getCompanyInvoice(serverParams)
    .then((response: any) => {
      rows.value = response.items;
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch((e: unknown) => {
      isLoading.value = false;
      showAlertError(e);
    });
}

onGetCompanyInvoice();
</script>
