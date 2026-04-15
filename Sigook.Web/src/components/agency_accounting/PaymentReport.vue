<template>
  <div>
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-table :data="rows" narrowed hoverable :mobile-cards="false" :loading="isLoading" paginated backend-pagination
          backend-sorting pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
          v-model:current-page="serverParams.pageIndex" @page-change="onPageChange">
          <template v-slot:empty>
            <p class="container text-center">No records available</p>
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
<script lang="ts">
import { showAlertError } from "@/utils/toast";
import { downloadFile } from '@/utils/downloadFile';
import { date, currency } from '@/utils/filters';
import { getPaymentReport, downloadWeeklyPayrollReport } from "@/api/agencyReportApi";

export default {
  data() {
    return {
      isLoading: false,
      totalItems: 0,
      rows: [],
      serverParams: {
        pageIndex: 1,
        pageSize: 30
      }
    }
  },
  created() {
    this.getReport();
  },
  methods: {
    downloadFile,
    date,
    currency,
    onPageChange(page) {
      this.serverParams.pageIndex = page;
      this.getReport();
    },
    getReport() {
      this.isLoading = true;
      getPaymentReport(this.serverParams)
        .then((response) => {
          this.rows = response.items.map((i) => ({ ...i, actions: null, reportDownloading: false }));
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    onDownloadWeeklyPayrollReport(row) {
      row.reportDownloading = true;
      downloadWeeklyPayrollReport(row.displayWeekEnding)
        .then(response => {
          row.reportDownloading = false;
          this.downloadFile(response, `Payment_${row.displayWeekEnding}`);
        })
        .catch(error => {
          row.reportDownloading = false;
          showAlertError(error);
        });
    }
  }
}
</script>