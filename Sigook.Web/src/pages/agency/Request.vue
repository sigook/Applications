<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <Breadcrumbs :crumbs="crumbs" :back-to="requestBase" />

    <request-header v-if="request" :number-id="request.numberId" :title="request.jobTitle" :subtitle="billingTitle"
      :org-name="request.fullName" :org-link="`${companyBase}/${request.companyProfileId}`" :logo="request.companyLogo"
      :status-label="statusLabel" :status-variant="statusVariant" :is-asap="request.isAsap"
      :is-direct-hiring="isDirectHiringComputed" :chips="chips" :workers-working="request.workersQuantityWorking"
      :workers-total="request.workersQuantity" :kpis="kpis">
      <template v-if="!canEdit && request.cancellationDetail" #alert>
        <b>Cancellation detail:</b> {{ request.cancellationDetail }}
      </template>

      <template #actions>
        <template v-if="canEdit">
          <b-button @click="showShiftModal = true">Edit shift</b-button>
          <b-button type="is-primary"
            @click="router.push({ path: `${requestBase}/update/${request.companyProfileId}/${request.id}` })">
            Edit request
          </b-button>
        </template>
        <b-button v-else type="is-primary" @click="onOpenRequest">Reopen</b-button>
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
          <template #trigger>
            <b-button icon-right="dots-vertical" aria-label="More actions" />
          </template>
          <b-dropdown-item aria-role="listitem"
            @click="router.push({ path: `${requestBase}/duplicate/${request.companyProfileId}/${request.id}` })">
            Duplicate request
          </b-dropdown-item>
          <template v-if="canEdit">
            <b-dropdown-item aria-role="listitem" @click="onToggleIsAsap">
              {{ request.isAsap ? 'Remove ASAP' : 'Mark as ASAP' }}
            </b-dropdown-item>
            <template v-if="request.status === RequestStatus.Open">
              <b-dropdown-item v-if="canSendInvitation" aria-role="listitem" @click="sendInvitation">
                Send an email invitation
              </b-dropdown-item>
              <b-dropdown-item v-else aria-role="listitem" :disabled="true" :title="warningMessage">
                Send an email invitation
                <span class="fz-1">(Sent {{ dateFromNow(request.invitationSentItAt) }})</span>
              </b-dropdown-item>
            </template>
            <b-dropdown-item v-if="canCancel" aria-role="listitem" @click="cancelRequestModal = true">
              Cancel request
            </b-dropdown-item>
          </template>
        </b-dropdown>
      </template>

      <template v-if="canEdit" #quantity-actions>
        <b-button size="is-small" icon-left="minus" aria-label="Remove one worker"
          :disabled="request.workersQuantityWorking >= request.workersQuantity || request.workersQuantity === 1"
          @click="onReduceWorkersQuantity" />
        <b-button size="is-small" icon-left="plus" aria-label="Add one worker" @click="onIncreaseWorkersQuantity" />
      </template>

      <template v-if="request.displayShift" #kpi-shift>
        <agency-shift :request-id="request.id" :display-shift="request.displayShift" :fetch-shift="getAgencyRequestShift" />
      </template>

      <b-tabs v-model="currentTab" @update:modelValue="changeTab">
        <b-tab-item value="Detail" label="Detail">
          <request-detail-tab v-if="visitedTabs.includes('Detail')" :fields="fields" show-skills
            :description="request.description" :responsibilities="request.responsibilities"
            :requirements="request.requirements" :internal-requirements="request.internalRequirements"
            :incentive="request.incentive" :incentive-description="request.incentiveDescription"
            :compliance-items="request.complianceItems">
            <template #skills>
              <agency-request-skills v-model="request.skills" :request-id="request.id" :can-edit="canEdit" />
            </template>
            <template #rail>
              <request-staffing-card :items="staffing" @action="changeTab('Applicants')" />
              <request-location-card :location="request.jobLocation" />
              <detail-card title="Contacts">
                <requested-by v-model="request.requestedBy" :canEdit="canEdit" :requestId="request.id"
                  :companyProfileId="request.companyProfileId" />
                <report-to v-model="request.reportTo" :canEdit="canEdit" :requestId="request.id"
                  :companyProfileId="request.companyProfileId" />
                <span class="request-created">
                  Created by {{ emailName(request.createdBy) }} · {{ dateFromNow(request.createdAt) }}
                </span>
              </detail-card>
              <detail-card>
                <notes :canEdit="canEdit" />
              </detail-card>
            </template>
          </request-detail-tab>
        </b-tab-item>
        <b-tab-item value="Applicants" label="Applicants">
          <template #header>
            Applicants<span class="detail-tab-count">{{ request.applicantsCount }}</span>
          </template>
          <applicants v-if="visitedTabs.includes('Applicants')" :request="request" />
        </b-tab-item>
        <b-tab-item v-if="hasRecruitingAccess" value="Runners" label="Runners">
          <template #header>
            Runners<span class="detail-tab-count">{{ request.runnersCount }}</span>
          </template>
          <runners v-if="visitedTabs.includes('Runners')" :request="request" />
        </b-tab-item>
        <b-tab-item value="Workers" label="Workers">
          <template #header>
            Workers<span class="detail-tab-count">{{ request.workersCount }}</span>
          </template>
          <workers v-if="visitedTabs.includes('Workers')" @refreshRequest="loadRequest" />
        </b-tab-item>
        <b-tab-item v-if="!isDirectHiringComputed" value="PunchCard" label="Punch Card">
          <punch-card v-if="visitedTabs.includes('PunchCard')" :request="request" />
        </b-tab-item>
      </b-tabs>
    </request-header>

    <b-modal custom-content-class="card" v-model="cancelRequestModal" width="500px">
      <cancel-list @sendReason="onCancelRequest"></cancel-list>
    </b-modal>

    <b-modal custom-content-class="card" v-model="showShiftModal" width="800px">
      <shift-modal @onUpdateShift="updateShift" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAppStore } from '@/stores/app';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/utils/toast';
