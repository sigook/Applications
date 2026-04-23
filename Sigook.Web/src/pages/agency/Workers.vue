<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Workers
        <span class="fw-100 fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <export :url="'/api/AgencyWorkerProfile/File'" :params="serverParams" :fileName="'Workers'"
        @onDataLoading="(value) => isLoading = value">
        <template v-slot:actions>
          <b-button tag="router-link" to="/agency-workers/register-worker" icon-left="plus">
            {{ 'Create' }}
          </b-button>
        </template>
      </export>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="fullName"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="profileImage" width="50" v-slot="props">
            <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image" class="img-30" />
            <default-image v-else :name="props.row.fullName" class="img-30"></default-image>
          </b-table-column>
          <b-table-column field="numberId" label="ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span :class="props.row.isSubcontractor ? 'Blue' : ''">{{ props.row.numberId }}</span>
            </template>
          </b-table-column>
          <b-table-column field="externalId" label="External ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.externalId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">{{ props.row.externalId }}</template>
          </b-table-column>
          <b-table-column field="fullName" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.fullName" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span class="d-block">
                <router-link :to="{ path: '/agency-workers/worker/' + props.row.id }">
                  {{ props.row.fullName }}
                </router-link>
                <b-icon v-if="props.row.approvedToWork" icon="check-all" size="is-small"></b-icon>
                <b-icon v-if="props.row.dnu" icon="alert" size="is-small" type="is-danger"></b-icon>
              </span>
              <p>
                <i class="fz-2 lowercase block">
                  <a :href="'mailto:' + props.row.email">{{ props.row.email }}</a>
                </i>
              </p>
            </template>
          </b-table-column>
          <b-table-column field="mobileNumber" label="Phone" searchable>
            <template v-slot:searchable>
              <b-input :model-value="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered" @update:modelValue="(v) => serverParams.phone = formatPhone(v)"></b-input>
            </template>
            <template v-slot="props">{{ props.row.mobileNumber }}</template>
          </b-table-column>
          <b-table-column field="location" label="Location" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.location" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">{{ props.row.address }}</template>
          </b-table-column>
          <b-table-column field="requests" label="Request ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.requestId" placeholder="Search..." icon="magnify" size="is-small"
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
              <span v-else class="op3 is-inline-block v-middle pr-0">Request ID</span>
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
            <template v-slot="props">{{ dateMonth(props.row.createdAt) }}</template>
          </b-table-column>
          <b-table-column field="skills" label="Skills" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.skills" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <div v-if="props.row.skills.length > 0" class="skills-inline">
                <b-tag v-for="(skill, index) in props.row.skills.slice(0, 3)" :key="`${skill}_${index}`" rounded>
                  {{ skill }}
                </b-tag>
                <b-tooltip v-if="props.row.skills.length > 3" :label="props.row.skills.join(', ')" type="is-dark" multilined append-to-body>
                  <b-tag rounded type="is-info is-light" class="see-more">See more...</b-tag>
                </b-tooltip>
              </div>
            </template>
          </b-table-column>
          <b-table-column field="isCurrentlyWorking" label="Details" searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="featuresSelected" autocomplete :data="features" open-on-focus
                field="value" icon="label" placeholder="Select Feature" @update:modelValue="onFeatureChange" append-to-body>
              </b-taginput>
            </template>
            <template v-slot="props">
              <b-taglist>
                <b-tag v-if="props.row.isCurrentlyWorking" type="is-primary" rounded>Working</b-tag>
                <b-tag v-if="props.row.dnu" type="is-danger" rounded>DNU</b-tag>
                <b-tag v-if="props.row.approvedToWork" type="is-success" rounded>Approved To Work</b-tag>
                <b-tag v-if="props.row.isSubcontractor" type="is-info is-light" rounded>Subcontractor</b-tag>
              </b-taglist>
            </template>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
              <template #trigger>
                <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
              </template>
              <b-dropdown-item aria-role="listitem" @click="router.push(`/agency-workers/worker/${props.row.id}`)">
                Edit
              </b-dropdown-item>
              <b-dropdown-item aria-role="listitem" v-if="!props.row.approvedToWork" @click="deleteWorker(props.row)">
                Approve to work
              </b-dropdown-item>
              <b-dropdown-item aria-role="listitem" v-if="props.row.approvedToWork" @click="confirmDelete(props.row)">
                Reject to work
              </b-dropdown-item>
            </b-dropdown>
          </b-table-column>
        </template>
      </b-table>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAgencyStore } from '@/stores/agency';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/utils/toast';
