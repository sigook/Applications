<template>
  <div class="recruiter-board">
    <b-loading v-model="isLoading"></b-loading>

    <b-navbar type="is-primary" class="stats-navbar" :mobile-burger="false">
      <template #brand>
        <b-navbar-item tag="div" class="stats-name">
          {{ board?.recruiterName || 'Recruiter' }}
        </b-navbar-item>
      </template>
      <template #end>
        <b-navbar-item tag="div" class="stat">
          <span class="stat-value">{{ board?.ordersCount ?? 0 }}</span>
          <span class="stat-label">Requests this week</span>
        </b-navbar-item>
        <b-navbar-item tag="div" class="stat">
          <span class="stat-value">{{ board?.workersSent ?? 0 }}</span>
          <span class="stat-label">Workers sent</span>
        </b-navbar-item>
      </template>
    </b-navbar>

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

    <div class="day-tabs">
      <button class="day-tab" :class="{ 'is-active': activeDay === null }" @click="activeDay = null">
        <span class="tab-name">ALL</span>
        <span class="tab-num">7d</span>
      </button>
      <button v-for="day in days" :key="day.date" class="day-tab" :class="{ 'is-active': activeDay === day.date }"
        @click="activeDay = day.date">
        <span class="tab-name">{{ day.weekday }}</span>
        <span class="tab-num">{{ day.dayNum }}</span>
        <span v-if="hasAssignments(day.date)" class="tab-dot"></span>
      </button>
    </div>

    <div v-if="visibleDays.length === 0" class="empty-board">
      No assigned orders for this {{ activeDay ? 'day' : 'week' }}.
    </div>

    <div v-for="day in visibleDays" :key="day.date" class="day-section">
      <p class="day-heading">
        <b-icon icon="calendar-blank" size="is-small"></b-icon>
        {{ day.weekday }} {{ day.monthShort }} {{ day.dayNum }}
      </p>
      <div class="cards">
        <div v-for="assignment in assignmentsFor(day.date)" :key="assignment.requestId" class="order-card"
          :class="statusClass(assignment.status)">
          <div class="card-top">
            <div class="card-head-left">
              <router-link class="card-num" :to="{ name: 'agency-request', params: { id: assignment.requestId } }"
                target="_blank"><b-icon icon="clipboard-text-outline" size="is-small"></b-icon>
                # {{ assignment.numberId }}
              </router-link>
              <span v-if="assignment.isAsap || isDirectHiring(assignment)" class="request-flags">
                <span v-if="assignment.isAsap" class="request-flag request-flag--asap">Asap</span>
                <span v-if="isDirectHiring(assignment)" class="request-flag request-flag--dh">DH</span>
              </span>
            </div>
            <b-tag :type="statusTagType(assignment.status)" rounded>{{ statusLabel(assignment.status) }}</b-tag>
          </div>
          <p class="card-position">{{ assignment.jobTitle }}</p>
          <div class="card-meta container-flex">
            <span class="col-12"><b-icon icon="office-building-outline" size="is-small"></b-icon>
              {{ assignment.companyName }}
            </span>
            <span v-if="locationLabel(assignment)" class="col-12"><b-icon icon="map-marker-outline"
                size="is-small"></b-icon>
              {{ locationLabel(assignment) }}
            </span>
          </div>

          <collapse-section v-if="assignment.dispatches.length > 0" class="sent-workers" variant="compact"
            :model-value="false">
            <template #title>SENT WORKERS ({{ assignment.dispatches.length }})</template>
            <div v-for="worker in assignment.dispatches" :key="worker.workerProfileId" class="sent-worker">
              <div class="sent-worker-info">
                <span class="sent-worker-avatar">{{ workerInitials(worker.fullName) }}</span>
                <div>
                  <router-link class="sent-worker-name"
                    :to="{ name: 'workerDetail', params: { id: worker.workerProfileId } }" target="_blank">
                    {{ worker.fullName }}
                  </router-link>
                  <p class="sent-worker-email">{{ worker.email }}</p>
                </div>
              </div>
              <b-button class="remove-worker" type="is-ghost" size="is-small" icon-right="close" :disabled="isSaving"
                @click="onRemove(assignment, worker.workerProfileId)"></b-button>
            </div>
          </collapse-section>

          <b-button v-if="!isPastDay(assignment.workDate)" type="is-primary" expanded icon-left="account-plus"
            class="add-worker-btn" @click="openAdd(assignment)">
            Add worker
          </b-button>
        </div>
      </div>
    </div>

    <b-modal v-model="showAdd" :width="560" scroll="keep" :destroy-on-hide="true">
      <add-workers-modal v-if="activeAssignment" :assignment="activeAssignment" :saving="isSaving" @send="onSend"
        @close="showAdd = false" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import dayjs from 'dayjs';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { getRecruiterWeeklyBoard, addWorkers, removeWorker } from '@/api/weeklyBoardApi';
