<template>
  <div class="company-requests white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        {{ $t("CompanyStaffRequests") }}
        <span class="fw-100 fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <b-field grouped position="is-right">
        <b-button tag="router-link" to="/create-request" icon-left="plus">Create Request</b-button>
      </b-field>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="numberId"
        :current-page.sync="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="numberId" label="Order ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <router-link :to="{ path: '/request/' + props.row.id }">
                <p>{{ props.row.numberId }}</p>
              </router-link>
              <p v-if="props.row.isAsap" class="asap">{{ $t("Asap") }}</p>
              <p v-if="props.row.isDirectHiring" class="asap">DH</p>
            </template>
          </b-table-column>
          <b-table-column field="jobTitle" label="Position" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.jobTitle" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              {{ props.row.jobTitle }}
              <i class="fz-2 block">{{ dateFromNow(props.row.createdAt) }}</i>
            </template>
          </b-table-column>
          <b-table-column field="location" label="Location" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.location" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              {{ props.row.location }}
              <span v-if="props.row.entrance"> - {{ props.row.entrance }}</span>
            </template>
          </b-table-column>
          <b-table-column field="displayShift" label="Shift" v-slot="props">
            <agency-shift class="d-block" :requestId="props.row.id"
              :displayShift="props.row.displayShift"></agency-shift>
          </b-table-column>
          <b-table-column field="workersQuantityWorking" sortable>
            <template v-slot:header>
              <p class="fw-600">Workers</p>
              <p class="fw-600">({{ totalQuantityWorking }} / {{ totalQuantity }})</p>
            </template>
            <template v-slot="props">
              {{ props.row.workersQuantityWorking }} / {{ props.row.workersQuantity }}
            </template>
          </b-table-column>
          <b-table-column field="requestStatus" label="Status" v-slot="props">
            <div class="text-center">
              <b-tooltip :label="$t(RequestStatusLabels[props.row.requestStatus])" type="is-dark" append-to-body>
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

<script lang="ts">
import toast from "@/mixins/toastMixin";
import { getRequests } from '@/api/companyApi';
import { RequestStatus, RequestStatusLabels } from '@/constants/enums';
import { dateFromNow } from '@/utils/filters';

export default {
  components: {
    AgencyShift: () => import("@/components/agency_request/AgencyShiftDetail.vue"),
  },
  data() {
    return {
      isLoading: true,
      totalItems: 0,
      rows: [],
      serverParams: {
        sortBy: 0,
        isDescending: true,
        pageIndex: 1,
        pageSize: 30
      }
    };
  },
  mixins: [toast],
  methods: {
    dateFromNow,
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getCompanyRequests();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'numberId':
          this.serverParams.sortBy = 0;
          break;
        case 'jobTitle':
          this.serverParams.sortBy = 2;
          break;
        case 'workersQuantityWorking':
          this.serverParams.sortBy = 6;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.getCompanyRequests();
    },
    onCellClick(row, column) {
      switch (column._props.field) {
        case 'displayShift':
          break;
        case 'workersQuantityWorking':
          this.$router.push({
            path: `/request/${row.id}`,
            query: {
              tab: 'Workers'
            }
          });
          break;
        default:
          this.$router.push(`/request/${row.id}`);
      }
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.getCompanyRequests();
      }
    },
    getStatusClass(row) {
      if (row.requestStatus === RequestStatus.Open &&
        row.workersQuantityWorking > 0 &&
        row.workersQuantityWorking < row.workersQuantity) {
        return 'status-inprogress';
      }
      return 'status-' + RequestStatusLabels[row.requestStatus].toLowerCase();
    },
    getCompanyRequests() {
      this.isLoading = true;
      this.$store.commit('company/setCompanyRequestFilter', this.serverParams);
      getRequests(this.serverParams)
        .then((requests) => {
          this.rows = requests.items;
          this.totalItems = requests.totalItems;
          this.isLoading = false;
        }).catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    }
  },
  created() {
    if (this.$store.state.company.companyRequestFilter) {
      this.serverParams = this.$store.state.company.companyRequestFilter;
    }
    this.getCompanyRequests();
  },
  computed: {
    RequestStatus: () => RequestStatus,
    RequestStatusLabels: () => RequestStatusLabels,
    totalQuantityWorking() {
      if (this.rows.length > 0) {
        return this.rows
          .map((r) => r.workersQuantityWorking)
          .reduce((a, b) => a + b);
      }
      return 0;
    },
    totalQuantity() {
      if (this.rows.length > 0) {
        return this.rows
          .map((r) => r.workersQuantity)
          .reduce((a, b) => a + b);
      }
      return 0;
    },
  },
};
</script>
