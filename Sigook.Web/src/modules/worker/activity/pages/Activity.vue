<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Activity" />
    <SummaryCards :summary="summary" :show-last-pay="!isUsa" @download="downloadPayStub" />
    <b-tabs v-model="currentTab" :destroy-on-hide="true" @update:modelValue="changeTab">
      <b-tab-item label="Pay Stubs" value="paystubs" :visible="!isUsa">
        <PayStubsTab @download="downloadPayStub" />
      </b-tab-item>
      <b-tab-item label="Timesheets" value="timesheets">
        <TimeSheetsTab />
      </b-tab-item>
      <b-tab-item label="History" value="history">
        <HistoryTab />
      </b-tab-item>
    </b-tabs>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import PageHeader from '@/shared/ui/PageHeader.vue';
import { downloadPDF, openFileUrl } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import SummaryCards from '@/modules/worker/activity/components/SummaryCards.vue';
import HistoryTab from '@/modules/worker/activity/components/HistoryTab.vue';
import PayStubsTab from '@/modules/worker/activity/components/PayStubsTab.vue';
import TimeSheetsTab from '@/modules/worker/activity/components/TimeSheetsTab.vue';
import { downloadWorkerPayStubPdf, getActivitySummary } from '@/modules/worker/activity/api';
import { weekStartOf } from '@/modules/worker/activity/activityFormat';
import { useWorkerStore } from '@/modules/worker/store';
import type { WorkerActivitySummary, WorkerPayStubListItem } from '@/modules/worker/activity/types';

const tabs = ['paystubs', 'timesheets', 'history'];

const route = useRoute();
const router = useRouter();
const workerStore = useWorkerStore();
const isLoading = ref(false);
const summary = ref<WorkerActivitySummary | null>(null);
const currentTab = ref(tabs.includes(String(route.query.tab)) ? String(route.query.tab) : 'paystubs');
const isUsa = computed(() => workerStore.workerProfile.location?.isUSA === true);

watch(isUsa, (value) => {
  if (value && currentTab.value === 'paystubs') currentTab.value = 'timesheets';
}, { immediate: true });

function changeTab(tab: string) {
  router.replace({ query: { ...route.query, tab } });
}

function downloadPayStub(payStub: WorkerPayStubListItem) {
  if (payStub.pdfUrl) {
    openFileUrl(payStub.pdfUrl);
    return;
  }
  isLoading.value = true;
  downloadWorkerPayStubPdf(payStub.id)
    .then((response) => {
      isLoading.value = false;
      downloadPDF(response, payStub.payStubNumber);
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

getActivitySummary(weekStartOf())
  .then((response) => { summary.value = response; })
  .catch((error: unknown) => showAlertError(error));
</script>
