<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Clients" :count="totalItems" :crumbs="moduleCrumbs" />
    <div>
      <SigookGrid ref="grid" :fetch="loadCompanies" v-model:params="serverParams" :sort-map="sortMap"
        :export="{ url: exportUrl, fileName: 'Companies' }" focusable
        @update:loading="(value) => isLoading = value" @loaded="(total) => totalItems = total" @cellclick="onCellClick">
        <template #actions>
          <b-button tag="router-link" :to="companyDetailBase + '/create'" icon-left="plus">Create</b-button>
        </template>
        <template #dropdown-actions>
          <b-dropdown-item aria-role="listitem" @click="addFile = true">
            <b-icon icon="file-plus"></b-icon>
            <span>Bulk Data</span>
          </b-dropdown-item>
          <b-dropdown-item v-if="isAdmin" aria-role="listitem" @click="exportWithDetails">
            <b-icon icon="file-excel"></b-icon>
            <span>Export with details</span>
          </b-dropdown-item>
        </template>
        <b-table-column field="fullName" label="Name" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.businessInfo" placeholder="Search..." :icon="isMobile ? '' : 'magnify'"
              size="is-small"></b-input>
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
              size="is-small"></b-input>
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
            <b-input v-model="serverParams.industry" placeholder="Search..." icon="magnify" size="is-small"></b-input>
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
                v-model="serverParams.salesRepresentative"></b-input>
            </b-field>
          </template>
          <template v-slot="props">
            {{ props.row.salesRepresentative }}
          </template>
        </b-table-column>
        <b-table-column field="createdAt" label="Creation Info" sortable :searchable="!isMobile">
          <template v-slot:searchable>
            <b-field>
              <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.createdBy"></b-input>
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
              <b-input placeholder="Updated By" size="is-small" icon="magnify" v-model="serverParams.updatedBy"></b-input>
              <b-datepicker placeholder="Updated At" size="is-small" :mobile-native="false"
                :icon-right="updatedAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                v-model="updatedAtDatesSelected" icon-right-clickable @icon-right-click="onUpdatedAtCleared"
                @update:modelValue="onUpdatedAtSelected" append-to-body>
              </b-datepicker>
            </b-field>
          </template>
          <template v-slot="props">
            <div v-if="props.row.companyStatus === CompanyStatus.Client && !props.row.updatedAt">
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
        <b-table-column field="actions" v-slot="props">
          <b-dropdown v-if="props.row.companyStatus === CompanyStatus.Client || isSuperAdmin" aria-role="list"
            position="is-bottom-left" append-to-body>
            <template #trigger>
              <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
            </template>
            <b-dropdown-item v-if="props.row.companyStatus === CompanyStatus.Client" aria-role="listitem"
              @click="router.push({ path: requestBase + '/create/' + props.row.id })">
              Create Request
            </b-dropdown-item>
            <b-dropdown-item v-if="isSuperAdmin" aria-role="listitem" class="has-text-danger"
              @click="onDeleteCompany(props.row)">
              Delete
            </b-dropdown-item>
          </b-dropdown>
        </b-table-column>
      </SigookGrid>
    </div>

    <b-modal custom-content-class="card" v-model="addFile" @close="addFile = false" width="500px">
      <bulk-data :upload-fn="bulkAgencyCompanies" :error-file-name="'BulkCompaniesError'"
        :title="'Bulk Companies'" :file-label="'Companies File'" @close="addFile = false" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, useTemplateRef } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAgencyStore } from '@/modules/agency/store';
import { useAppStore } from '@/app/stores/app';
import { downloadFile } from '@/shared/utils/downloadFile';
import { useAdmin } from '@/shared/composables/useAdmin';
import { useSuperAdmin } from '@/shared/composables/useSuperAdmin';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { getAgencyCompanies, bulkAgencyCompanies, getCompanyDeletionCheck, deleteAgencyCompany } from '@/modules/agency/recruiting/clients/api';
import { getSalesCompanies } from '@/modules/agency/sales/api';
import { downloadAgencyReport } from '@/shared/api/reportApi';
import { getAgencyCompanyNotes, createAgencyCompanyNote, deleteAgencyCompanyNote } from '@/modules/agency/shared/notes/api';
import type { NotesFetchPayload, NotesCreatePayload, NotesDeletePayload } from '@/modules/agency/shared/notes/types';
import { dateMonth } from '@/shared/format';
import { CompanyStatus } from '@/shared/constants/enums';
import type { AgencyCompanyFilter, AgencyCompanyListItem } from '@/modules/agency/recruiting/clients/types';
import type { CatalogItem, GridHandle, TableColumnRef } from '@/shared/types/common';
import { useModuleBase } from '@/modules/agency/shared/useModuleBase';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import NotesPopover from '@/modules/agency/shared/notes/NotesPopover.vue';
import BulkData from '@/modules/agency/recruiting/candidates/components/BulkData.vue';
import PageHeader from '@/shared/ui/PageHeader.vue';

