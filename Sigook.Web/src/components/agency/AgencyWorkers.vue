<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-field grouped position="is-right">
        <b-button @click="modalManageWorkers = true" icon-right="calendar-start">
          {{ $t("AgencyManageWorkers") }}
        </b-button>
        <b-button type="is-ghost" icon-right="file-excel"
          @click="downloadWorkersReportDocument">Export</b-button>
      </b-field>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="name"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick" @mouseleave="hideNotes">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="profileImage" width="50" v-slot="props">
            <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image"
              class="img-30 img-rounded" />
            <default-image v-else :name="props.row.fullName" class="img-30"></default-image>
          </b-table-column>
          <b-table-column field="numberId" label="ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span :class="props.row.isSubcontractor ? 'Blue' : ''">{{ props.row.numberId }}</span>
            </template>
          </b-table-column>
          <b-table-column field="externalId" label="External ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.externalId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">{{ props.row.externalId }}</template>
          </b-table-column>
          <b-table-column field="name" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              {{ props.row.name }}
            </template>
          </b-table-column>
          <b-table-column field="mobileNumber" label="Phone" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered" v-cleave="mask"></b-input>
            </template>
            <template v-slot="props">{{ props.row.mobileNumber }}</template>
          </b-table-column>
          <b-table-column field="socialInsurance" label="SIN/SSN" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.socialInsurance" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <div v-if="props.row.socialInsurance">
                {{ props.row.socialInsurance }}
                <i class="fz-2 block">{{ dateMonth(props.row.dueDate) }}</i>
              </div>
              <span v-else class="op3">SIN/SNN</span>
            </template>
          </b-table-column>
          <b-table-column field="startWorking" label="Start Working" sortable searchable>
            <template v-slot:searchable>
              <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
                :icon-right="startWorkingDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
                @icon-right-click="onStartWorkingCleared" range v-model="startWorkingDatesSelected"
                @input="onStartWorkingSelected" append-to-body>
              </b-datepicker>
            </template>
            <template v-slot="props">
              <b-button type="is-ghost" icon-right="pencil" @click="onShowModalStartWorking(props.row)">
                {{ dateMonth(props.row.startWorking) }}
              </b-button>
            </template>
          </b-table-column>
          <b-table-column field="createdBy" label="Created By" sortable searchable>
            <template v-slot:searchable>
              <b-field>
                <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.createdBy"
                  @keypress.native="onInputEntered"></b-input>
                <b-datepicker size="is-small" :mobile-native="false" placeholder="Created At"
                  :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                  v-model="createdAtDatesSelected" icon-right-clickable @icon-right-click="onCreatedAtCleared"
                  @input="onCreatedAtSelected" append-to-body></b-datepicker>
              </b-field>
            </template>
            <template v-slot="props">
              {{ emailName(props.row.createdBy) }}
              <i class="fz-2 block">{{ dateMonth(props.row.createdAt) }}</i>
            </template>
          </b-table-column>
          <b-table-column field="rejectedBy" label="Rejected By" sortable searchable>
            <template v-slot:searchable>
              <b-field>
                <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.rejectedBy"
                  @keypress.native="onInputEntered"></b-input>
                <b-datepicker size="is-small" :mobile-native="false" placeholder="Created At"
                  :icon-right="rejectedAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                  v-model="rejectedAtDatesSelected" icon-right-clickable @icon-right-click="onRejectedAtCleared"
                  @input="onRejectedAtSelected" append-to-body></b-datepicker>
              </b-field>
            </template>
            <template v-slot="props">
              <div v-if="props.row.rejectedBy">
                {{ emailName(props.row.rejectedBy) }}
                <i class="fz-2 block">{{ dateMonth(props.row.rejectedAt) }}</i>
              </div>
              <span v-else class="op3">Rejected by</span>
            </template>
          </b-table-column>
          <b-table-column field="notesCount" label="Notes" v-slot="props">
            <b-tag icon="note-multiple" rounded>
              <label v-if="props.row.notesCount">{{ props.row.notesCount }}</label>
            </b-tag>
            <div v-if="props.row.showNotes" class="notes-tooltip">
              <modal-notes :can-create="false" :user-id="props.row.id" :request-id="serverParams.requestId" :on-get="getNotes"
                :on-create="createNote" :on-update="updateNote"
                :on-delete="deleteNote"
                @onUpdateNote="(val) => onUpdateNote(props.row, val.size)">
              </modal-notes>
            </div>
          </b-table-column>
          <b-table-column field="status" label="Status" sortable searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
                field="value" icon="label" placeholder="Select Status" @input="onStatusSelected" append-to-body>
              </b-taginput>
            </template>
            <template v-slot="props">
              <span class="uppercase fw-700 fz-1" :class="props.row.status">{{ props.row.status }}</span>
              <i class="fz-1 block" v-html="props.row.rejectComments"></i>
            </template>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-field>
              <b-tooltip label="Reject" type="is-dark" position="is-top" append-to-body>
                <b-button type="is-danger" outlined rounded icon-right="close"
                  v-if="props.row.status === 'Booked'" @click="confirmDelete(props.row)"></b-button>
              </b-tooltip>
            </b-field>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <!-- custom modal Manage Workers-->
    <b-modal v-model="modalManageWorkers" width="500px">
      <workers-list :request-id="serverParams.requestId" @workerBooked="onWorkerBooked"></workers-list>
    </b-modal>

    <b-modal v-model="modalRejectWorker" width="500px">
      <edit-textarea title="Reject Worker" :subtitle="'Please indicate the reason.'" :min-length="10"
        class="sm-edit-textarea" @updateContent="(data) => rejectWorker(data)" />
    </b-modal>

    <b-modal v-model="modalStartWorking" width="415px">
      <datepicker-modal v-if="currentWorker" v-model:startWorking="currentWorker.startWorking"
        @onSelectCalendar="(date) => onUpdateRequestWorkerStartDate(date)" />
    </b-modal>
  </div>
