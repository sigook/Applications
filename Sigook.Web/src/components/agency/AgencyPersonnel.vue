<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :data="users" :refresh="getUsers">
      <template #actions>
        <b-button v-if="isAdmin" icon-left="clock-outline" @click="showAttendance = true">Attendance report</b-button>
        <b-button v-if="isAdmin" icon-left="plus" @click="openCreateModal">Add</b-button>
      </template>
      <b-table-column field="name" label="Name" searchable v-slot="props">
        {{ props.row.name }}
      </b-table-column>
      <b-table-column field="email" label="Email" searchable v-slot="props">
        {{ props.row.email }}
      </b-table-column>
      <b-table-column field="role" label="Role" v-slot="props">
        {{ roleLabels[props.row.role] || props.row.role }}
      </b-table-column>
      <b-table-column field="today" label="Today" :visible="isAdmin" v-slot="props">
        <b-tag :type="todayTag(props.row.userId).type">{{ todayTag(props.row.userId).label }}</b-tag>
      </b-table-column>
      <b-table-column field="actions" :visible="isAdmin" v-slot="props">
        <b-button type="is-info" outlined rounded icon-right="pencil" class="mr-2"
          @click="openEditModal(props.row)"></b-button>
        <b-button type="is-danger" outlined rounded icon-right="delete"
          @click="deleteUser(props.row.id)"></b-button>
      </b-table-column>
    </SigookGrid>

    <!-- Create / edit user modal-->
    <b-modal custom-content-class="card" v-model="showModal" @close="closeModal" width="500px">
      <personnel-form :personnel="currentPersonnel" @updateUsers="() => updateList()" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="showAttendance" width="1120px" @close="getAttendancesToday">
      <AttendanceReportModal v-if="showAttendance" :users="users" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertConfirm, showAlertError } from "@/utils/toast";
import { getAgencyPersonnel, deleteAgencyPersonnel, getAttendancesTodayForUsers } from "@/api/agencyApi";
import { useAdmin } from "@/composables/useAdmin";
import { roleLabels } from "@/security/roles";
import { formatHours } from "@/utils/attendance";
import { AttendanceStatus, type AgencyPersonnelListItem, type UserAttendanceToday } from "@/types/agency";
import PersonnelForm from "./AgencyPersonnelModal.vue";
import AttendanceReportModal from "./AttendanceReportModal.vue";
import dayjs from 'dayjs';
import SigookGrid from '@/components/SigookGrid.vue';

const { isAdmin } = useAdmin();

const isLoading = ref(false);
const showModal = ref(false);
const users = ref<AgencyPersonnelListItem[]>([]);
const currentPersonnel = ref<AgencyPersonnelListItem | null>(null);
const showAttendance = ref(false);
const attendancesToday = ref<UserAttendanceToday[]>([]);

function todayTag(userId: string): { label: string; type: string } {
  const attendance = attendancesToday.value.find(a => a.userId === userId);
  if (attendance?.status === AttendanceStatus.ClockedIn)
    return { label: `In since ${dayjs(attendance.clockIn).format('HH:mm')}`, type: 'is-danger is-light' };
  if (attendance?.status === AttendanceStatus.ClockedOut)
    return { label: `Closed · ${formatHours(attendance.workedHours)} h`, type: 'is-success is-light' };
  return { label: 'Not clocked in', type: '' };
}

function getAttendancesToday() {
  if (!isAdmin.value) return;
  getAttendancesTodayForUsers()
    .then(response => { attendancesToday.value = response; })
    .catch(showAlertError);
}

function getUsers() {
  isLoading.value = true;
  getAgencyPersonnel()
    .then((response) => {
      isLoading.value = false;
      users.value = response;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function openCreateModal() {
  currentPersonnel.value = null;
  showModal.value = true;
}

function openEditModal(personnel: AgencyPersonnelListItem) {
  currentPersonnel.value = personnel;
  showModal.value = true;
}

function closeModal() {
  currentPersonnel.value = null;
  showModal.value = false;
}

function updateList() {
  closeModal();
  getUsers();
}

function deleteUser(id: string) {
  showAlertConfirm('Are you sure?', 'You want to delete user.')
    .then(response => {
      if (response) {
        isLoading.value = true;
        deleteAgencyPersonnel(id)
          .then(() => {
            isLoading.value = false;
            getUsers();
          })
          .catch(error => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    });
}

getUsers();
getAttendancesToday();
</script>
