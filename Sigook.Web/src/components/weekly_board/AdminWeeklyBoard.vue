<template>
  <div class="weekly-board">
    <b-loading v-model="isLoading"></b-loading>

    <b-navbar type="is-primary" class="stats-navbar" :mobile-burger="false">
      <template #brand>
        <b-navbar-item tag="div" class="stats-name">{{ userName }}</b-navbar-item>
      </template>
      <template #end>
        <b-navbar-item tag="div" class="stat">
          <span class="stat-value">{{ board?.totalAssignments ?? 0 }}</span>
          <span class="stat-label">Assignments</span>
        </b-navbar-item>
        <b-navbar-item tag="div" class="stat">
          <span class="stat-value">{{ board?.totalWorkersSent ?? 0 }}</span>
          <span class="stat-label">Workers sent</span>
        </b-navbar-item>
      </template>
    </b-navbar>

    <div class="board-toolbar">
      <div class="range-nav">
        <b-button icon-right="chevron-left" @click="shiftWeek(-1)"></b-button>
        <b-datepicker v-model="range" range :mobile-native="false" icon="calendar-today">
          <template v-slot:trigger>
            <b-button icon-left="calendar-today">{{ rangeLabel }}</b-button>
          </template>
        </b-datepicker>
        <b-button icon-right="chevron-right" @click="shiftWeek(1)"></b-button>
        <b-button icon-right="refresh" title="Refresh" :loading="isLoading" @click="loadBoard()"></b-button>
      </div>
      <div class="toolbar-right">
        <div class="legend">
          <span class="legend-item"><span class="dot is-open"></span> Open</span>
          <span class="legend-item"><span class="dot is-filled"></span> Filled</span>
        </div>
        <b-button type="is-primary" icon-left="plus" @click="openAssign()">Assign recruiter</b-button>
      </div>
    </div>

    <div class="grid-wrapper">
      <table class="board-grid">
        <thead>
          <tr>
            <th class="recruiter-col">Recruiter</th>
            <th v-for="day in days" :key="day.date" :class="{ 'is-today': day.isToday }">
              <span class="dow">{{ day.weekday }}</span>
              <span class="dom">{{ day.monthShort }} {{ day.dayNum }}</span>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="!board || board.recruiters.length === 0">
            <td :colspan="days.length + 1" class="empty-row">No assignments this week</td>
          </tr>
          <tr v-for="row in board?.recruiters ?? []" :key="row.recruiterId">
            <td class="recruiter-col">
              <div class="recruiter-name">{{ row.recruiterName }}</div>
              <div class="recruiter-meta">{{ row.ordersCount }} orders · {{ row.workersSent }} sent</div>
            </td>
            <td v-for="day in days" :key="day.date" class="day-col"
              :class="{ 'is-drop-target': isDropTarget(row.recruiterId, day.date) }"
              @dragover.prevent="onDragOver(row.recruiterId, day.date)" @dragleave="onDragLeave"
              @drop.prevent="onDrop(row.recruiterId, day.date)">
              <div v-for="assignment in cardsFor(row, day.date)" :key="assignment.requestId" class="order-card"
                :class="[statusClass(assignment.status), { 'is-dragging': isDragging(assignment) }]" draggable="true"
                @dragstart="onDragStart(assignment)" @dragend="onDragEnd" @click="openDetail(assignment)">
                <div class="card-top">
                  <div class="card-head-left">
                    <router-link class="card-num"
                      :to="{ name: 'agency-request', params: { id: assignment.requestId } }" target="_blank"
                      @click.stop>#{{ assignment.numberId }}</router-link>
                    <span v-if="assignment.isAsap || isDirectHiring(assignment)" class="request-flags">
                      <span v-if="assignment.isAsap" class="request-flag request-flag--asap">Asap</span>
                      <span v-if="isDirectHiring(assignment)" class="request-flag request-flag--dh">DH</span>
                    </span>
                  </div>
                  <b-button class="card-remove" type="is-ghost" size="is-small" icon-right="close"
                    title="Remove assignment" :disabled="isSaving" @click.stop="confirmUnassign(assignment)"></b-button>
                </div>
                <div class="card-position">{{ assignment.jobTitle }}</div>
                <div class="card-company">{{ assignment.companyName }}</div>
                <div class="card-sent">{{ assignment.workersSent }} sent</div>
              </div>
              <b-button class="add-card" type="is-ghost" expanded icon-left="plus"
                @click="openAssign({ recruiterId: row.recruiterId, workDate: day.date })"></b-button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <b-modal v-model="showAssign" :width="720" scroll="keep" :destroy-on-hide="true">
      <assign-recruiter-modal :days="days" :preset="assignPreset" :saving="isSaving" @assign="onAssign"
        @close="showAssign = false" />
    </b-modal>

    <b-modal v-model="showDetail" :width="480" scroll="keep">
      <div v-if="detail" class="modal-card detail-card">
        <header class="modal-card-head is-flex-direction-column is-align-items-start">
          <p class="modal-card-title">Request #{{ detail.numberId }}</p>
          <p class="has-text-grey is-size-7">{{ detail.companyName }} · {{ detail.jobTitle }}</p>
        </header>
        <section class="modal-card-body">
          <div class="detail-row"><b-icon icon="account" size="is-small"></b-icon> Recruiter <strong>{{
            detail.recruiterName
              }}</strong></div>
          <div class="detail-row"><b-icon icon="calendar" size="is-small"></b-icon>
            {{ formatDetailDate(detail.workDate) }}
          </div>
          <div class="detail-row">
            <b-tag :type="statusTagType(detail.status)" rounded>{{ statusLabel(detail.status) }}</b-tag>
          </div>
          <div v-if="detailDispatchesLoading" class="detail-workers-loading has-text-grey is-size-7">
            Loading workers…
          </div>
          <collapse-section v-else-if="detailDispatches.length > 0" :key="detail.requestId" class="detail-workers"
            variant="compact" :model-value="false">
            <template #title>Workers ({{ detailDispatches.length }})</template>
            <ul>
              <li v-for="(worker, index) in detailDispatches" :key="`${worker.workerProfileId}-${index}`">
                <div class="detail-worker-main">
                  <router-link :to="{ name: 'workerDetail', params: { id: worker.workerProfileId } }" target="_blank">{{
                    worker.fullName }}</router-link>
                  <span class="detail-worker-email">{{ worker.email }}</span>
                </div>
                <span v-if="worker.sentAt" class="detail-worker-sent">{{ formatSentAt(worker.sentAt) }}</span>
              </li>
            </ul>
          </collapse-section>
        </section>
        <footer class="modal-card-foot is-justify-content-space-between">
          <b-button type="is-danger" icon-left="trash-can-outline" :loading="isSaving" @click="onUnassign(detail)">
            Remove assignment
          </b-button>
          <b-button @click="showDetail = false">Close</b-button>
        </footer>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import dayjs from 'dayjs';
