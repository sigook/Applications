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
          <b-button @click="createCandidate = true" icon-left="plus">{{ 'Create' }}</b-button>
        </template>
        <template v-slot:dropdown-actions>
          <b-dropdown-item aria-role="listitem" @click="addFile = true">
            <b-icon icon="file-plus"></b-icon>
            <span>Bulk Data</span>
          </b-dropdown-item>
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
          <b-table-column field="name" label="Name" sortable searchable>
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
          <b-table-column field="skills" label="Skills" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.skills" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <skills-form :existingSkills="props.row.skills"
                @onPressAdd="(item) => addCandidateSkills(props.row.id, item)"
                @onDelete="(item) => onDeleteCandidateSkill(props.row.id, item)" />
            </template>
          </b-table-column>
          <b-table-column field="requests" label="Order ID" searchable>
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
                @input="onCreatedAtSelected" append-to-body>
              </b-datepicker>
            </template>
            <template v-slot="props">
              <span class="d-block">{{ dateMonth(props.row.createdAt) }}</span>
            </template>
          </b-table-column>
          <b-table-column field="recruiter" label="Recruiter" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.recruiter" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <div class="capitalize is-inline-block v-middle pr-0" v-if="props.row.recruiter">
                {{ emailName(props.row.recruiter) }}
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
              <modal-notes :can-create="false" :user-id="props.row.id" :on-get="getCandidateNotes"
                :on-create="addCandidateNote" :on-delete="deleteCandidateNote"
                @onUpdateNote="(val) => onUpdateNote(props.row, val.size)" @close="onNote(props.row, false)">
              </modal-notes>
            </div>
          </b-table-column>
          <b-table-column field="residencyStatus" label="Status" sortable searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="residencyList" open-on-focus
                field="value" icon="label" placeholder="Select Status" @input="onStatusSelected" append-to-body>
              </b-taginput>
            </template>
            <template v-slot="props">
              <b-tag v-if="props.row.residencyStatus" size="is-medium" rounded>
                {{ props.row.residencyStatus }}
              </b-tag>
            </template>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
              <template #trigger>
                <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
              </template>
              <b-dropdown-item aria-role="listitem" @click="showCandidateDetail(props.row.id)">
                Edit
              </b-dropdown-item>
              <b-dropdown-item aria-role="listitem" @click="showDocumentsCandidate(props.row.id)">
                Documents
              </b-dropdown-item>
              <b-dropdown-item aria-role="listitem" :disabled="!props.row.email || props.row.dnu"
                @click="convertToWorker(props.row.id)">
                Convert to Worker
              </b-dropdown-item>
              <b-dropdown-item aria-role="listitem" @click="onDeleteCandidate(props.row.id)">
                Delete
              </b-dropdown-item>
            </b-dropdown>
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
      <bulk-data :upload-fn="bulkAgencyCandidates" :error-file-name="'BulkCandidatesError'"
        :title="'Bulk Candidates'" :file-label="'Candidates File'" @close="addFile = false" />
    </b-modal>

    <b-modal v-model="showRequestModal" width="500px">
      <candidate-request :candidate-id="detailId" @onSelectRequest="onSelectRequest" />
    </b-modal>
  </div>
</template>
<script lang="ts">
import { mapStores } from 'pinia';
import { useAgencyStore } from '@/stores/agency';
import { showAlertConfirm, showAlertError } from "@/utils/toast";
import { phoneMask as mask } from '@/constants/phoneMask';
import { residencyList } from "@/constants/catalog";
import {
  getAgencyCandidates,
  addCandidatePhoneNumber,
  deleteCandidatePhoneNumber,
  addCandidateSkill,
  deleteCandidateSkill,
  deleteAgencyCandidate,
  updateAgencyCandidateRecruiter,
  convertCandidateToWorker,
  bulkAgencyCandidates
} from "@/api/agencyCandidateApi";
import { dateMonth, emailName } from '@/utils/filters';
import {
  getCandidateNotes,
  createCandidateNote,
  deleteCandidateNote
} from "@/api/agencyNoteApi";
import type { NotesFetchPayload, NotesCreatePayload, NotesDeletePayload } from '@/types/agency';

