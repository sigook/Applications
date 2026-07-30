<template>
  <div class="wrapper-job-positions">
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-field grouped position="is-right">
        <b-button v-if="isAdmin" type="is-ghost" icon-right="plus-circle"
          @click="showModal = true">Add</b-button>
        <b-button v-else type="is-ghost" icon-right="forum" @click="showModalRole = true">Ask for a new
          role</b-button>
      </b-field>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated detailed show-detail-icon
        pagination-rounded :per-page="pageSize" detail-transition="fade" v-model:current-page="pageIndex" pagination-size="is-small"
        :has-detailed-visible="(row) => row.description">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="jobPosition" label="Role" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.role" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              {{ props.row.jobPosition }}
            </template>
          </b-table-column>
          <b-table-column field="rate" label="Agency Rate" :visible="isAdmin" v-slot="props">
            {{ currency(props.row.rate) }}
          </b-table-column>
          <b-table-column field="workerRate" label="Worker Rate" v-slot="props">
            {{ currency(props.row.workerRate) }}
            <div class="is-inline-block">
              <b-tooltip label="Max" type="is-dark" append-to-body>
                <span v-if="props.row.workerRateMax">- {{ currency(props.row.workerRateMax) }}
                </span>
              </b-tooltip>
            </div>
          </b-table-column>
          <b-table-column field="createdAt" label="Created" v-slot="props">
            {{ emailName(props.row.createdBy) }}
            <i class="fz-2 block">{{ dateMonth(props.row.createdAt) }}</i>
          </b-table-column>
          <b-table-column field="displayShift" label="Shift" v-slot="props">
            <roles-shift v-if="props.row.displayShift" :displayShift="props.row.displayShift" :roleId="props.row.id"
              :companyProfileId="profileId" />
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-button type="is-info" outlined rounded icon-right="pencil" class="me-2"
              @click="openEditModal(props.row)"></b-button>
            <b-button type="is-danger" outlined rounded icon-right="delete"
              @click="onDeleteJobPosition(props.row.id)"></b-button>
          </b-table-column>
        </template>
        <template #detail="props">
          <p v-if="props.row.description">{{ props.row.description }}</p>
        </template>
      </b-table>
    </div>

    <!-- Custom modal -->
    <b-modal v-model="showModal" @close="closeVueModal" width="850px">
      <position-form :current-position="currentPosition" :profile-id="profileId"
        @updateContent="onUpdateModal"></position-form>
    </b-modal>

    <!-- Request Role Modal -->
    <b-modal v-model="showModalRole" @close="closeRequestPositionModal" width="500px">
      <request-position-form :profile-id="profileId" @closeModal="closeRequestPositionModal" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertConfirm, showAlertError, showAlertSuccess } from "@/utils/toast";
import { useAdmin } from '@/composables/useAdmin';
import { currency, emailName, dateMonth } from "@/utils/filters";
import { getAgencyCompanyJobPositions, deleteAgencyCompanyJobPosition } from "@/api/agencyCompanyApi";
import type { AgencyCompanyJobPositionFilter } from "@/types/agency";
import PositionForm from "@/components/agency_company/JobPositionForm.vue";
import RequestPositionForm from "../../components/agency_company/RequestJobPositionForm.vue";
import RolesShift from "../agency_company/RolesShiftDetail.vue";

const route = useRoute();
const { isAdmin } = useAdmin();

const isLoading = ref(true);
const rows = ref<any[]>([]);
const pageIndex = ref(1);
const pageSize = 30;
const profileId = route.params.id;
const showModal = ref(false);
const currentPosition = ref<any>(null);
const showModalRole = ref(false);
const serverParams = reactive<AgencyCompanyJobPositionFilter>({ role: '' });

async function loadJobPositions() {
  isLoading.value = true;
  const data = await getAgencyCompanyJobPositions(profileId as string, serverParams);
  rows.value = data.map((i: any) => ({ ...i, actions: null }));
  isLoading.value = false;
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    loadJobPositions();
  }
}

function openEditModal(item: any) {
  currentPosition.value = item;
  showModal.value = true;
}

async function onUpdateModal() {
  closeVueModal();
  await loadJobPositions();
}

function closeVueModal() {
  showModal.value = false;
  currentPosition.value = null;
}

function onDeleteJobPosition(id: any) {
  showAlertConfirm("Are you sure", "You want to delete this position")
    .then((response) => {
      if (response) {
        isLoading.value = true;
        deleteAgencyCompanyJobPosition(profileId as string, id)
          .then(async () => {
            isLoading.value = false;
            showAlertSuccess("Deleted");
            await loadJobPositions();
          })
          .catch((error) => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    });
}

function closeRequestPositionModal() {
  showModalRole.value = false;
}

(async () => {
  await loadJobPositions();
})();
</script>
