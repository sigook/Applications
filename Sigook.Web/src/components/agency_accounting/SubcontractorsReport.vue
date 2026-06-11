<template>
  <div>
    <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" :loading="isLoading" paginated pagination-size="is-small" backend-pagination
      backend-sorting pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
      v-model:current-page="serverParams.pageIndex" @page-change="onPageChange">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="weekEnding" label="Week Ending" v-slot="props">
          {{ date(props.row.weekEnding) }}
        </b-table-column>
        <b-table-column field="numberOfWorkers" label="Workers" v-slot="props">
          {{ props.row.numberOfWorkers }}
        </b-table-column>
        <b-table-column field="totalNet" label="Total" v-slot="props">
          {{ currency(props.row.totalNet) }}
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-tooltip label="Download Report" type="is-dark" position="is-top" append-to-body>
            <b-button type="is-success" outlined rounded icon-right="file-excel" :loading="props.row.reportDownloading"
              @click="downloadSubcontractor(props.row)">
            </b-button>
          </b-tooltip>
        </b-table-column>
      </template>
    </b-table>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import dayjs from "dayjs";
import { downloadFile } from "@/utils/downloadFile";
import { date, currency } from '@/utils/filters';
import { getPayrollSubcontractors, downloadSubcontractorReport } from "@/api/agencyPayStubApi";

const isLoading = ref(false);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const serverParams = ref({
  sortBy: 3,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30
});

function onPageChange(page: number) {
  serverParams.value.pageIndex = page;
  loadSubcontractors();
}

function loadSubcontractors() {
  isLoading.value = true;
  getPayrollSubcontractors(serverParams.value)
    .then((response) => {
      rows.value = response.items.map((item: any) => ({ ...item, actions: null, reportDownloading: false }));
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function downloadSubcontractor(subcontractor: any) {
  const weekEnding = dayjs(subcontractor.weekEnding).format('MM-DD-YYYY');
  subcontractor.reportDownloading = true;
  downloadSubcontractorReport(weekEnding)
    .then(response => {
      subcontractor.reportDownloading = false;
      downloadFile(response, `Subcontractor_${weekEnding}`);
    })
    .catch(error => {
      subcontractor.reportDownloading = false;
      showAlertError(error.data);
    });
}

loadSubcontractors();
</script>
