<template>
  <div>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" :loading="isLoading" paginated pagination-size="is-small" backend-pagination
          backend-sorting pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
          v-model:current-page="serverParams.pageIndex" @page-change="onPageChange">
          <template v-slot:empty>
            <p class="container has-text-centered">No records available</p>
          </template>
          <template>
            <b-table-column field="weekEnding" label="Payment Date" v-slot="props">
              {{ date(props.row.weekEnding) }}
            </b-table-column>
            <b-table-column field="numberOfPayStubs" label="PayStubs" v-slot="props">
              {{ props.row.numberOfPayStubs }}
            </b-table-column>
            <b-table-column field="totalNet" label="Total Net" v-slot="props">
              {{ currency(props.row.totalNet) }}
            </b-table-column>
            <b-table-column field="actions" v-slot="props">
              <b-tooltip label="Download" type="is-dark" position="is-top" append-to-body>
                <b-button type="is-success" outlined rounded icon-right="file-excel"
                  :loading="props.row.reportDownloading" @click="onDownloadWeeklyPayrollReport(props.row)">
                </b-button>
              </b-tooltip>
            </b-table-column>
          </template>
        </b-table>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { downloadFile } from '@/utils/downloadFile';
import { date, currency } from '@/utils/filters';
import { getPaymentReport, downloadWeeklyPayrollReport } from "@/api/agencyReportApi";

const isLoading = ref(false);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const serverParams = ref({
  pageIndex: 1,
  pageSize: 30
});

function onPageChange(page: number) {
  serverParams.value.pageIndex = page;
  getReport();
}

function getReport() {
  isLoading.value = true;
  getPaymentReport(serverParams.value)
    .then((response) => {
      rows.value = response.items.map((i: any) => ({ ...i, actions: null, reportDownloading: false }));
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onDownloadWeeklyPayrollReport(row: any) {
  row.reportDownloading = true;
  downloadWeeklyPayrollReport(row.displayWeekEnding)
    .then(response => {
      row.reportDownloading = false;
      downloadFile(response, `Payment_${row.displayWeekEnding}`);
    })
    .catch(error => {
      row.reportDownloading = false;
      showAlertError(error);
    });
}

getReport();
</script>
