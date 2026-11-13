<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <SigookGrid ref="grid" :fetch="loadReport" v-model:params="serverParams"
          @update:loading="(value) => isLoading = value">
          <template #filters>
            <AgencyFilter v-model="serverParams.agencyIds" @change="grid?.search()" />
          </template>
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
            <HoverTooltip label="Download">
              <b-button type="is-success" outlined rounded icon-right="file-excel"
                :loading="props.row.reportDownloading" @click="onDownloadWeeklyPayrollReport(props.row)">
              </b-button>
            </HoverTooltip>
          </b-table-column>
        </SigookGrid>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import HoverTooltip from '@/shared/ui/HoverTooltip.vue';
import { ref, useTemplateRef } from 'vue';
import dayjs from 'dayjs';
import { showAlertError } from "@/shared/utils/toast";
import { downloadFile } from '@/shared/utils/downloadFile';
import { date, currency } from '@/shared/format';
import { getPaymentReport, downloadWeeklyPayrollReport } from '@/modules/agency/accounting/reports/api';
import type { AgencyReportFilter, WeeklyPayrollRow } from '@/modules/agency/accounting/reports/types';
import type { GridHandle, PaginatedList } from '@/shared/types/common';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import AgencyFilter from '@/modules/agency/shared/AgencyFilter.vue';

const isLoading = ref(false);
const grid = useTemplateRef<GridHandle>('grid');
const serverParams = ref<AgencyReportFilter>({});

async function loadReport(params: AgencyReportFilter): Promise<PaginatedList<WeeklyPayrollRow>> {
  const response = await getPaymentReport(params);
  return { ...response, items: response.items.map((i) => ({ ...i, reportDownloading: false })) };
}

async function onDownloadWeeklyPayrollReport(row: WeeklyPayrollRow) {
  row.reportDownloading = true;
  try {
    const response = await downloadWeeklyPayrollReport(row.displayWeekEnding);
    row.reportDownloading = false;
    downloadFile(response, `Payroll ${dayjs(row.weekEnding).format('MMMM D YYYY')}`);
  } catch (error) {
    row.reportDownloading = false;
    showAlertError(error);
  }
}
</script>
