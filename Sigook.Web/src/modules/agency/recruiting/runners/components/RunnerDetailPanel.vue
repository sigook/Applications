<template>
  <div class="runner-detail">
    <b-loading v-model="isLoading" :is-full-page="false" />
    <template v-if="detail">
      <div class="runner-detail-card">
        <div class="runner-detail-card-head">History</div>
        <div class="runner-detail-card-body">
          <ul v-if="detail.statusHistory.length" class="status-timeline">
            <li v-for="h in detail.statusHistory" :key="h.id" class="status-timeline-item">
              <span class="status-timeline-dot" :class="runnerStatusTagType(h.newStatus)" />
              <div>
                <b-tag :type="runnerStatusTagType(h.newStatus)">{{ runnerStatusLabel(h.newStatus) }}</b-tag>
                <span class="fz-2 ml-2 op7">{{ h.changedByEmail }} · {{ dateMonth(h.changedAt) }} {{ dateHHmm(h.changedAt) }}</span>
                <p v-if="h.comments" class="fz-2 mt-1 op7">{{ h.comments }}</p>
              </div>
            </li>
          </ul>
          <p v-else class="op3">No history yet</p>
        </div>
      </div>

      <div class="runner-detail-card">
        <div class="runner-detail-card-head">
          Interviews
          <b-button v-if="canEdit && canAddInterview(detail.status)" size="is-small" icon-left="plus"
            @click="showAddInterview = true">
            Add interview
          </b-button>
        </div>
        <SigookGrid :data="detail.interviews" :paginated="false" :fit-viewport="false" empty-text="No interviews yet">
          <b-table-column field="scheduledDate" label="Scheduled" v-slot="props">
            {{ dateMonth(props.row.scheduledDate) }} {{ dateHHmm(props.row.scheduledDate) }}
            <b-tag size="is-small" :type="props.row.status === InterviewStatus.Rescheduled ? 'is-warning' : 'is-info'"
              class="ml-1">
              {{ INTERVIEW_STATUS_LABELS[props.row.status] }}
            </b-tag>
          </b-table-column>
          <b-table-column field="type" label="Type" v-slot="props">{{ INTERVIEW_TYPE_LABELS[props.row.type] }}</b-table-column>
          <b-table-column field="interviewer" label="Interviewer" v-slot="props">{{ props.row.interviewer }}</b-table-column>
          <b-table-column field="notes" label="Notes" v-slot="props">{{ props.row.notes }}</b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-button v-if="canEdit && canAddInterview(detail.status)" size="is-small" icon-left="calendar-edit"
              @click="openReschedule(props.row.id)">
              Reschedule
            </b-button>
          </b-table-column>
        </SigookGrid>
      </div>
    </template>

    <b-modal has-modal-card v-model="showAddInterview" width="540px" :destroy-on-hide="true">
      <runner-interview-modal :request-id="requestId" :runner-id="runnerId" @updated="onInterviewAdded"
        @close="showAddInterview = false" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { getAgencyRunner, rescheduleRunnerInterview } from '@/modules/agency/recruiting/requests/runners/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { dateMonth, dateHHmm } from '@/shared/format';
import {
  INTERVIEW_TYPE_LABELS,
  INTERVIEW_STATUS_LABELS,
  InterviewStatus,
  canAddInterview,
  runnerStatusLabel,
  runnerStatusTagType,
} from '@/modules/agency/recruiting/requests/runners/types';
import type { RunnerDetail } from '@/modules/agency/recruiting/requests/runners/types';
import RunnerInterviewModal from '@/modules/agency/recruiting/requests/runners/RunnerInterviewModal.vue';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const props = withDefaults(defineProps<{ requestId: string; runnerId: string; canEdit?: boolean }>(), { canEdit: true });
const emit = defineEmits<{ (e: 'updated'): void; (e: 'loaded', detail: RunnerDetail): void }>();

const detail = ref<RunnerDetail | null>(null);
const isLoading = ref(false);
const showAddInterview = ref(false);

function load() {
  isLoading.value = true;
  getAgencyRunner(props.requestId, props.runnerId)
    .then((response) => {
      detail.value = response;
      emit('loaded', response);
    })
    .catch((error) => showAlertError(error))
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
  getDialog().prompt({
    title: 'Reschedule interview',
    message: 'New date and time',
    inputAttrs: {
      type: 'datetime-local',
      required: true,
    },
    confirmText: 'Reschedule',
    onConfirm: (value: string, dialog: { close: () => void }) => {
      isLoading.value = true;
      rescheduleRunnerInterview(props.requestId, props.runnerId, interviewId, { newDate: new Date(value).toISOString() })
        .then(() => {
          dialog.close();
          showAlertSuccess('Interview rescheduled');
          emit('updated');
          load();
        })
        .catch((error) => {
          isLoading.value = false;
          showAlertError(error);
        });
    },
  });
}

load();

defineExpose({ reload: load });
</script>

<style scoped lang="scss">
@import "@/assets/scss/variables";

.runner-detail {
  position: relative;
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 14px;
}

.runner-detail-card {
  min-width: 0;
  border: 1px solid $gray-border;
  border-radius: 8px;
  background: $white;
}

.runner-detail-card-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 12px;
  border-bottom: 1px solid $gray-border;
  font-weight: 600;
}

.runner-detail-card-body {
  padding: 12px;
}

.status-timeline {
  position: relative;
  margin: 0;
  padding-left: 22px;
}

.status-timeline-item {
  position: relative;
  padding-bottom: 14px;

  &:last-child {
    padding-bottom: 0;
  }

  &::before {
    content: '';
    position: absolute;
    left: -16px;
    top: 8px;
    bottom: -6px;
    width: 2px;
    background: $gray-border;
  }

  &:last-child::before {
    display: none;
  }
}

.status-timeline-dot {
  position: absolute;
  left: -21px;
  top: 5px;
  width: 12px;
  height: 12px;
  border-radius: 50%;
  border: 2px solid $white;
  box-shadow: 0 0 0 1px $border-input;

  &.is-success { background: $green; }
  &.is-danger { background: $danger; }
  &.is-warning { background: $accent; }
  &.is-info { background: $blue; }
}

@media (max-width: 900px) {
  .runner-detail {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
