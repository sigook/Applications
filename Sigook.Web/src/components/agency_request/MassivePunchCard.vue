<template>
  <div class="mt-1">
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <export :url="getTimeSheetUrl" :params="serverParams" :fileName="'Timesheet'"
        @onDataLoading="(value) => isLoading = value"></export>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        detailed show-detail-icon pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
        detail-transition="fade" default-sort="name" :current-page.sync="serverParams.pageIndex"
        @page-change="onPageChange" @sort="onSortChange">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="profileImage" width="50" v-slot="props">
            <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image" class="img-30" />
            <default-image v-else :name="props.row.fullName" class="img-30"></default-image>
          </b-table-column>
          <b-table-column field="numberId" width="100" label="ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span :class="props.row.isSubcontractor ? 'Blue' : ''">{{ props.row.numberId }}</span>
            </template>
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
            {{ props.row.totalHoursApproved | hour }}
          </b-table-column>
          <b-table-column field="totalHoursWorker" label="Total Hours" sortable v-slot="props">
            {{ props.row.totalHoursWorker | hour }}
          </b-table-column>
          <b-table-column field="status" label="Status" width="250px" sortable searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
                field="value" icon="label" placeholder="Select Status" @input="onStatusSelected">
              </b-taginput>
            </template>
            <template v-slot="props">
              <span class="uppercase fw-700 fz-1" :class="props.row.status">{{ props.row.status }}</span>
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
<script>

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
      modalPunchCard: false,
      currentWorker: {},
      serverParams: {
        sortBy: 1,
        requestId: this.$route.params.id,
        pageIndex: 1,
        pageSize: 30
      }
    }
  },
  components: {
    PunchCard: () => import("@/components/agency_request/AgencyPunchCardWorkerContainer"),
    AgencyPunchCard: () => import("@/components/agency/AgencyPunchCard"),
    Export: () => import("@/components/Export"),
  },
  methods: {
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
      this.$store.dispatch('agency/getAgencyRequestsWorkers', this.serverParams)
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
    this.getWorkers();
  },
  computed: {
    getTimeSheetUrl() {
      return `/api/AgencyRequest/${this.serverParams.requestId}/TimeSheet`;
    }
  }
}
</script>