import { isDirectHiring } from '@/utils/directHiring';
import {
  getAgencyRequest,
  cancelAgencyRequest,
  agencyRequestOpen,
  agencyRequestSendInvitation,
  getAgencyRequestShift,
  increaseWorkersQuantityByOne,
  reduceWorkersQuantityByOne,
  updateAgencyRequestIsAsap,
} from '@/api/agencyRequestApi';
import { DurationTerm, DurationTermLabels, EmploymentType, EmploymentTypeLabels, RequestStatus } from '@/constants/enums';
import { currency, dateFromNow, emailName } from '@/utils/filters';
import { datesKpi, formatBreak, rateKpi, shiftKpi, staffingItems } from '@/utils/requestDetail';
import { useModuleBase } from '@/composables/useModuleBase';
import { useRecruitingAccess } from '@/composables/useRecruitingAccess';
import { useRequestStatus } from '@/composables/useRequestStatus';
import Breadcrumbs from '@/components/Breadcrumbs.vue';
import type { PageBreadcrumb } from '@/types/common';
import type { AgencyRequestDetail } from '@/types/agency';
import type { RequestFact, RequestKpi } from '@/types/requestDetail';
import RequestHeader from '@/components/request_detail/RequestHeader.vue';
import RequestDetailTab from '@/components/request_detail/RequestDetailTab.vue';
import DetailCard from '@/components/detail_page/DetailCard.vue';
import RequestStaffingCard from '@/components/request_detail/RequestStaffingCard.vue';
import RequestLocationCard from '@/components/request_detail/RequestLocationCard.vue';
import AgencyShift from '@/components/agency_request/AgencyShiftDetail.vue';
import AgencyRequestSkills from '@/components/agency_request/AgencyRequestSkills.vue';
import RequestedBy from '@/components/agency_request/RequestedBy.vue';
import ReportTo from '@/components/agency_request/ReportTo.vue';
import Notes from '@/components/agency_request/RequestNotes.vue';
import Workers from '@/components/agency/AgencyWorkers.vue';
import PunchCard from '@/components/agency_request/MassivePunchCard.vue';
import CancelList from '@/components/company/CompanyCancelList.vue';
import Applicants from '@/components/agency_request/Applicants.vue';
import Runners from '@/components/agency_request/Runners.vue';
import ShiftModal from '@/components/request/ShiftEditModal.vue';

const route = useRoute();
const router = useRouter();
const appStore = useAppStore();
const { requestBase, companyBase, moduleCrumbs } = useModuleBase();
const crumbs = computed<PageBreadcrumb[]>(() => [...moduleCrumbs.value, { label: 'Requests', to: requestBase.value }]);
const { hasRecruitingAccess } = useRecruitingAccess();

const isLoading = ref(true);
const request = ref<AgencyRequestDetail | null>(null);
const cancelRequestModal = ref(false);
const currentTab = ref<string>('Detail');
const visitedTabs = ref<string[]>(['Detail']);
const showShiftModal = ref(false);
const canSendInvitation = ref(false);
const warningMessage = 'The invitation must be sent only once every seven days.';

const { canEdit, canCancel, statusLabel, statusVariant } = useRequestStatus(request);

const isDirectHiringComputed = computed(() => isDirectHiring(request.value));

const billingTitle = computed(() =>
  request.value?.billingTitle && request.value.billingTitle !== request.value.jobTitle
    ? `Billed as ${request.value.billingTitle}`
    : '');

const employmentLabel = computed(() => {
  const r = request.value;
  if (!r) return '';
  return [EmploymentTypeLabels[r.employmentType as EmploymentType], DurationTermLabels[r.durationTerm as DurationTerm]]
    .filter(Boolean).join(' · ');
});

const chips = computed(() => {
  const r = request.value;
  if (!r) return [];
  return employmentLabel.value ? [employmentLabel.value] : [];
});

const showFinish = computed(() => {
  const r = request.value;
  if (!r) return false;
  return r.durationTerm !== DurationTerm.LongTerm ||
    r.status === RequestStatus.Filled || r.status === RequestStatus.Cancelled;
});

