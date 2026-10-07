<template>
  <div class="has-menu-bottom">
    <b-loading v-model="isLoading"></b-loading>

    <Breadcrumbs :crumbs="crumbs" back-to="/company-requests" />
    <RequestHeader v-if="request" :number-id="request.numberId" :title="request.jobTitle"
      :subtitle="request.jobPositionRate?.value" :status-label="statusLabel" :status-variant="statusVariant"
      :is-asap="request.isAsap" :is-direct-hiring="isDirectHiringComputed" :chips="chips"
      :workers-working="request.workersQuantityWorking" :workers-total="request.workersQuantity" :kpis="kpis">
      <template v-if="canEdit" #actions>
        <b-button type="is-primary" @click="alertRequestAnotherWorker">Request another worker</b-button>
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
          <template #trigger>
            <b-button icon-right="dots-vertical" aria-label="More actions" />
          </template>
          <b-dropdown-item aria-role="listitem" @click="editContentModal = true">
            Edit requirements
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" v-if="canCancel" @click="modalValidation = true">
            Cancel request
          </b-dropdown-item>
        </b-dropdown>
      </template>

      <template v-if="request.displayShift" #kpi-shift>
        <AgencyShift :request-id="request.id" :display-shift="request.displayShift" :fetch-shift="getRequestShift" />
      </template>

      <b-tabs v-model="currentTab" @update:modelValue="changeTab">
        <b-tab-item label="Detail" value="Detail">
          <RequestDetailTab v-if="visitedTabs.includes('Detail')" :fields="fields" :description="request.description"
            :responsibilities="request.responsibilities" :requirements="request.requirements"
            :incentive="request.incentive" :incentive-description="request.incentiveDescription">
            <template v-if="canEdit" #requirements-actions>
              <b-button type="is-ghost" size="is-small" class="request-link" @click="editContentModal = true">
                Edit
              </b-button>
            </template>
            <template #rail>
              <RequestStaffingCard :items="staffing" />
              <RequestLocationCard :location="request.jobLocation" />
            </template>
          </RequestDetailTab>
        </b-tab-item>
        <b-tab-item label="Workers" value="Workers">
          <Workers v-if="visitedTabs.includes('Workers')" :request="request" />
        </b-tab-item>
        <b-tab-item label="Punch Card" value="PunchCard" v-if="!isDirectHiringComputed">
          <PunchCard v-if="visitedTabs.includes('PunchCard')" :request="request" />
        </b-tab-item>
      </b-tabs>
    </RequestHeader>

    <b-modal custom-content-class="card" v-model="modalValidation" width="500px">
      <CancelList @sendReason="(reason) => onCancelRequest(reason)"></CancelList>
    </b-modal>

    <b-modal custom-content-class="card" v-model="modalValidationRequestAnotherWorker" width="500px">
      <RequestAnotherWorker
        @sendAnotherWorker="(comment) => onRequestAnotherWorker(comment)"></RequestAnotherWorker>
    </b-modal>

    <b-modal custom-content-class="card" v-model="editContentModal" width="800px">
      <div class="p-3">
        <div class="columns is-multiline">
          <div class="column is-12">
            <b-field label="Requirements" :type="requirementsError ? 'is-danger' : ''"
              :message="requirementsError || ''">
              <div class="vue-trix-editor">
                <QuillEditor theme="snow" content-type="html" v-model:content="requirements" />
              </div>
            </b-field>
          </div>
          <div class="column is-12">
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
import { DurationTerm, DurationTermLabels, RequestStatus } from '@/constants/enums';
import {
  getRequest,
  getRequestShift,
  cancelRequest as cancelRequestApi,
  editRequest,
  requestAnotherWorker as requestAnotherWorkerApi,
} from '@/api/companyApi';
import { datesKpi, formatBreak, rateKpi, shiftKpi, staffingItems } from '@/utils/requestDetail';
import { useRequestStatus } from '@/composables/useRequestStatus';
import CancelList from '../../components/company/CompanyCancelList.vue';
import RequestAnotherWorker from '../../components/company/DialogRequestWorker.vue';
import Workers from '../../components/company_request/CompanyRequestWorkers.vue';
import PunchCard from '../../components/company_request/CompanyRequestPunchCard.vue';
import AgencyShift from '@/components/agency_request/AgencyShiftDetail.vue';
import RequestHeader from '@/components/request_detail/RequestHeader.vue';
import RequestDetailTab from '@/components/request_detail/RequestDetailTab.vue';
import RequestStaffingCard from '@/components/request_detail/RequestStaffingCard.vue';
import RequestLocationCard from '@/components/request_detail/RequestLocationCard.vue';
import Breadcrumbs from '@/components/Breadcrumbs.vue';
import type { PageBreadcrumb } from '@/types/common';
import type { CompanyRequestDetail } from '@/types/company';
import type { RequestFact, RequestKpi } from '@/types/requestDetail';

const route = useRoute();
const router = useRouter();
const crumbs: PageBreadcrumb[] = [{ label: 'Requests', to: '/company-requests' }];

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

const request = ref<CompanyRequestDetail | null>(null);
const isLoading = ref(true);
const modalValidation = ref(false);
const modalValidationRequestAnotherWorker = ref(false);
const currentTab = ref<string>('Detail');
const visitedTabs = ref<string[]>(['Detail']);
const editContentModal = ref(false);

const isDirectHiringComputed = computed(() => isDirectHiring(request.value));
const { canEdit, canCancel, statusLabel, statusVariant } = useRequestStatus(request);

const termLabel = computed(() =>
  request.value ? DurationTermLabels[request.value.durationTerm as DurationTerm] ?? '' : '');

const chips = computed(() => (termLabel.value ? [termLabel.value] : []));

const kpis = computed<RequestKpi[]>(() => {
  const r = request.value;
  if (!r) return [];
  const showFinish = r.durationTerm !== DurationTerm.LongTerm ||
    r.status === RequestStatus.Filled || r.status === RequestStatus.Cancelled;
  return [
    rateKpi(r.agencyRate, r.workerSalary, r.incentive),
    datesKpi(r.startAt, r.finishAt, showFinish),
    shiftKpi(r.displayShift, r.durationBreak, r.breakIsPaid),
  ];
});

const fields = computed<RequestFact[]>(() => {
  const r = request.value;
  if (!r) return [];
  return [
    { label: 'Role (position)', value: r.jobPositionRate?.value || r.jobTitle },
    { label: 'Term', value: termLabel.value || '—' },
    { label: 'Break', value: formatBreak(r.durationBreak, r.breakIsPaid) },
    { label: 'Holiday', value: r.holidayIsPaid ? 'Paid' : 'Not paid' },
  ];
});

const staffing = computed(() => {
  const r = request.value;
  if (!r) return [];
  return staffingItems({
    startAt: r.startAt,
    openSpots: r.workersQuantity - r.workersQuantityWorking,
    isOpen: r.status === RequestStatus.Open,
  });
});

function onCancelRequest(reason: { reasonId: string; otherMessage: string }) {
  if (!request.value) return;
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
    .then((response) => {
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
        if (request.value) request.value.requirements = values.requirements;
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
  if (val) form.hydrate({ requirements: request.value?.requirements || '' });
});
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.button.request-link {
  height: auto;
  padding: 0 4px;
  font-weight: 600;
  color: $blue;

  &:hover,
  &:focus {
    color: $blue-dark;
    text-decoration: underline;
  }
}
</style>
