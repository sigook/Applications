<template>
  <div>
    <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" :loading="isLoading" paginated pagination-size="is-small" backend-pagination
      backend-sorting pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
      v-model:current-page="serverParams.pageIndex" @page-change="onPageChange">
      <template v-slot:empty>
        <p class="container has-text-centered">No records available</p>
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
          <b-field>
            <b-tooltip label="Download Report" type="is-dark" position="is-top" append-to-body>
              <b-button type="is-success" outlined rounded icon-right="file-excel" class="mr-2"
                :loading="props.row.reportDownloading" @click="downloadSubcontractor(props.row)">
              </b-button>
            </b-tooltip>
            <b-tooltip label="Delete Report" type="is-dark" position="is-top" append-to-body>
              <b-button type="is-danger" outlined rounded icon-right="delete" @click="onDeleteSubcontractor(props.row)">
              </b-button>
            </b-tooltip>
          </b-field>
        </b-table-column>
      </template>
    </b-table>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import dayjs from "dayjs";
import { downloadFile } from "@/utils/downloadFile";
import { date, currency } from '@/utils/filters';
import { getDialog } from '@/utils/buefyProgrammatic';
import { getPayrollSubcontractors, downloadSubcontractorReport, deleteSubcontractorReport } from "@/api/agencyPayStubApi";
import type { PayrollSubContractorRow } from '@/types/accounting';

const isLoading = ref(false);
const totalItems = ref(0);
const rows = ref<PayrollSubContractorRow[]>([]);
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
      rows.value = response.items.map((item) => ({ ...item, reportDownloading: false }));
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function downloadSubcontractor(subcontractor: PayrollSubContractorRow) {
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

function onDeleteSubcontractor(subcontractor: PayrollSubContractorRow) {
  const weekEnding = dayjs(subcontractor.weekEnding).format('MM-DD-YYYY');
  const message = `You are about to delete the subcontractor report for week ending <b>${weekEnding}</b>,
        including its <b>${subcontractor.numberOfWorkers}</b> worker report(s).
        <br>
        <br>
        Their timesheets will be released and included again the next time a report is generated.`;
  getDialog().confirm({
    title: 'Are you sure you want to delete?',
    message: message,
    confirmText: 'Yes, I read and I want to delete',
    type: 'is-danger',
    hasIcon: true,
    onConfirm: () => {
      isLoading.value = true;
      deleteSubcontractorReport(weekEnding)
        .then(() => {
          showAlertSuccess(`Subcontractor report ${weekEnding} deleted successfully`);
          loadSubcontractors();
        })
        .catch(error => {
          isLoading.value = false;
          showAlertError(error.data);
        });
    },
  });
}

loadSubcontractors();
</script>