import { useAgencyStore } from '@/stores/agency';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { getDialog } from '@/utils/buefyProgrammatic';
import { getWeeklyBoard, assignRecruiters, unassignRecruiter, moveAssignment, getRequestDispatches } from '@/api/weeklyBoardApi';
import { isDirectHiring } from '@/utils/directHiring';
import { RequestStatus, RequestStatusLabels } from '@/constants/enums';
import type {
  WeeklyBoard,
  WeeklyBoardAssignment,
  WeeklyBoardRecruiterRow,
  WeeklyBoardDispatch,
  AssignRecruitersPayload,
  WeekDay,
  AssignPreset,
} from '@/types/weeklyBoard';
import AssignRecruiterModal from './AssignRecruiterModal.vue';
import CollapseSection from '@/components/CollapseSection.vue';

const dateFormat = 'YYYY-MM-DD';

const agencyStore = useAgencyStore();
const userName = computed(() => agencyStore.agency?.fullName || 'Weekly Board');

function startOfWeek(date: dayjs.Dayjs): dayjs.Dayjs {
  const offset = (date.day() + 6) % 7;
  return date.subtract(offset, 'day').startOf('day');
}

const initialStart = startOfWeek(dayjs());
const range = ref<Date[]>([initialStart.toDate(), initialStart.add(6, 'day').toDate()]);

const isLoading = ref(false);
const isSaving = ref(false);
const board = ref<WeeklyBoard | null>(null);

const showAssign = ref(false);
const assignPreset = ref<AssignPreset | undefined>(undefined);

const showDetail = ref(false);
const detail = ref<WeeklyBoardAssignment | null>(null);
const detailDispatches = ref<WeeklyBoardDispatch[]>([]);
const detailDispatchesLoading = ref(false);

const draggedAssignment = ref<WeeklyBoardAssignment | null>(null);
const dropTarget = ref<{ recruiterId: string; date: string } | null>(null);

const days = computed<WeekDay[]>(() => {
  const [from, to] = range.value;
  if (!from || !to) return [];
  const start = dayjs(from);
  const end = dayjs(to);
  const today = dayjs().format(dateFormat);
  const result: WeekDay[] = [];
  let cursor = start;
  while (!cursor.isAfter(end, 'day')) {
    const date = cursor.format(dateFormat);
    result.push({
      date,
      weekday: cursor.format('ddd').toUpperCase(),
      monthShort: cursor.format('MMM'),
      dayNum: cursor.date(),
      isToday: date === today,
    });
    cursor = cursor.add(1, 'day');
  }
  return result;
});

