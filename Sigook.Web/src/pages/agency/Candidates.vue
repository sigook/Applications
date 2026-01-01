<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Candidates
        <span class="fw-100 fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <export :url="'/api/AgencyCandidate/File'" :params="serverParams" :fileName="'Candidates'"
        @onDataLoading="(value) => isLoading = value">
        <template v-slot:actions>
          <b-button @click="addFile = true" icon-left="file-plus">Bulk Data</b-button>
          <b-button @click="createCandidate = true" icon-left="plus">{{ $t('Create') }}</b-button>
        </template>
      </export>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="name"
        :current-page.sync="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="name" label="Name" sortable searchable width="400px">
            <template v-slot:searchable>
              <b-field grouped>
                <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small" expanded
                  @keypress.native="onInputEntered"></b-input>
                <b-checkbox v-model="serverParams.resumeOnly" @input="onInputEntered" size="is-small">
                  <b-icon icon="file-download" size="is-small"></b-icon>
                </b-checkbox>
              </b-field>
            </template>
            <template v-slot="props">
              <span class="d-block">
                {{ props.row.name }}
                <b-icon v-if="props.row.hasVehicle" icon="car-back" size="is-small"></b-icon>
                <b-icon v-if="props.row.dnu" icon="alert" size="is-small" type="is-danger"></b-icon>
                <b-icon v-if="props.row.hasDocuments" icon="file-download" size="is-small"
                  class="cursor-poiner"></b-icon>
              </span>
              <i class="fz-2 ellipsis-150 lowercase">
                <a :href="'mailto:' + props.row.email">{{ props.row.email }}</a>
              </i>
            </template>
          </b-table-column>
          <b-table-column field="phoneNumbers" label="Phone" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered" v-cleave="mask"></b-input>
            </template>
            <template v-slot="props">
              <b-taginput size="is-small" v-model="props.row.phoneNumbers" v-cleave="mask" placeholder="Add Phone"
                field="phoneNumber" allow-new @add="addCandidatePhoneNumber(props.row.id, $event)"
                @remove="deleteCandidateNumber(props.row.id, $event)">
              </b-taginput>
            </template>
          </b-table-column>
          <b-table-column field="address" label="Address" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.address" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <p class="capitalize">{{ props.row.address }}</p>
              <i class="fz-2 d-block pl-1">
                {{ props.row.postalCode }}
              </i>
            </template>
          </b-table-column>
          <b-table-column field="skills" label="Skills" width="400px" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.skills" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <skills-form :existingSkills="props.row.skills"
                @onPressAdd="(item) => addCandidateSkills(props.row.id, item)"
                @onDelete="(item) => deleteCandidateSkill(props.row.id, item)" />
            </template>
          </b-table-column>
          <b-table-column field="requests" label="Order ID" width="200px" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <div v-if="props.row.requests && props.row.requests.length > 0">
                <b-taglist>
                  <b-tag v-for="request in props.row.requests" :key="request.id" rounded
                    @click="goToApplicants(request)">
                    {{ request.value }}
                  </b-tag>
                </b-taglist>
              </div>
              <b-button size="is-small" type="primary" icon-right="plus" rounded
                @click="showCandidateRequests(props.row.id, props.row.index)">
              </b-button>
            </template>
          </b-table-column>
          <b-table-column field="source" label="Source" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.source" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span class="d-block">{{ props.row.source }}</span>
            </template>
          </b-table-column>
          <b-table-column field="createdAt" label="Created At" sortable searchable>
            <template v-slot:searchable>
              <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
                :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
                @icon-right-click="onCreatedAtCleared" range v-model="createdAtDatesSelected"
                @input="onCreatedAtSelected">
              </b-datepicker>
            </template>
            <template v-slot="props">
              <span class="d-block">{{ props.row.createdAt | dateMonth }}</span>
            </template>
          </b-table-column>
          <b-table-column field="recruiter" label="Recruiter" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.recruiter" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <div class="capitalize is-inline-block v-middle pr-0" v-if="props.row.recruiter">
                {{ props.row.recruiter | emailName }}
              </div>
              <div v-else class="op3 is-inline-block v-middle pr-0">
                Recruiter
              </div>
              <button type="button" class="btn-icon-sm btn-icon-worker-plus is-inline-block v-middle"
                @click="updateCandidateRecruiter(props.row.id)" style="position: relative; top: 2px"></button>
            </template>
          </b-table-column>
          <b-table-column field="notesCount" label="Notes" v-slot="props">
            <div @click="onNote(props.row, true)">
              <b-tag icon="note-multiple" rounded>
                <label v-if="props.row.notesCount">{{ props.row.notesCount }}</label>
              </b-tag>
            </div>
            <div v-if="props.row.showNotes" class="notes-tooltip">
              <modal-notes :can-create="false" :user-id="props.row.id" :on-get="'agency/getCandidateNotes'"
                :on-create="'agency/addCandidateNote'" :on-delete="'agency/deleteCandidateNote'"
                @onUpdateNote="(val) => onUpdateNote(props.row, val.size)" @close="onNote(props.row, false)">
              </modal-notes>
            </div>
          </b-table-column>
          <b-table-column field="residencyStatus" label="Status" width="250px" sortable searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="residencyList" open-on-focus
                field="value" icon="label" placeholder="Select Status" @input="onStatusSelected">
              </b-taginput>
            </template>
            <template v-slot="props">
              <b-tag v-if="props.row.residencyStatus" size="is-medium" rounded>
                {{ props.row.residencyStatus }}
              </b-tag>
            </template>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <floating-menu class="text-center">
              <template v-slot:options>
                <button class="floating-menu-item" @click="showCandidateDetail(props.row.id)">
                  Edit
                </button>
                <button class="floating-menu-item" @click="showDocumentsCandidate(props.row.id)">
                  Documents
                </button>
                <button class="floating-menu-item" :disabled="!props.row.email || props.row.dnu"
                  @click="convertToWorker(props.row.id)">
                  Convert to Worker
                </button>
                <button class="floating-menu-item" @click="deleteCandidate(props.row.id)">
                  Delete
                </button>
              </template>
            </floating-menu>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <b-modal v-model="showDocuments" @close="showDocuments = false" width="500px">
      <modal-documents :candidateId="detailId" />
    </b-modal>


    <b-modal v-model="createCandidate" @close="createCandidate = false" width="500px">
      <create-candidate @onClose="onCandidateCreated()"></create-candidate>
    </b-modal>

    <b-modal v-model="detailCandidate" @close="detailCandidate = false" width="500px">
      <detail-candidate :candidate-id="detailId" @onUpdateWorker="() => updateCandidate()"></detail-candidate>
    </b-modal>

    <b-modal v-model="addFile" @close="addFile = false" width="500px">
      <bulk-data :store-action="'agency/bulkCandidates'" :error-file-name="'BulkCandidatesError'"
        :title="'Bulk Candidates'" :file-label="'Candidates File'" @close="addFile = false" />
    </b-modal>

    <b-modal v-model="showRequestModal" width="500px">
      <candidate-request :candidate-id="detailId" @onSelectRequest="onSelectRequest" />
    </b-modal>
  </div>
