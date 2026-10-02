<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <!-- If is not approved to work -->
    <b-message type="is-warning" v-if="!currentUser.approvedToWork" :closable="false">
      You are not approved to work
    </b-message>
    <b-message type="is-warning" v-if="hasMissingDocuments" :closable="false">
      <p><b>Your Profile is Incomplete</b></p>
      <p class="fz-1">Please update the following information to be able to work with us.</p>
      <ul class="normal-list fz-1">
        <li v-if="!currentUser.hasSocialInsurance || !currentUser.hasSocialInsuranceFile">
          Social Insurance
        </li>
        <li v-if="!currentUser.hasIdentificationType1File || !currentUser.hasIdentificationNumber1">
          Identification Document
        </li>
        <li v-if="!currentUser.hasIdentificationType2File || !currentUser.hasIdentificationNumber2">
          Second Identification Document
        </li>
        <li v-if="!currentUser.hasResume">Resume</li>
      </ul>
      <b-button tag="router-link" to="/worker-profile" type="is-danger" class="mtop-5">
        UPDATE DOCUMENTS
      </b-button>
    </b-message>
    <h2 class="fz1 pt-3">Jobs</h2>
    <div>
      <SigookGrid :fetch="getJobs" v-model:params="serverParams" @update:loading="(value) => isLoading = value" @click="onRowClick">
        <template #mobile-card="{ row }">
          <div class="rcard is-clickable" @click="onRowClick(row)">
            <div class="rcard__head">
              <div>
                <span class="rcard__title">{{ row.numberId }}</span>
                <span v-if="row.isAsap" class="asap ml-2">Asap</span>
              </div>
              <div v-if="row.status && row.status !== 'None'" class="capitailized has-text-weight-bold"
                :class="row.status">
                {{ row.status }}
              </div>
            </div>
            <p class="rcard__title">{{ row.jobTitle }}</p>
            <div class="rcard__rows">
              <div class="rcard__row">
                <span class="rcard__label">Location</span>
                <span>{{ row.location }}<span v-if="row.entrance"> - {{ row.entrance }}</span></span>
              </div>
              <div class="rcard__row">
                <span class="rcard__label">Duration</span>
                <span>
                  {{ dateMonth(row.startAt) }}
                  <span v-if="row.durationTerm !== appGlobals.$longTerm"> - {{ dateMonth(row.finishAt) }}</span>
                  <span
                    v-if="(row.status === appGlobals.$statusFilled || row.status === appGlobals.$statusCancelled) && row.durationTerm === appGlobals.$longTerm">
                    - {{ dateMonth(row.finishAt) }}</span>
                  <i class="fz-2 block">{{ splitCapital(row.durationTerm) }}</i>
                </span>
              </div>
              <div class="rcard__row">
                <span class="rcard__label">Rate / Salary</span>
                <span>{{ currency(row.workerRate) }}</span>
              </div>
              <div class="rcard__row">
                <span class="rcard__label">Spots</span>
                <span>{{ row.workersQuantity }}</span>
              </div>
            </div>
          </div>
        </template>
        <b-table-column field="numberId" label="Request ID" v-slot="props">
          {{ props.row.numberId }}
          <p v-if="props.row.isAsap" class="asap">{{ "Asap" }}</p>
        </b-table-column>
        <b-table-column field="jobTitle" label="Position" v-slot="props">
          {{ props.row.jobTitle }}
        </b-table-column>
        <b-table-column field="location" label="Location" v-slot="props">
          {{ props.row.location }}
          <span v-if="props.row.entrance"> - {{ props.row.entrance }}</span>
        </b-table-column>
        <b-table-column field="startAt">
          <template v-slot:header>
            <p class="has-text-weight-semibold">Duration</p>
            <p class="has-text-weight-semibold">(Start - End)</p>
          </template>
          <template v-slot="props">
            {{ dateMonth(props.row.startAt) }}
            <span v-if="props.row.durationTerm !== appGlobals.$longTerm">
              - {{ dateMonth(props.row.finishAt) }}
            </span>
            <span
              v-if="(props.row.status === appGlobals.$statusFilled || props.row.status === appGlobals.$statusCancelled) && props.row.durationTerm === appGlobals.$longTerm">
              - {{ dateMonth(props.row.finishAt) }}
            </span>
            <i class="fz-2 block">{{ splitCapital(props.row.durationTerm) }}</i>
          </template>
        </b-table-column>
        <b-table-column field="workerRate" label="Rate / Salary" v-slot="props">
          {{ currency(props.row.workerRate) }}
        </b-table-column>
        <b-table-column field="workersQuantity" label="Spots" v-slot="props">
          {{ props.row.workersQuantity }}
        </b-table-column>
        <b-table-column field="status" v-slot="props">
          <div v-if="props.row.status && props.row.status !== 'None'" class="capitailized has-text-weight-bold has-text-centered"
            :class="props.row.status">
            {{ props.row.status }}
          </div>
        </b-table-column>
      </SigookGrid>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useWorkerStore } from '@/stores/worker';
import { getJobs } from '@/api/workerApi';
import type { WorkerRequestFilter, WorkerRequestListItem } from '@/types/worker';
import { dateMonth, splitCapital, currency } from '@/utils/filters';
import { appGlobals } from '@/varaibles';
import SigookGrid from '@/components/SigookGrid.vue';

const router = useRouter();
const workerStore = useWorkerStore();

const isLoading = ref(true);
const serverParams = ref<WorkerRequestFilter>({
  isDescending: false,
});

const currentUser = computed<any>(() => workerStore.workerProfile);
const hasMissingDocuments = computed(() => {
  if (!currentUser.value.hasSocialInsurance || !currentUser.value.hasSocialInsuranceFile) {
    return true;
  } else if (!currentUser.value.hasIdentificationType1File || !currentUser.value.hasIdentificationNumber1) {
    return true;
  } else if (!currentUser.value.hasIdentificationType2File || !currentUser.value.hasIdentificationNumber2) {
    return true;
  } else {
    return !currentUser.value.hasResume;
  }
});

function onRowClick(row: WorkerRequestListItem) {
  switch (row.status) {
    case appGlobals.$statusApply:
    case appGlobals.$statusBook:
      router.push({ path: `/worker-requests/applied/${row.id}` });
      break;
    default:
      router.push({ path: `/worker-requests/${row.id}` });
  }
}
</script>
