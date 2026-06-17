<template>
  <div>
    <Export :url="'/api/AgencyRequest/File'" :params="serverParams" :fileName="'Requests'"
      @onDataLoading="(value) => emit('onDataLoading', value)">
      <template v-slot:actions>
        <b-checkbox v-if="tableConfig.showMyRequestsCheckbox" v-model="serverParams.onlyMine">My Requests</b-checkbox>
        <b-dropdown v-if="tableConfig.showQuickActions"
          :key="quickActionsKey"
          aria-role="menu" position="is-bottom-left" :triggers="['click']" :close-on-click="false" append-to-body>
          <template #trigger="{ active }">
            <b-button :icon-right="active ? 'menu-up' : 'menu-down'">
              Quick Actions
            </b-button>
          </template>
          <b-dropdown-item aria-role="menuitem" custom :disabled="checkedRows.length < 1">
            <div class="quick-action-item">
              <span>Asap</span>
              <b-switch v-model="quickActions.isAsap" :disabled="checkedRows.length < 1"
                @update:modelValue="bulkUpdateIsAsap">
                {{ quickActions.isAsap ? 'Yes' : 'No' }}
              </b-switch>
            </div>
          </b-dropdown-item>
          <b-dropdown-item aria-role="menuitem" :disabled="checkedRows.length < 1"
            @click="onShowBulkCancelModal">
            Cancel requests
          </b-dropdown-item>
        </b-dropdown>
      </template>
    </Export>
    <div v-if="jobBoardsSummary.length" class="job-boards-summary">
      <span class="job-boards-summary__label">Posted in:</span>
      <b-tag v-for="s in jobBoardsSummary" :key="s.sourceId" rounded type="is-info is-light">
        {{ s.value }} ({{ s.count }})
      </b-tag>
    </div>
    <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
      :checkable="tableConfig.enableCheckable" pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" pagination-size="is-small"
      focuseable :default-sort="['numberId', 'desc']" v-model:current-page="serverParams.pageIndex" v-model:checked-rows="checkedRows"
      @page-change="onPageChange" @sort="onSortChange" @cellclick="onCellClick">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="numberId" label="ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
              @keypress="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            <div class="request-id-cell">
              <div v-if="props.row.isAsap || props.row.workerSalary" class="request-flags">
                <span v-if="props.row.isAsap" class="request-flag request-flag--asap">Asap</span>
                <span v-if="props.row.workerSalary" class="request-flag request-flag--dh">DH</span>
              </div>
              <router-link :to="{ path: '/agency-request/' + props.row.id }">
                <p>{{ props.row.numberId }}</p>
              </router-link>
              <b-icon v-if="props.row.vaccinationRequired" icon="needle" size="is-small"></b-icon>
            </div>
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
          </template>
        </b-table-column>
        <b-table-column field="createdAt" label="Created" sortable searchable>
          <template v-slot:searchable>
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
              :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onCreatedAtCleared" range v-model="createdAtDatesSelected"
              @update:modelValue="onCreatedAtSelected" append-to-body>
            </b-datepicker>
          </template>
          <template v-slot="props">
            {{ dateMonth(props.row.createdAt) }}
            <AgencyShift class="fz-2 d-block" :requestId="props.row.id" :displayShift="props.row.displayShift" />
          </template>
        </b-table-column>
        <b-table-column field="displayRecruiters" label="Recruiter" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.displayRecruiters" placeholder="Search..." icon="magnify" size="is-small"
              @keypress="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            <div v-if="props.row.displayRecruiters" class="text-capitalize is-inline-block align-middle">
              {{ breakWord(props.row.displayRecruiters) }}
            </div>
            <span v-else class="op3">—</span>
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
            <p class="fw-semibold">Workers</p>
            <p class="fw-semibold">({{ totalQuantityWorking }} / {{ totalQuantity }})</p>
          </template>
          <template v-slot="props">
            {{ props.row.workersQuantityWorking }} / {{ props.row.workersQuantity }}
          </template>
        </b-table-column>
        <b-table-column field="notesCount" label="Notes" :visible="tableConfig.showNotesColumn" v-slot="props">
          <div @click="onNote(props.row, true)">
            <b-tag icon="note-text" rounded>
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
        <b-table-column field="jobBoards" label="Job Boards" searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="jobBoardsSelected" autocomplete :data="availableJobBoards"
              open-on-focus field="value" icon="bullhorn" placeholder="Select Job Boards"
              @update:modelValue="onJobBoardsChange" append-to-body>
            </b-taginput>
          </template>
          <template v-slot="props">
            <div class="job-boards-cell">
              <b-tag v-for="jb in props.row.jobBoards" :key="jb.sourceId" rounded type="is-info is-light">
                {{ jb.value }}
              </b-tag>
              <b-tooltip v-if="!props.row.jobBoards || props.row.jobBoards.length === 0"
                label="Add job boards" type="is-dark" append-to-body>
                <b-icon icon="plus-circle-outline" size="is-small" class="job-boards-cell__add"></b-icon>
              </b-tooltip>
            </div>
          </template>
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

    <!-- bulk cancel -->
    <b-modal v-model="showBulkCancelModal" @close="showBulkCancelModal = false" width="500px">
      <CancelList @sendReason="onBulkCancelConfirmed" />
    </b-modal>

    <!-- job boards -->
    <b-modal v-model="showJobBoardsModal" @close="showJobBoardsModal = false" width="520px" :destroy-on-hide="true">
      <JobBoardsModal v-if="currentJobBoardsRequest"
        :request-id="currentJobBoardsRequest.id"
        :number-id="currentJobBoardsRequest.numberId"
        :current-boards="currentJobBoardsRequest.jobBoards || []"
        @saved="onJobBoardsSaved"
        @close="showJobBoardsModal = false" />
    </b-modal>

  </div>