const route = useRoute();
const router = useRouter();
const { isSalesView, requestBase, companyBase: companyDetailBase, moduleCrumbs } = useModuleBase();
const exportUrl = computed(() =>
  isSalesView.value ? '/api/agency/sales/companyprofiles/File' : '/api/agency/recruiting/clients/File');
const agencyStore = useAgencyStore();
const appStore = useAppStore();
const { isAdmin } = useAdmin();
const { isSuperAdmin } = useSuperAdmin();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  fullName: 0,
  industry: 1,
  createdAt: 2,
  updatedAt: 3,
  salesRepresentative: 4,
};

const isLoading = ref(true);
const totalItems = ref(0);
const statuses = ref<CatalogItem<CompanyStatus>[]>([]);
const statusesSelected = ref<CatalogItem<CompanyStatus>[]>([]);
const createdAtDatesSelected = ref<Date[]>([]);
const updatedAtDatesSelected = ref<Date[]>([]);
const addFile = ref(false);
const serverParams = ref<AgencyCompanyFilter>(agencyStore.agencyCompanyProfileFilter ?? {
  sortBy: 3,
  isDescending: true,
});

const getCompanyNotes = ({ userId, pagination }: NotesFetchPayload) => getAgencyCompanyNotes(userId, pagination);
const createCompanyNote = ({ userId, model }: NotesCreatePayload) => createAgencyCompanyNote(userId, model);
const deleteCompanyNote = ({ userId, id }: NotesDeletePayload) => deleteAgencyCompanyNote(userId, id);

const isMobile = computed(() => appStore.isMobile);

statuses.value = route.meta.companyStatuses as CatalogItem<CompanyStatus>[];
if (serverParams.value.companyStatuses) {
  statusesSelected.value = statuses.value.filter((s) => serverParams.value.companyStatuses?.includes(s.id));
}
if (serverParams.value.createdAtFrom && serverParams.value.createdAtTo) {
  createdAtDatesSelected.value[0] = new Date(serverParams.value.createdAtFrom);
  createdAtDatesSelected.value[1] = new Date(serverParams.value.createdAtTo);
}

function loadCompanies(params: AgencyCompanyFilter) {
  agencyStore.updateAgencyCompanyProfileFilter(params);
  const fetchCompanies = isSalesView.value ? getSalesCompanies : getAgencyCompanies;
  return fetchCompanies(params);
}

function onStatusSelected() {
  serverParams.value.companyStatuses = statusesSelected.value.map((ss) => ss.id);
  grid.value?.search();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function onUpdatedAtCleared() {
  updatedAtDatesSelected.value = [];
  onUpdatedAtSelected();
}

function onUpdatedAtSelected() {
  serverParams.value.updatedAtFrom = updatedAtDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.updatedAtTo = updatedAtDatesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function onCellClick(row: AgencyCompanyListItem, column: TableColumnRef) {
  switch (column.field) {
    case 'notesCount':
    case 'email':
    case 'actions':
      break;
    default:
      router.push({ path: `${companyDetailBase.value}/${row.id}` });
  }
}

async function onDeleteCompany(row: AgencyCompanyListItem) {
  isLoading.value = true;
  try {
    const check = await getCompanyDeletionCheck(row.id);
    isLoading.value = false;
    if (!check.canDelete) {
      const detail = check.blockers.map((b) => `${b.entity}: ${b.count}`).join(', ');
      showAlertError(`${check.fullName} cannot be deleted because it has related records (${detail}).`);
      return;
    }
    const confirmed = await showAlertConfirm(
      'Delete client',
      `${check.fullName} and its users will be permanently deleted. This action cannot be undone.`,
      'Delete',
    );
    if (!confirmed) return;
    isLoading.value = true;
    await deleteAgencyCompany(row.id);
    isLoading.value = false;
    showAlertSuccess(`${check.fullName} was deleted`);
    grid.value?.reload();
  } catch (error: unknown) {
    isLoading.value = false;
    showAlertError(error);
  }
}

function exportWithDetails() {
  isLoading.value = true;
  downloadAgencyReport('/api/agency/recruiting/clients/FileWithDetails', serverParams.value)
    .then((file) => {
      isLoading.value = false;
      downloadFile(file, `Companies_Details_${new Date().toLocaleDateString()}`);
    })
    .catch(() => (isLoading.value = false));
}
</script>
<style lang="scss">
tr {
  cursor: pointer;
}
</style>