const rangeLabel = computed(() => {
  const [from, to] = range.value;
  if (!from || !to) return '';
  return `${dayjs(from).format('MMM D')} – ${dayjs(to).format('MMM D, YYYY')}`;
});

function loadBoard(): void {
  const [from, to] = range.value;
  if (!from || !to) return;
  isLoading.value = true;
  getWeeklyBoard({ from: dayjs(from).format(dateFormat), to: dayjs(to).format(dateFormat) })
    .then(response => {
      board.value = response;
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isLoading.value = false;
    });
}

function shiftWeek(direction: number): void {
  const [from, to] = range.value;
  range.value = [
    dayjs(from).add(direction * 7, 'day').toDate(),
    dayjs(to).add(direction * 7, 'day').toDate(),
  ];
}

function cardsFor(row: WeeklyBoardRecruiterRow, date: string): WeeklyBoardAssignment[] {
  return row.assignments.filter(a => dayjs(a.workDate).format(dateFormat) === date);
}

function statusClass(status: RequestStatus): string {
  if (status === RequestStatus.Filled) return 'is-filled';
  if (status === RequestStatus.Open) return 'is-open';
  return '';
}

function statusTagType(status: RequestStatus): string {
  if (status === RequestStatus.Filled) return 'is-success';
  if (status === RequestStatus.Open) return 'is-warning';
  return 'is-light';
}

function statusLabel(status: RequestStatus): string {
  return RequestStatusLabels[status] ?? '';
}

function formatDetailDate(date: string): string {
  return dayjs(date).format('ddd MMM D');
}

function openAssign(preset?: AssignPreset): void {
  assignPreset.value = preset;
  showAssign.value = true;
}

function openDetail(assignment: WeeklyBoardAssignment): void {
  detail.value = assignment;
  detailDispatches.value = [];
  showDetail.value = true;
  detailDispatchesLoading.value = true;
  getRequestDispatches(assignment.requestId)
    .then(response => {
      detailDispatches.value = response;
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      detailDispatchesLoading.value = false;
    });
}

function formatSentAt(sentAt: string | null | undefined): string {
  return sentAt ? dayjs(sentAt).format('ddd MMM D') : '';
}

function onAssign(payload: AssignRecruitersPayload): void {
  isSaving.value = true;
  assignRecruiters(payload)
    .then(() => {
      showAlertSuccess('Recruiter assigned');
      showAssign.value = false;
      loadBoard();
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isSaving.value = false;
    });
}

function onDragStart(assignment: WeeklyBoardAssignment): void {
  draggedAssignment.value = assignment;
}

function onDragEnd(): void {
  draggedAssignment.value = null;
  dropTarget.value = null;
}

function onDragOver(recruiterId: string, date: string): void {
  if (!draggedAssignment.value) return;
  dropTarget.value = { recruiterId, date };
}

function onDragLeave(): void {
  dropTarget.value = null;
}

function isDragging(assignment: WeeklyBoardAssignment): boolean {
  const dragged = draggedAssignment.value;
  return !!dragged && dragged.requestId === assignment.requestId
    && dragged.recruiterId === assignment.recruiterId
    && dayjs(dragged.workDate).format(dateFormat) === dayjs(assignment.workDate).format(dateFormat);
}

function isDropTarget(recruiterId: string, date: string): boolean {
  return !!draggedAssignment.value && dropTarget.value?.recruiterId === recruiterId && dropTarget.value?.date === date;
}

function onDrop(toRecruiterId: string, toDate: string): void {
  const assignment = draggedAssignment.value;
  dropTarget.value = null;
  draggedAssignment.value = null;
  if (!assignment) return;

  const fromDate = dayjs(assignment.workDate).format(dateFormat);
  if (assignment.recruiterId === toRecruiterId && fromDate === toDate) return;

  isLoading.value = true;
  isSaving.value = true;
  moveAssignment({
    requestId: assignment.requestId,
    fromRecruiterId: assignment.recruiterId,
    fromWorkDate: fromDate,
    toRecruiterId,
    toWorkDate: toDate,
  })
    .then(() => {
      showAlertSuccess('Assignment moved');
      loadBoard();
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isLoading.value = false;
      isSaving.value = false;
    });
}

function confirmUnassign(assignment: WeeklyBoardAssignment): void {
  getDialog().confirm({
    title: 'Remove assignment',
    message: `Remove <strong>${assignment.recruiterName}</strong> from order #${assignment.numberId}?`,
    confirmText: 'Remove',
    cancelText: 'Cancel',
    type: 'is-danger',
    hasIcon: true,
    onConfirm: () => onUnassign(assignment),
  });
}

