<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>

    <RequestHeader v-if="request" :number-id="request.numberId ?? 0" :title="request.jobTitle ?? ''"
      :subtitle="request.jobPosition" :org-name="request.agencyFullName" :logo="request.agencyLogo"
      :status-label="statusLabel" :status-variant="statusVariant" :is-asap="request.isAsap" :chips="chips"
      :workers-total="request.workersQuantity" :kpis="kpis">
      <template v-if="!currentUser?.approvedToWork" #alert>You are not approved to work</template>
      <template v-if="currentUser?.approvedToWork && canApply" #actions>
        <b-button type="is-primary" @click="modalMessage = true">Apply</b-button>
      </template>

      <b-tabs model-value="Detail">
        <b-tab-item label="Detail" value="Detail">
          <RequestDetailTab :fields="fields" :description="request.description"
            :responsibilities="request.responsibilities" :requirements="request.requirements"
            :incentive="request.incentive" :incentive-description="request.incentiveDescription"
            :skills="request.skills">
            <template #rail>
              <RequestLocationCard :location="request.jobLocation" />
            </template>
          </RequestDetailTab>
        </b-tab-item>
      </b-tabs>
    </RequestHeader>

    <b-modal custom-content-class="card" v-model="modalMessage" width="500px" :destroy-on-hide="true">
      <EditTextarea title="Additional Comments" :min-length="0" class="sm-edit-textarea"
        @updateContent="(data) => applyToRequest(data)" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useWorkerStore } from '@/stores/worker';
import { showAlertError } from '@/utils/toast';
import { getWorkerRequest, getWorkerRequestHistoryDetail, workerRequestApplySelf } from '@/api/workerApi';
import { appGlobals } from '@/varaibles';
import { useWorkerRequestSummary } from '@/composables/useWorkerRequestSummary';
import type { WorkerRequestDetail } from '@/types/worker';
import EditTextarea from '../../components/agency_request/EditTextarea.vue';
import RequestHeader from '@/components/request_detail/RequestHeader.vue';
import RequestDetailTab from '@/components/request_detail/RequestDetailTab.vue';
import RequestLocationCard from '@/components/request_detail/RequestLocationCard.vue';

const route = useRoute();
const router = useRouter();
const workerStore = useWorkerStore();

const isLoading = ref(true);
const request = ref<WorkerRequestDetail | null>(null);
const modalMessage = ref(false);

const currentUser = computed(() => workerStore.workerProfile);
const { statusLabel, statusVariant, kpis, fields, chips } = useWorkerRequestSummary(request);

const canApply = computed(() => {
  if (!request.value) return false;
  let available = false;
  switch (request.value.requestStatus) {
    case appGlobals.$statusOpen:
      available = true;
      break;
    case appGlobals.$statusFilled:
    case appGlobals.$statusCancelled:
      available = false;
      break;
    default:
      available = false;
      break;
  }
  switch (request.value.status) {
    case appGlobals.$statusReject:
    case appGlobals.$statusInQueue:
    case appGlobals.$statusDecline:
      available = false;
      break;
  }
  if (request.value.isApplicant) {
    available = false;
  }
  return available;
});

function getWorkerHistoryRequest() {
  getWorkerRequestHistoryDetail(route.params.id as string)
    .then((response) => {
      isLoading.value = false;
      request.value = response;
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
    })
    .catch(() => {
      isLoading.value = false;
    });
}

function applyToRequest(comment: string) {
  if (!request.value) return;
  const id = request.value.id;
  isLoading.value = true;
  const model = { comments: comment };
  workerRequestApplySelf(id, model)
    .then(() => {
      isLoading.value = false;
      router.push({ path: '/worker-requests/applied/' + id });
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

if (route.query.history) {
  getWorkerHistoryRequest();
} else {
  getWorkerRequestFn();
}
</script>
