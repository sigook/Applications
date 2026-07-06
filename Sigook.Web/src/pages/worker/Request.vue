<template>
  <div class="wrapper-request">
    <b-loading v-model="isLoading"></b-loading>

    <section class="wrapper-request-top" v-if="request">
      <div>
        <img v-if="request.agencyLogo" :src="request.agencyLogo" />
        <h2 class="text-capitalize fz1 fw-bold">{{ request.jobTitle }}</h2>
      </div>

      <div>
        <div v-if="request.status && request.status !== 'None'"
          class="option-request-top capitailized fw-bold is-inline-block" :class="request.status">
          {{ request.status }}
        </div>
        <div v-else class="option-request-top capitailized fw-bold is-inline-block" :class="request.requestStatus">
          {{ request.requestStatus }}
        </div>
        <div v-if="currentUser.approvedToWork" class="d-inline-block">
          <b-button v-if="canApply" type="is-primary" rounded @click="modalMessage = true">
            Apply
          </b-button>
        </div>
      </div>
    </section>

    <ul class="tabs-basic">
      <li class="active">Detail</li>
    </ul>

    <div class="container-flex">
      <section class="col-md-9 col-sm-12 section-left">
        <RequestDetail v-if="request" :request="request"></RequestDetail>
        <div v-if="currentUser.approvedToWork" class="mt-5">
          <div v-if="canApply">
            <b-button type="is-primary" rounded @click="modalMessage = true">
              Apply
            </b-button>
          </div>
        </div>
        <div class="alert-warning text-center" v-else>
          You are not approved to work
        </div>
      </section>
      <aside class="col-md-3 col-sm-12 section-right">
        <Location :jobLocation="request.jobLocation" />
      </aside>
    </div>

    <!-- custom modal TextArea-->
    <transition name="modal">
      <div v-if="modalMessage" class="vue-modal header-fixed">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container small-container modal-light overflow-initial border-radius">
              <button @click="modalMessage = false" type="button" class="cross-icon">
                close
              </button>
              <EditTextarea :title="'Additional Comments'" subtitle="Comments" :min-length="0" class="sm-edit-textarea"
                @updateContent="(data) => applyToRequest(data)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal TextArea-->
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useWorkerStore } from '@/stores/worker';
import { showAlertError } from '@/utils/toast';
import { getWorkerRequest, getWorkerRequestHistoryDetail, workerRequestApplySelf } from '@/api/workerApi';
import { appGlobals } from '@/varaibles';
import RequestDetail from '../../components/worker/RequestDetail.vue';
import Location from '../../components/request/RequestLocation.vue';
import EditTextarea from '../../components/agency_request/EditTextarea.vue';

const route = useRoute();
const router = useRouter();
const workerStore = useWorkerStore();

const isLoading = ref(true);
const request = ref<any>({});
const modalMessage = ref(false);

const currentUser = computed<any>(() => workerStore.workerProfile);

const canApply = computed(() => {
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
    .then((response: any) => {
      isLoading.value = false;
      request.value = response;
    })
    .catch(() => {
      isLoading.value = false;
    });
}

function getWorkerRequestFn() {
  getWorkerRequest(route.params.id as string)
    .then((response: any) => {
      isLoading.value = false;
      request.value = response;
    })
    .catch(() => {
      isLoading.value = false;
    });
}

function applyToRequest(comment: string) {
  isLoading.value = true;
  const model = { comments: comment };
  workerRequestApplySelf(request.value.id, model)
    .then(() => {
      isLoading.value = false;
      router.push({ path: '/worker-requests/applied/' + request.value.id });
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