function onUnassign(assignment: WeeklyBoardAssignment): void {
  isSaving.value = true;
  unassignRecruiter({
    requestId: assignment.requestId,
    recruiterId: assignment.recruiterId,
    workDate: dayjs(assignment.workDate).format(dateFormat),
  })
    .then(() => {
      showAlertSuccess('Assignment removed');
      showDetail.value = false;
      loadBoard();
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isSaving.value = false;
    });
}

watch(range, loadBoard, { immediate: true });
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.weekly-board {
  .board-toolbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1rem;

    .toolbar-right {
      display: flex;
      align-items: center;
      gap: 1.25rem;
    }

    .legend {
      display: flex;
      gap: 1rem;
      font-size: 0.85rem;
      color: $grey-dark;
    }

    .legend-item {
      display: inline-flex;
      align-items: center;
      gap: 0.35rem;
    }

    .dot {
      width: 0.6rem;
      height: 0.6rem;
      border-radius: 50%;
      display: inline-block;

      &.is-open {
        background: $accent;
      }

      &.is-filled {
        background: $green;
      }
    }
  }

  .grid-wrapper {
    overflow-x: auto;
    border: 1px solid $gray-border;
    border-radius: 10px;
  }

  .board-grid {
    width: 100%;
    border-collapse: collapse;
    min-width: 980px;

    th {
      text-align: left;
      padding: 0.75rem 0.85rem;
      border-bottom: 1px solid $gray-border;
      background: $gray-bg;
      font-weight: 700;

      .dow {
        display: block;
        font-size: 0.7rem;
        color: $grey-light;
      }

      .dom {
        display: block;
        font-size: 1rem;
      }

      &.is-today .dom {
        color: $primary;
      }
    }

    td {
      vertical-align: top;
      padding: 0.6rem;
      border-bottom: 1px solid $gray-border;
      border-left: 1px solid $gray-border;
    }

    .recruiter-col {
      width: 200px;

      .recruiter-name {
        font-weight: 700;
      }

      .recruiter-meta {
        font-size: 0.78rem;
        color: $grey-light;
      }
    }

    .day-col {
      min-width: 150px;
      transition: background 0.12s ease;

      &.is-drop-target {
        background: rgba($primary, 0.08);
        box-shadow: inset 0 0 0 2px $primary;
      }
    }

    .empty-row {
      text-align: center;
      color: $grey-light;
      padding: 2rem;
    }
  }

  .order-card {
    border: 1px solid $gray-border;
    border-left-width: 4px;
    border-radius: 8px;
    padding: 0.55rem 0.65rem;
    margin-bottom: 0.5rem;
    cursor: grab;
    transition: box-shadow 0.15s ease, opacity 0.15s ease;

    &:active {
      cursor: grabbing;
    }

    &.is-dragging {
      opacity: 0.4;
    }

    &:hover {
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
    }

    &.is-open {
      border-left-color: $accent;
    }

    &.is-filled {
      border-left-color: $green;
    }

    .card-company {
      font-size: 0.8rem;
      color: $grey-font;
    }

    .card-position {
      font-size: 0.8rem;
      font-weight: 600;
      color: $grey-dark;
    }

    .card-sent {
      font-size: 0.75rem;
      color: $grey-light;
      margin-top: 0.2rem;
    }
  }

  .add-card.button {
    border: 1px dashed $border-input;
    border-radius: 8px;
    background: transparent;
    color: $grey-light;
    height: auto;
    padding: 0.35rem;

    &:hover {
      border-color: $primary;
      color: $primary;
      background: transparent;
    }
  }
}

.detail-card {
  width: 100%;

  .detail-row {
    display: flex;
    align-items: center;
    gap: 0.4rem;
    margin-bottom: 0.6rem;
  }

  .detail-workers {
    margin-top: 0.6rem;
    border-top: 1px solid $gray-border;
    padding-top: 0.6rem;

    li {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 0.5rem;
      padding: 0.3rem 0;
      font-size: 0.85rem;

      .detail-worker-main {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        min-width: 0;
      }

      a {
        font-weight: 600;
        color: $grey-font;

        &:hover {
          color: $primary;
          text-decoration: underline;
        }
      }

      .detail-worker-email {
        font-size: 0.75rem;
        color: $grey-light;
      }

      .detail-worker-sent {
        flex-shrink: 0;
        font-size: 0.75rem;
        color: $grey-light;
        white-space: nowrap;
      }
    }
  }
}
</style>
