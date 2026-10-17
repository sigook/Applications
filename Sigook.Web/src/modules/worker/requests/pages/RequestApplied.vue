<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>

    <RequestHeader v-if="request" :number-id="request.numberId ?? 0" :title="request.jobTitle ?? ''"
      logo-icon="briefcase-outline" :kpis="[]">
    <template #subtitle>
      <JobHeaderLocation :request="request" />
    </template>
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
        <PunchCard v-if="visitedTabs.includes('Punch Card')" :requestId="request.id" :request="request"
          @refreshTimeSheet="refreshTimeSheet" @refreshRequest="refreshRequest" />
      </b-tab-item>
      <b-tab-item v-if="request.punchCardOptionEnabled" label="Time Sheet" value="Time Sheet">
        <TimeSheet v-if="visitedTabs.includes('Time Sheet') && timesheet" :data="timesheet" />
      </b-tab-item>
    </b-tabs>
    </RequestHeader>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { getWorkerRequest, workerGetTimeSheet } from '@/modules/worker/requests/api';
import { useWorkerRequestSummary } from '@/modules/worker/requests/useWorkerRequestSummary';
import type { WorkerRequestDetail, WorkerTimeSheetItem } from '@/modules/worker/requests/types';
import type { PaginatedList } from '@/shared/types/common';
import RequestHeader from '@/shared/request-detail/RequestHeader.vue';
import RequestDetailTab from '@/shared/request-detail/RequestDetailTab.vue';
import RequestLocationCard from '@/shared/request-detail/RequestLocationCard.vue';
import JobHeaderLocation from '@/modules/worker/requests/components/JobHeaderLocation.vue';
import PunchCard from '@/modules/worker/requests/components/PunchCard.vue';
import TimeSheet from '@/modules/worker/requests/pages/TimeSheet.vue';

const route = useRoute();

const isLoading = ref(true);
const request = ref<WorkerRequestDetail | null>(null);
const { fields } = useWorkerRequestSummary(request);
const currentTab = ref<string>('Summary Request');
const visitedTabs = ref<string[]>(['Summary Request']);
const timesheet = ref<PaginatedList<WorkerTimeSheetItem> | null>(null);

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  if (tab === 'Time Sheet' && !timesheet.value) {
    getTimeSheet();
  }
}

function refreshTimeSheet() {
  if (timesheet.value) getTimeSheet();
}

function getTimeSheet() {
  if (!request.value) return;
  isLoading.value = true;
  workerGetTimeSheet(request.value.id)
    .then((response) => {
      isLoading.value = false;
      timesheet.value = response;
    })
    .catch(() => {
      isLoading.value = false;
    });
}

function refreshRequest() {
  getWorkerRequest(route.params.id as string)
    .then((response) => {
      request.value = response;
    })
    .catch(() => undefined);
}

function getWorkerRequestFn() {
  getWorkerRequest(route.params.id as string)
    .then((response) => {
      isLoading.value = false;
      request.value = response;
    })
    .catch(() => {
      isLoading.value = false;
    });
}

getWorkerRequestFn();
</script>
