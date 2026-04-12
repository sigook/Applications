<template>
  <div class="mt-1">
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <export :url="getTimeSheetUrl" :params="serverParams" :fileName="'Timesheet'"
        @onDataLoading="(value) => isLoading = value"></export>
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
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span :class="props.row.isSubcontractor ? 'Blue' : ''">{{ props.row.numberId }}</span>
            </template>
          </b-table-column>
          <b-table-column field="externalId" label="External ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.externalId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">{{ props.row.externalId }}</template>
          </b-table-column>
          <b-table-column field="name" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
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
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="filteredStatuses" open-on-focus
                field="value" icon="label" placeholder="Select Status" @typing="filterStatuses" @input="onStatusSelected" append-to-body>
              </b-taginput>
            </template>
            <template v-slot="props">
              <b-tag rounded :type="props.row.workerRequestStatus === 3 ? 'is-success' : 'is-danger'">{{ props.row.workerRequestStatus === 3 ? 'Booked' : 'Rejected' }}</b-tag>
            </template>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-tooltip label="Punch Card" type="is-dark" position="is-top">
              <b-button type="is-info" outlined rounded icon-right="timetable" class="mr-2"
                @click="showModalPunchCard(props.row)"></b-button>
            </b-tooltip>
          </b-table-column>
        </template>
        <template #detail="props">
          <punch-card ref="punchCard" :workerId="props.row.workerId" :worker="props.row"
            :requestId="serverParams.requestId" :request="request" />
        </template>
      </b-table>
    </div>
    <b-modal v-model="modalPunchCard">
      <agency-punch-card :requestId="serverParams.requestId" :workerName="currentWorker.name"
        :workerId="currentWorker.workerId" @created="onModalPunchCardClose" />
    </b-modal>
  </div>
</template>
<script lang="ts">
import { hour } from '@/utils/filters';
import { getAgencyRequestsWorkers } from "@/api/agencyRequestApi";

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
      filteredStatuses: [],
      modalPunchCard: false,
      currentWorker: {},
      serverParams: {
        sortBy: 2,
        requestId: this.$route.params.id,
        pageIndex: 1,
        pageSize: 30,
        isDescending: true,
      }
    }
  },
  components: {
    PunchCard: () => import("@/components/agency_request/AgencyPunchCardWorkerContainer.vue"),
    AgencyPunchCard: () => import("@/components/agency/AgencyPunchCard.vue"),
    Export: () => import("@/components/Export.vue"),
  },
  methods: {
    hour,
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.loadRequestWorkers();
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
        case 'externalId':
          this.serverParams.sortBy = 6;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.loadRequestWorkers();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.loadRequestWorkers();
      }
    },
    filterStatuses(text) {
      this.filteredStatuses = this.statuses.filter(s =>
        !this.statusesSelected.some(ss => ss.id === s.id) &&
        s.value.toLowerCase().includes(text.toLowerCase())
      );
    },
    onStatusSelected() {
      this.filteredStatuses = this.statuses.filter(s =>
        !this.statusesSelected.some(ss => ss.id === s.id)
      );
      this.serverParams.statuses = this.statusesSelected.map(ss => ss.id);
      this.loadRequestWorkers();
    },
    loadRequestWorkers() {
      this.isLoading = true;
      getAgencyRequestsWorkers(this.serverParams)
        .then(response => {
          this.rows = response.items.map(i => ({ ...i, actions: null }));
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    showModalPunchCard(worker) {
      this.currentWorker = worker;
      this.modalPunchCard = true
    },
    onModalPunchCardClose() {
      this.modalPunchCard = false;
      if (this.$refs.punchCard) {
        this.$refs.punchCard.updateCell();
      }
    },
  },
  created() {
    this.filteredStatuses = this.statuses;
    this.loadRequestWorkers();
  },
  computed: {
    getTimeSheetUrl() {
      return `/api/AgencyRequest/${this.serverParams.requestId}/TimeSheet`;
    }
  }
}
</script>