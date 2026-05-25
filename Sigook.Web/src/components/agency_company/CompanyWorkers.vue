<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="fullName"
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
          <b-table-column field="fullName" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.fullName" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span class="d-block">
                {{ props.row.fullName }}
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
          <b-table-column field="requestsNumberId" label="Request ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.requestId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <div v-if="props.row.requests && props.row.requests.length > 0">
                <b-taglist>
                  <b-tag v-for="request in props.row.requests" :key="request.id" rounded>
                    {{ request.value }}
                  </b-tag>
                </b-taglist>
              </div>
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
              <div v-if="props.row.skills.length > 0">
                <span v-for="(skill, index) in props.row.skills" :key="`${skill}_${index}`"
                  class="tag-sm-gray mb-1 mr-1 ellipsis-full">
                  {{ skill }}
                </span>
              </div>
              <span v-else class="op3 is-inline-block v-middle pr-0">Skill</span>
            </template>
          </b-table-column>
          <b-table-column field="isCurrentlyWorking" label="Details" searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="featuresSelected" autocomplete :data="features" open-on-focus
                field="value" icon="label" placeholder="Select Details" @update:modelValue="onFeatureChange" append-to-body>
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
        </template>
      </b-table>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { showAlertError } from "@/utils/toast";
import { workerFeatures as features } from '@/constants/workerFeatures';
import { formatPhone } from '@/utils/phoneFormat';
import { dateMonth } from "@/utils/filters";
import { getAgencyWorkers } from "@/api/agencyWorkerApi";

const props = defineProps<{ company: any }>();
const router = useRouter();

const isLoading = ref(false);
const totalItems = ref(0);
const createdAtDatesSelected = ref<any[]>([]);
const featuresSelected = ref<any[]>([]);
const rows = ref<any[]>([]);
const serverParams = reactive<any>({
  sortBy: 0,
  isDescending: false,
  pageIndex: 1,
  pageSize: 30,
  companyProfileId: props.company.id,
});

function onPageChange(params: any) {
  serverParams.pageIndex = params;
  getAgencyCompanyWorkers();
}

function onSortChange(field: string, order: string) {
  switch (field) {
    case 'fullName':
      serverParams.sortBy = 0;
      break;
    case 'numberId':
      serverParams.sortBy = 1;
      break;
    case 'requestsNumberId':
      serverParams.sortBy = 2;
      break;
    case 'createdAt':
      serverParams.sortBy = 3;
      break;
    case 'skills':
      serverParams.sortBy = 4;
      break;
  }
  serverParams.isDescending = order !== 'asc';
  getAgencyCompanyWorkers();
}

function onCellClick(row: any) {
  router.push(`/agency-workers/worker/${row.id}`);
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    getAgencyCompanyWorkers();
  }
}

function onCreatedAtSelected() {
  serverParams.createdAtFrom = createdAtDatesSelected.value[0];
  serverParams.createdAtTo = createdAtDatesSelected.value[1];
  getAgencyCompanyWorkers();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onFeatureChange() {
  serverParams.features = featuresSelected.value.map(fs => fs.id);
  getAgencyCompanyWorkers();
}

function getAgencyCompanyWorkers() {
  isLoading.value = true;
  getAgencyWorkers(serverParams)
    .then(response => {
      rows.value = response.items;
      totalItems.value = response.totalItems;
      isLoading.value = false;
    }).catch((error) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

getAgencyCompanyWorkers();
</script>
