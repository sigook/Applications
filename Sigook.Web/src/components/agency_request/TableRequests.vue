<template>
  <div>
    <export :url="'/api/AgencyRequest/File'" :params="serverParams" :fileName="'Requests'"
      @onDataLoading="(value) => $emit('onDataLoading', value)">
      <template v-slot:actions>
        <b-checkbox v-if="tableConfig.showMyOrdersCheckbox" v-model="serverParams.onlyMine">My Orders</b-checkbox>
        <b-button v-if="tableConfig.showQuickActions" :disabled="checkedRows.length < 1"
          @click="onShowQuickActionsModal">
          Quick Actions
        </b-button>
      </template>
    </export>
    <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
      :checkable="tableConfig.enableCheckable" pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
      focuseable default-sort="updatedAt" :current-page.sync="serverParams.pageIndex" :checked-rows.sync="checkedRows"
      @page-change="onPageChange" @sort="onSortChange" @cellclick="onCellClick">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="numberId" label="Order ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
              @keypress.native="onInputEntered"></b-input>
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
              @keypress.native="onInputEntered"></b-input>
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
              @keypress.native="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.location }}
            <span v-if="props.row.entrance"> - {{ props.row.entrance }}</span>
          </template>
        </b-table-column>
        <b-table-column field="jobTitle" label="Position" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.jobTitle" placeholder="Search..." icon="magnify" size="is-small"
              @keypress.native="onInputEntered"></b-input>
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
              @input="onLastUpdateSelected" append-to-body>
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
              @icon-right-click="onStartAtCleared" range v-model="startAtDatesSelected" @input="onStartAtSelected"
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
            <agency-shift class="fz-2 d-block" :requestId="props.row.id" :displayShift="props.row.displayShift" />
            <i class="fz-2 d-block">
              {{ DurationTermLabels[props.row.durationTerm] }} - {{ EmploymentTypeLabels[props.row.employmentType] }}
            </i>
          </template>
        </b-table-column>
        <b-table-column field="displayRecruiters" label="Recruiter" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.displayRecruiters" placeholder="Search..." icon="magnify" size="is-small"
              @keypress.native="onInputEntered"></b-input>
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
              @keypress.native="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.salesRepresentative || '' }}
          </template>
        </b-table-column>
        <b-table-column field="workerRate" label="Rate / Salary" sortable searchable>
          <template v-slot:searchable>
            <b-field>
              <b-input placeholder="From" icon="magnify" size="is-small" v-model="serverParams.rateFrom"
                @keypress.native="onInputEntered"></b-input>
              <b-input placeholder="To" icon="magnify" size="is-small" v-model="serverParams.rateTo"
                @keypress.native="onInputEntered"></b-input>
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
            <modal-notes :can-create="false" :user-id="props.row.id" :on-get="getNotes"
              :on-create="createNote" :on-update="updateNote"
              :on-delete="deleteNote" @onUpdateNote="(val) => onUpdateNote(props.row, val.size)"
              @close="onNote(props.row, false)">
            </modal-notes>
          </div>
        </b-table-column>
        <b-table-column field="status" label="Status" searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
              field="value" icon="label" placeholder="Select Status" @input="onStatusChange" append-to-body>
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
              @click="$router.push({ path: '/agency-request/' + props.row.id, query: { tab: 'Applicants' } })">
              Applicants
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem"
              @click="$router.push({ path: '/agency-request/' + props.row.id, query: { tab: 'Workers' } })">
              Workers
            </b-dropdown-item>
          </b-dropdown>
        </b-table-column>
      </template>
    </b-table>

    <!-- recruiters list -->
    <b-modal v-if="tableConfig.showRecruiterModal" v-model="showRecruitersModal" @close="showRecruitersModal = false"
      width="500px">
      <personnel-list :recruiters="recruiters" :request="currentRequest" @selectUser="() => onUpdateRecruiter()"
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
<script lang="ts">
import { mapStores } from 'pinia';
import { useAgencyStore } from '@/stores/agency';
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

