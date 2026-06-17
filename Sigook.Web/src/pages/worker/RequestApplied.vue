<template>
  <div class="worker-detail-job">
    <b-loading v-model="isLoading"></b-loading>

    <section class="wrapper-request-top" v-if="request">
      <div>
        <img v-if="request.agencyLogo" :src="request.agencyLogo" />
        <h2 class="text-capitalize fz1 fw-bold">{{ request.jobTitle }}</h2>
      </div>

      <div>
        <div v-if="request.status && request.status !== 'None'" class="capitailized fw-bold is-inline-block"
          :class="request.status">
          {{ request.status }}
        </div>
      </div>
    </section>

    <b-tabs v-model="currentTab" @update:modelValue="changeTab" v-if="request">
      <b-tab-item label="Summary Request" value="Summary Request">
        <div v-if="visitedTabs.includes('Summary Request')" class="container-flex">
          <section class="col-md-9 col-sm-12 section-left">
            <RequestDetail :request="request" />
          </section>
          <aside class="col-md-3 col-sm-12 section-right">
            <Location :jobLocation="request.jobLocation" />
          </aside>
        </div>
      </b-tab-item>
      <b-tab-item v-if="request.punchCardOptionEnabled" label="Punch Card" value="Punch Card">
        <PunchCard v-if="visitedTabs.includes('Punch Card') && timesheet" :requestId="request.id" :timesheet="timesheet" @refreshTimeSheet="getTimeSheet" />
      </b-tab-item>
      <b-tab-item v-if="request.punchCardOptionEnabled" label="Time Sheet" value="Time Sheet">
        <TimeSheet v-if="visitedTabs.includes('Time Sheet')" :data="timesheet" />
      </b-tab-item>
    </b-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { getWorkerRequest, workerGetTimeSheet } from '@/api/workerApi';
import RequestDetail from '../../components/worker/RequestDetail.vue';
import Location from '../../components/request/RequestLocation.vue';
import PunchCard from './PunchCard.vue';
import TimeSheet from './TimeSheet.vue';

const route = useRoute();

const isLoading = ref(true);
const request = ref<any>({});
const currentTab = ref<string>('Summary Request');
const visitedTabs = ref<string[]>(['Summary Request']);
const timesheet = ref<any>({});

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
}

function getTimeSheet() {
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
  getWorkerRequest(route.params.id)
    .then((response: any) => {
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
