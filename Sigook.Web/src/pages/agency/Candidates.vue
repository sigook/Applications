<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Candidates
        <span class="fw-light fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <export :url="'/api/agency/candidates/File'" :params="serverParams" :fileName="'Candidates'"
        @onDataLoading="(value) => isLoading = value">
        <template v-slot:actions>
          <b-button @click="showCreateCandidate = true" icon-left="plus">{{ 'Create' }}</b-button>
        </template>
        <template v-slot:dropdown-actions>
          <b-dropdown-item aria-role="listitem" @click="addFile = true">
            <b-icon icon="file-plus"></b-icon>
            <span>Bulk Data</span>
          </b-dropdown-item>
        </template>
      </export>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="name"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="name" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-field grouped>
                <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small" expanded
                  @keypress="onInputEntered"></b-input>
                <b-checkbox v-model="serverParams.resumeOnly" @update:modelValue="onInputEntered" size="is-small">
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
              <i class="fz-2 ellipsis-150 text-lowercase">
                <a :href="'mailto:' + props.row.email">{{ props.row.email }}</a>
              </i>
            </template>
          </b-table-column>
          <b-table-column field="phoneNumbers" label="Phone" searchable>
            <template v-slot:searchable>
              <b-input :model-value="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered" @update:modelValue="(v) => serverParams.phone = formatPhone(v)"></b-input>
            </template>
            <template v-slot="props">
              <b-taginput size="is-small" v-model="props.row.phoneNumbers" :before-adding="formatPhone" placeholder="Add Phone"
                field="phoneNumber" allow-new @add="addCandidatePhoneNumberHandler(props.row.id, $event)"
                @remove="deleteCandidateNumber(props.row.id, $event)">
              </b-taginput>
            </template>
          </b-table-column>
          <b-table-column field="address" label="Address" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.address" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <p class="text-capitalize">{{ props.row.address }}</p>
              <i class="fz-2 d-block ps-1">
                {{ props.row.postalCode }}
              </i>
            </template>
          </b-table-column>
          <b-table-column field="skills" label="Skills" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.skills" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <skills-form :existingSkills="props.row.skills"
                @onPressAdd="(item) => addCandidateSkills(props.row.id, item)"
                @onDelete="(item) => onDeleteCandidateSkill(props.row.id, item)" />
            </template>
          </b-table-column>
          <b-table-column field="requests" label="Request ID" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
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
                @click="showCandidateRequests(props.row.id)">
              </b-button>
            </template>
          </b-table-column>
          <b-table-column field="source" label="Source" sortable searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="sourcesSelected" autocomplete :data="sourceList" open-on-focus
                field="value" icon="label" placeholder="Select Source" @update:modelValue="onSourceSelected"
                append-to-body>
              </b-taginput>
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
                @update:modelValue="onCreatedAtSelected" append-to-body>
              </b-datepicker>
            </template>
            <template v-slot="props">
              <span class="d-block">{{ dateMonth(props.row.createdAt) }}</span>
            </template>
          </b-table-column>
          <b-table-column field="recruiter" label="Recruiter" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.recruiter" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <div class="text-capitalize is-inline-block align-middle pe-0" v-if="props.row.recruiter">
                {{ emailName(props.row.recruiter) }}
              </div>
              <div v-else class="op3 is-inline-block align-middle pe-0">
                Recruiter
              </div>
              <button type="button" class="btn-icon-sm btn-icon-worker-plus is-inline-block align-middle"
                @click="updateCandidateRecruiter(props.row.id)" style="position: relative; top: 2px"></button>
            </template>
          </b-table-column>
          <b-table-column field="notesCount" label="Notes" v-slot="props">
            <div @click="onNote(props.row, true)">
              <b-tag icon="note-text" rounded>
                <label v-if="props.row.notesCount">{{ props.row.notesCount }}</label>
              </b-tag>
            </div>
            <div v-if="props.row.showNotes" class="notes-tooltip">
              <modal-notes :can-create="false" :user-id="props.row.id" :on-get="getCandidateNotesFn"
                :on-create="addCandidateNoteFn" :on-delete="deleteCandidateNoteFn"
                @onUpdateNote="(val) => onUpdateNote(props.row, val.size)" @close="onNote(props.row, false)">
              </modal-notes>
            </div>
          </b-table-column>
          <b-table-column field="residencyStatus" label="Status" sortable searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="residencyListValue" open-on-focus
                field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusSelected" append-to-body>
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


    <b-modal v-model="showCreateCandidate" @close="showCreateCandidate = false" width="500px">
      <create-candidate @onClose="onCandidateCreated()"></create-candidate>
    </b-modal>

    <b-modal v-model="showDetailCandidate" @close="showDetailCandidate = false" width="500px">
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
<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAgencyStore } from '@/stores/agency';
import { showAlertConfirm, showAlertError } from '@/utils/toast';
import { formatPhone } from '@/utils/phoneFormat';
import { residencyList } from '@/constants/catalog';
import {
  getAgencyCandidates,
  addCandidatePhoneNumber,
  deleteCandidatePhoneNumber,
  addCandidateSkill,
  deleteCandidateSkill,
  deleteAgencyCandidate,
  updateAgencyCandidateRecruiter,
  convertCandidateToWorker,
  bulkAgencyCandidates,
} from '@/api/agencyCandidateApi';
import { getSources } from '@/api/catalogApi';
import type { Source } from '@/types/common';
import { dateMonth, emailName } from '@/utils/filters';
import {
  getCandidateNotes,
  createCandidateNote,
  deleteCandidateNote,
} from '@/api/agencyNoteApi';
import type { NotesFetchPayload, NotesCreatePayload, NotesDeletePayload } from '@/types/agency';
import CreateCandidate from '@/components/candidate/CreateCandidate.vue';
import DetailCandidate from '@/components/candidate/DetailCandidate.vue';
import ModalDocuments from '@/components/candidate/ModalDocuments.vue';
import ModalNotes from '@/components/notes/ModalNotes.vue';
import SkillsForm from '@/components/FormSkillAdd.vue';
import CandidateRequest from '@/components/candidate/ModalCandidateRequests.vue';
import BulkData from '@/components/agency/BulkData.vue';
import Export from '@/components/Export.vue';

