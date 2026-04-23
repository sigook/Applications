<template>
  <div class="white-container-mobile">
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
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="numberId"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @click="onRowClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="numberId" label="Order ID" v-slot="props">
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
              <p class="fw-600">Duration</p>
              <p class="fw-600">(Start - End)</p>
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
            <div v-if="props.row.status && props.row.status !== 'None'" class="capitailized fw-700 text-center"
              :class="props.row.status">
              {{ props.row.status }}
            </div>
          </b-table-column>
        </template>
      </b-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useWorkerStore } from '@/stores/worker';
import { getJobs } from '@/api/workerApi';
import { dateMonth, splitCapital, currency } from '@/utils/filters';
import { appGlobals } from '@/varaibles';

const router = useRouter();
const workerStore = useWorkerStore();

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const serverParams = reactive<any>({
  sortBy: 0,
  isDescending: false,
  pageIndex: 1,
  pageSize: 30,
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

function onPageChange(params: number) {
  serverParams.pageIndex = params;
  getWorkerRequests();
}

function onRowClick(row: any) {
  switch (row.status) {
    case appGlobals.$statusApply:
    case appGlobals.$statusBook:
      router.push({ path: `/worker-request-applied/${row.id}` });
      break;
    default:
      router.push({ path: `/worker-request/${row.id}` });
  }
}

function getWorkerRequests() {
  isLoading.value = true;
  getJobs(serverParams)
    .then((response: any) => {
      rows.value = response.items;
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch(() => {
      isLoading.value = false;
    });
}

getWorkerRequests();
</script>
