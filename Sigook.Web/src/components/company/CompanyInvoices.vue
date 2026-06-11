<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="invoiceNumberId"
        v-model:current-page="serverParams.pageIndex">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
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
import { downloadPDF } from '@/utils/downloadFile';
import { fetchInvoicePdf } from "@/api/downloadApi";
import { getCompanyInvoice } from "@/api/companyApi";

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const invoiceId = ref<any>(null);
const invoiceNumber = ref<any>(null);
const showModalCompanyInvoicePay = ref(false);
const showModalPaymentSupport = ref(false);
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
    .catch((e: any) => {
      isLoading.value = false;
      showAlertError(e);
    });
}

function downloadInvoicePdf(item: any) {
  isLoading.value = true;
  fetchInvoicePdf(item.id)
    .then((response: any) => {
      isLoading.value = false;
      downloadPDF(response, "Invoice_" + item.numberId);
    })
    .catch((error: any) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function openCompanyInvoicePay(id: any, num: any) {
  invoiceId.value = id;
  showModalCompanyInvoicePay.value = true;
  invoiceNumber.value = num;
}

function openPaymentSupportModal(id: any, num: any) {
  showModalPaymentSupport.value = true;
  invoiceId.value = id;
  invoiceNumber.value = num;
}

function onModalCompanyInvoicePayClose(change: boolean) {
  showModalCompanyInvoicePay.value = false;
  if (change) {
    onGetCompanyInvoice();
  }
}

onGetCompanyInvoice();

defineExpose({ downloadInvoicePdf, openCompanyInvoicePay, openPaymentSupportModal, onModalCompanyInvoicePayClose });
</script>
