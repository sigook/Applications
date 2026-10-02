<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <SigookGrid :fetch="loadReport" v-model:params="serverParams"
          @update:loading="(value) => isLoading = value">
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
        </SigookGrid>
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
import type { AgencyReportFilter, WeeklyPayrollRow } from '@/types/agency';
import type { PaginatedList } from '@/types/common';
import SigookGrid from '@/components/SigookGrid.vue';

const isLoading = ref(false);
const serverParams = ref<AgencyReportFilter>({});

function loadReport(params: AgencyReportFilter): Promise<PaginatedList<WeeklyPayrollRow>> {
  return getPaymentReport(params)
    .then((response) => ({ ...response, items: response.items.map((i) => ({ ...i, reportDownloading: false })) }));
}

function onDownloadWeeklyPayrollReport(row: WeeklyPayrollRow) {
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
</script>
