<template>
  <div>
    <export :url="'/api/AgencyRequest/File'" :params="serverParams" :fileName="'Requests'"
      @onDataLoading="(value) => $emit('onDataLoading', value)">
      <template v-slot:actions>
        <b-checkbox v-if="tableConfig.showMyOrdersCheckbox" v-model="serverParams.onlyMine">My Orders</b-checkbox>
        <b-button v-if="tableConfig.showQuickActions" :disabled="checkedRows.length < 1" @click="onShowQuickActionsModal">
          Quick Actions
        </b-button>
      </template>
    </export>
    <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
      :checkable="tableConfig.enableCheckable" pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable
      default-sort="updatedAt" :current-page.sync="serverParams.pageIndex" :checked-rows.sync="checkedRows"
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
            <p v-if="props.row.isAsap" class="asap">{{ $t("Asap") }}</p>
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
              <i class="fz-2 block">
                {{ props.row.location }}
                <i v-if="props.row.entrance"> - {{ props.row.entrance }}</i>
              </i>
            </router-link>
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
            <i class="fz-2 block">{{ props.row.createdAt | dateFromNow }}</i>
          </template>
        </b-table-column>
        <b-table-column field="updatedAt" label="Last Update" sortable searchable>
          <template v-slot:searchable>
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
              :icon-right="lastUpdateDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onLastUpdateCleared" range v-model="lastUpdateDatesSelected"
              @input="onLastUpdateSelected">
            </b-datepicker>
          </template>
          <template v-slot="props">
            {{ props.row.updatedAt | dateMonth }}
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
              @icon-right-click="onStartAtCleared" range v-model="startAtDatesSelected" @input="onStartAtSelected">
            </b-datepicker>
          </template>
          <template v-slot="props">
            {{ props.row.startAt | dateMonth }}
            <span v-if="props.row.durationTerm !== $longTerm">
              - {{ props.row.finishAt | dateMonth }}
            </span>
            <span
              v-if="(props.row.status === $statusFinalized || props.row.status === $statusCancelled) && props.row.durationTerm === $longTerm">
              - {{ props.row.finishAt | dateMonth }}
            </span>
            <agency-shift class="fz-2 d-block" :requestId="props.row.id" :displayShift="props.row.displayShift" />
            <i class="fz-2 d-block">
              {{ props.row.durationTerm | splitCapital }} - {{ props.row.employmentType | splitCapital }}
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
              {{ props.row.displayRecruiters | breakWord }}
              <button v-if="tableConfig.showRecruiterModal" type="button" class="btn-icon-sm btn-icon-worker-plus is-inline-block v-middle"></button>
            </div>
            <div v-else>
              <span class="op3">Recruiter</span>
              <button v-if="tableConfig.showRecruiterModal" type="button" class="btn-icon-sm btn-icon-worker-plus is-inline-block v-middle"></button>
            </div>
          </template>
        </b-table-column>
        <b-table-column field="salesRepresentative" label="Sales Rep" :visible="tableConfig.showSalesRepColumn" sortable searchable>
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
            {{ (props.row.workerRate || props.row.workerSalary) | currency }}
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
            <modal-notes :can-create="false" :user-id="props.row.id" :on-get="'agency/getAgencyRequestNote'"
              :on-create="'agency/createAgencyRequestNote'" :on-update="'agency/updateAgencyRequestNote'"
              :on-delete="'agency/deleteAgencyRequestNote'" @onUpdateNote="(val) => onUpdateNote(props.row, val.size)"
              @close="onNote(props.row, false)">
            </modal-notes>
          </div>
        </b-table-column>
        <b-table-column field="status" label="Status" width="250px" searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
              field="value" icon="label" placeholder="Select Status" @input="onStatusChange">
            </b-taginput>
          </template>
          <template v-slot="props">
            <b-tooltip :label="$t(props.row.status)" type="is-dark">
              <img
                v-if="props.row.workersQuantityWorking >= props.row.workersQuantity && props.row.status !== $statusCancelled"
                src="../../assets/images/check_white.png" alt="check" class="request-check" />
              <div class="dot-status" :class="'status-' + props.row.status.toLowerCase()"></div>
            </b-tooltip>
            <i v-if="props.row.isOpen" class="fz-2 block">
              <span v-if="canEdit(props.row.status)" class="tag-yellow">
                {{ $statusOpen }}
              </span>
              <span v-else>
                {{ $statusNotFilled }}
              </span>
            </i>
            <i class="fz-2 block" v-else>
              <span v-if="props.row.status === $statusCancelled">
                {{ $statusCancelled }}
              </span>
              <span v-else-if="props.row.workersQuantityWorking < props.row.workersQuantity">
                {{ $statusNotFilled }}
              </span>
              <span v-else>
                {{ $statusFilled }}
              </span>
            </i>
          </template>
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <floating-menu class="text-center">
            <template v-slot:options>
              <button class="floating-menu-item" @click="goToDetail(props.row.id)">
                <span>Detail</span>
              </button>
              <router-link :to="{ path: '/agency-request/' + props.row.id, query: { tab: 'Applicants' } }">
                <button class="floating-menu-item">
                  <span>Applicants</span>
                </button>
              </router-link>
              <router-link :to="{ path: '/agency-request/' + props.row.id, query: { tab: 'Workers' } }">
                <button class="floating-menu-item">
                  <span>Workers</span>
                </button>
              </router-link>
            </template>
          </floating-menu>
        </b-table-column>
      </template>
    </b-table>

    <!-- recruiters list -->
    <b-modal v-if="tableConfig.showRecruiterModal" v-model="showRecruitersModal" @close="showRecruitersModal = false" width="500px">
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
<script>
import billingAdminMixin from '@/mixins/billingAdminMixin'

