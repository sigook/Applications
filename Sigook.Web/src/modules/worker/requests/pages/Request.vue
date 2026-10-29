<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <Breadcrumbs :crumbs="crumbs" :back-to="crumbs[0].to" />

    <RequestHeader v-if="request" :number-id="request.numberId ?? 0" :title="request.jobTitle ?? ''"
      logo-icon="briefcase-outline" :kpis="[]">
      <template #subtitle>
        <JobHeaderLocation :request="request" />
      </template>
      <template v-if="currentUser?.id && !currentUser.approvedToWork" #alert>You are not approved to work</template>
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
import { useWorkerStore } from '@/modules/worker/store';
import { showAlertError } from '@/shared/utils/toast';
import { getWorkerRequest, workerRequestApplySelf } from '@/modules/worker/requests/api';
import { appGlobals } from '@/app/globals';
import { useWorkerRequestSummary } from '@/modules/worker/requests/useWorkerRequestSummary';
import type { WorkerRequestDetail } from '@/modules/worker/requests/types';
import EditTextarea from '@/shared/ui/EditTextarea.vue';
import RequestHeader from '@/shared/request-detail/RequestHeader.vue';
import RequestDetailTab from '@/shared/request-detail/RequestDetailTab.vue';
import RequestLocationCard from '@/shared/request-detail/RequestLocationCard.vue';
import JobHeaderLocation from '@/modules/worker/requests/components/JobHeaderLocation.vue';
import Breadcrumbs from '@/shared/ui/Breadcrumbs.vue';
import type { PageBreadcrumb } from '@/shared/types/common';

const route = useRoute();
const crumbs: PageBreadcrumb[] = [{ label: 'Jobs', to: '/worker-requests' }];
const router = useRouter();
const workerStore = useWorkerStore();

const isLoading = ref(true);
const request = ref<WorkerRequestDetail | null>(null);
const modalMessage = ref(false);

const currentUser = computed(() => workerStore.workerProfile);
const { fields } = useWorkerRequestSummary(request);

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

getWorkerRequestFn();
</script>
