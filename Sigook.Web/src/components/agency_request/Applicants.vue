<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-field grouped position="is-right">
        <b-button @click="modalManageWorkers = true">Manage Applicants</b-button>
      </b-field>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
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
              <b-input :model-value="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered" @update:modelValue="(v) => serverParams.phone = formatPhone(v)"></b-input>
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
      <ManageTabs @updateApplicants="(args) => addApplicant(args.model)" />
    </b-modal>

    <b-modal v-model="modalComment" width="500px">
      <EditTextarea v-if="currentItem" :title="'Comments'" subtitle="Comments" :min-length="0" :data="currentItem.comments"
        @updateContent="(data) => saveApplicantComment(data)"></EditTextarea>
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { showAlertError } from "@/utils/toast";
import { emailName, dateMonth } from '@/utils/filters';
import { formatPhone } from '@/utils/phoneFormat';
import {
  getAgencyRequestApplicant,
  postAgencyRequestApplicant,
  deleteAgencyRequestApplicant,
  updateAgencyRequestApplicant
} from "@/api/agencyRequestApi";
import { convertCandidateToWorker } from "@/api/agencyCandidateApi";
import ManageTabs from './ManageApplicantsModal.vue';
import EditTextarea from '@/components/agency_request/EditTextarea.vue';

defineProps<{ request?: any }>();

const route = useRoute();
const router = useRouter();

const isLoading = ref(false);
const currentItem = ref<any>(null);
const createdAtDatesSelected = ref<any[]>([]);
const modalManageWorkers = ref(false);
const modalComment = ref(false);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const serverParams = reactive<any>({
  sortBy: 1,
  requestId: route.params.id,
  pageIndex: 1,
  pageSize: 30,
  isDescending: true
});

function onPageChange(params: any) {
  serverParams.pageIndex = params;
  loadApplicants();
}

function onSortChange(field: string, order: string) {
  switch (field) {
    case 'name':
      serverParams.sortBy = 0;
      break;
    case 'createdBy':
      serverParams.sortBy = 1;
      break;
  }
  serverParams.isDescending = order !== 'asc';
  loadApplicants();
}

function onCellClick(row: any, column: any) {
  switch (column.field) {
    case 'comments':
    case 'actions':
      break;
    default:
      if (row.workerProfileId) {
        router.push(`/agency-workers/worker/${row.workerProfileId}`);
      }
  }
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    loadApplicants();
  }
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onCreatedAtSelected() {
  serverParams.createdAtFrom = createdAtDatesSelected.value[0];
  serverParams.createdAtTo = createdAtDatesSelected.value[1];
  loadApplicants();
}

function loadApplicants() {
  isLoading.value = true;
  getAgencyRequestApplicant(serverParams)
    .then((response) => {
      rows.value = response.items.map((c: any) => ({ ...c, actions: null }));
      totalItems.value = response.totalItems;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function addApplicant(model: any) {
  modalManageWorkers.value = false;
  isLoading.value = true;
  postAgencyRequestApplicant(serverParams.requestId, model).then(() => {
    isLoading.value = false;
    loadApplicants();
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error);
  });
}

function removeApplicant(item: any) {
  isLoading.value = true;
  deleteAgencyRequestApplicant(serverParams.requestId, item.id).then(() => {
    isLoading.value = false;
    loadApplicants();
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error);
  });
}

function showEditModal(item: any) {
  currentItem.value = item;
  modalComment.value = true;
}

function saveApplicantComment(comment: string) {
  modalComment.value = false;
  isLoading.value = true;
  updateAgencyRequestApplicant(serverParams.requestId, currentItem.value.id, { comments: comment })
    .then(() => {
      isLoading.value = false;
      loadApplicants();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function convertToWorker(candidateId: any) {
  isLoading.value = true;
  convertCandidateToWorker(candidateId)
    .then(() => {
      isLoading.value = false;
      loadApplicants();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

loadApplicants();
</script>