export default {
  data() {
    return {
      mask,
      isLoading: false,
      totalItems: 0,
      createdAtDatesSelected: [],
      statusesSelected: [],
      createCandidate: false,
      addFile: false,
      bulkAgencyCandidates,
      getCandidateNotes: ({ userId, pagination }: NotesFetchPayload) => getCandidateNotes(userId, pagination),
      addCandidateNote: ({ userId, model }: NotesCreatePayload) => createCandidateNote(userId, model),
      deleteCandidateNote: ({ userId, id }: NotesDeletePayload) => deleteCandidateNote(userId, id),
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
  components: {
    CreateCandidate: () => import("@/components/candidate/CreateCandidate.vue"),
    DetailCandidate: () => import("@/components/candidate/DetailCandidate.vue"),
    ModalDocuments: () => import("@/components/candidate/ModalDocuments.vue"),
    ModalNotes: () => import("@/components/notes/ModalNotes.vue"),
    SkillsForm: () => import("@/components/FormSkillAdd.vue"),
    CandidateRequest: () => import("@/components/candidate/ModalCandidateRequests.vue"),
    BulkData: () => import("@/components/agency/BulkData.vue"),
    Export: () => import("@/components/Export.vue")
  },
  methods: {
    dateMonth,
    emailName,
    onCellClick(row, column) {
      if (column._props.field === 'name' && row.hasDocuments) {
        this.showDocumentsCandidate(row.id);
      }
    },
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.loadCandidates();
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
      this.loadCandidates();
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.loadCandidates();
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    onStatusSelected() {
      this.serverParams.statuses = this.statusesSelected;
      this.loadCandidates();
    },
    onInputEntered(event) {
      if (typeof event === 'boolean') {
        this.loadCandidates();
      }
      else if (event.key === 'Enter') {
        this.loadCandidates();
      }
    },
    loadCandidates() {
      this.isLoading = true;
      this.agencyStore.updateAgencyCandidateFilter(this.serverParams);
      getAgencyCandidates(this.serverParams)
        .then(candidates => {
          this.rows = candidates.items.map(c => ({ ...c, actions: null, showNotes: false, notesCount: c.notesCount || 0 }));
          this.totalItems = candidates.totalItems;
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    showCandidateDetail(id) {
      this.detailId = id;
      this.detailCandidate = true;
    },
    onNote(row, status) {
      const index = this.rows.findIndex(r => r.id === row.id);
      this.rows[index].showNotes = status;
    },
    onUpdateNote(row, size) {
      const index = this.rows.findIndex(r => r.id === row.id);
      this.rows[index].notesCount = size;
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
      this.loadCandidates();
    },
    addCandidatePhoneNumber(candidateId, phone) {
      this.isLoading = true;
      addCandidatePhoneNumber(candidateId, { phoneNumber: phone })
        .then(() => {
          this.loadCandidates();
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    deleteCandidateNumber(candidateId, number) {
      this.isLoading = true;
      deleteCandidatePhoneNumber(candidateId, number.id)
        .then(() => {
          this.isLoading = false;
          this.loadCandidates();
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    addCandidateSkills(id, model) {
      this.isLoading = true;
      addCandidateSkill(id, model)
        .then(() => {
          this.isLoading = false;
          this.loadCandidates();
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    onDeleteCandidateSkill(candidateId, skill) {
      this.isLoading = true;
      deleteCandidateSkill(candidateId, skill.id)
        .then(() => {
          this.isLoading = false;
          this.loadCandidates();
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    onDeleteCandidate(candidateId) {
      showAlertConfirm("Are you sure", "You want to delete this candidate")
        .then((response) => {
          if (response) {
            this.isLoading = true;
            deleteAgencyCandidate(candidateId)
              .then(() => {
                this.isLoading = false;
                this.loadCandidates();
              })
              .catch((error) => {
                this.isLoading = false;
                showAlertError(error);
              });
          }
        })
        .catch((error) => {
          showAlertError(error);
        });
    },
    updateCandidateRecruiter(candidateId) {
      showAlertConfirm("Do you want to manage this candidate?", "")
        .then((response) => {
          if (response) {
            this.isLoading = true;
            updateAgencyCandidateRecruiter(candidateId)
              .then(() => {
                this.isLoading = false;
                this.loadCandidates();
              })
              .catch((error) => {
                this.isLoading = false;
                showAlertError(error);
              });
          }
        })
        .catch((error) => {
          showAlertError(error);
        });
    },
    convertToWorker(candidateId) {
      this.isLoading = true;
      convertCandidateToWorker(candidateId)
        .then(() => {
          this.isLoading = false;
          this.loadCandidates();
        })
        .catch((error) => {
          this.isLoading = false
          showAlertError(error);
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
      this.loadCandidates();
    },
    updateCandidate() {
      this.detailCandidate = false
      this.loadCandidates();
    }
  },
  created() {
    if (this.agencyStore.agencyCandidateFilter) {
      this.serverParams = this.agencyStore.agencyCandidateFilter;
      if (this.serverParams.statuses) {
        this.statusesSelected = this.residencyList.filter(s => this.serverParams.statuses.some(sps => sps == s));
      }
      if (this.serverParams.createdAtFrom && this.serverParams.createdAtTo) {
        this.createdAtDatesSelected[0] = this.serverParams.createdAtFrom;
        this.createdAtDatesSelected[1] = this.serverParams.createdAtTo;
      }
    }
    this.loadCandidates();
  },
  computed: {
    ...mapStores(useAgencyStore),
    residencyList() {
      return residencyList;
    },
    agencies() {
      return this.agencyStore.personnelAgencies;
    }
  }
};
</script>
