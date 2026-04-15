<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-field grouped position="is-right">
        <b-button @click="modalManageWorkers = true">Manage Applicants</b-button>
      </b-field>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="createdBy"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="profileImage" width="50" v-slot="props">
            <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image" class="img-30" />
            <default-image v-else :name="props.row.name" class="img-30"></default-image>
          </b-table-column>
          <b-table-column field="name" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span class="d-block">
                {{ props.row.name }}
                <b-tooltip label="Candidate" type="is-dark" append-to-body>
                  <b-icon v-if="props.row.candidateId" icon="account-group" size="is-small"></b-icon>
                </b-tooltip>
                <b-tooltip label="Worker" type="is-dark" append-to-body>
                  <b-icon v-if="props.row.workerProfileId" icon="badge-account-outline" size="is-small"></b-icon>
                </b-tooltip>
              </span>
              <i class="fz-2 ellipsis-150 lowercase">
                <a :href="'mailto:' + props.row.email">{{ props.row.email }}</a>
              </i>
            </template>
          </b-table-column>
          <b-table-column field="phoneNumber" label="Phone" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered" v-cleave="mask"></b-input>
            </template>
            <template v-slot="props">
              <div v-if="props.row.phoneNumber">
                {{ props.row.phoneNumber }}
              </div>
              <div v-else class="op3">Phone</div>
            </template>
          </b-table-column>
          <b-table-column field="createdBy" label="Added By" sortable searchable>
            <template v-slot:searchable>
              <b-field>
                <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.createdBy"
                  @keypress="onInputEntered"></b-input>
                <b-datepicker size="is-small" :mobile-native="false" placeholder="Created At"
                  :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                  v-model="createdAtDatesSelected" icon-right-clickable @icon-right-click="onCreatedAtCleared"
                  @update:modelValue="onCreatedAtSelected" append-to-body></b-datepicker>
              </b-field>
            </template>
            <template v-slot="props">
              <div class="capitalize" v-if="props.row.createdBy">
                <p>{{ emailName(props.row.createdBy) }}</p>
              </div>
              <div v-else class="op3">Added by</div>
              <i class="fz-2">{{ dateMonth(props.row.createdAt) }}</i>
            </template>
          </b-table-column>
          <b-table-column field="comments" label="Comments" v-slot="props">
            <span v-html="props.row.comments"></span>
            <b-button type="is-ghost" icon-right="pencil" @click="showEditModal(props.row, props.row.index)">
            </b-button>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
              <template #trigger>
                <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
              </template>
              <b-dropdown-item aria-role="listitem" v-if="props.row.candidateId"
                @click="convertToWorker(props.row.candidateId)">
                Convert to Worker
              </b-dropdown-item>
              <b-dropdown-item aria-role="listitem" @click="removeApplicant(props.row)">
                Delete
              </b-dropdown-item>
            </b-dropdown>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <b-modal v-model="modalManageWorkers" width="800px">
      <manage-tabs @updateApplicants="(args) => addApplicant(args.model)" />
    </b-modal>

    <b-modal v-model="modalComment" width="500px">
      <edit-textarea v-if="currentItem" :title="'Comments'" subtitle="Comments" :min-length="0" :data="currentItem.comments"
        @updateContent="(data) => saveApplicantComment(data)"></edit-textarea>
    </b-modal>
  </div>
</template>
<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { showAlertError } from "@/utils/toast";
import { emailName, dateMonth } from '@/utils/filters';
import { phoneMask as mask } from '@/constants/phoneMask';
import {
  getAgencyRequestApplicant,
  postAgencyRequestApplicant,
  deleteAgencyRequestApplicant,
  updateAgencyRequestApplicant
} from "@/api/agencyRequestApi";
import { convertCandidateToWorker } from "@/api/agencyCandidateApi";

export default {
  props: ["request"],
  data() {
    return {
      mask,
      isLoading: false,
      currentItem: null,
      createdAtDatesSelected: [],
      modalManageWorkers: false,
      modalComment: false,
      totalItems: 0,
      rows: [],
      serverParams: {
        sortBy: 1,
        requestId: this.$route.params.id,
        pageIndex: 1,
        pageSize: 30,
        isDescending: true
      }
    };
  },
  components: {
    manageTabs: defineAsyncComponent(() => import("./ManageApplicantsModal.vue")),
    EditTextarea: defineAsyncComponent(() => import("../../components/agency_request/EditTextarea.vue"))
  },
  methods: {
    emailName,
    dateMonth,
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.loadApplicants();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'name':
          this.serverParams.sortBy = 0;
          break;
        case 'createdBy':
          this.serverParams.sortBy = 1;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.loadApplicants();
    },
    onCellClick(row, column) {
      switch (column.field) {
        case 'comments':
        case 'actions':
          break;
        default:
          if (row.workerProfileId) {
            this.$router.push(`/agency-workers/worker/${row.workerProfileId}`);
          }
      }
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.loadApplicants();
      }
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.loadApplicants();
    },
    loadApplicants() {
      this.isLoading = true;
      getAgencyRequestApplicant(this.serverParams)
        .then((response) => {
          this.rows = response.items.map(c => ({ ...c, actions: null }));
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    addApplicant(model) {
      this.modalManageWorkers = false;
      this.isLoading = true;
      postAgencyRequestApplicant(this.serverParams.requestId, model).then(() => {
        this.isLoading = false;
        this.loadApplicants();
      }).catch((error) => {
        this.isLoading = false;
        showAlertError(error);
      });
    },
    removeApplicant(item) {
      this.isLoading = true;
      deleteAgencyRequestApplicant(this.requestId, item.id).then(() => {
        this.isLoading = false;
        this.loadApplicants();
      }).catch((error) => {
        this.isLoading = false;
        showAlertError(error);
      });
    },
    showEditModal(item) {
      this.currentItem = item;
      this.modalComment = true;
    },
    saveApplicantComment(comment) {
      this.modalComment = false;
      this.isLoading = true;
      updateAgencyRequestApplicant(this.requestId, this.currentItem.id, { comments: comment })
        .then(() => {
          this.isLoading = false;
          this.loadApplicants();
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    convertToWorker(candidateId) {
      this.isLoading = true;
      convertCandidateToWorker(candidateId)
        .then(() => {
          this.isLoading = false;
          this.loadApplicants();
        })
        .catch((error) => {
          this.isLoading = false
          showAlertError(error);
        })
    }
  },
  created() {
    this.loadApplicants();
  }
};
</script>