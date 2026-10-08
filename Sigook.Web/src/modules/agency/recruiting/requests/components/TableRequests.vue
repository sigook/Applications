<template>
  <div>
    <div v-if="jobBoardsSummary.length" class="job-boards-summary">
      <span class="job-boards-summary__label">Posted in:</span>
      <b-tag v-for="s in jobBoardsSummary" :key="s.sourceId" rounded type="is-info is-light">
        {{ s.value }} ({{ s.count }})
      </b-tag>
    </div>
    <SigookGrid ref="grid" :fetch="loadRequests" v-model:params="serverParams" :sort-map="sortMap"
      :export="{ url: exportUrl, fileName: 'Requests' }" focusable :checkable="tableConfig.enableCheckable"
      v-model:checked-rows="checkedRows" @update:loading="(value) => emit('onDataLoading', value)"
      @loaded="(total) => emit('update:totalItems', total)" @cellclick="onCellClick">
      <template v-if="tableConfig.showAssignedToMe" #filters>
        <b-switch v-model="serverParams.onlyMine" @update:modelValue="onOnlyMineChange">Assigned to me</b-switch>
      </template>
      <template #actions>
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
          <b-dropdown-item v-if="isAdmin" aria-role="menuitem" :disabled="checkedRows.length < 1"
            @click="onShowBulkRecruitersModal">
            Assign / Unassign recruiters
          </b-dropdown-item>
        </b-dropdown>
      </template>
      <b-table-column field="numberId" label="ID" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="props">
          <div class="request-id-cell">
            <div v-if="props.row.isAsap || props.row.workerSalary" class="request-flags">
              <span v-if="props.row.isAsap" class="request-flag request-flag--asap">Asap</span>
              <span v-if="props.row.workerSalary" class="request-flag request-flag--dh">DH</span>
            </div>
            <router-link :to="{ path: requestDetailBase + '/' + props.row.id }">
              <p>{{ props.row.numberId }}</p>
            </router-link>
            <b-icon v-if="props.row.vaccinationRequired" icon="needle" size="is-small"></b-icon>
          </div>
        </template>
      </b-table-column>
      <b-table-column field="companyFullName" label="Client" :visible="!companyProfileId" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.companyFullName" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="props">
          <router-link :to="{ path: companyDetailBase + '/' + props.row.companyProfileId }">
            {{ props.row.companyFullName }}
          </router-link>
          <i class="fz-2 block mb-0" v-if="props.row.displayReportTo">{{ props.row.displayReportTo }}</i>
        </template>
      </b-table-column>
      <b-table-column field="location" label="Location" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.location" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="props">
          {{ props.row.location }}
          <span v-if="props.row.entrance"> - {{ props.row.entrance }}</span>
        </template>
      </b-table-column>
      <b-table-column field="jobTitle" label="Position" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.jobTitle" placeholder="Search..." icon="magnify" size="is-small"></b-input>
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
          <AgencyShift class="fz-2 is-block" :requestId="props.row.id" :displayShift="props.row.displayShift"
            :fetchShift="getAgencyRequestShift" />
        </template>
      </b-table-column>
      <b-table-column field="displayRecruiters" label="Recruiter" sortable searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.displayRecruiters" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="props">
          <div v-if="props.row.displayRecruiters" class="is-capitalized is-inline-block valign-middle">
            {{ breakWord(props.row.displayRecruiters) }}
          </div>
          <span v-else class="op3">—</span>
        </template>
      </b-table-column>
      <b-table-column field="salesRepresentative" label="Sales Rep" :visible="tableConfig.showSalesRepColumn" sortable
        searchable>
        <template v-slot:searchable>
          <b-input v-model="serverParams.salesRepresentative" placeholder="Search..." icon="magnify" size="is-small"></b-input>
        </template>
        <template v-slot="props">
          {{ props.row.salesRepresentative || '' }}
        </template>
      </b-table-column>
      <b-table-column field="workerRate" label="Rate / Salary" sortable searchable>
        <template v-slot:searchable>
          <b-field>
            <b-input placeholder="From" icon="magnify" size="is-small" v-model="serverParams.rateFrom"></b-input>
            <b-input placeholder="To" icon="magnify" size="is-small" v-model="serverParams.rateTo"></b-input>
          </b-field>
        </template>
        <template v-slot="props">
          {{ currency(props.row.workerRate || props.row.workerSalary) }}
        </template>
      </b-table-column>
      <b-table-column field="workersQuantityWorking" sortable>
        <template v-slot:header>
          <p class="has-text-weight-semibold">Workers</p>
          <p class="has-text-weight-semibold">({{ totalQuantityWorking }} / {{ totalQuantity }})</p>
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
          <div class="has-text-centered">
            <b-tooltip :label="RequestStatusLabels[props.row.requestStatus]" type="is-dark" append-to-body>
              <div class="status-dot-container">
                <img v-if="props.row.requestStatus === RequestStatus.Filled" src="@/assets/images/check_white.png" alt="check"
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
          <b-dropdown-item v-if="props.row.requestStatus !== RequestStatus.Cancelled" aria-role="listitem"
            @click="router.push({ path: requestDetailBase + '/update/' + props.row.companyProfileId + '/' + props.row.id })">
            Edit Request
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem"
            @click="router.push({ path: requestDetailBase + '/duplicate/' + props.row.companyProfileId + '/' + props.row.id })">
            Duplicate Request
          </b-dropdown-item>
        </b-dropdown>
      </b-table-column>
    </SigookGrid>

    <!-- bulk cancel -->
    <b-modal custom-content-class="card" v-model="showBulkCancelModal" @close="showBulkCancelModal = false" width="500px">
      <CancelList @sendReason="onBulkCancelConfirmed" />
    </b-modal>

    <!-- bulk recruiters -->
    <b-modal custom-content-class="card" v-model="showBulkRecruitersModal" @close="showBulkRecruitersModal = false" width="500px" :destroy-on-hide="true">
      <BulkRecruiterModal :request-count="checkedRows.length"
        @submit="onBulkRecruitersConfirmed"
        @cancel="showBulkRecruitersModal = false" />
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
import { ref, reactive, computed, watch, useTemplateRef } from 'vue';
import { useRouter } from 'vue-router';
import { useAgencyStore } from '@/modules/agency/store';
import { appGlobals } from '@/app/globals';
import { showAlertError, showAlertSuccess } from "@/shared/utils/toast";
import { dateMonth, breakWord, currency } from '@/shared/format';
import { useAdmin } from '@/shared/composables/useAdmin';
import { updateIsAsapRequests } from '@/modules/agency/recruiting/clients/api';
import { getAgencyRequests, bulkCancelRequests, bulkUpdateRecruiters } from "@/modules/agency/recruiting/requests/api";
import { getSalesRequests } from "@/modules/agency/sales/api";
import { useModuleBase } from '@/modules/agency/shared/useModuleBase';
import { getSourcesForRequests } from "@/shared/api/catalogApi";
import type { RequestJobBoardSummary, AgencyRequestFilter, AgencyRequestListItem, TableRequestsConfig } from '@/modules/agency/recruiting/requests/types';
import type { CatalogItem, GridHandle, PaginatedList, Source, TableColumnRef } from '@/shared/types/common';
import {
  getAgencyRequestNotes,
  createAgencyRequestNote,
  updateAgencyRequestNote,
  deleteAgencyRequestNote
} from "@/modules/agency/shared/notes/api";
import type { NotesFetchPayload, NotesCreatePayload, NotesUpdatePayload, NotesDeletePayload } from '@/modules/agency/shared/notes/types';
import { RequestStatus, RequestStatusLabels } from "@/shared/constants/enums";
import ModalNotes from '@/modules/agency/shared/notes/ModalNotes.vue';
import JobBoardsModal from '@/modules/agency/recruiting/requests/components/JobBoardsModal.vue';
import BulkRecruiterModal from '@/modules/agency/recruiting/requests/components/BulkRecruiterModal.vue';
import AgencyShift from '@/shared/request-detail/AgencyShiftDetail.vue';
import { getAgencyRequestShift } from '@/modules/agency/recruiting/requests/api';
import CancelList from '@/shared/request-detail/CompanyCancelList.vue';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const props = defineProps<{ totalItems?: number; companyProfileId?: string; agencyId?: string; config?: Partial<TableRequestsConfig> }>();
const emit = defineEmits<{
  (e: 'onDataLoading', value: boolean): void;
  (e: 'update:totalItems', value: number): void;
}>();