</template>
<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useAgencyStore } from '@/stores/agency';
import { appGlobals } from '@/varaibles';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { dateMonth, breakWord, currency } from '@/utils/filters';
import { useBillingAdmin } from '@/composables/useBillingAdmin';
import { updateIsAsapRequests } from "@/api/agencyCompanyApi";
import { getAgencyRequests, bulkCancelRequests } from "@/api/agencyRequestApi";
import { getSourcesForRequests } from "@/api/catalogApi";
import type { RequestJobBoardSummary, AgencyRequestListItem } from '@/types/agency';
import type { Source } from '@/types/common';
import {
  getAgencyRequestNotes,
  createAgencyRequestNote,
  updateAgencyRequestNote,
  deleteAgencyRequestNote
} from "@/api/agencyNoteApi";
import type { NotesFetchPayload, NotesCreatePayload, NotesUpdatePayload, NotesDeletePayload } from '@/types/agency';
import { RequestStatus, RequestStatusLabels } from "@/constants/enums";
import ModalNotes from '../notes/ModalNotes.vue';
import JobBoardsModal from '../../components/agency_request/JobBoardsModal.vue';
import AgencyShift from '../../components/agency_request/AgencyShiftDetail.vue';
import CancelList from '@/components/company/CompanyCancelList.vue';
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
  showMyRequestsCheckbox: true,
  showQuickActions: true,
  enableCheckable: true,
  showSalesRepColumn: true,
  showNotesColumn: true
};
const showBulkCancelModal = ref(false);
const showJobBoardsModal = ref(false);
const currentJobBoardsRequest = ref<AgencyRequestListItem | null>(null);
const availableJobBoards = ref<Source[]>([]);
const jobBoardsSelected = ref<Source[]>([]);
const jobBoardsSummary = ref<RequestJobBoardSummary[]>([]);
const quickActionsKey = ref(0);
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
const createdAtDatesSelected = ref<any[]>([]);
const rows = ref<any[]>([]);
const checkedRows = ref<any[]>([]);
const serverParams = reactive<any>({
  onlyMine: false,
  sortBy: 0,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30
});
const quickActions = reactive<any>({ isAsap: false });

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
      break;
    case 'jobBoards':
      currentJobBoardsRequest.value = row;
      showJobBoardsModal.value = true;
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
    case 'createdAt':
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
    case 'salesRepresentative':
      serverParams.sortBy = 7;
      break;
  }
  serverParams.isDescending = order !== 'asc';
  loadRequests();
}