export default {
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
      statuses: [
        { id: 0, value: this.$statusDisplayRequested },
        { id: 1, value: this.$statusDisplayInProcess },
        { id: 2, value: this.$statusDisplayCancelled },
        { id: 3, value: this.$statusDisplayOpen },
        { id: 4, value: this.$statusDisplayNoOpen }
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
  mixins: [billingAdminMixin],
  components: {
    FloatingMenu: () => import("../../components/FloatingMenuDots"),
    ModalNotes: () => import("../notes/ModalNotes"),
    ShiftDetail: () => import("../request/ShiftDetail"),
    PersonnelList: () => import("../../components/agency_request/PersonnelListModal"),
    AgencyShift: () => import("../../components/agency_request/AgencyShiftDetail"),
    Export: () => import("@/components/Export"),
    DefaultImage: () => import("@/components/DefaultImage")
  },
  methods: {
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
      this.getAgencyRequests();
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
      this.getAgencyRequests();
    },
    onStatusChange() {
      this.serverParams.statuses = this.statusesSelected.map(ss => ss.id);
      this.getAgencyRequests();
    },
    onLastUpdateSelected() {
      this.serverParams.lastUpdateFrom = this.lastUpdateDatesSelected[0];
      this.serverParams.lastUpdateTo = this.lastUpdateDatesSelected[1];
      this.getAgencyRequests();
    },
    onLastUpdateCleared() {
      this.lastUpdateDatesSelected = [];
      this.onLastUpdateSelected();
    },
    onStartAtSelected() {
      this.serverParams.startAtFrom = this.startAtDatesSelected[0];
      this.serverParams.startAtTo = this.startAtDatesSelected[1];
      this.getAgencyRequests();
    },
    onStartAtCleared() {
      this.startAtDatesSelected = [];
      this.onStartAtSelected();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.getAgencyRequests();
      }
    },
    onNote(row, status) {
      const index = this.rows.findIndex(r => r.id === row.id);
      this.$set(this.rows[index], "showNotes", status);
    },
    onUpdateNote(row, size) {
      const index = this.rows.findIndex(r => r.id === row.id);
      this.$set(this.rows[index], "notesCount", size);
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
      this.getAgencyRequests();
    },
    canEdit(status) {
      return (
        status === this.$statusRequested || status === this.$statusInProcess
      );
    },
    updateWorkers(item) {
      this.rows[this.currentIndex].workersQuantityWorking = item;
    },
    getAgencyRequests() {
      this.checkedRows = [];
      this.$emit("onDataLoading", true);
      if (!this.companyId && !this.agencyId) {
        this.$store.dispatch("agency/updateAgencyRequestFilter", this.serverParams);
      }
      this.$store.dispatch("agency/getAgencyRequests", this.serverParams)
        .then((requests) => {
          this.rows = requests.items.map(i => ({ ...i, actions: null }));
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
      this.$store.dispatch('agency/updateIsAsapRequests', payload)
        .then(() => {
          this.showQuickActions = false;
          this.getAgencyRequests();
        }).catch((error) => {
          this.showQuickActions = false;
          this.showAlertError(error);
          this.$emit("onDataLoading", false);
        });
    }
  },
  created() {
    if (!this.companyId && !this.agencyId) {
      if (this.$store.state.agency.agencyRequestFilter) {
        this.serverParams = this.$store.state.agency.agencyRequestFilter;
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
    this.getAgencyRequests();
  },
  computed: {
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
    },
  },
  watch: {
    'serverParams.onlyMine': function () {
      this.getAgencyRequests();
    }
  }
};
</script>
<style lang="scss">
tr {
  cursor: pointer;
}

td {
  vertical-align: middle !important;
}
</style>