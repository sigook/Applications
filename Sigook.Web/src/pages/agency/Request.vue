<template>
  <div class="wrapper-request">
    <b-loading v-model="isLoading"></b-loading>
    <!-- CANCELLED -->
    <div v-if="request && !request.canEdit && request.cancellationDetail" class="alert-warning">
      <b>Cancellation detail: </b> {{ request.cancellationDetail }}
    </div>
    <section class="wrapper-request-top" v-if="request">
      <div>
        <router-link :to="'/agency-companies/company/' + request.companyProfileId">
          <img v-if="request.companyLogo" :src="request.companyLogo" alt="logo" />
        </router-link>
        <h2 class="text-capitalize fz1 fw-bold">
          <span v-if="request.isAsap || isDirectHiringComputed" class="request-flags">
            <span v-if="request.isAsap" class="request-flag request-flag--asap">Asap</span>
            <span v-if="isDirectHiringComputed" class="request-flag request-flag--dh">DH</span>
          </span>
          <span class="fw-normal fz-0">{{ request.numberId }}</span>
          {{ request.jobTitle }}
          <i class="fz-2 block">{{ billingTitle }}</i>
        </h2>
      </div>
      <div>
        <div class="d-inline-block option-request-top">
          {{ breakWord(request.displayRecruiters) }}
        </div>
        <div v-if="request.status && request.status !== 'None'"
          class="option-request-top text-uppercase fw-bold is-inline-block" :class="getStatusColorClass(request)">
          {{ RequestStatusLabels[request.status] }}
        </div>
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body class="is-inline-block" v-if="request.canEdit">
          <template #trigger>
            <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
          </template>
          <b-dropdown-item aria-role="listitem"
            @click="router.push({ path: `/agency-update-request/${request.companyProfileId}/${request.id}` })">
            Edit Request
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" @click="showShiftModal = true">
            Edit Shift
          </b-dropdown-item>
          <template v-if="request.status === RequestStatus.Open">
            <b-dropdown-item v-if="canSendInvitation" aria-role="listitem" @click="sendInvitation(request.id)">
              Send an email invitation
            </b-dropdown-item>
            <b-dropdown-item v-else aria-role="listitem" :disabled="true" :title="warningMessage">
              Send an email invitation
              <span class="fz-1">(Sent it {{ dateFromNow(request.invitationSentItAt) }})</span>
            </b-dropdown-item>
          </template>
          <b-dropdown-item aria-role="listitem" v-if="request.canCancel" @click="cancelRequestModal = true">
            Cancel Request
          </b-dropdown-item>
        </b-dropdown>
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body class="is-inline-block" v-if="!request.canEdit">
          <template #trigger>
            <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
          </template>
          <b-dropdown-item aria-role="listitem" @click="onOpenRequest(request.id)">
            Reopen
          </b-dropdown-item>
        </b-dropdown>
      </div>
    </section>
    <b-tabs v-model="currentTab" @update:modelValue="changeTab" v-if="request">
      <b-tab-item label="Detail" value="Detail">
        <detail v-if="visitedTabs.includes('Detail')" :key="request.id + '-' + request.workersQuantity + '-' + request.status" :request="request" class="p-2 p-sm-0" @refreshRequest="onRefreshRequest" />
      </b-tab-item>
      <b-tab-item label="Applicants" value="Applicants">
        <applicants v-if="visitedTabs.includes('Applicants')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
      <b-tab-item label="Workers" value="Workers">
        <workers v-if="visitedTabs.includes('Workers')" :request="request" class="p-2 p-sm-0" @refreshRequest="onRefreshRequest" />
      </b-tab-item>
      <b-tab-item label="Punch Card" value="PunchCard" v-if="!isDirectHiringComputed">
        <punch-card v-if="visitedTabs.includes('PunchCard')" :request="request" class="p-2 p-sm-0" />
      </b-tab-item>
    </b-tabs>

    <div v-if="request">

      <b-modal v-model="cancelRequestModal" width="500px">
        <cancel-list @sendReason="(reason) => onCancelRequest(reason)"></cancel-list>
      </b-modal>

       <b-modal v-model="showShiftModal" width="800px">
        <shift-modal @onUpdateShift="(val) => updateShift(val)" />
      </b-modal>
    </div>
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
} from '@/api/agencyRequestApi';
import { RequestStatus, RequestStatusLabels } from '@/constants/enums';
import { breakWord, dateFromNow } from '@/utils/filters';
import Detail from '@/components/agency_request/AgencyRequestDetail.vue';
import Workers from '@/components/agency/AgencyWorkers.vue';
import PunchCard from '@/components/agency_request/MassivePunchCard.vue';
import CancelList from '@/components/company/CompanyCancelList.vue';
import Applicants from '@/components/agency_request/Applicants.vue';
import ShiftModal from '@/components/request/ShiftEditModal.vue';

