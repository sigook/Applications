<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>

    <RequestHeader v-if="request" :number-id="request.numberId ?? 0" :title="request.jobTitle ?? ''"
      :subtitle="request.jobPosition" :org-name="request.agencyFullName" :logo="request.agencyLogo"
      :status-label="statusLabel" :status-variant="statusVariant" :is-asap="request.isAsap" :chips="chips"
      :workers-total="request.workersQuantity" :kpis="kpis">
    <b-tabs v-model="currentTab" @update:modelValue="changeTab">
      <b-tab-item label="Detail" value="Summary Request">
        <RequestDetailTab v-if="visitedTabs.includes('Summary Request')" :fields="fields"
          :description="request.description" :responsibilities="request.responsibilities"
          :requirements="request.requirements" :incentive="request.incentive"
          :incentive-description="request.incentiveDescription" :skills="request.skills">
          <template #rail>
            <RequestLocationCard :location="request.jobLocation" />
          </template>
        </RequestDetailTab>
      </b-tab-item>
      <b-tab-item v-if="request.punchCardOptionEnabled" label="Punch Card" value="Punch Card">
        <PunchCard v-if="visitedTabs.includes('Punch Card') && timesheet" :requestId="request.id" :timesheet="timesheet"
          :request="request" @refreshTimeSheet="getTimeSheet" />
      </b-tab-item>
      <b-tab-item v-if="request.punchCardOptionEnabled" label="Time Sheet" value="Time Sheet">
        <TimeSheet v-if="visitedTabs.includes('Time Sheet')" :data="timesheet" />
      </b-tab-item>
    </b-tabs>
    </RequestHeader>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { getWorkerRequest, workerGetTimeSheet } from '@/api/workerApi';
import { useWorkerRequestSummary } from '@/composables/useWorkerRequestSummary';
import type { WorkerRequestDetail } from '@/types/worker';
import RequestHeader from '@/components/request_detail/RequestHeader.vue';
import RequestDetailTab from '@/components/request_detail/RequestDetailTab.vue';
import RequestLocationCard from '@/components/request_detail/RequestLocationCard.vue';
import PunchCard from './PunchCard.vue';
import TimeSheet from './TimeSheet.vue';

const route = useRoute();

const isLoading = ref(true);
const request = ref<WorkerRequestDetail | null>(null);
const { statusLabel, statusVariant, kpis, fields, chips } = useWorkerRequestSummary(request);
const currentTab = ref<string>('Summary Request');
const visitedTabs = ref<string[]>(['Summary Request']);
const timesheet = ref<any>({});

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
}

function getTimeSheet() {
  if (!request.value) return;
  workerGetTimeSheet(request.value.id)
    .then((response: any) => {
      isLoading.value = false;
      timesheet.value = response;
    })
    .catch(() => {
      isLoading.value = false;
    });
}

function getWorkerRequestFn() {
  getWorkerRequest(route.params.id as string)
    .then((response) => {
      isLoading.value = false;
      request.value = response;
      getTimeSheet();
    })
    .catch(() => {
      isLoading.value = false;
    });
}

getWorkerRequestFn();
</script>
