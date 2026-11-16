<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid ref="grid" :fetch="loadSubcontractors" v-model:params="serverParams"
      @update:loading="(value) => isLoading = value">
      <template #filters>
        <AgencyFilter v-model="serverParams.agencyIds" @change="grid?.search()" />
      </template>
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
          <HoverTooltip label="Download Report">
            <b-button type="is-success" outlined rounded icon-right="file-excel" class="mr-2"
              :loading="props.row.reportDownloading" @click="downloadSubcontractor(props.row)">
            </b-button>
          </HoverTooltip>
          <HoverTooltip label="Delete Report">
            <b-button type="is-danger" outlined rounded icon-right="delete" @click="onDeleteSubcontractor(props.row)">
            </b-button>
          </HoverTooltip>
        </b-field>
      </b-table-column>
    </SigookGrid>
  </div>
</template>
<script setup lang="ts">
import HoverTooltip from '@/shared/ui/HoverTooltip.vue';
import { ref, useTemplateRef } from 'vue';
import { showAlertError, showAlertSuccess } from "@/shared/utils/toast";
import dayjs from "dayjs";
import { downloadFile } from "@/shared/utils/downloadFile";
import { date, currency } from '@/shared/format';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { getPayrollSubcontractors, downloadSubcontractorReport, deleteSubcontractorReport } from "@/modules/agency/accounting/paystubs/api";
import type { PayrollSubContractorRow, SubcontractorPayrollFilter } from '@/modules/agency/accounting/paystubs/types';
import type { GridHandle, PaginatedList } from '@/shared/types/common';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import AgencyFilter from '@/modules/agency/shared/AgencyFilter.vue';

const grid = useTemplateRef<GridHandle>('grid');

const isLoading = ref(false);
const serverParams = ref<SubcontractorPayrollFilter>({});

async function loadSubcontractors(params: SubcontractorPayrollFilter): Promise<PaginatedList<PayrollSubContractorRow>> {
  const response = await getPayrollSubcontractors(params);
  return { ...response, items: response.items.map((item) => ({ ...item, reportDownloading: false })) };
}

async function downloadSubcontractor(subcontractor: PayrollSubContractorRow) {
  const weekEnding = dayjs(subcontractor.weekEnding).format('MM-DD-YYYY');
  subcontractor.reportDownloading = true;
  try {
    const response = await downloadSubcontractorReport(weekEnding);
    subcontractor.reportDownloading = false;
    downloadFile(response, `Subcontractor_${weekEnding}`);
  } catch (error) {
    subcontractor.reportDownloading = false;
    showAlertError((error as { data?: unknown }).data);
  }
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
    onConfirm: () => deleteSubcontractor(weekEnding),
  });
}

async function deleteSubcontractor(weekEnding: string) {
  isLoading.value = true;
  try {
    await deleteSubcontractorReport(weekEnding);
    showAlertSuccess(`Subcontractor report ${weekEnding} deleted successfully`);
    grid.value?.reload();
  } catch (error) {
    isLoading.value = false;
    showAlertError((error as { data?: unknown }).data);
  }
}
</script>