const router = useRouter();
const agencyStore = useAgencyStore();

const isLoading = ref(false);
const totalItems = ref(0);
const createdAtDatesSelected = ref<any[]>([]);
const statusesSelected = ref<any[]>([]);
const showCreateCandidate = ref(false);
const addFile = ref(false);
const showDetailCandidate = ref(false);
const detailId = ref<any>(null);
const showDocuments = ref(false);
const showRequestModal = ref(false);
const rows = ref<any[]>([]);
const serverParams = ref<any>({
  sortBy: 0,
  isDescending: false,
  pageIndex: 1,
  pageSize: 30,
});

const residencyListValue = residencyList;
const sourceList = ref<Source[]>([]);
const sourcesSelected = ref<Source[]>([]);

getSources()
  .then((sources) => {
    sourceList.value = sources;
    if (serverParams.value.sources) {
      sourcesSelected.value = sources.filter((s) => serverParams.value.sources.includes(s.value));
    }
  })
  .catch((error) => {
    showAlertError(error);
  });

const getCandidateNotesFn = ({ userId, pagination }: NotesFetchPayload) => getCandidateNotes(userId, pagination);
const addCandidateNoteFn = ({ userId, model }: NotesCreatePayload) => createCandidateNote(userId, model);
const deleteCandidateNoteFn = ({ userId, id }: NotesDeletePayload) => deleteCandidateNote(userId, id);

if (agencyStore.agencyCandidateFilter) {
  serverParams.value = agencyStore.agencyCandidateFilter;
  if (serverParams.value.statuses) {
    statusesSelected.value = residencyList.filter((s: any) => serverParams.value.statuses.some((sps: any) => sps == s));
  }
  if (serverParams.value.createdAtFrom && serverParams.value.createdAtTo) {
    createdAtDatesSelected.value[0] = serverParams.value.createdAtFrom;
    createdAtDatesSelected.value[1] = serverParams.value.createdAtTo;
  }
}
loadCandidates();

function onCellClick(row: any, column: any) {
  if (column.field === 'name' && row.hasDocuments) {
    showDocumentsCandidate(row.id);
  }
}

function onPageChange(params: number) {
  serverParams.value.pageIndex = params;
  loadCandidates();
}

