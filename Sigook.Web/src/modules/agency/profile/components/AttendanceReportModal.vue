<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="title is-5 mb-1">Attendance report</h2>
    <p class="mb-4">Mon–Fri: 8 h regular after 1 h lunch, the rest is overtime. Sat/Sun: all hours are overtime.</p>

    <div class="columns is-multiline">
      <div class="column is-3-desktop is-6-tablet is-12-mobile">
        <b-field label="User">
          <b-select v-model="userId" expanded>
            <option value="">All users</option>
            <option v-for="user in users" :key="user.userId" :value="user.userId">{{ user.name }}</option>
          </b-select>
        </b-field>
      </div>
      <div class="column is-3-desktop is-6-tablet is-12-mobile">
        <b-field label="Dates (From - To)">
          <b-datepicker v-model="dates" range :first-day-of-week="1" />
        </b-field>
      </div>
    </div>

    <div class="columns is-mobile is-multiline">
      <div class="column is-4-desktop is-12-mobile">
        <div class="box">
          <p class="heading">Worked</p>
          <p class="title is-4">{{ formatHours(report.totals.workedHours) }} h</p>
        </div>
      </div>
      <div class="column is-4-desktop is-12-mobile">
        <div class="box">
          <p class="heading">Regular</p>
          <p class="title is-4">{{ formatHours(report.totals.regularHours) }} h</p>
        </div>
      </div>
      <div class="column is-4-desktop is-12-mobile">
        <div class="box attendance-overtime">
          <p class="heading">Overtime</p>
          <p class="title is-4">{{ formatHours(report.totals.overtimeHours) }} h</p>
        </div>
      </div>
    </div>

    <SigookGrid :data="report.items" :paginated="false" :fit-viewport="false">
      <template #actions>
        <b-button icon-left="file-excel" :disabled="report.items.length === 0" @click="exportReport">
          Export Excel
        </b-button>
      </template>
      <b-table-column field="name" label="User" v-slot="props">
        {{ props.row.name }}
      </b-table-column>
      <b-table-column field="date" label="Date" v-slot="props">
        {{ dayjs(props.row.date).format('ddd, MMM D') }}
      </b-table-column>
      <b-table-column field="clockIn" label="In" v-slot="props">
        {{ dayjs(props.row.clockIn).format('HH:mm') }}
      </b-table-column>
      <b-table-column field="clockOut" label="Out" v-slot="props">
        {{ props.row.clockOut ? dayjs(props.row.clockOut).format('HH:mm') : '—' }}
      </b-table-column>
      <b-table-column field="workedHours" label="Worked" numeric v-slot="props">
        {{ formatHours(props.row.workedHours) }}
      </b-table-column>
      <b-table-column field="lunchHours" label="Lunch" numeric v-slot="props">
        {{ formatHours(props.row.lunchHours) }}
      </b-table-column>
      <b-table-column field="regularHours" label="Regular" numeric v-slot="props">
        {{ formatHours(props.row.regularHours) }}
      </b-table-column>
      <b-table-column field="overtimeHours" label="Overtime" numeric cell-class="attendance-overtime-cell"
        v-slot="props">
        {{ formatHours(props.row.overtimeHours) }}
      </b-table-column>
      <b-table-column field="status" label="Status" v-slot="props">
        <b-taglist>
          <b-tag v-if="props.row.isMissingClockOut" type="is-danger is-light">Missing out</b-tag>
          <b-tag v-if="props.row.isWeekend" type="is-warning is-light">Weekend</b-tag>
          <b-tooltip v-if="props.row.isEdited" type="is-dark" append-to-body
            :label="`${props.row.editedBy ?? ''}: ${props.row.editReason ?? ''}`">
            <b-tag type="is-info is-light">Edited</b-tag>
          </b-tooltip>
        </b-taglist>
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <b-button type="is-info" outlined rounded icon-right="pencil" aria-label="Edit punch"
          @click="openEdit(props.row)"></b-button>
      </b-table-column>
      <template #footer>
        <template v-if="report.items.length > 0">
          <th>Total</th>
          <th></th>
          <th></th>
          <th></th>
          <th class="has-text-right">{{ formatHours(report.totals.workedHours) }}</th>
          <th class="has-text-right">{{ formatHours(report.totals.lunchHours) }}</th>
          <th class="has-text-right">{{ formatHours(report.totals.regularHours) }}</th>
          <th class="has-text-right attendance-overtime-cell">{{ formatHours(report.totals.overtimeHours) }}</th>
          <th></th>
          <th></th>
        </template>
      </template>
    </SigookGrid>

    <b-modal custom-content-class="card" v-model="showEdit" width="520px">
      <AttendanceEditModal v-if="editing" :attendance="editing" @saved="onSaved" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import dayjs from 'dayjs';
import { downloadAttendanceReport, getAttendanceReport } from '@/modules/agency/profile/api';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { formatHours } from '@/modules/agency/profile/attendance';
import type { AgencyPersonnelListItem, UserAttendanceListItem, UserAttendanceReport, UserAttendanceReportFilter } from '@/modules/agency/profile/types';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import AttendanceEditModal from '@/modules/agency/profile/components/AttendanceEditModal.vue';

defineProps<{ users: AgencyPersonnelListItem[] }>();

function thisWeek(): Date[] {
  const start = dayjs().subtract((dayjs().day() + 6) % 7, 'day').startOf('day');
  return [start.toDate(), start.add(6, 'day').toDate()];
}

const isLoading = ref(false);
const userId = ref('');
const dates = ref<Date[]>(thisWeek());
const report = ref<UserAttendanceReport>({
  items: [],
  totals: { workedHours: 0, lunchHours: 0, regularHours: 0, overtimeHours: 0 },
});
const showEdit = ref(false);
const editing = ref<UserAttendanceListItem | null>(null);

const filter = computed<UserAttendanceReportFilter | null>(() => {
  if (dates.value?.length !== 2) return null;
  return {
    userId: userId.value || undefined,
    from: dayjs(dates.value[0]).format('YYYY-MM-DD'),
    to: dayjs(dates.value[1]).format('YYYY-MM-DD'),
  };
});

function load() {
  if (!filter.value) return;
  isLoading.value = true;
  getAttendanceReport(filter.value)
    .then(response => { report.value = response; })
    .catch(showAlertError)
    .finally(() => { isLoading.value = false; });
}

function exportReport() {
  if (!filter.value) return;
  isLoading.value = true;
  downloadAttendanceReport(filter.value)
    .then(blob => downloadFile(blob, `Attendance_${filter.value?.from}_${filter.value?.to}`))
    .catch(showAlertError)
    .finally(() => { isLoading.value = false; });
}

function openEdit(row: UserAttendanceListItem) {
  editing.value = row;
  showEdit.value = true;
}

function onSaved() {
  showEdit.value = false;
  editing.value = null;
  load();
}

watch(filter, load, { immediate: true });
</script>

<style scoped lang="scss">
@import "@/assets/scss/variables";

.attendance-overtime {
  border: 2px solid $accent;
  background-color: rgba($accent, 0.12);

  .heading,
  .title {
    color: $accent-text;
  }
}

:deep(.attendance-overtime-cell) {
  color: $accent-text;
  font-weight: 700;
}
</style>