</template>

<script lang="ts">
import { downloadFile } from "@/utils/downloadFile";
import { phoneMask as mask } from '@/constants/phoneMask';
import {
  getAgencyRequestsWorkers,
  rejectAgencyRequestWorker,
  updateAgencyRequestWorkerStartDate,
} from "@/api/agencyRequestApi";
import { WorkerRequestStatusLabels } from "@/constants/enums";
import { getWorkersReportDocument } from "@/api/agencyReportApi";
import { dateMonth, emailName } from '@/utils/filters';
import {
  getAgencyRequestWorkerNotes,
  createAgencyRequestWorkerNote,
  updateAgencyRequestWorkerNote,
  deleteAgencyRequestWorkerNote,
} from "@/api/agencyNoteApi";
import type { RequestNotesFetchPayload, RequestNotesCreatePayload, RequestNotesUpdatePayload, RequestNotesDeletePayload } from '@/types/agency';

export default {
  props: ["request", "id", "showTitle"],
  data() {
    return {
      mask,
      isLoading: true,
      totalItems: 0,
      rows: [],
      statuses: [
        { id: 2, value: 'Rejected' },
        { id: 3, value: 'Booked' },
      ],
      statusesSelected: [],
      startWorkingDatesSelected: [],
      createdAtDatesSelected: [],
      rejectedAtDatesSelected: [],
      modalManageWorkers: false,
      currentWorker: null,
      modalRejectWorker: false,
      modalStartWorking: false,
      getNotes: ({ requestId, userId, pagination }: RequestNotesFetchPayload) => getAgencyRequestWorkerNotes(requestId, userId, pagination),
      createNote: ({ requestId, userId, model }: RequestNotesCreatePayload) => createAgencyRequestWorkerNote(requestId, userId, model),
      updateNote: ({ requestId, userId, id, model }: RequestNotesUpdatePayload) => updateAgencyRequestWorkerNote(requestId, userId, id, model),
      deleteNote: ({ requestId, userId, id }: RequestNotesDeletePayload) => deleteAgencyRequestWorkerNote(requestId, userId, id),
      serverParams: {
        sortBy: 2,
        requestId: this.id || this.$route.params.id,
        pageIndex: 1,
        pageSize: 30,
        isDescending: true,
      }
    };
  },
  created() {
    this.loadRequestWorkers();
  },
  components: {
    WorkersList: () => import("./AgencyWorkersList.vue"),
    ModalNotes: () => import("../../components/notes/ModalNotes.vue"),
    EditTextarea: () => import("../../components/agency_request/EditTextarea.vue"),
    DatepickerModal: () => import("@/components/agency_request/DatepickerModal.vue"),
  },
  methods: {
    downloadFile,
    dateMonth,
    emailName,
    onCellClick(row, column, rowIndex) {
      switch (column._props.field) {
        case 'startWorking':
          this.onShowModalStartWorking(row);
          break;
        case 'notesCount':
          this.showNotes(rowIndex);
          break;
        case 'actions':
          break;
        default:
          this.$router.push(`/agency-workers/worker/${row.workerProfileId}`);
          break;
      }
    },
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.loadRequestWorkers();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'numberId':
          this.serverParams.sortBy = 0;
          break;
        case 'name':
          this.serverParams.sortBy = 1;
          break;
        case 'status':
          this.serverParams.sortBy = 2;
          break;
        case 'startWorking':
          this.serverParams.sortBy = 3;
          break;
        case 'createdBy':
          this.serverParams.sortBy = 4;
          break;
        case 'rejectedBy':
          this.serverParams.sortBy = 5;
          break;
        case 'externalId':
          this.serverParams.sortBy = 6;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.loadRequestWorkers();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.loadRequestWorkers();
      }
    },
    onStatusSelected() {
      this.serverParams.statuses = this.statusesSelected.map(ss => ss.id);
      this.loadRequestWorkers();
    },
    onStartWorkingSelected() {
      this.serverParams.startWorkingFrom = this.startWorkingDatesSelected[0];
      this.serverParams.startWorkingTo = this.startWorkingDatesSelected[1];
      this.loadRequestWorkers();
    },
    onStartWorkingCleared() {
      this.startWorkingDatesSelected = [];
      this.onStartWorkingSelected();
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.loadRequestWorkers();
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    onRejectedAtSelected() {
      this.serverParams.rejectedAtFrom = this.rejectedAtDatesSelected[0];
      this.serverParams.rejectedAtTo = this.rejectedAtDatesSelected[1];
      this.loadRequestWorkers();
    },
    onRejectedAtCleared() {
      this.rejectedAtDatesSelected = [];
      this.onRejectedAtSelected();
    },
    loadRequestWorkers() {
      this.isLoading = true;
      getAgencyRequestsWorkers(this.serverParams)
        .then((response) => {
          this.rows = response.items.map(i => ({
            ...i,
            status: WorkerRequestStatusLabels[i.workerRequestStatus],
            actions: null,
            showNotes: false,
          }));
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    confirmDelete(worker) {
      this.currentWorker = worker;
      this.modalRejectWorker = true;
    },
    rejectWorker(comments) {
      this.modalRejectWorker = false;
      this.isLoading = true;
      rejectAgencyRequestWorker(this.serverParams.requestId, this.currentWorker.workerId, { comments: comments }).then(() => {
        this.isLoading = false;
        this.loadRequestWorkers();
        this.$emit('refreshRequest');
      }).catch((error) => {
        this.isLoading = false;
        this.showAlertError(error.data);
      });
    },
    showNotes(index) {
      this.rows[index].showNotes = true;
    },
    hideNotes(row) {
      const index = this.rows.findIndex(r => r.id === row.id);
      this.rows[index].showNotes = false;
    },
    onShowModalStartWorking(worker) {
      this.currentWorker = worker;
      this.modalStartWorking = true;
    },
    onUpdateRequestWorkerStartDate(date) {
      this.modalStartWorking = false;
      this.isLoading = true;
      updateAgencyRequestWorkerStartDate(this.serverParams.requestId, this.currentWorker.id, { startWorking: date }).then(() => {
        this.isLoading = false;
        this.loadRequestWorkers();
      }).catch((error) => {
        this.isLoading = false;
        this.showAlertError(error.data);
      });
    },
    downloadWorkersReportDocument() {
      this.isLoading = true;
      getWorkersReportDocument(this.serverParams.requestId)
        .then((response) => {
          this.isLoading = false;
          this.downloadFile(response, `WorkersReport_${this.serverParams.requestId}`);
        })
        .catch((err) => {
          this.isLoading = false;
          this.showAlertError(err);
        });
    },
    onWorkerBooked() {
      this.modalManageWorkers = false;
      this.loadRequestWorkers();
      // Emit event to refresh request status (Open/Filled state may have changed)
      this.$emit('refreshRequest');
    }
  },
};
</script>