</template>
<script>
import download from "@/mixins/downloadFileMixin";
import phoneMaskMixin from "@/mixins/phoneMaskMixin"
import phoneFormat from "@/mixins/phoneFormatMixin";

export default {
  data() {
    return {
      isLoading: false,
      totalItems: 0,
      createdAtDatesSelected: [],
      statusesSelected: [],
      createCandidate: false,
      addFile: false,
      detailCandidate: false,
      detailId: null,
      showDocuments: false,
      showRequestModal: false,
      rows: [],
      serverParams: {
        sortBy: 0,
        isDescending: false,
        pageIndex: 1,
        pageSize: 30
      }
    };
  },
  mixins: [phoneMaskMixin, download, phoneFormat],
  components: {
    CreateCandidate: () => import("@/components/candidate/CreateCandidate"),
    DetailCandidate: () => import("@/components/candidate/DetailCandidate"),
    ModalDocuments: () => import("@/components/candidate/ModalDocuments"),
    ModalNotes: () => import("@/components/notes/ModalNotes"),
    SkillsForm: () => import("@/components/FormSkillAdd.vue"),
    FloatingMenu: () => import("@/components/FloatingMenuDots"),
    CandidateRequest: () => import("@/components/candidate/ModalCandidateRequests"),
    BulkData: () => import("@/components/agency/BulkData"),
    Export: () => import("@/components/Export")
  },
  methods: {
    onCellClick(row, column, rowIndex) {
      if (column._props.field === 'name' && row.hasDocuments) {
        this.showDocumentsCandidate(row.id);
      }
    },
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getAgencyCandidates();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'name':
          this.serverParams.sortBy = 0;
          break
        case 'address':
          this.serverParams.sortBy = 1;
          break;
        case 'skills':
          this.serverParams.sortBy = 2;
          break;
        case 'createdAt':
          this.serverParams.sortBy = 3;
          break;
        case 'recruiter':
          this.serverParams.sortBy = 4;
          break;
        case 'residencyStatus':
          this.serverParams.sortBy = 5;
          break;
        case 'source':
          this.serverParams.sortBy = 6;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.getAgencyCandidates();
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.getAgencyCandidates();
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    onStatusSelected() {
      this.serverParams.statuses = this.statusesSelected;
      this.getAgencyCandidates();
    },
    onInputEntered(event) {
      if (typeof event === 'boolean') {
        this.getAgencyCandidates();
      }
      else if (event.key === 'Enter') {
        this.getAgencyCandidates();
      }
    },
    getAgencyCandidates() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyCandidateFilter", this.serverParams);
      this.$store.dispatch("agency/getAgencyCandidates", this.serverParams)
        .then(candidates => {
          this.rows = candidates.items.map(c => ({ ...c, actions: null }));
          this.totalItems = candidates.totalItems;
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    showCandidateDetail(id) {
      this.detailId = id;
      this.detailCandidate = true;
    },
    onNote(row, status) {
      const index = this.rows.findIndex(r => r.id === row.id);
      this.$set(this.rows[index], "showNotes", status);
    },
    onUpdateNote(row, size) {
      const index = this.rows.findIndex(r => r.id === row.id);
      this.$set(this.rows[index], "notesCount", size);
    },
    showDocumentsCandidate(id) {
      this.detailId = id;
      this.showDocuments = true;
    },
    showCandidateRequests(id) {
      this.detailId = id;
      this.showRequestModal = true;
    },
    onSelectRequest() {
      this.showRequestModal = false;
      this.getAgencyCandidates();
    },
    addCandidatePhoneNumber(candidateId, phone) {
      this.isLoading = true;
      this.$store.dispatch('agency/addCandidateNumber', { candidateId: candidateId, model: { phoneNumber: phone } })
        .then(() => {
          this.getAgencyCandidates();
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    deleteCandidateNumber(candidateId, number) {
      this.isLoading = true;
      this.$store.dispatch("agency/deleteCandidateNumber", { candidateId: candidateId, number: number.id })
        .then(() => {
          this.isLoading = false;
          this.getAgencyCandidates();
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    addCandidateSkills(id, model) {
      this.isLoading = true;
      this.$store.dispatch("agency/addCandidateSkill", { candidateId: id, model: model })
        .then(() => {
          this.isLoading = false;
          this.getAgencyCandidates();
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    deleteCandidateSkill(candidateId, skill) {
      this.isLoading = true;
      this.$store
        .dispatch("agency/deleteCandidateSkill", { candidateId: candidateId, skill: skill.id })
        .then(() => {
          this.isLoading = false;
          this.getAgencyCandidates();
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    deleteCandidate(candidateId) {
      this.showAlertConfirm("Are you sure", "You want to delete this candidate")
        .then((response) => {
          if (response) {
            this.isLoading = true;
            this.$store.dispatch("agency/deleteCandidate", candidateId)
              .then(() => {
                this.isLoading = false;
                this.getAgencyCandidates();
              })
              .catch((error) => {
                this.isLoading = false;
                this.showAlertError(error);
              });
          }
        })
        .catch((error) => {
          this.showAlertError(error);
        });
    },
    updateCandidateRecruiter(candidateId) {
      let vm = this;
      this.showAlertConfirm("Do you want to manage this candidate?", "")
        .then((response) => {
          if (response) {
            vm.isLoading = true;
            vm.$store
              .dispatch("agency/updateCandidateRecruiter", candidateId)
              .then(() => {
                vm.isLoading = false;
                vm.getAgencyCandidates(this.currentPage);
              })
              .catch((error) => {
                vm.isLoading = false;
                vm.showAlertError(error);
              });
          }
        })
        .catch((error) => {
          this.showAlertError(error);
        });
    },
    convertToWorker(candidateId) {
      this.isLoading = true;
      this.$store.dispatch("agency/convertToWorker", candidateId)
        .then(() => {
          this.isLoading = false;
          this.getAgencyCandidates();
        })
        .catch((error) => {
          this.isLoading = false
          this.showAlertError(error);
        })
    },
    goToApplicants(item) {
      this.$router.push({
        path: `/agency-request/${item.id}`,
        query: { tab: 'Applicants' }
      });
    },
    onCandidateCreated() {
      this.createCandidate = false;
      this.getAgencyCandidates();
    },
    updateCandidate() {
      this.detailCandidate = false
      this.getAgencyCandidates();
    },
  },
  created() {
    if (this.$store.state.agency.agencyCandidateFilter) {
      this.serverParams = this.$store.state.agency.agencyCandidateFilter;
      if (this.serverParams.statuses) {
        this.statusesSelected = this.residencyList.filter(s => this.serverParams.statuses.some(sps => sps == s));
      }
      if (this.serverParams.createdAtFrom && this.serverParams.createdAtTo) {
        this.createdAtDatesSelected[0] = this.serverParams.createdAtFrom;
        this.createdAtDatesSelected[1] = this.serverParams.createdAtTo;
      }
    }
    this.getAgencyCandidates();
  },
  computed: {
    residencyList() {
      return this.$store.state.catalog.residencyList;
    },
    agencies() {
      return this.$store.state.agency.personnelAgencies;
    }
  }
};
</script>