const router = useRouter();
const agencyStore = useAgencyStore();
const { isAdmin } = useAdmin();
const grid = useTemplateRef<GridHandle>('grid');

const { isSalesView, requestBase: requestDetailBase, companyBase: companyDetailBase } = useModuleBase();
const exportUrl = computed(() =>
  isSalesView.value ? '/api/agency/sales/requests/File' : '/api/agency/recruiting/requests/File');

const sortMap = {
  numberId: 0,
  companyFullName: 1,
  jobTitle: 2,
  createdAt: 3,
  displayRecruiters: 4,
  workerRate: 5,
  workersQuantityWorking: 6,
  salesRepresentative: 7,
};

const defaultConfig: TableRequestsConfig = {
  showAssignedToMe: true,
  showQuickActions: true,
  enableCheckable: true,
  showSalesRepColumn: true,
  showNotesColumn: true
};
const showBulkCancelModal = ref(false);
const showBulkRecruitersModal = ref(false);
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
const statuses: CatalogItem<number>[] = [
  { id: 1, value: appGlobals.$statusDisplayOpen },
  { id: 3, value: appGlobals.$statusDisplayFilled },
  { id: 4, value: appGlobals.$statusDisplayCancelled }
];
const statusesSelected = ref<CatalogItem<number>[]>([]);
const createdAtDatesSelected = ref<Date[]>([]);
const rows = ref<(AgencyRequestListItem & { showNotes: boolean })[]>([]);
const checkedRows = ref<AgencyRequestListItem[]>([]);
const serverParams = ref<AgencyRequestFilter>({
  onlyMine: false,
  sortBy: 0,
  isDescending: true
});
const quickActions = reactive({ isAsap: false });