import { isDirectHiring } from '@/utils/directHiring';
import { locationLabel } from '@/utils/locationLabel';
import { RequestStatus, RequestStatusLabels } from '@/constants/enums';
import type { RecruiterWeeklyBoard, WeeklyBoardAssignment, WeekDay } from '@/types/weeklyBoard';
import AddWorkersModal from './AddWorkersModal.vue';
import CollapseSection from '@/components/CollapseSection.vue';

const dateFormat = 'YYYY-MM-DD';

function startOfWeek(date: dayjs.Dayjs): dayjs.Dayjs {
  const offset = (date.day() + 6) % 7;
  return date.subtract(offset, 'day').startOf('day');
}

const initialStart = startOfWeek(dayjs());
const range = ref<Date[]>([initialStart.toDate(), initialStart.add(6, 'day').toDate()]);

const isLoading = ref(false);
const isSaving = ref(false);
const board = ref<RecruiterWeeklyBoard | null>(null);
const activeDay = ref<string | null>(null);

const showAdd = ref(false);
const activeAssignment = ref<WeeklyBoardAssignment | null>(null);

const days = computed<WeekDay[]>(() => {
  const [from, to] = range.value;
  if (!from || !to) return [];
  const end = dayjs(to);
  const result: WeekDay[] = [];
  let cursor = dayjs(from);
  while (!cursor.isAfter(end, 'day')) {
    result.push({
      date: cursor.format(dateFormat),
      weekday: cursor.format('ddd').toUpperCase(),
      monthShort: cursor.format('MMM').toUpperCase(),
      dayNum: cursor.date(),
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

const visibleDays = computed<WeekDay[]>(() => {
  if (activeDay.value) return days.value.filter(d => d.date === activeDay.value);
  return days.value.filter(d => hasAssignments(d.date));
});

function assignmentsFor(date: string): WeeklyBoardAssignment[] {
  return (board.value?.assignments ?? []).filter(a => dayjs(a.workDate).format(dateFormat) === date);
}

function isPastDay(date: string): boolean {
  return dayjs(date).isBefore(dayjs(), 'day');
}

function hasAssignments(date: string): boolean {
  return assignmentsFor(date).length > 0;
}

function workerInitials(name: string): string {
  return name
    .split(' ')
    .filter(Boolean)
    .slice(0, 2)
    .map(part => part[0]?.toUpperCase() ?? '')
    .join('');
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

function loadBoard(): void {
  const [from, to] = range.value;
  if (!from || !to) return;
  isLoading.value = true;
  getRecruiterWeeklyBoard({ from: dayjs(from).format(dateFormat), to: dayjs(to).format(dateFormat) })
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
  activeDay.value = null;
}

function openAdd(assignment: WeeklyBoardAssignment): void {
  activeAssignment.value = assignment;
  showAdd.value = true;
}

function onSend(workerProfileIds: string[]): void {
  if (!activeAssignment.value) return;
  isSaving.value = true;
  addWorkers({
    requestId: activeAssignment.value.requestId,
    workDate: dayjs(activeAssignment.value.workDate).format(dateFormat),
    workerProfileIds,
  })
    .then(() => {
      showAlertSuccess('Workers sent');
      showAdd.value = false;
      loadBoard();
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isSaving.value = false;
    });
}

function onRemove(assignment: WeeklyBoardAssignment, workerProfileId: string): void {
  isLoading.value = true;
  isSaving.value = true;
  removeWorker({
    requestId: assignment.requestId,
    workDate: dayjs(assignment.workDate).format(dateFormat),
    workerProfileId,
  })
    .then(() => {
      showAlertSuccess('Worker removed');
      loadBoard();
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isLoading.value = false;
      isSaving.value = false;
    });
}

watch(range, loadBoard, { immediate: true });
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.recruiter-board {
  .range-nav {
    margin-bottom: 1.25rem;
  }

  .day-tabs {
    display: flex;
    gap: 0.6rem;
    margin-bottom: 1.5rem;
    flex-wrap: wrap;
  }

  .day-tab {
    position: relative;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.15rem;
    min-width: 3.4rem;
    padding: 0.5rem 0.6rem;
    border: 1px solid $gray-border;
    border-radius: 10px;
    background: $white;
    cursor: pointer;
    transition: all 0.15s ease;

    .tab-name {
      font-size: 0.7rem;
      font-weight: 700;
      color: $grey-light;
    }

    .tab-num {
      font-size: 1.1rem;
      font-weight: 800;
    }

    .tab-dot {
      width: 0.35rem;
      height: 0.35rem;
      border-radius: 50%;
      background: $primary;
    }

    &.is-active {
      background: $primary;
      border-color: $primary;

      .tab-name,
      .tab-num {
        color: $white;
      }

      .tab-dot {
        background: $white;
      }
    }
  }

  .empty-board {
    text-align: center;
    color: $grey-light;
    padding: 2.5rem;
    border: 1px dashed $gray-border;
    border-radius: 10px;
  }

  .day-section {
    margin-bottom: 1.75rem;

    .day-heading {
      font-size: 0.8rem;
      font-weight: 700;
      color: $grey-dark;
      text-transform: uppercase;
      margin-bottom: 0.75rem;
    }

    .cards {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
      gap: 1rem;
    }
  }

  .order-card {
    border: 1px solid $gray-border;
    border-top-width: 3px;
    border-radius: 10px;
    padding: 1rem;
    background: $white;

    &.is-open {
      border-top-color: $accent;
    }

    &.is-filled {
      border-top-color: $green;
    }

    .card-position {
      font-weight: 700;
      margin-bottom: 0.4rem;
    }

    .card-meta {
      font-size: 0.82rem;
      color: $grey-dark;
      margin-bottom: 0.75rem;
      row-gap: 0.25rem;

      .col-6 {
        min-width: 0;
      }
    }

    .sent-workers {
      margin-top: 0.75rem;
    }

    .sent-worker {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.45rem 0.55rem;
      border: 1px solid $gray-border;
      border-radius: 8px;
      margin-bottom: 0.4rem;

      .sent-worker-info {
        display: flex;
        align-items: center;
        gap: 0.55rem;
      }

      .sent-worker-avatar {
        width: 2rem;
        height: 2rem;
        border-radius: 50%;
        background: $primary;
        color: $white;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 0.72rem;
        font-weight: 700;
      }

      .sent-worker-name {
        display: inline-block;
        font-weight: 600;
        font-size: 0.85rem;
        color: $grey-font;

        &:hover {
          color: $primary;
          text-decoration: underline;
        }
      }

      .sent-worker-email {
        font-size: 0.75rem;
        color: $grey-light;
      }
    }

    .add-worker-btn {
      margin-top: 0.85rem;
    }
  }
}
</style>
