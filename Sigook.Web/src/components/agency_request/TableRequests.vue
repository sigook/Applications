<template>
  <div>
    <Export :url="'/api/AgencyRequest/File'" :params="serverParams" :fileName="'Requests'"
      @onDataLoading="(value) => emit('onDataLoading', value)">
      <template v-slot:actions>
        <b-checkbox v-if="tableConfig.showMyOrdersCheckbox" v-model="serverParams.onlyMine">My Orders</b-checkbox>
        <b-button v-if="tableConfig.showQuickActions" :disabled="checkedRows.length < 1"
          @click="onShowQuickActionsModal">
          Quick Actions
        </b-button>
      </template>
    </Export>
    <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
      :checkable="tableConfig.enableCheckable" pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
      focuseable default-sort="updatedAt" v-model:current-page="serverParams.pageIndex" v-model:checked-rows="checkedRows"
      @page-change="onPageChange" @sort="onSortChange" @cellclick="onCellClick">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="numberId" label="Order ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
              @keypress="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            <router-link :to="{ path: '/agency-request/' + props.row.id }">
              <p>{{ props.row.numberId }}</p>
            </router-link>
            <p v-if="props.row.isAsap" class="asap">{{ "Asap" }}</p>
            <p v-if="props.row.workerSalary" class="asap">DH</p>
            <b-icon v-if="props.row.vaccinationRequired" icon="needle" size="is-small"></b-icon>
          </template>
        </b-table-column>
        <b-table-column field="companyFullName" label="Client" :visible="!companyId" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.companyFullName" placeholder="Search..." icon="magnify" size="is-small"
              @keypress="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            <router-link :to="{ path: '/agency-companies/company/' + props.row.companyProfileId }">
              {{ props.row.companyFullName }}
            </router-link>
          </template>
        </b-table-column>
        <b-table-column field="location" label="Location" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.location" placeholder="Search..." icon="magnify" size="is-small"
              @keypress="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.location }}
            <span v-if="props.row.entrance"> - {{ props.row.entrance }}</span>
          </template>
        </b-table-column>
        <b-table-column field="jobTitle" label="Position" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.jobTitle" placeholder="Search..." icon="magnify" size="is-small"
              @keypress="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.jobTitle }}
            <i class="fz-2 block mb-0" v-if="props.row.billingTitle">{{ props.row.billingTitle }}</i>
            <i class="fz-2 block">{{ dateFromNow(props.row.createdAt) }}</i>
          </template>
        </b-table-column>
        <b-table-column field="updatedAt" label="Last Update" sortable searchable>
          <template v-slot:searchable>
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
              :icon-right="lastUpdateDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onLastUpdateCleared" range v-model="lastUpdateDatesSelected"
              @update:modelValue="onLastUpdateSelected" append-to-body>
            </b-datepicker>
          </template>
          <template v-slot="props">
            {{ dateMonth(props.row.updatedAt) }}
          </template>
        </b-table-column>
        <b-table-column field="startAt" sortable searchable>
          <template v-slot:header>
            <p class="fw-600">Duration</p>
            <p class="fw-600">(Start - End)</p>
          </template>
          <template v-slot:searchable>
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
              :icon-right="startAtDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onStartAtCleared" range v-model="startAtDatesSelected" @update:modelValue="onStartAtSelected"
              append-to-body>
            </b-datepicker>
          </template>
          <template v-slot="props">
            {{ dateMonth(props.row.startAt) }}
            <span v-if="props.row.durationTerm !== DurationTerm.LongTerm">
              - {{ dateMonth(props.row.finishAt) }}
            </span>
            <span
              v-if="(props.row.requestStatus === RequestStatus.Filled || props.row.requestStatus === RequestStatus.Cancelled) && props.row.durationTerm === DurationTerm.LongTerm">
              - {{ dateMonth(props.row.finishAt) }}
            </span>
            <AgencyShift class="fz-2 d-block" :requestId="props.row.id" :displayShift="props.row.displayShift" />
            <i class="fz-2 d-block">
              {{ DurationTermLabels[props.row.durationTerm] }} - {{ EmploymentTypeLabels[props.row.employmentType] }}
            </i>
          </template>
        </b-table-column>
        <b-table-column field="displayRecruiters" label="Recruiter" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.displayRecruiters" placeholder="Search..." icon="magnify" size="is-small"
              @keypress="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            <div v-if="props.row.displayRecruiters" class="capitalize is-inline-block v-middle">
              {{ breakWord(props.row.displayRecruiters) }}
              <button v-if="tableConfig.showRecruiterModal" type="button"
                class="btn-icon-sm btn-icon-worker-plus is-inline-block v-middle"></button>
            </div>
            <div v-else>
              <span class="op3">Recruiter</span>
              <button v-if="tableConfig.showRecruiterModal" type="button"
                class="btn-icon-sm btn-icon-worker-plus is-inline-block v-middle"></button>
            </div>
          </template>
        </b-table-column>
        <b-table-column field="salesRepresentative" label="Sales Rep" :visible="tableConfig.showSalesRepColumn" sortable
          searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.salesRepresentative" placeholder="Search..." icon="magnify" size="is-small"
              @keypress="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.salesRepresentative || '' }}
          </template>
        </b-table-column>
        <b-table-column field="workerRate" label="Rate / Salary" sortable searchable>
          <template v-slot:searchable>
            <b-field>
              <b-input placeholder="From" icon="magnify" size="is-small" v-model="serverParams.rateFrom"
                @keypress="onInputEntered"></b-input>
              <b-input placeholder="To" icon="magnify" size="is-small" v-model="serverParams.rateTo"
                @keypress="onInputEntered"></b-input>
            </b-field>
          </template>
          <template v-slot="props">
            {{ currency(props.row.workerRate || props.row.workerSalary) }}
          </template>
        </b-table-column>
        <b-table-column field="workersQuantityWorking" sortable>
          <template v-slot:header>
            <p class="fw-600">Workers</p>
            <p class="fw-600">({{ totalQuantityWorking }} / {{ totalQuantity }})</p>
          </template>
          <template v-slot="props">
            {{ props.row.workersQuantityWorking }} / {{ props.row.workersQuantity }}
          </template>
        </b-table-column>
        <b-table-column field="notesCount" label="Notes" :visible="tableConfig.showNotesColumn" v-slot="props">
          <div @click="onNote(props.row, true)">
            <b-tag icon="note-multiple" rounded>
              <label v-if="props.row.notesCount">{{ props.row.notesCount }}</label>
            </b-tag>
          </div>
          <div v-if="props.row.showNotes" class="notes-tooltip">
            <ModalNotes :can-create="false" :user-id="props.row.id" :on-get="getNotes"
              :on-create="createNote" :on-update="updateNote"
              :on-delete="deleteNote" @onUpdateNote="(val) => onUpdateNote(props.row, val.size)"
              @close="onNote(props.row, false)">
            </ModalNotes>
          </div>
        </b-table-column>
        <b-table-column field="status" label="Status" searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
              field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusChange" append-to-body>
            </b-taginput>
          </template>
          <template v-slot="props">
            <div class="text-center">
              <b-tooltip :label="RequestStatusLabels[props.row.requestStatus]" type="is-dark" append-to-body>
                <div class="status-dot-container">
                  <img v-if="props.row.requestStatus === RequestStatus.Filled" src="../../assets/images/check_white.png" alt="check"
                    class="request-check" />
                  <div class="dot-status" :class="getStatusClass(props.row)"></div>
                </div>
              </b-tooltip>
            </div>
          </template>
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
            <template #trigger>
              <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
            </template>
            <b-dropdown-item aria-role="listitem"
              @click="router.push({ path: '/agency-request/' + props.row.id, query: { tab: 'Applicants' } })">
              Applicants
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem"
              @click="router.push({ path: '/agency-request/' + props.row.id, query: { tab: 'Workers' } })">
              Workers
            </b-dropdown-item>
          </b-dropdown>
        </b-table-column>
      </template>
    </b-table>

    <!-- recruiters list -->
    <b-modal v-if="tableConfig.showRecruiterModal" v-model="showRecruitersModal" @close="showRecruitersModal = false"
      width="500px">
      <PersonnelList :recruiters="recruiters" :request="currentRequest" @selectUser="() => onUpdateRecruiter()"
        @removeUser="() => onUpdateRecruiter()" />
    </b-modal>

    <!-- Quick Actions -->
    <b-modal v-model="showQuickActions" @close="showQuickActions = false" width="500px">
      <div class="p-3">
        <div class="container-flex">
          <div class="col-12 col-padding">
            <b-field label="Is Asap">
              <b-switch v-model="quickActions.isAsap">
                {{ quickActions.isAsap ? 'Yes' : 'No' }}
              </b-switch>
            </b-field>
          </div>
          <div class="col-12 col-padding">
            <b-button type="is-primary" @click="bulkUpdateIsAsap">Save</b-button>
          </div>
        </div>
      </div>
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useAgencyStore } from '@/stores/agency';
import { appGlobals } from '@/varaibles';
import { showAlertError } from "@/utils/toast";
import { dateFromNow, dateMonth, breakWord, currency } from '@/utils/filters';
import { useBillingAdmin } from '@/composables/useBillingAdmin';
import { updateIsAsapRequests } from "@/api/agencyCompanyApi";
import { getAgencyRequests } from "@/api/agencyRequestApi";
import {
  getAgencyRequestNotes,
  createAgencyRequestNote,
  updateAgencyRequestNote,
  deleteAgencyRequestNote
} from "@/api/agencyNoteApi";
import type { NotesFetchPayload, NotesCreatePayload, NotesUpdatePayload, NotesDeletePayload } from '@/types/agency';
import {
  DurationTerm,
  DurationTermLabels,
  EmploymentTypeLabels,
  RequestStatus,
  RequestStatusLabels
} from "@/constants/enums";
import ModalNotes from '../notes/ModalNotes.vue';
import PersonnelList from '../../components/agency_request/PersonnelListModal.vue';
import AgencyShift from '../../components/agency_request/AgencyShiftDetail.vue';
import Export from '@/components/Export.vue';

