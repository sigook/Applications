<template>
  <div class="wrapper-job-positions">
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <SigookGrid ref="grid" :fetch="loadJobPositions" v-model:params="serverParams" :sort-map="sortMap"
        detailed show-detail-icon :has-detailed-visible="(row) => !!row.description"
        @update:loading="(value) => isLoading = value">
        <template #actions>
          <b-button icon-left="plus" @click="showModal = true">Add</b-button>
        </template>
        <b-table-column field="jobPosition" label="Role" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.role" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.jobPosition }}
          </template>
        </b-table-column>
        <b-table-column field="rate" label="Agency Rate" :visible="isAdmin" sortable v-slot="props">
          <b-tag v-if="!props.row.rate" type="is-warning">Rates pending</b-tag>
          <template v-else>{{ currency(props.row.rate) }}</template>
        </b-table-column>
        <b-table-column field="workerRate" label="Worker Rate" sortable v-slot="props">
          <b-tag v-if="!props.row.workerRate" type="is-warning">Rates pending</b-tag>
          <template v-else>{{ currency(props.row.workerRate) }}</template>
          <div class="is-inline-block">
            <b-tooltip label="Max" type="is-dark" append-to-body>
              <span v-if="props.row.workerRateMax">- {{ currency(props.row.workerRateMax) }}
              </span>
            </b-tooltip>
          </div>
        </b-table-column>
        <b-table-column field="createdAt" label="Created" sortable v-slot="props">
          {{ emailName(props.row.createdBy) }}
          <i class="fz-2 block">{{ dateMonth(props.row.createdAt) }}</i>
        </b-table-column>
        <b-table-column field="displayShift" label="Shift" v-slot="props">
          <roles-shift v-if="props.row.displayShift" :displayShift="props.row.displayShift" :roleId="props.row.id"
            :companyProfileId="profileId" />
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
            <template #trigger>
              <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
            </template>
            <b-dropdown-item aria-role="listitem" @click="openEditModal(props.row)">
              Edit Role
            </b-dropdown-item>
            <b-dropdown-item v-if="isAdmin" aria-role="listitem" @click="openDuplicateModal(props.row)">
              Duplicate Role
            </b-dropdown-item>
            <b-dropdown-item aria-role="listitem" class="has-text-danger" @click="onDeleteJobPosition(props.row.id)">
              Delete Role
            </b-dropdown-item>
          </b-dropdown>
        </b-table-column>
        <template #detail="props">
          <p v-if="props.row.description">{{ props.row.description }}</p>
        </template>
      </SigookGrid>
    </div>

    <!-- Custom modal -->
    <b-modal custom-content-class="card" v-model="showModal" @close="closeVueModal" width="850px">
      <position-form :kind="isAdmin ? 'full' : 'basic'" :create="createPosition" :update="updatePosition"
        :load="loadPosition" :current-position="currentPosition" :is-duplicate="isDuplicate"
        @updateContent="onUpdateModal"></position-form>
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertConfirm, showAlertError, showAlertSuccess } from "@/shared/utils/toast";
import { useAdmin } from '@/shared/composables/useAdmin';
import { currency, emailName, dateMonth } from "@/shared/format";
import { getAgencyCompanyJobPositions, deleteAgencyCompanyJobPosition, createAgencyCompanyJobPosition, createAgencyCompanyJobPositionWithoutRates, updateAgencyCompanyJobPosition, getAgencyCompanyJobPositionById } from '@/modules/agency/recruiting/clients/api';
import { JobPositionSortBy } from '@/modules/agency/recruiting/clients/types';
import type { AgencyCompanyJobPosition, AgencyCompanyJobPositionFilter } from '@/modules/agency/recruiting/clients/types';
import type { GridHandle, PaginatedList } from '@/shared/types/common';
import PositionForm from "@/shared/company-profile/JobPositionForm.vue";
import RolesShift from "@/modules/agency/recruiting/clients/components/RolesShiftDetail.vue";
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const route = useRoute();
const { isAdmin } = useAdmin();

const isLoading = ref(true);
const profileId = route.params.id as string;
const showModal = ref(false);
const currentPosition = ref<AgencyCompanyJobPosition | null>(null);
const isDuplicate = ref(false);
const grid = useTemplateRef<GridHandle>('grid');
const sortMap = {
  jobPosition: JobPositionSortBy.Role,
  rate: JobPositionSortBy.AgencyRate,
  workerRate: JobPositionSortBy.WorkerRate,
  createdAt: JobPositionSortBy.CreatedAt,
};
const serverParams = ref<AgencyCompanyJobPositionFilter>({
  sortBy: JobPositionSortBy.Role,
  isDescending: false,
  role: '',
});

function createPosition(model: AgencyCompanyJobPosition): Promise<{ id: string }> {
  return isAdmin.value
    ? createAgencyCompanyJobPosition(profileId, model)
    : createAgencyCompanyJobPositionWithoutRates(profileId, model);
}

function updatePosition(id: string, model: AgencyCompanyJobPosition): Promise<void> {
  return updateAgencyCompanyJobPosition(profileId, id, model);
}

function loadPosition(id: string): Promise<AgencyCompanyJobPosition> {
  return getAgencyCompanyJobPositionById(profileId, id);
}

function loadJobPositions(params: AgencyCompanyJobPositionFilter): Promise<PaginatedList<AgencyCompanyJobPosition>> {
  return getAgencyCompanyJobPositions(profileId, params);
}

function reload(): void {
  grid.value?.reload();
}

function openEditModal(item: AgencyCompanyJobPosition) {
  currentPosition.value = item;
  isDuplicate.value = false;
  showModal.value = true;
}

function openDuplicateModal(item: AgencyCompanyJobPosition) {
  currentPosition.value = item;
  isDuplicate.value = true;
  showModal.value = true;
}

function onUpdateModal() {
  const duplicated = isDuplicate.value;
  closeVueModal();
  if (duplicated) showAlertSuccess('Role duplicated');
  reload();
}

function closeVueModal() {
  showModal.value = false;
  currentPosition.value = null;
  isDuplicate.value = false;
}

function onDeleteJobPosition(id: string) {
  showAlertConfirm("Are you sure", "You want to delete this position")
    .then((response) => {
      if (response) {
        isLoading.value = true;
        deleteAgencyCompanyJobPosition(profileId, id)
          .then(() => {
            showAlertSuccess("Deleted");
            reload();
          })
          .catch((error) => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    });
}
</script>
