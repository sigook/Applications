<template>
  <div class="mt-1">
    <b-loading v-model="isLoading"></b-loading>
    <data-entry-terms></data-entry-terms>
    <div>
      <b-field grouped position="is-right">
        <b-button size="is-small" type="is-ghost" icon-right="file-excel"
          @click="downloadTimeSheetDocument">Export</b-button>
      </b-field>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        detailed show-detail-icon pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
        detail-transition="fade" default-sort="name" v-model:current-page="serverParams.pageIndex"
        @page-change="onPageChange" @sort="onSortChange">
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
          <b-table-column field="name" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              {{ props.row.name }}
            </template>
          </b-table-column>
          <b-table-column field="totalHoursApproved" label="Approved Hours" sortable v-slot="props">
            {{ hour(props.row.totalHoursApproved) }}
          </b-table-column>
          <b-table-column field="totalHoursWorker" label="Total Hours" sortable v-slot="props">
            {{ hour(props.row.totalHoursWorker) }}
          </b-table-column>
          <b-table-column field="status" label="Status" sortable searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
                field="value" icon="label" placeholder="Select Status" @update:modelValue="onStatusSelected" append-to-body>
              </b-taginput>
            </template>
            <template v-slot="props">
              <b-tag rounded :type="props.row.workerRequestStatus === 3 ? 'is-success' : 'is-danger'">{{ props.row.workerRequestStatus === 3 ? 'Booked' : 'Rejected' }}</b-tag>
            </template>
          </b-table-column>
        </template>
        <template #detail="props">
          <table-punch-card :workerId="props.row.workerId" :requestId="serverParams.requestId" :request="request"
            :worker="props.row.worker"></table-punch-card>
        </template>
      </b-table>
    </div>
  </div>
</template>
<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { showAlertError } from "@/utils/toast";
import { hour } from '@/utils/filters';
import { downloadFile } from '@/utils/downloadFile';
import { getRequestWorkers } from '@/api/companyApi';
import { getRequestTimeSheetDocument } from "@/api/agencyReportApi";
import { WorkerRequestStatusLabels } from "@/constants/enums";

export default {
  props: ['request'],
  data() {
    return {
      isLoading: true,
      totalItems: 0,
      rows: [],
      statuses: [
        { id: 2, value: 'Rejected' },
        { id: 3, value: 'Booked' },
      ],
      statusesSelected: [],
      serverParams: {
        sortBy: 1,
        requestId: this.$route.params.id,
        pageIndex: 1,
        pageSize: 30
      }
    }
  },
  components: {
    TablePunchCard: defineAsyncComponent(() => import("@/components/company_request/CompanyPunchCardWorkerContainer.vue")),
    DataEntryTerms: defineAsyncComponent(() => import("@/components/DataEntryTerms.vue"))
  },
  methods: {
    downloadFile,
    hour,
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getWorkers();
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
      }
      this.serverParams.isDescending = order !== 'asc';
      this.getWorkers();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.getWorkers();
      }
    },
    onStatusSelected() {
      this.serverParams.statuses = this.statusesSelected.map(ss => ss.id);
      this.getWorkers();
    },
    getWorkers() {
      this.isLoading = true;
      getRequestWorkers(this.serverParams)
        .then((response) => {
          this.rows = response.items.map(i => ({
            ...i,
            status: WorkerRequestStatusLabels[i.workerRequestStatus]
          }));
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch(error => {
          showAlertError(error.data);
          this.isLoading = false;
        })
    },
    downloadTimeSheetDocument() {
      this.isLoading = true;
      getRequestTimeSheetDocument(this.serverParams.requestId)
        .then(response => {
          this.isLoading = false;
          this.downloadFile(response, `TimeSheet_${this.serverParams.requestId}`);
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        })
    }
  },
  created() {
    this.getWorkers();
  }
}
</script>