<template>
  <div>
    <b-table :data="rows" narrowed hoverable :mobile-cards="false" :loading="isLoading" paginated backend-pagination
      backend-sorting pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
      :current-page.sync="serverParams.pageIndex" @page-change="onPageChange">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="weekEnding" label="Week Ending" v-slot="props">
          {{ props.row.weekEnding | date }}
        </b-table-column>
        <b-table-column field="numberOfWorkers" label="Workers" v-slot="props">
          {{ props.row.numberOfWorkers }}
        </b-table-column>
        <b-table-column field="totalNet" label="Total" v-slot="props">
          {{ props.row.totalNet | currency }}
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-tooltip label="Download Report" type="is-dark" position="is-top">
            <b-button type="is-success" outlined rounded icon-right="file-excel" :loading="props.row.reportDownloading"
              @click="downloadSubcontractor(props.row)">
            </b-button>
          </b-tooltip>
        </b-table-column>
      </template>
    </b-table>
  </div>
</template>
<script>
import dayjs from "dayjs";
import download from "@/mixins/downloadFileMixin";

export default {
  data() {
    return {
      isLoading: false,
      totalItems: 0,
      rows: [],
      serverParams: {
        sortBy: 3,
        isDescending: true,
        pageIndex: 1,
        pageSize: 30
      }
    };
  },
  mixins: [download],
  methods: {
    onPageChange(page) {
      this.serverParams.pageIndex = page;
      this.getPayrollSubcontractors();
    },
    getPayrollSubcontractors() {
      this.isLoading = true;
      this.$store.dispatch("agency/getPayrollSubcontractors", this.serverParams)
        .then((response) => {
          this.rows = response.items.map(item => ({ ...item, actions: null, reportDownloading: false }));
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    downloadSubcontractor(subcontractor) {
      const weekEnding = dayjs(subcontractor.weekEnding).format('MM-DD-YYYY');
      subcontractor.reportDownloading = true;
      this.$store.dispatch("agency/downloadSubcontractorReport", weekEnding)
        .then(response => {
          subcontractor.reportDownloading = false;
          this.downloadFile(response, `Subcontractor_${weekEnding}`);
        })
        .catch(error => {
          subcontractor.reportDownloading = false;
          this.showAlertError(error.data);
        });
    }
  },
  created() {
    this.getPayrollSubcontractors();
  }
};
</script>