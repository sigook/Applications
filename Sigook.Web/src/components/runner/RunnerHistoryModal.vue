<template>
  <div class="modal-card" style="width: auto">
    <header class="modal-card-head">
      <div class="runner-head">
        <div>
          <p class="modal-card-title">{{ detail?.name || 'History' }}</p>
          <i v-if="detail?.email" class="fz-2 text-lowercase">{{ detail.email }}</i>
        </div>
      </div>
    </header>
    <section class="modal-card-body" style="min-width: 620px; position: relative">
      <b-loading v-model="isLoading" :is-full-page="false" />
      <template v-if="detail">
        <collapse-section v-model="historyOpen" title="History">
          <ul v-if="detail.statusHistory.length" class="status-timeline">
            <li v-for="h in detail.statusHistory" :key="h.id" class="status-timeline__item">
              <span class="status-timeline__dot" :class="statusType(h.newStatus)" />
              <div class="status-timeline__content">
                <b-tag :type="statusType(h.newStatus)">{{ statusLabel(h.newStatus) }}</b-tag>
                <span class="fz-2 ml-2 op7">{{ emailName(h.changedBy) }} · {{ dateMonth(h.changedAt) }} {{ dateHHmm(h.changedAt) }}</span>
                <p v-if="h.comments" class="fz-2 mt-1 op7">{{ h.comments }}</p>
              </div>
            </li>
          </ul>
          <p v-else class="op3">No history yet</p>
        </collapse-section>

        <collapse-section v-model="interviewsOpen" title="Interviews">
          <div v-if="canAddInterview(detail.status)" class="has-text-right mb-2">
            <b-button type="is-primary" size="is-small" icon-left="plus" @click="showAddInterview = true">
              Add interview
            </b-button>
          </div>
          <b-table :data="detail.interviews" narrowed hoverable :mobile-cards="false">
          <template #empty>
            <p class="text-center op3">No interviews yet</p>
          </template>
          <b-table-column field="scheduledDate" label="Scheduled" v-slot="props">
            {{ dateMonth(props.row.scheduledDate) }}
            <b-tag size="is-small" :type="props.row.status === InterviewStatus.Rescheduled ? 'is-warning' : 'is-info'" class="ml-1">
              {{ interviewStatusLabel(props.row.status) }}
            </b-tag>
          </b-table-column>
          <b-table-column field="type" label="Type" v-slot="props">{{ interviewTypeLabel(props.row.type) }}</b-table-column>
          <b-table-column field="interviewer" label="Interviewer" v-slot="props">{{ props.row.interviewer }}</b-table-column>
          <b-table-column field="notes" label="Notes" v-slot="props">{{ props.row.notes }}</b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-button v-if="canAddInterview(detail.status)" size="is-small" icon-left="calendar-edit"
              @click="openReschedule(props.row.id)">
              Reschedule
            </b-button>
          </b-table-column>
        </b-table>
        </collapse-section>
      </template>
    </section>
    <footer class="modal-card-foot">
      <b-button @click="emit('close')">Close</b-button>
    </footer>

    <b-modal v-model="showAddInterview" width="540px">
      <runner-interview-modal :request-id="requestId" :runner-id="runnerId"
        @updated="onInterviewAdded" @close="showAddInterview = false" />
    </b-modal>

    <b-modal v-model="showReschedule" width="460px">
      <div class="modal-card" style="width: auto">
        <header class="modal-card-head"><p class="modal-card-title">Reschedule interview</p></header>
        <section class="modal-card-body" style="min-width: 400px">
          <b-field label="New date">
            <b-datetimepicker v-model="rescheduleDate" :mobile-native="false" placeholder="Select date and time" append-to-body />
          </b-field>
        </section>
        <footer class="modal-card-foot">
          <b-button @click="showReschedule = false">Cancel</b-button>
          <b-button type="is-primary" :disabled="!rescheduleDate" @click="reschedule">Reschedule</b-button>
        </footer>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { getAgencyRunner, rescheduleRunnerInterview } from '@/api/agencyRunnerApi';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { dateMonth, dateHHmm, emailName } from '@/utils/filters';