export default {
  setup() {
    return { ...useBillingAdmin() };
  },
  props: ["totalItems", "companyId", "agencyId", "config"],
  data() {
    return {
      defaultConfig: {
        showMyOrdersCheckbox: true,
        showQuickActions: true,
        enableCheckable: true,
        showRecruiterModal: true,
        showSalesRepColumn: true,
        showNotesColumn: true
      },
      showRecruitersModal: false,
      showQuickActions: false,
      recruiters: null,
      currentRequest: null,
      currentIndex: null,
      getNotes: ({ userId, pagination }: NotesFetchPayload) => getAgencyRequestNotes(userId, pagination),
      createNote: ({ userId, model }: NotesCreatePayload) => createAgencyRequestNote(userId, model),
      updateNote: ({ userId, id, model }: NotesUpdatePayload) => updateAgencyRequestNote(userId, id, model),
      deleteNote: ({ userId, id }: NotesDeletePayload) => deleteAgencyRequestNote(userId, id),
      statuses: [
        { id: 1, value: this.$statusDisplayOpen },
        { id: 3, value: this.$statusDisplayFilled },
        { id: 4, value: this.$statusDisplayCancelled }
      ],
      statusesSelected: [],
      lastUpdateDatesSelected: [],
      startAtDatesSelected: [],
      rows: [],
      checkedRows: [],
      serverParams: {
        onlyMine: false,
        sortBy: 7,
        isDescending: true,
        pageIndex: 1,
        pageSize: 30
      },
      quickActions: {}
    };
  },
  components: {
    ModalNotes: () => import("../notes/ModalNotes.vue"),
    PersonnelList: () => import("../../components/agency_request/PersonnelListModal.vue"),
    AgencyShift: () => import("../../components/agency_request/AgencyShiftDetail.vue"),
    Export: () => import("@/components/Export.vue")
  },
  methods: {
    dateFromNow,
    dateMonth,
    breakWord,
    currency,
    onCellClick(row, column, rowIndex) {
      switch (column._props.field) {
        case 'workersQuantityWorking':
          this.$router.push({
            path: `/agency-request/${row.id}`,
            query: {
              tab: 'Workers'
            }
          });
          break;
        case 'displayRecruiters':
          if (this.tableConfig.showRecruiterModal) {
            this.onShowRecruitersModal(row, rowIndex);
          }
          break;
        case 'notesCount':
        case 'actions':
          break;
        default:
          this.$router.push(`/agency-request/${row.id}`);
          break;
      }
    },
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.loadRequests();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'numberId':
          this.serverParams.sortBy = 0;
          break
        case 'companyFullName':
          this.serverParams.sortBy = 1;
          break;
        case 'jobTitle':
          this.serverParams.sortBy = 2;
          break;
        case 'startAt':
          this.serverParams.sortBy = 3;
          break;
        case 'displayRecruiters':
          this.serverParams.sortBy = 4;
          break;
        case 'workerRate':
          this.serverParams.sortBy = 5;
          break;
        case 'workersQuantityWorking':
          this.serverParams.sortBy = 6;
          break;
        case 'updatedAt':
          this.serverParams.sortBy = 7;
          break;
        case 'salesRepresentative':
          this.serverParams.sortBy = 8;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.loadRequests();
    },
    onStatusChange() {
      this.serverParams.statuses = this.statusesSelected.map(ss => ss.id);
      this.loadRequests();
    },
    onLastUpdateSelected() {
      this.serverParams.lastUpdateFrom = this.lastUpdateDatesSelected[0];
      this.serverParams.lastUpdateTo = this.lastUpdateDatesSelected[1];
      this.loadRequests();
    },
    onLastUpdateCleared() {
      this.lastUpdateDatesSelected = [];
      this.onLastUpdateSelected();
    },
    onStartAtSelected() {
      this.serverParams.startAtFrom = this.startAtDatesSelected[0];
      this.serverParams.startAtTo = this.startAtDatesSelected[1];
      this.loadRequests();
    },
    onStartAtCleared() {
      this.startAtDatesSelected = [];
      this.onStartAtSelected();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.loadRequests();
      }
    },
    onNote(row, status) {
      const index = this.rows.findIndex(r => r.id === row.id);
      this.rows[index].showNotes = status;
    },
    onUpdateNote(row, size) {
      const index = this.rows.findIndex(r => r.id === row.id);
      this.rows[index].notesCount = size;
    },
    onShowRecruitersModal(item, index) {
      this.currentRequest = item;
      this.currentIndex = index;
      this.recruiters = item.displayRecruiters
        ? item.displayRecruiters.split("|")
        : [];
      this.showRecruitersModal = true;
    },
    onUpdateRecruiter() {
      this.showRecruitersModal = false;
      this.loadRequests();
    },
    canEdit(status) {
      return (
        status === RequestStatus.Open ||
        status === RequestStatus.Filled
      );
    },
    getStatusClass(row) {
      if (row.requestStatus === RequestStatus.Open &&
        row.workersQuantityWorking > 0 &&
        row.workersQuantityWorking < row.workersQuantity) {
        return 'status-inprogress';
      }
      return 'status-' + RequestStatusLabels[row.requestStatus].toLowerCase();
    },
    updateWorkers(item) {
      this.rows[this.currentIndex].workersQuantityWorking = item;
    },
    loadRequests() {
      this.checkedRows = [];
      this.$emit("onDataLoading", true);
      if (!this.companyId && !this.agencyId) {
        this.agencyStore.updateAgencyRequestFilter(this.serverParams);
      }
      getAgencyRequests(this.serverParams)
        .then((requests) => {
          this.rows = requests.items.map(i => ({ ...i, actions: null, showNotes: false, notesCount: i.notesCount || 0 }));
          this.$emit('update:totalItems', requests.totalItems);
          this.$emit("onDataLoading", false);
        })
        .catch(() => this.$emit("onDataLoading", false));
    },
    onShowQuickActionsModal() {
      this.quickActions.isAsap = false;
      this.showQuickActions = true;
    },
    bulkUpdateIsAsap() {
      this.$emit("onDataLoading", true);
      const payload = {
        ids: this.checkedRows.map(cr => cr.id),
        isAsap: this.quickActions.isAsap
      };
      updateIsAsapRequests(payload)
        .then(() => {
          this.showQuickActions = false;
          this.loadRequests();
        }).catch((error) => {
          this.showQuickActions = false;
          showAlertError(error);
          this.$emit("onDataLoading", false);
        });
    }
  },
  created() {
    if (!this.companyId && !this.agencyId) {
      if (this.agencyStore.agencyRequestFilter) {
        this.serverParams = this.agencyStore.agencyRequestFilter;
        if (this.serverParams.statuses) {
          this.statusesSelected = this.statuses.filter(s => this.serverParams.statuses.some(sps => sps == s.id));
        }
        if (this.serverParams.lastUpdateFrom && this.serverParams.lastUpdateTo) {
          this.lastUpdateDatesSelected[0] = this.serverParams.lastUpdateFrom;
          this.lastUpdateDatesSelected[1] = this.serverParams.lastUpdateTo;
        }
        if (this.serverParams.startAtFrom && this.serverParams.startAtTo) {
          this.startAtDatesSelected[0] = this.serverParams.startAtFrom;
          this.startAtDatesSelected[1] = this.serverParams.startAtTo;
        }
      } else {
        this.serverParams.onlyMine = !this.isPayrollManager;
      }
    } else {
      this.serverParams.onlyMine = false;
      if (this.companyId) {
        this.serverParams.companyId = this.companyId;
      }
      if (this.agencyId) {
        this.serverParams.agencyId = this.agencyId;
      }
    }
    this.loadRequests();
  },
  computed: {
    ...mapStores(useAgencyStore),
    DurationTerm: () => DurationTerm,
    DurationTermLabels: () => DurationTermLabels,
    EmploymentTypeLabels: () => EmploymentTypeLabels,
    RequestStatus: () => RequestStatus,
    RequestStatusLabels: () => RequestStatusLabels,
    tableConfig() {
      return { ...this.defaultConfig, ...this.config };
    },
    totalQuantityWorking() {
      if (this.rows.length > 0) {
        return this.rows
          .map((r) => r.workersQuantityWorking)
          .reduce((a, b) => a + b);
      }
      return 0;
    },
    totalQuantity() {
      if (this.rows.length > 0) {
        return this.rows
          .map((r) => r.workersQuantity)
          .reduce((a, b) => a + b);
      }
      return 0;
    }
  },
  watch: {
    'serverParams.onlyMine': function () {
      this.loadRequests();
    }
  }
};
</script>