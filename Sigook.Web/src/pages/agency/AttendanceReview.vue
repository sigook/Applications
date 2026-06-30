<template>
  <div class="p-2 p-sm-4">
    <b-loading v-model="isLoading"></b-loading>

    <h2 class="fz1 fw-bold mb-4">Workers to review attendance</h2>

    <b-table :data="workers" :mobile-cards="true" :striped="true" :hoverable="true">
      <b-table-column field="workerName" label="Worker" v-slot="props">
        {{ props.row.workerName }}
      </b-table-column>
      <b-table-column field="requestNumberId" label="Order" v-slot="props">
        #{{ props.row.requestNumberId }}
      </b-table-column>
      <b-table-column field="companyName" label="Company" v-slot="props">
        {{ props.row.companyName }}
      </b-table-column>
      <b-table-column field="jobTitle" label="Position" v-slot="props">
        {{ props.row.jobTitle }}
      </b-table-column>
      <b-table-column field="startDate" label="Start date" v-slot="props">
        {{ date(props.row.startDate) }}
      </b-table-column>
      <b-table-column field="dayNumber" label="Day" v-slot="props">
        <b-tag type="is-info is-light">Day {{ props.row.dayNumber }} of {{ followUpDays }}</b-tag>
      </b-table-column>
      <b-table-column label="" v-slot="props" cell-class="has-text-right">
        <b-button type="is-primary" size="is-small" icon-right="arrow-right" @click="goToPunchCard(props.row)">
          Punch card
        </b-button>
      </b-table-column>

      <template #empty>
        <div class="has-text-centered p-4 has-text-grey">No workers to review</div>
      </template>
    </b-table>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { getNotifications } from '@/api/notificationApi';
import { showAlertError } from '@/utils/toast';
import { date } from '@/utils/filters';
import type { RunnerStartingToday } from '@/types/runner';

const followUpDays = 3;

const router = useRouter();
const isLoading = ref(false);
const workers = ref<RunnerStartingToday[]>([]);

function load() {
  isLoading.value = true;
  getNotifications()
    .then((response) => {
      workers.value = response.workersToReview;
    })
    .catch((error) => {
      showAlertError(error);
    })
    .finally(() => {
      isLoading.value = false;
    });
}

function goToPunchCard(worker: RunnerStartingToday) {
  router.push({ path: `/agency-request/${worker.requestId}`, query: { tab: 'PunchCard' } });
}

load();
</script>