const tableConfig = computed(() => ({ ...defaultConfig, ...props.config }));
const totalQuantityWorking = computed(() =>
  rows.value.reduce((total, r) => total + r.workersQuantityWorking, 0));
const totalQuantity = computed(() =>
  rows.value.reduce((total, r) => total + r.workersQuantity, 0));

function onCellClick(row: AgencyRequestListItem, column: TableColumnRef) {
  switch (column.field) {
    case 'workersQuantityWorking':
      router.push({
        path: `${requestDetailBase.value}/${row.id}`,
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
      router.push(`${requestDetailBase.value}/${row.id}`);
      break;
  }
}

function onStatusChange() {
  serverParams.value.statuses = statusesSelected.value.map((ss) => ss.id);
  grid.value?.search();
}

function onJobBoardsChange() {
  serverParams.value.jobBoardIds = jobBoardsSelected.value.map((jb) => jb.id);
  grid.value?.search();
}

function onOnlyMineChange() {
  grid.value?.search();
}

function onJobBoardsSaved() {
  reloadRequests();
}

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onNote(row: AgencyRequestListItem & { showNotes: boolean }, status: boolean) {
  row.showNotes = status;
}

function onUpdateNote(row: AgencyRequestListItem, size: number) {
  row.notesCount = size;
}

function getStatusClass(row: AgencyRequestListItem) {
  if (row.requestStatus === RequestStatus.Open &&
    row.workersQuantityWorking > 0 &&
    row.workersQuantityWorking < row.workersQuantity) {
    return 'status-inprogress';
  }
  return 'status-' + RequestStatusLabels[row.requestStatus].toLowerCase();
}

function loadRequests(params: AgencyRequestFilter): Promise<PaginatedList<AgencyRequestListItem & { showNotes: boolean }>> {
  if (!props.companyProfileId && !props.agencyId) {
    agencyStore.updateAgencyRequestFilter(params);
  }
  const fetchRequests = isSalesView.value ? getSalesRequests : getAgencyRequests;
  return fetchRequests(params)
    .then((response) => {
      rows.value = response.items.map((i) => ({ ...i, showNotes: false, notesCount: i.notesCount || 0 }));
      jobBoardsSummary.value = response.jobBoardsSummary || [];
      return { ...response, items: rows.value };
    });
}

function reloadRequests() {
  grid.value?.reload();
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
      reloadRequests();
    }).catch((error) => {
      showAlertError(error);
      emit('onDataLoading', false);
    });
}

