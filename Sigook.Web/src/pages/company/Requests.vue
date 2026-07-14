<template>
  <div class="company-requests">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        {{ "Staff Requests" }}
        <span class="fw-light fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <b-field grouped position="is-right">
        <b-button tag="router-link" to="/company-requests/create" icon-left="plus">Create Request</b-button>
      </b-field>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="numberId"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="numberId" label="Request ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <router-link :to="{ path: '/company-requests/' + props.row.id }">
                <p>{{ props.row.numberId }}</p>
              </router-link>
              <p v-if="props.row.isAsap" class="asap">{{ "Asap" }}</p>
              <p v-if="props.row.isDirectHiring" class="asap">DH</p>
            </template>
          </b-table-column>
          <b-table-column field="jobTitle" label="Position" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.jobTitle" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              {{ props.row.jobTitle }}
              <i class="fz-2 block">{{ dateFromNow(props.row.createdAt) }}</i>
            </template>
          </b-table-column>
          <b-table-column field="location" label="Location" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.location" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              {{ props.row.location }}
              <span v-if="props.row.entrance"> - {{ props.row.entrance }}</span>
            </template>
          </b-table-column>
          <b-table-column field="displayShift" label="Shift" v-slot="props">
            <AgencyShift class="d-block" :requestId="props.row.id"
              :displayShift="props.row.displayShift"></AgencyShift>
          </b-table-column>
          <b-table-column field="workersQuantityWorking" sortable>
            <template v-slot:header>
              <p class="fw-semibold">Workers</p>
              <p class="fw-semibold">({{ totalQuantityWorking }} / {{ totalQuantity }})</p>
            </template>
            <template v-slot="props">
              {{ props.row.workersQuantityWorking }} / {{ props.row.workersQuantity }}
            </template>
          </b-table-column>
          <b-table-column field="requestStatus" label="Status" v-slot="props">
            <div class="text-center">
              <b-tooltip :label="RequestStatusLabels[props.row.requestStatus]" type="is-dark" append-to-body>
                <div class="status-dot-container">
                  <img v-if="props.row.requestStatus === RequestStatus.Filled"
                    src="../../assets/images/check_white.png" alt="check" class="request-check" />
                  <div class="dot-status" :class="getStatusClass(props.row)"></div>
                </div>
              </b-tooltip>
            </div>
          </b-table-column>
        </template>
      </b-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useCompanyStore } from '@/stores/company';
import { showAlertError } from '@/utils/toast';
import { getRequests } from '@/api/companyApi';
import { RequestStatus, RequestStatusLabels } from '@/constants/enums';
import { dateFromNow } from '@/utils/filters';
import AgencyShift from '@/components/agency_request/AgencyShiftDetail.vue';

const router = useRouter();
const companyStore = useCompanyStore();

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const serverParams = reactive<any>({
  sortBy: 0,
  isDescending: true,
  pageIndex: 1,
  pageSize: 30,
});

const totalQuantityWorking = computed(() => {
  if (rows.value.length > 0) {
    return rows.value.map((r: any) => r.workersQuantityWorking).reduce((a: number, b: number) => a + b);
  }
  return 0;
});

const totalQuantity = computed(() => {
  if (rows.value.length > 0) {
    return rows.value.map((r: any) => r.workersQuantity).reduce((a: number, b: number) => a + b);
  }
  return 0;
});

function getCompanyRequests() {
  isLoading.value = true;
  companyStore.setCompanyRequestFilter(serverParams);
  getRequests(serverParams)
    .then((requests: any) => {
      rows.value = requests.items;
      totalItems.value = requests.totalItems;
      isLoading.value = false;
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onPageChange(params: number) {
  serverParams.pageIndex = params;
  getCompanyRequests();
}

function onSortChange(field: string, order: string) {
  switch (field) {
    case 'numberId':
      serverParams.sortBy = 0;
      break;
    case 'jobTitle':
      serverParams.sortBy = 2;
      break;
    case 'workersQuantityWorking':
      serverParams.sortBy = 6;
      break;
  }
  serverParams.isDescending = order !== 'asc';
  getCompanyRequests();
}

function onCellClick(row: any, column: any) {
  switch (column.field) {
    case 'displayShift':
      break;
    case 'workersQuantityWorking':
      router.push({
        path: `/company-requests/${row.id}`,
        query: { tab: 'Workers' },
      });
      break;
    default:
      router.push(`/company-requests/${row.id}`);
  }
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    getCompanyRequests();
  }
}

function getStatusClass(row: any) {
  if (row.requestStatus === RequestStatus.Open &&
    row.workersQuantityWorking > 0 &&
    row.workersQuantityWorking < row.workersQuantity) {
    return 'status-inprogress';
  }
  return 'status-' + RequestStatusLabels[row.requestStatus].toLowerCase();
}

if (companyStore.companyRequestFilter) {
  Object.assign(serverParams, companyStore.companyRequestFilter);
}
getCompanyRequests();
</script>