const props = defineProps<{ totalItems?: number; companyId?: any; agencyId?: any; config?: any }>();
const emit = defineEmits<{
  (e: 'onDataLoading', value: boolean): void;
  (e: 'update:totalItems', value: number): void;
}>();

const router = useRouter();
const agencyStore = useAgencyStore();
const { isPayrollManager } = useBillingAdmin();

const defaultConfig = {
  showMyOrdersCheckbox: true,
  showQuickActions: true,
  enableCheckable: true,
  showRecruiterModal: true,
  showSalesRepColumn: true,
  showNotesColumn: true
};
const showRecruitersModal = ref(false);
const showQuickActions = ref(false);
const recruiters = ref<any>(null);
const currentRequest = ref<any>(null);
const currentIndex = ref<number | null>(null);
const getNotes = ({ userId, pagination }: NotesFetchPayload) => getAgencyRequestNotes(userId, pagination);
const createNote = ({ userId, model }: NotesCreatePayload) => createAgencyRequestNote(userId, model);
const updateNote = ({ userId, id, model }: NotesUpdatePayload) => updateAgencyRequestNote(userId, id, model);
const deleteNote = ({ userId, id }: NotesDeletePayload) => deleteAgencyRequestNote(userId, id);
const statuses = [
  { id: 1, value: appGlobals.$statusDisplayOpen },
  { id: 3, value: appGlobals.$statusDisplayFilled },
  { id: 4, value: appGlobals.$statusDisplayCancelled }
];
const statusesSelected = ref<any[]>([]);
const lastUpdateDatesSelected = ref<any[]>([]);
const startAtDatesSelected = ref<any[]>([]);
const rows = ref<any[]>([]);
const checkedRows = ref<any[]>([]);
const serverParams = reactive<any>({
  onlyMine: false,
  sortBy: 7,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30
});
const quickActions = reactive<any>({});