function onShowBulkCancelModal() {
  quickActionsKey.value++;
  showBulkCancelModal.value = true;
}

function onShowBulkRecruitersModal() {
  quickActionsKey.value++;
  showBulkRecruitersModal.value = true;
}

function onBulkRecruitersConfirmed(recruiterIds: string[]) {
  emit('onDataLoading', true);
  bulkUpdateRecruiters({
    ids: checkedRows.value.map((cr) => cr.id),
    recruiterIds
  })
    .then(() => {
      showBulkRecruitersModal.value = false;
      showAlertSuccess(recruiterIds.length === 0 ? 'Recruiters unassigned' : 'Recruiters assigned');
      reloadRequests();
    })
    .catch((error) => {
      showBulkRecruitersModal.value = false;
      showAlertError(error);
      emit('onDataLoading', false);
    });
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
      reloadRequests();
    })
    .catch((error) => {
      showBulkCancelModal.value = false;
      showAlertError(error);
      emit('onDataLoading', false);
    });
}

watch(checkedRows, (selected) => {
  quickActions.isAsap = selected.length > 0 && selected.every((r) => r.isAsap);
});

if (!props.companyProfileId && !props.agencyId) {
  if (agencyStore.agencyRequestFilter) {
    serverParams.value = { ...agencyStore.agencyRequestFilter };
    const selectedStatuses = serverParams.value.statuses;
    if (selectedStatuses) {
      statusesSelected.value = statuses.filter((s) => selectedStatuses.some((sps) => sps == s.id));
    }
    if (serverParams.value.createdAtFrom && serverParams.value.createdAtTo) {
      createdAtDatesSelected.value[0] = new Date(serverParams.value.createdAtFrom);
      createdAtDatesSelected.value[1] = new Date(serverParams.value.createdAtTo);
    }
  } else {
    serverParams.value.onlyMine = !isAdmin.value;
  }
} else {
  serverParams.value.onlyMine = false;
  if (props.companyProfileId) {
    serverParams.value.companyProfileId = props.companyProfileId;
  }
  if (props.agencyId) {
    serverParams.value.agencyId = props.agencyId;
  }
}
getSourcesForRequests().then((sources) => {
  availableJobBoards.value = sources;
  const selectedJobBoards = serverParams.value.jobBoardIds;
  if (selectedJobBoards && selectedJobBoards.length) {
    jobBoardsSelected.value = sources.filter((s) => selectedJobBoards.includes(s.id));
  }
});
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
  letter-spacing: 0.5px;
  text-transform: uppercase;
  // convex right-pointing arrow; the tip pokes into the next (solid) flag,
  // so the seam is always backed by color and never reveals a white sub-pixel gap
  clip-path: polygon(0 0, calc(100% - 6px) 0, 100% 50%, calc(100% - 6px) 100%, 0 100%);

  &--asap {
    background: #ff9932;
    color: #1e1e1e;
    z-index: 2;
  }

  &--dh {
    background: #1d4ed8;
    color: #fff;
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