const route = useRoute();
const router = useRouter();
const appStore = useAppStore();

const isLoading = ref(true);
const request = ref<any>(null);
const cancelRequestModal = ref(false);
const currentTab = ref<string>('Detail');
const visitedTabs = ref<string[]>(['Detail']);
const showShiftModal = ref(false);
const canSendInvitation = ref(false);
const warningMessage = 'The invitation must be sent only once every seven days.';

const isDirectHiringComputed = computed(() => isDirectHiring(request.value));

const billingTitle = computed(() => {
  if (request.value.billingTitle && request.value.jobTitle !== request.value.billingTitle) {
    return `${request.value.billingTitle}`;
  }
  return '';
});

loadRequest();
if (route.query && route.query.tab) {
  currentTab.value = route.query.tab as string;
  if (!visitedTabs.value.includes(route.query.tab as string)) {
    visitedTabs.value.push(route.query.tab as string);
  }
}

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({
    path: `/agency-request/${route.params.id}`,
    query: { tab: tab },
  });
}

function canEditRequest(r: any) {
  return r.status === RequestStatus.Open || r.status === RequestStatus.Filled;
}

function canCancelRequest(r: any) {
  return r.status === RequestStatus.Open &&
    (!r.workersQuantityWorking || r.workersQuantityWorking === 0);
}

function loadRequest() {
  isLoading.value = true;
  getAgencyRequest(route.params.id as any)
    .then((response: any) => {
      const updatedRequest = Object.assign({}, response, {
        canEdit: canEditRequest(response),
        canCancel: canCancelRequest(response),
      });
      request.value = updatedRequest;
      setCanSendInvitation(request.value);
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onCancelRequest(reason: any) {
  cancelRequestModal.value = false;
  isLoading.value = true;
  cancelAgencyRequest(request.value.id, {
    cancellationReasonId: reason.reasonId,
    otherCancellationReason: reason.otherMessage,
  })
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Cancelled');
      router.push('/recruiting/requests');
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateShift(shift: any) {
  request.value.displayShift = shift;
  showShiftModal.value = false;
}

function onOpenRequest(id: any) {
  isLoading.value = true;
  agencyRequestOpen(id)
    .then(() => {
      isLoading.value = false;
      request.value.status = RequestStatus.Open;
      request.value.canEdit = canEditRequest(request.value);
      request.value.canCancel = canCancelRequest(request.value);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function setCanSendInvitation(r: any) {
  if (r.status === RequestStatus.Filled) {
    canSendInvitation.value = false;
    return;
  }
  if (r && !r.invitationSentItAt) {
    canSendInvitation.value = true;
    return;
  }
  appStore.getCurrentDate().then((now: Date) => {
    const invitationSentItAt = new Date(r.invitationSentItAt);
    invitationSentItAt.setDate(invitationSentItAt.getDate() + 7);
    if (invitationSentItAt <= now) {
      canSendInvitation.value = true;
    } else {
      canSendInvitation.value = false;
    }
  });
}

function onRefreshRequest() {
  loadRequest();
}

function sendInvitation(id: any) {
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

function getStatusColorClass(r: any) {
  if (r.status === RequestStatus.Open &&
    r.workersQuantityWorking > 0 &&
    r.workersQuantityWorking < r.workersQuantity) {
    return 'Book';
  }
  return RequestStatusLabels[r.status];
}
</script>

<style scoped lang="scss">
// Top-align the right-side group (recruiter, status, actions) with the
// title's first line instead of vertically centering against the whole h2.
.wrapper-request-top {
  align-items: flex-start;

  // keep the action group's own items centered relative to each other,
  // and nudge it down so its text sits on the title's first line
  & > div:last-child {
    display: flex;
    align-items: center;
    flex-wrap: wrap;
    padding-top: 4px;
  }
}

.request-flags {
  display: inline-flex;
  align-items: center;
  vertical-align: middle;
  margin-right: 6px;
}

.request-flag {
  position: relative;
  display: inline-block;
  padding: 2px 12px 2px 6px;
  font-size: 10px;
  font-weight: 700;
  line-height: 1;
  color: #fff;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  // convex right-pointing arrow; the tip pokes into the next (solid) flag,
  // so the seam is always backed by color and never reveals a white sub-pixel gap
  clip-path: polygon(0 0, calc(100% - 6px) 0, 100% 50%, calc(100% - 6px) 100%, 0 100%);

  &--asap {
    background: #ff9932;
    z-index: 2;
  }

  &--dh {
    background: #1d4ed8;
    z-index: 1;
  }

  // the second flag tucks under the first arrow's tip; its flat left edge keeps
  // solid colour behind that tip in both expanded and collapsed layouts
  & + & {
    margin-left: -6px;
  }
}
</style>