const tableConfig = computed(() => ({ ...defaultConfig, ...props.config }));
const totalQuantityWorking = computed(() => {
  if (rows.value.length > 0) {
    return rows.value.map((r) => r.workersQuantityWorking).reduce((a, b) => a + b);
  }
  return 0;
});
const totalQuantity = computed(() => {
  if (rows.value.length > 0) {
    return rows.value.map((r) => r.workersQuantity).reduce((a, b) => a + b);
  }
  return 0;
});

function onCellClick(row: any, column: any, rowIndex: number) {
  switch (column.field) {
    case 'workersQuantityWorking':
      router.push({
        path: `/agency-request/${row.id}`,
        query: { tab: 'Workers' }
      });
      break;
    case 'displayRecruiters':
      if (tableConfig.value.showRecruiterModal) {
        onShowRecruitersModal(row, rowIndex);
      }
      break;
    case 'notesCount':
    case 'actions':
      break;
    default:
      router.push(`/agency-request/${row.id}`);
      break;
  }
}

function onPageChange(params: any) {
  serverParams.pageIndex = params;
  loadRequests();
}

function onSortChange(field: string, order: string) {
  switch (field) {
    case 'numberId':
      serverParams.sortBy = 0;
      break;
    case 'companyFullName':
      serverParams.sortBy = 1;
      break;
    case 'jobTitle':
      serverParams.sortBy = 2;
      break;
    case 'startAt':
      serverParams.sortBy = 3;
      break;
    case 'displayRecruiters':
      serverParams.sortBy = 4;
      break;
    case 'workerRate':
      serverParams.sortBy = 5;
      break;
    case 'workersQuantityWorking':
      serverParams.sortBy = 6;
      break;
    case 'updatedAt':
      serverParams.sortBy = 7;
      break;
    case 'salesRepresentative':
      serverParams.sortBy = 8;
      break;
  }
  serverParams.isDescending = order !== 'asc';
  loadRequests();
}

