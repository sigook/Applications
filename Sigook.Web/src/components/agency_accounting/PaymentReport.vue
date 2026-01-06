<template>
  <div>
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-table :data="rows" narrowed hoverable :mobile-cards="false" :loading="isLoading" paginated backend-pagination
          backend-sorting pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
          :current-page.sync="serverParams.pageIndex" @page-change="onPageChange">
          <template v-slot:empty>
            <p class="container text-center">No records available</p>
          </template>
          <template>
            <b-table-column field="weekEnding" label="Payment Date" v-slot="props">
              {{ props.row.weekEnding | date }}
            </b-table-column>
            <b-table-column field="numberOfPayStubs" label="PayStubs" v-slot="props">
              {{ props.row.numberOfPayStubs }}
            </b-table-column>
            <b-table-column field="totalNet" label="Total Net" v-slot="props">
              {{ props.row.totalNet | currency }}
            </b-table-column>
            <b-table-column field="actions" v-slot="props">
              <b-tooltip label="Download" type="is-dark" position="is-top">
                <b-button type="is-success" outlined rounded icon-right="file-excel"
                  :loading="props.row.reportDownloading" @click="downloadWeeklyPayrollReport(props.row)">
                </b-button>
              </b-tooltip>
            </b-table-column>
          </template>
        </b-table>
      </div>
    </div>
  </div>
</template>
<script>
import download from '@/mixins/downloadFileMixin';

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
  mixins: [download],
  created() {
    this.getReport();
  },
  methods: {
    onPageChange(page) {
      this.serverParams.pageIndex = page;
      this.getReport();
    },
    getReport() {
      this.isLoading = true;
      this.$store.dispatch('agency/getPaymentReport', this.serverParams)
        .then(response => {
          this.rows = response.items.map(i => ({ ...i, actions: null, reportDownloading: false }));
          console.log(response);
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    downloadWeeklyPayrollReport(row) {
      row.reportDownloading = true;
      this.$store.dispatch('agency/downloadWeeklyPayrollReport', row.displayWeekEnding)
        .then(response => {
          row.reportDownloading = false;
          this.downloadFile(response, `Payment_${row.displayWeekEnding}`);
        })
        .catch(error => {
          row.reportDownloading = false;
          this.showAlertError(error);
        });
    }
  }
}
</script>