import { workerFeatures as features } from '@/constants/workerFeatures';
import { formatPhone } from '@/utils/phoneFormat';
import { getAgencyWorkers, updateApprovedToWork } from '@/api/agencyWorkerApi';
import { dateMonth } from '@/utils/filters';
import Export from '@/components/Export.vue';

const router = useRouter();
const agencyStore = useAgencyStore();

const isLoading = ref(true);
const totalItems = ref(0);
const createdAtDatesSelected = ref<any[]>([]);
const featuresSelected = ref<any[]>([]);
const rows = ref<any[]>([]);
const serverParams = ref<any>({
  sortBy: 0,
  isDescending: false,
  pageIndex: 1,
  pageSize: 30,
});

if (agencyStore.agencyWorkerProfileFilter) {
  serverParams.value = agencyStore.agencyWorkerProfileFilter;
  if (serverParams.value.features) {
    featuresSelected.value = features.filter((s: any) => serverParams.value.features.some((sps: any) => sps == s.id));
  }
  if (serverParams.value.createdAtFrom && serverParams.value.createdAtTo) {
    createdAtDatesSelected.value[0] = serverParams.value.createdAtFrom;
    createdAtDatesSelected.value[1] = serverParams.value.createdAtTo;
  }
}
loadWorkers();

function onPageChange(params: number) {
  serverParams.value.pageIndex = params;
  loadWorkers();
}

function onSortChange(field: string, order: string) {
  switch (field) {
    case 'fullName':
      serverParams.value.sortBy = 0;
      break;
    case 'numberId':
      serverParams.value.sortBy = 1;
      break;
    case 'requests':
      serverParams.value.sortBy = 2;
      break;
    case 'createdAt':
      serverParams.value.sortBy = 3;
      break;
    case 'skills':
      serverParams.value.sortBy = 4;
      break;
    case 'externalId':
      serverParams.value.sortBy = 5;
      break;
  }
  serverParams.value.isDescending = order !== 'asc';
  loadWorkers();
}

function onCellClick(row: any, column: any) {
  switch (column.field) {
    case 'actions':
      break;
    case 'requests':
      break;
    default:
      router.push(`/agency-workers/worker/${row.id}`);
  }
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    loadWorkers();
  }
}

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0];
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1];
  loadWorkers();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onFeatureChange() {
  serverParams.value.features = featuresSelected.value.map((fs: any) => fs.id);
  loadWorkers();
}

function goToApplicants(item: any) {
  router.push({
    path: `/agency-request/${item.id}`,
    query: { tab: 'Applicants' },
  });
}

function loadWorkers() {
  isLoading.value = true;
  agencyStore.updateAgencyWorkerProfileFilter(serverParams.value);
  getAgencyWorkers(serverParams.value)
    .then((workers: any) => {
      rows.value = workers.items.map((w: any) => ({ ...w, actions: null }));
      totalItems.value = workers.totalItems;
      isLoading.value = false;
    })
    .catch(() => {
      isLoading.value = false;
    });
}

function deleteWorker(worker: any) {
  isLoading.value = true;
  updateApprovedToWork(worker.id)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Updated');
      loadWorkers();
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
      loadWorkers();
    });
}

function confirmDelete(worker: any) {
  showAlertConfirm(
    'Are you sure?',
    'You want to disable the worker' + '. ' + 'This worker will not be able to apply to new requests',
  ).then((response) => {
    if (response) {
      deleteWorker(worker);
    }
  }).catch((error) => {
    showAlertError(error);
  });
}
</script>
<style lang="scss" scoped>
.skills-inline {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  align-items: center;
}

.see-more {
  cursor: pointer;
}
</style>
