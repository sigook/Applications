<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-2">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        {{ 'Clients' }}
        <span class="fw-light fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <export :url="exportUrl" :params="serverParams" :fileName="'Companies'"
        @onDataLoading="(value) => isLoading = value">
        <template v-slot:actions>
          <b-button tag="router-link" :to="companyDetailBase + '/create'" icon-left="plus">
            {{ 'Create' }}
          </b-button>
        </template>
        <template v-slot:dropdown-actions>
          <b-dropdown-item aria-role="listitem" @click="addFile = true">
            <b-icon icon="file-plus"></b-icon>
            <span>Bulk Data</span>
          </b-dropdown-item>
          <b-dropdown-item v-if="isAccountingManager" aria-role="listitem" @click="exportWithDetails">
            <b-icon icon="file-excel"></b-icon>
            <span>Export with details</span>
          </b-dropdown-item>
        </template>
      </export>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="updatedAt"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="fullName" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.businessInfo" placeholder="Search..." :icon="isMobile ? '' : 'magnify'"
                size="is-small" @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <router-link :to="{ path: companyDetailBase + '/' + props.row.id }">
                {{ props.row.fullName }}
                <template v-for="(location, index) in props.row.locations">
                  <p v-if="index < 2" :key="location">
                    <i class="fz-2 block">{{ location }}</i>
                  </p>
                </template>
                <p>
                  <i v-if="props.row.locations.length > 2" class="fz-2 block">See details...</i>
                </p>
              </router-link>
            </template>
          </b-table-column>
          <b-table-column field="email" label="Contact Info" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.contactInfo" placeholder="Search..." :icon="isMobile ? '' : 'magnify'"
                size="is-small" @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              {{ props.row.contactName || 'No contact name' }}
              <p><i class="fz-2 block">{{ props.row.email }}</i></p>
              <p><i class="fz-2 block">{{ props.row.phone || 'No phone' }}</i></p>
              <p><i class="fz-2 block">{{ props.row.contactRole || 'No role' }}</i></p>
              <p>
                <i v-if="!props.row.website" class="fz-2 block">No website</i>
                <a v-else :href="props.row.website" target="_blank" class="fz-2 block">{{ props.row.website }}</a>
              </p>
            </template>
          </b-table-column>
          <b-table-column field="industry" label="Industry" :visible="!isMobile" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.industry" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <b-tag size="is-medium" rounded>
                {{ props.row.industry }}
              </b-tag>
            </template>
          </b-table-column>
          <b-table-column field="salesRepresentative" label="Sales Rep" sortable searchable>
            <template v-slot:searchable>
              <b-field>
                <b-input size="is-small" icon="magnify" placeholder="Sales Rep"
                  v-model="serverParams.salesRepresentative" @keypress="onInputEntered"></b-input>
              </b-field>
            </template>
            <template v-slot="props">
              {{ props.row.salesRepresentative }}
            </template>
          </b-table-column>
          <b-table-column field="createdAt" label="Creation Info" sortable :searchable="!isMobile">
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
              {{ dateMonth(props.row.createdAt) }}
              <p><i class="fz-2 block">{{ props.row.createdBy || 'Sigook' }}</i></p>
            </template>
          </b-table-column>
          <b-table-column field="updatedAt" label="Update Info" :visible="!isMobile" sortable searchable>
            <template v-slot:searchable>
              <b-field>
                <b-input placeholder="Updated By" size="is-small" icon="magnify" v-model="serverParams.updatedBy"
                  @keypress="onInputEntered"></b-input>
                <b-datepicker placeholder="Updated At" size="is-small" :mobile-native="false"
                  :icon-right="updatedAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                  v-model="updatedAtDatesSelected" icon-right-clickable @icon-right-click="onUpdatedAtCleared"
                  @update:modelValue="onUpdatedAtSelected" append-to-body>
                </b-datepicker>
              </b-field>
            </template>
            <template v-slot="props">
              <div v-if="props.row.companyStatus === 5 && !props.row.updatedAt">
                Existing client
              </div>
              <div v-else-if="props.row.updatedAt">
                {{ dateMonth(props.row.updatedAt) }}
                <p><i class="fz-2 block">{{ props.row.updatedBy }}</i></p>
              </div>
              <div v-else>
                No updates
              </div>
            </template>
          </b-table-column>
          <b-table-column field="notesCount" label="Notes" :visible="!isMobile" v-slot="props">
            <NotesPopover :can-create="false" :user-id="props.row.id" :notes-count="props.row.notesCount"
              :on-get="getCompanyNotes" :on-create="createCompanyNote" :on-delete="deleteCompanyNote"
              @update:count="(size) => props.row.notesCount = size">
            </NotesPopover>
          </b-table-column>
          <b-table-column field="companyStatus" label="Status" :searchable="!isMobile">
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
                field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusSelected" append-to-body>
              </b-taginput>
            </template>
            <template v-slot="props">
              <b-tag size="is-medium" rounded>
                {{statuses.find(s => s.id === props.row.companyStatus).value}}
              </b-tag>
            </template>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <b-modal v-model="addFile" @close="addFile = false" width="500px">
      <bulk-data :upload-fn="bulkAgencyCompanies" :error-file-name="'BulkCompaniesError'"
        :title="'Bulk Companies'" :file-label="'Companies File'" @close="addFile = false" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAgencyStore } from '@/stores/agency';
