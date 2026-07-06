<template>
  <div class="has-menu-bottom">
    <b-loading v-model="isLoading"></b-loading>

    <section class="wrapper-request-top" v-if="request">
      <div>
        <router-link :to="'/recruiting/companies/' + request.companyProfileId">
          <img v-if="request.companyLogo" :src="request.companyLogo" />
        </router-link>
        <h2 class="text-capitalize fz1 fw-bold">
          <span class="fw-normal fz-0">{{ request.numberId }}</span>
          {{ request.jobTitle }}
        </h2>
      </div>
      <div>
        <div v-if="request.status"
          class="option-request-top capitailized fw-bold is-inline-block" :class="RequestStatusLabels[request.status]">
          {{ RequestStatusLabels[request.status] }}
        </div>
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body class="is-inline-block" v-if="canEdit">
          <template #trigger>
            <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
          </template>
          <b-dropdown-item aria-role="listitem" @click="alertRequestAnotherWorker">
            Request another worker
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" @click="editContentModal = true">
            Edit Requirements
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" v-if="canCancel" @click="modalValidation = true">
            Cancel Request
          </b-dropdown-item>
        </b-dropdown>
      </div>
    </section>

    <b-tabs v-model="currentTab" @update:modelValue="changeTab" v-if="request">
      <b-tab-item label="Detail" value="Detail">
        <Detail v-if="visitedTabs.includes('Detail')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
      <b-tab-item label="Workers" value="Workers">
        <Workers v-if="visitedTabs.includes('Workers')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
      <b-tab-item label="Punch Card" value="PunchCard" v-if="!isDirectHiringComputed">
        <PunchCard v-if="visitedTabs.includes('PunchCard')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
    </b-tabs>

    <b-modal v-model="modalValidation" width="500px">
      <CancelList @sendReason="(reason) => onCancelRequest(reason)"></CancelList>
    </b-modal>

    <b-modal v-model="modalValidationRequestAnotherWorker" width="500px">
      <RequestAnotherWorker
        @sendAnotherWorker="(comment) => onRequestAnotherWorker(comment)"></RequestAnotherWorker>
    </b-modal>

    <b-modal v-model="editContentModal" width="800px">
      <div class="p-3">
        <div class="container-flex">
          <div class="col-12 col-padding">
            <b-field label="Requirements" :type="requirementsError ? 'is-danger' : ''"
              :message="requirementsError || ''">
              <div class="vue-trix-editor">
                <QuillEditor theme="snow" content-type="html" v-model:content="requirements" />
              </div>
            </b-field>
          </div>
          <div class="col-12 col-padding">
            <b-button type="is-primary" @click="onUpdateRequirements()">Save</b-button>
          </div>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { isDirectHiring } from '@/utils/directHiring';
import { RequestStatus, RequestStatusLabels } from '@/constants/enums';
import {
  getRequest,
  cancelRequest as cancelRequestApi,
  editRequest,
  requestAnotherWorker as requestAnotherWorkerApi,
} from '@/api/companyApi';
import CancelList from '../../components/company/CompanyCancelList.vue';
import RequestAnotherWorker from '../../components/company/DialogRequestWorker.vue';
import Detail from '../../components/company_request/CompanyRequestDetail.vue';
import Workers from '../../components/company_request/CompanyRequestWorkers.vue';
import PunchCard from '../../components/company_request/CompanyRequestPunchCard.vue';

const route = useRoute();
const router = useRouter();

const requirementsSchema = yup.object({
  requirements: yup.string().test('min-text', 'Requirements must be at least 100 characters', (v) => {
    const text = (v || '').replace(/<[^>]*>/g, '').trim();
    return text.length >= 100;
  }),
});

const form = useStickyForm({
  schema: requirementsSchema,
  initialValues: { requirements: '' },
});
const { requirements } = form.fields;
const requirementsError = computed(() => form.errors.value.requirements || '');

const request = ref<any>({});
const isLoading = ref(true);
const modalValidation = ref(false);
const modalValidationRequestAnotherWorker = ref(false);
const currentTab = ref<string>('Detail');
const visitedTabs = ref<string[]>(['Detail']);
const editContentModal = ref(false);

const isDirectHiringComputed = computed(() => isDirectHiring(request.value));
const canEdit = computed(() =>
  request.value.status === RequestStatus.Open ||
  request.value.status === RequestStatus.Filled
);
const canCancel = computed(() =>
  request.value.status === RequestStatus.Open &&
  (!request.value.workersQuantityWorking || request.value.workersQuantityWorking === 0)
);

function onCancelRequest(reason: any) {
  modalValidation.value = false;
  isLoading.value = true;
  cancelRequestApi(request.value.id, reason.reasonId, reason.otherMessage)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Cancelled');
      router.push('/company-requests');
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function getData() {
  getRequest(route.params.id as string)
    .then((response: any) => {
      isLoading.value = false;
      request.value = response;
    })
    .catch((error: unknown) => {
      showAlertError((error as { data?: unknown }).data);
      isLoading.value = false;
    });
}

function alertRequestAnotherWorker() {
  modalValidationRequestAnotherWorker.value = true;
}

function onRequestAnotherWorker(comment: string) {
  modalValidationRequestAnotherWorker.value = false;
  isLoading.value = true;
  requestAnotherWorkerApi(route.params.id as string, { comments: comment })
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Requested');
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onUpdateRequirements() {
  form.markInteracted(['requirements']);
  form.handleSubmit((values) => {
    isLoading.value = true;
    editRequest(route.params.id as string, { requirements: values.requirements })
      .then(() => {
        isLoading.value = false;
        showAlertSuccess('Updated');
        request.value.requirements = values.requirements;
        editContentModal.value = false;
      })
      .catch((error: unknown) => {
        isLoading.value = false;
        showAlertError((error as { data?: unknown }).data);
      });
  })();
}

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({
    path: `/company-requests/${route.params.id}`,
    query: { tab: tab },
  });
}

getData();
if (route.query && route.query.tab) {
  const tab = route.query.tab as string;
  currentTab.value = tab;
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
}

watch(editContentModal, (val) => {
  if (val) form.hydrate({ requirements: request.value.requirements || '' });
});
</script>
