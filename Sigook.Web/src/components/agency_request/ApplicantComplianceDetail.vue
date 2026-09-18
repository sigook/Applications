<template>
  <div>
    <ApplicantComplianceItems :request-id="requestId" :applicant-id="applicantId" :status="currentStatus"
      :worker-profile-id="workerProfileId" @loaded="onItemsLoaded" />
    <div class="applicants-detail-actions">
      <b-button v-if="canCancel" type="is-danger" outlined size="is-small" :loading="isChangingStatus"
        @click="confirmCancel(target)">
        Cancel applicant
      </b-button>
      <b-button v-if="currentStatus === RequestApplicantStatus.Pending" type="is-primary" size="is-small"
        :loading="isChangingStatus" @click="start(target)">
        Start
      </b-button>
      <b-button v-if="currentStatus === RequestApplicantStatus.Cancelled" type="is-primary" size="is-small"
        :loading="isChangingStatus" @click="reopen(target)">
        Reopen
      </b-button>
      <b-tooltip v-if="currentStatus === RequestApplicantStatus.InProgress" :label="confirmBlockedReason"
        :active="!canConfirm" type="is-dark" position="is-left">
        <b-button type="is-primary" size="is-small" :disabled="!canConfirm" :loading="isChangingStatus"
          @click="confirm(target)">
          Confirm applicant
        </b-button>
      </b-tooltip>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { RequestApplicantStatus } from '@/types/requestApplicant';
import { useApplicantStatusActions, type ApplicantStatusTarget } from '@/composables/useApplicantStatusActions';
import ApplicantComplianceItems from '@/components/agency_request/ApplicantComplianceItems.vue';

const props = defineProps<{
  requestId: string;
  applicantId: string;
  name: string;
  status: RequestApplicantStatus;
  workerProfileId?: string | null;
}>();
// Checking an item only moves the row's counters, which travel in `loaded`.
// `statusChanged` is what asks the owning list to reload.
const emit = defineEmits<{
  (e: 'statusChanged'): void;
  (e: 'loaded', value: { mandatoryCompleted: boolean; itemsCount: number; completedCount: number }): void;
}>();

// The status drives which buttons show, and a change is applied here before the
// list that owns the row reloads, so the actions never lag a click behind.
const currentStatus = ref<RequestApplicantStatus>(props.status);
const mandatoryCompleted = ref(false);

const target = computed<ApplicantStatusTarget>(() => ({
  requestId: props.requestId,
  applicantId: props.applicantId,
  name: props.name,
}));

const isCandidate = computed(() => !props.workerProfileId);
const canCancel = computed(() =>
  currentStatus.value === RequestApplicantStatus.Pending || currentStatus.value === RequestApplicantStatus.InProgress);
const canConfirm = computed(() =>
  currentStatus.value === RequestApplicantStatus.InProgress && !isCandidate.value && mandatoryCompleted.value);

const confirmBlockedReason = computed(() => {
  if (isCandidate.value) return 'Convert the candidate to a worker first';
  if (!mandatoryCompleted.value) return 'Complete all mandatory items first';
  return '';
});

const { isChangingStatus, start, reopen, confirm, confirmCancel } = useApplicantStatusActions((status) => {
  currentStatus.value = status;
  emit('statusChanged');
});

function onItemsLoaded(value: { mandatoryCompleted: boolean; itemsCount: number; completedCount: number }) {
  mandatoryCompleted.value = value.mandatoryCompleted;
  emit('loaded', value);
}

watch(() => props.status, (value) => {
  currentStatus.value = value;
});
</script>