import {
  RUNNER_STATUS_LABELS,
  INTERVIEW_TYPE_LABELS,
  INTERVIEW_STATUS_LABELS,
  RunnerStatus,
  InterviewType,
  InterviewStatus,
  canAddInterview,
} from '@/types/runner';
import type { RunnerDetail } from '@/types/runner';
import CollapseSection from '@/components/CollapseSection.vue';
import RunnerInterviewModal from '@/components/runner/RunnerInterviewModal.vue';

const props = defineProps<{ requestId: string; runnerId: string }>();
const emit = defineEmits<{ (e: 'updated'): void; (e: 'close'): void }>();

const detail = ref<RunnerDetail | null>(null);
const isLoading = ref(false);

const historyOpen = ref(true);
const interviewsOpen = ref(false);

const showAddInterview = ref(false);
const showReschedule = ref(false);
const rescheduleDate = ref<Date | null>(null);
const rescheduleInterviewId = ref<string | null>(null);

function statusLabel(status: RunnerStatus): string {
  return RUNNER_STATUS_LABELS[status];
}

function interviewTypeLabel(type: InterviewType): string {
  return INTERVIEW_TYPE_LABELS[type];
}

function interviewStatusLabel(status: InterviewStatus): string {
  return INTERVIEW_STATUS_LABELS[status];
}

function statusType(status: RunnerStatus): string {
  if (status === RunnerStatus.Hired) return 'is-success';
  if (status === RunnerStatus.Rejected || status === RunnerStatus.NoShow || status === RunnerStatus.NoLongerAvailable) return 'is-danger';
  if (status === RunnerStatus.WaitingForInterviewFeedback || status === RunnerStatus.WaitingForFinalDecision) return 'is-warning';
  return 'is-info';
}

function load() {
  isLoading.value = true;
  getAgencyRunner(props.requestId, props.runnerId)
    .then(res => {
      detail.value = res;
    })
    .catch(err => showAlertError(err))
    .finally(() => {
      isLoading.value = false;
    });
}

function onInterviewAdded() {
  showAddInterview.value = false;
  emit('updated');
  load();
}

function openReschedule(interviewId: string) {
  rescheduleInterviewId.value = interviewId;
  rescheduleDate.value = null;
  showReschedule.value = true;
}

function reschedule() {
  if (!rescheduleDate.value || !rescheduleInterviewId.value) return;
  isLoading.value = true;
  rescheduleRunnerInterview(props.requestId, props.runnerId, rescheduleInterviewId.value, {
    newDate: rescheduleDate.value.toISOString(),
  })
    .then(() => {
      showReschedule.value = false;
      showAlertSuccess('Interview rescheduled');
      emit('updated');
      load();
    })
    .catch(err => {
      isLoading.value = false;
      showAlertError(err);
    });
}

load();
</script>

<style scoped lang="scss">
.runner-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 8px;
  width: 100%;
}

.status-timeline {
  position: relative;
  margin: 0;
  padding-left: 22px;

  &__item {
    position: relative;
    padding-bottom: 18px;

    &:last-child {
      padding-bottom: 0;
    }

    // connector line running down to the next dot
    &::before {
      content: '';
      position: absolute;
      left: -16px;
      top: 6px;
      bottom: -6px;
      width: 2px;
      background: #e6e6e6;
    }

    &:last-child::before {
      display: none;
    }
  }

  &__dot {
    position: absolute;
    left: -21px;
    top: 4px;
    width: 12px;
    height: 12px;
    border-radius: 50%;
    border: 2px solid #fff;
    box-shadow: 0 0 0 1px #dbdbdb;

    &.is-success { background: #48c78e; }
    &.is-danger { background: #f14668; }
    &.is-warning { background: #ffe08a; }
    &.is-info { background: #3e8ed0; }
  }
}
</style>