function onStatusChange() {
  serverParams.statuses = statusesSelected.value.map((ss) => ss.id);
  loadRequests();
}

function onLastUpdateSelected() {
  serverParams.lastUpdateFrom = lastUpdateDatesSelected.value[0];
  serverParams.lastUpdateTo = lastUpdateDatesSelected.value[1];
  loadRequests();
}

function onLastUpdateCleared() {
  lastUpdateDatesSelected.value = [];
  onLastUpdateSelected();
}

function onStartAtSelected() {
  serverParams.startAtFrom = startAtDatesSelected.value[0];
  serverParams.startAtTo = startAtDatesSelected.value[1];
  loadRequests();
}

function onStartAtCleared() {
  startAtDatesSelected.value = [];
  onStartAtSelected();
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    loadRequests();
  }
}

function onNote(row: any, status: boolean) {
  const index = rows.value.findIndex((r) => r.id === row.id);
  rows.value[index].showNotes = status;
}

function onUpdateNote(row: any, size: number) {
  const index = rows.value.findIndex((r) => r.id === row.id);
  rows.value[index].notesCount = size;
}

function onShowRecruitersModal(item: any, index: number) {
  currentRequest.value = item;
  currentIndex.value = index;
  recruiters.value = item.displayRecruiters ? item.displayRecruiters.split('|') : [];
  showRecruitersModal.value = true;
}

function onUpdateRecruiter() {
  showRecruitersModal.value = false;
  loadRequests();
}

function getStatusClass(row: any) {
  if (row.requestStatus === RequestStatus.Open &&
    row.workersQuantityWorking > 0 &&
    row.workersQuantityWorking < row.workersQuantity) {
    return 'status-inprogress';
  }
  return 'status-' + RequestStatusLabels[row.requestStatus].toLowerCase();
}

function loadRequests() {
  checkedRows.value = [];
  emit('onDataLoading', true);
  if (!props.companyId && !props.agencyId) {
    agencyStore.updateAgencyRequestFilter(serverParams);
  }
  getAgencyRequests(serverParams)
    .then((requestsResponse) => {
      rows.value = requestsResponse.items.map((i: any) => ({ ...i, actions: null, showNotes: false, notesCount: i.notesCount || 0 }));
      emit('update:totalItems', requestsResponse.totalItems);
      emit('onDataLoading', false);
    })
    .catch(() => emit('onDataLoading', false));
}

function onShowQuickActionsModal() {
  quickActions.isAsap = false;
  showQuickActions.value = true;
}

function bulkUpdateIsAsap() {
  emit('onDataLoading', true);
  const payload = {
    ids: checkedRows.value.map((cr) => cr.id),
    isAsap: quickActions.isAsap
  };
  updateIsAsapRequests(payload)
    .then(() => {
      showQuickActions.value = false;
      loadRequests();
    }).catch((error) => {
      showQuickActions.value = false;
      showAlertError(error);
      emit('onDataLoading', false);
    });
}

watch(() => serverParams.onlyMine, () => {
  loadRequests();
});

if (!props.companyId && !props.agencyId) {
  if (agencyStore.agencyRequestFilter) {
    Object.assign(serverParams, agencyStore.agencyRequestFilter);
    if (serverParams.statuses) {
      statusesSelected.value = statuses.filter((s) => serverParams.statuses.some((sps: any) => sps == s.id));
    }
    if (serverParams.lastUpdateFrom && serverParams.lastUpdateTo) {
      lastUpdateDatesSelected.value[0] = serverParams.lastUpdateFrom;
      lastUpdateDatesSelected.value[1] = serverParams.lastUpdateTo;
    }
    if (serverParams.startAtFrom && serverParams.startAtTo) {
      startAtDatesSelected.value[0] = serverParams.startAtFrom;
      startAtDatesSelected.value[1] = serverParams.startAtTo;
    }
  } else {
    serverParams.onlyMine = !isPayrollManager.value;
  }
} else {
  serverParams.onlyMine = false;
  if (props.companyId) {
    serverParams.companyId = props.companyId;
  }
  if (props.agencyId) {
    serverParams.agencyId = props.agencyId;
  }
}
loadRequests();
</script>