function onStatusChange() {
  serverParams.statuses = statusesSelected.value.map((ss) => ss.id);
  loadRequests();
}

function onJobBoardsChange() {
  serverParams.jobBoardIds = jobBoardsSelected.value.map((jb) => jb.id);
  loadRequests();
}

function onJobBoardsSaved() {
  loadRequests();
}

function onCreatedAtSelected() {
  serverParams.createdAtFrom = createdAtDatesSelected.value[0];
  serverParams.createdAtTo = createdAtDatesSelected.value[1];
  loadRequests();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
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
      jobBoardsSummary.value = requestsResponse.jobBoardsSummary || [];
      emit('update:totalItems', requestsResponse.totalItems);
      emit('onDataLoading', false);
    })
    .catch(() => emit('onDataLoading', false));
}

function bulkUpdateIsAsap() {
  quickActionsKey.value++;
  emit('onDataLoading', true);
  const payload = {
    ids: checkedRows.value.map((cr) => cr.id),
    isAsap: quickActions.isAsap
  };
  updateIsAsapRequests(payload)
    .then(() => {
      loadRequests();
    }).catch((error) => {
      showAlertError(error);
      emit('onDataLoading', false);
    });
}

function onShowBulkCancelModal() {
  quickActionsKey.value++;
  showBulkCancelModal.value = true;
}


function onBulkCancelConfirmed({ reasonId, otherMessage }: { reasonId: string; otherMessage: string }) {
  emit('onDataLoading', true);
  bulkCancelRequests({
    ids: checkedRows.value.map((cr) => cr.id),
    cancellationReasonId: reasonId,
    otherCancellationReason: otherMessage
  })
    .then((result) => {
      showBulkCancelModal.value = false;
      showAlertSuccess(`Cancelled ${result.cancelled} order(s), skipped ${result.skipped}`);
      loadRequests();
    })
    .catch((error) => {
      showBulkCancelModal.value = false;
      showAlertError(error);
      emit('onDataLoading', false);
    });
}

watch(() => serverParams.onlyMine, () => {
  loadRequests();
});

watch(checkedRows, (rows) => {
  quickActions.isAsap = rows.length > 0 && rows.every((r: any) => r.isAsap);
});

if (!props.companyId && !props.agencyId) {
  if (agencyStore.agencyRequestFilter) {
    Object.assign(serverParams, agencyStore.agencyRequestFilter);
    if (serverParams.statuses) {
      statusesSelected.value = statuses.filter((s) => serverParams.statuses.some((sps: any) => sps == s.id));
    }
    if (serverParams.createdAtFrom && serverParams.createdAtTo) {
      createdAtDatesSelected.value[0] = serverParams.createdAtFrom;
      createdAtDatesSelected.value[1] = serverParams.createdAtTo;
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
getSourcesForRequests().then((sources) => {
  availableJobBoards.value = sources;
  if (serverParams.jobBoardIds && serverParams.jobBoardIds.length) {
    jobBoardsSelected.value = sources.filter((s) => serverParams.jobBoardIds.includes(s.id));
  }
});
loadRequests();
</script>

<style scoped lang="scss">
.request-id-cell {
  position: relative;
  padding-top: 14px;
}

.request-flags {
  position: absolute;
  top: -8px;
  left: -10px;
  display: flex;
  flex-direction: row;
  gap: 2px;
  z-index: 1;
}

.quick-action-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  min-width: 180px;
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

.job-boards-summary {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 6px;
  padding: 8px 4px 12px;

  &__label {
    font-size: 12px;
    font-weight: 600;
    color: #555;
    margin-right: 4px;
  }
}

.job-boards-cell {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 4px;

  &__add {
    color: #888;
    cursor: pointer;
    transition: color 0.15s ease;

    &:hover { color: #1d4ed8; }
  }
}
</style>