function onSortChange(field: string, order: string) {
  switch (field) {
    case 'name':
      serverParams.value.sortBy = 0;
      break;
    case 'address':
      serverParams.value.sortBy = 1;
      break;
    case 'skills':
      serverParams.value.sortBy = 2;
      break;
    case 'createdAt':
      serverParams.value.sortBy = 3;
      break;
    case 'recruiter':
      serverParams.value.sortBy = 4;
      break;
    case 'residencyStatus':
      serverParams.value.sortBy = 5;
      break;
    case 'source':
      serverParams.value.sortBy = 6;
      break;
  }
  serverParams.value.isDescending = order !== 'asc';
  loadCandidates();
}

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0];
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1];
  loadCandidates();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onSourceSelected() {
  serverParams.value.sources = sourcesSelected.value.map((s) => s.value);
  loadCandidates();
}

function onStatusSelected() {
  serverParams.value.statuses = statusesSelected.value;
  loadCandidates();
}

function onInputEntered(event: any) {
  if (typeof event === 'boolean') {
    loadCandidates();
  } else if (event.key === 'Enter') {
    loadCandidates();
  }
}

function loadCandidates() {
  isLoading.value = true;
  agencyStore.updateAgencyCandidateFilter(serverParams.value);
  getAgencyCandidates(serverParams.value)
    .then((candidates: any) => {
      rows.value = candidates.items.map((c: any) => ({ ...c, actions: null, showNotes: false, notesCount: c.notesCount || 0 }));
      totalItems.value = candidates.totalItems;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function showCandidateDetail(id: any) {
  detailId.value = id;
  showDetailCandidate.value = true;
}

function onNote(row: any, status: boolean) {
  const index = rows.value.findIndex((r) => r.id === row.id);
  rows.value[index].showNotes = status;
}

function onUpdateNote(row: any, size: number) {
  const index = rows.value.findIndex((r) => r.id === row.id);
  rows.value[index].notesCount = size;
}

function showDocumentsCandidate(id: any) {
  detailId.value = id;
  showDocuments.value = true;
}

function showCandidateRequests(id: any) {
  detailId.value = id;
  showRequestModal.value = true;
}

function onSelectRequest() {
  showRequestModal.value = false;
  loadCandidates();
}

function addCandidatePhoneNumberHandler(candidateId: any, phone: any) {
  isLoading.value = true;
  addCandidatePhoneNumber(candidateId, { phoneNumber: phone })
    .then(() => {
      loadCandidates();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function deleteCandidateNumber(candidateId: any, number: any) {
  isLoading.value = true;
  deleteCandidatePhoneNumber(candidateId, number.id)
    .then(() => {
      isLoading.value = false;
      loadCandidates();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function addCandidateSkills(id: any, model: any) {
  isLoading.value = true;
  addCandidateSkill(id, model)
    .then(() => {
      isLoading.value = false;
      loadCandidates();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onDeleteCandidateSkill(candidateId: any, skill: any) {
  isLoading.value = true;
  deleteCandidateSkill(candidateId, skill.id)
    .then(() => {
      isLoading.value = false;
      loadCandidates();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onDeleteCandidate(candidateId: any) {
  showAlertConfirm('Are you sure', 'You want to delete this candidate')
    .then((response) => {
      if (response) {
        isLoading.value = true;
        deleteAgencyCandidate(candidateId)
          .then(() => {
            isLoading.value = false;
            loadCandidates();
          })
          .catch((error) => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    })
    .catch((error) => {
      showAlertError(error);
    });
}

function updateCandidateRecruiter(candidateId: any) {
  showAlertConfirm('Do you want to manage this candidate?', '')
    .then((response) => {
      if (response) {
        isLoading.value = true;
        updateAgencyCandidateRecruiter(candidateId)
          .then(() => {
            isLoading.value = false;
            loadCandidates();
          })
          .catch((error) => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    })
    .catch((error) => {
      showAlertError(error);
    });
}

function convertToWorker(candidateId: any) {
  isLoading.value = true;
  convertCandidateToWorker(candidateId)
    .then(() => {
      isLoading.value = false;
      loadCandidates();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function goToApplicants(item: any) {
  router.push({
    path: `/recruiting/requests/${item.id}`,
    query: { tab: 'Applicants' },
  });
}

function onCandidateCreated() {
  showCreateCandidate.value = false;
  loadCandidates();
}

function updateCandidate() {
  showDetailCandidate.value = false;
  loadCandidates();
}
</script>