import { useAppStore } from '@/stores/app';
import { downloadFile } from '@/utils/downloadFile';
import { useAccountingAdmin } from '@/composables/useAccountingAdmin';
import { getAgencyCompanies, bulkAgencyCompanies } from '@/api/agencyCompanyApi';
import { getSalesCompanies } from '@/api/salesApi';
import { downloadAgencyReport } from '@/api/agencyReportApi';
import { getAgencyCompanyNotes, createAgencyCompanyNote, deleteAgencyCompanyNote } from '@/api/agencyNoteApi';
import type { NotesFetchPayload, NotesCreatePayload, NotesDeletePayload } from '@/types/agency';
import { dateMonth } from '@/utils/filters';
import { useModuleBase } from '@/composables/useModuleBase';
import Export from '@/components/Export.vue';
import NotesPopover from '@/components/notes/NotesPopover.vue';
import BulkData from '@/components/agency/BulkData.vue';

const route = useRoute();
const router = useRouter();
const { isSalesView, companyBase: companyDetailBase } = useModuleBase();
const exportUrl = computed(() =>
  isSalesView.value ? '/api/agency/sales/companyprofiles/File' : '/api/agency/recruiting/companyprofiles/File');
const agencyStore = useAgencyStore();
const appStore = useAppStore();
const { isAccountingManager } = useAccountingAdmin();

const isLoading = ref(true);
const totalItems = ref(0);
const statuses = ref<any[]>([]);
const statusesSelected = ref<any[]>([]);
const createdAtDatesSelected = ref<any[]>([]);
const updatedAtDatesSelected = ref<any[]>([]);
const rows = ref<any[]>([]);
const addFile = ref(false);
const serverParams = ref<any>({
  sortBy: 3,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
});

const getCompanyNotes = ({ userId, pagination }: NotesFetchPayload) => getAgencyCompanyNotes(userId, pagination);
const createCompanyNote = ({ userId, model }: NotesCreatePayload) => createAgencyCompanyNote(userId, model);
const deleteCompanyNote = ({ userId, id }: NotesDeletePayload) => deleteAgencyCompanyNote(userId, id);

const isMobile = computed(() => appStore.isMobile);

statuses.value = route.meta.companyStatuses as unknown[];
if (agencyStore.agencyCompanyProfileFilter) {
  serverParams.value = agencyStore.agencyCompanyProfileFilter;
  if (serverParams.value.companyStatuses) {
    statusesSelected.value = statuses.value.filter((s: any) => serverParams.value.companyStatuses.some((sps: any) => sps == s.id));
  }
  if (serverParams.value.createdAtFrom && serverParams.value.createdAtTo) {
    createdAtDatesSelected.value[0] = serverParams.value.createdAtFrom;
    createdAtDatesSelected.value[1] = serverParams.value.createdAtTo;
  }
}
loadCompanies();

function onPageChange(params: number) {
  serverParams.value.pageIndex = params;
  loadCompanies();
}

function onSortChange(field: string, order: string) {
  switch (field) {
    case 'fullName':
      serverParams.value.sortBy = 0;
      break;
    case 'industry':
      serverParams.value.sortBy = 1;
      break;
    case 'createdAt':
      serverParams.value.sortBy = 2;
      break;
    case 'updatedAt':
      serverParams.value.sortBy = 3;
      break;
    case 'salesRepresentative':
      serverParams.value.sortBy = 4;
      break;
  }
  serverParams.value.isDescending = order !== 'asc';
  loadCompanies();
}

function onStatusSelected() {
  serverParams.value.companyStatuses = statusesSelected.value.map((ss) => ss.id);
  loadCompanies();
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    loadCompanies();
  }
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0];
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1];
  loadCompanies();
}

function onUpdatedAtCleared() {
  updatedAtDatesSelected.value = [];
  onUpdatedAtSelected();
}

function onUpdatedAtSelected() {
  serverParams.value.updatedAtFrom = updatedAtDatesSelected.value[0];
  serverParams.value.updatedAtTo = updatedAtDatesSelected.value[1];
  loadCompanies();
}

function onCellClick(row: any, column: any) {
  switch (column.field) {
    case 'notesCount':
    case 'email':
      break;
    default:
      router.push({ path: `${companyDetailBase.value}/${row.id}` });
  }
}


function exportWithDetails() {
  isLoading.value = true;
  downloadAgencyReport('/api/agency/recruiting/companyprofiles/FileWithDetails', serverParams.value)
    .then((file) => {
      isLoading.value = false;
      downloadFile(file, `Companies_Details_${new Date().toLocaleDateString()}`);
    })
    .catch(() => (isLoading.value = false));
}

function loadCompanies() {
  isLoading.value = true;
  agencyStore.updateAgencyCompanyProfileFilter(serverParams.value);
  const fetchCompanies = isSalesView.value ? getSalesCompanies : getAgencyCompanies;
  fetchCompanies(serverParams.value)
    .then((companies: any) => {
      rows.value = companies.items.map((c: any) => ({ ...c }));
      totalItems.value = companies.totalItems;
      isLoading.value = false;
    })
    .catch(() => {
      isLoading.value = false;
    });
}
</script>
<style lang="scss">
tr {
  cursor: pointer;
}
</style>