const kpis = computed<RequestKpi[]>(() => {
  const r = request.value;
  if (!r) return [];
  const items = r.complianceItems ?? [];
  const list: RequestKpi[] = [
    rateKpi(r.workerRate, r.workerSalary, r.incentive),
    datesKpi(r.startAt, r.finishAt, showFinish.value),
    shiftKpi(r.displayShift, r.durationBreak, r.breakIsPaid),
    {
      key: 'compliance',
      label: 'Compliance',
      value: items.length ? `${items.length} ${items.length === 1 ? 'item' : 'items'}` : 'None',
      hint: r.vaccinationRequired ? '· vaccination required' : '· no vaccination',
    },
  ];
  if (r.displayRecruiters) {
    list.push({ key: 'recruiters', label: 'Recruiters', value: r.displayRecruiters.split('|').join(', ') });
  }
  return list;
});

const fields = computed<RequestFact[]>(() => {
  const r = request.value;
  if (!r) return [];
  return [
    { label: 'Role (position)', value: r.jobPosition || '—' },
    { label: 'Billing title', value: r.billingTitle || '—' },
    { label: 'Employment', value: employmentLabel.value || '—' },
    { label: 'Break', value: formatBreak(r.durationBreak, r.breakIsPaid) },
    { label: 'Holiday', value: r.holidayIsPaid ? 'Paid' : 'Not paid' },
    { label: 'Bill rate', value: r.agencyRate ? `${currency(r.agencyRate)} / hr` : '—' },
    {
      label: 'Vaccination',
      value: r.vaccinationRequired ? 'Required' : 'Not required',
      to: `${companyBase.value}/${r.companyProfileId}`,
      linkLabel: 'Change in company',
    },
  ];
});

const staffing = computed(() => {
  const r = request.value;
  if (!r) return [];
  return staffingItems({
    startAt: r.startAt,
    openSpots: r.workersQuantity - r.workersQuantityWorking,
    applicantsCount: r.applicantsCount,
    invitationSentAt: r.invitationSentItAt,
    isOpen: r.status === RequestStatus.Open,
  });
});

loadRequest();
if (route.query && route.query.tab) {
  const requestedTab = route.query.tab as string;
  const tab = requestedTab === 'Runners' && !hasRecruitingAccess.value ? 'Detail' : requestedTab;
  currentTab.value = tab;
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
}

function changeTab(tab: string) {
  currentTab.value = tab;
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({
    path: `${requestBase.value}/${route.params.id}`,
    query: { tab: tab },
  });
}

function loadRequest() {
  isLoading.value = true;
  getAgencyRequest(route.params.id as string)
    .then((response) => {
      request.value = response;
      setCanSendInvitation(response);
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onCancelRequest(reason: { reasonId: string; otherMessage: string }) {
  if (!request.value) return;
  cancelRequestModal.value = false;
  isLoading.value = true;
  cancelAgencyRequest(request.value.id, {
    cancellationReasonId: reason.reasonId,
    otherCancellationReason: reason.otherMessage,
  })
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Cancelled');
      router.push(requestBase.value);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateShift(shift: string) {
  if (request.value) request.value.displayShift = shift;
  showShiftModal.value = false;
}

function onOpenRequest() {
  if (!request.value) return;
  isLoading.value = true;
  agencyRequestOpen(request.value.id)
    .then(() => {
      isLoading.value = false;
      if (request.value) request.value.status = RequestStatus.Open;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onToggleIsAsap() {
  if (!request.value) return;
  isLoading.value = true;
  updateAgencyRequestIsAsap(request.value.id)
    .then(() => {
      isLoading.value = false;
      if (request.value) request.value.isAsap = !request.value.isAsap;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onIncreaseWorkersQuantity() {
  if (!request.value) return;
  isLoading.value = true;
  increaseWorkersQuantityByOne(request.value.id)
    .then(loadRequest)
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onReduceWorkersQuantity() {
  if (!request.value) return;
  isLoading.value = true;
  reduceWorkersQuantityByOne(request.value.id)
    .then(loadRequest)
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function setCanSendInvitation(r: AgencyRequestDetail) {
  if (r.status === RequestStatus.Filled) {
    canSendInvitation.value = false;
    return;
  }
  if (!r.invitationSentItAt) {
    canSendInvitation.value = true;
    return;
  }
  const sentAt = r.invitationSentItAt;
  appStore.getCurrentDate().then((now: Date) => {
    const nextAllowed = new Date(sentAt);
    nextAllowed.setDate(nextAllowed.getDate() + 7);
    canSendInvitation.value = nextAllowed <= now;
  });
}

function sendInvitation() {
  if (!request.value) return;
  const id = request.value.id;
  showAlertConfirm('Are you sure?', warningMessage).then((response) => {
    if (response) {
      isLoading.value = true;
      agencyRequestSendInvitation(id)
        .then(() => {
          canSendInvitation.value = false;
          isLoading.value = false;
          showAlertSuccess('Sent it!');
        })
        .catch((error) => {
          isLoading.value = false;
          showAlertError(error);
        });
    }
  });
}
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.request-created {
  padding-top: 12px;
  border-top: 1px solid $gray-border;
  font-size: 0.8rem;
  color: $grey-font;
}
</style>
