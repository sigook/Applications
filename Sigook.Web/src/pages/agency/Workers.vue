<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Workers
        <span class="fw-100 fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <export :url="'/api/AgencyWorkerProfile/File'" :params="serverParams" :fileName="'Workers'"
        @onDataLoading="(value) => isLoading = value">
        <template v-slot:actions>
          <b-button tag="router-link" to="/agency-workers/register-worker" icon-left="plus">
            {{ $t('Create') }}
          </b-button>
        </template>
      </export>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="fullName"
        :current-page.sync="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
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
          <b-table-column field="fullName" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.fullName" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span class="d-block">
                <router-link :to="{ path: '/agency-workers/worker/' + props.row.id }">
                  {{ props.row.fullName }}
                </router-link>
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
              <b-input v-model="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered" v-cleave="mask"></b-input>
            </template>
            <template v-slot="props">{{ props.row.mobileNumber }}</template>
          </b-table-column>
          <b-table-column field="location" label="Location" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.location" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">{{ props.row.address }}</template>
          </b-table-column>
          <b-table-column field="requests" label="Request ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.requestId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <div v-if="props.row.requests && props.row.requests.length > 0">
                <b-taglist>
                  <b-tag v-for="request in props.row.requests" :key="request.id" rounded
                    @click="goToApplicants(request)">
                    {{ request.value }}
                  </b-tag>
                </b-taglist>
              </div>
              <span v-else class="op3 is-inline-block v-middle pr-0">Request ID</span>
            </template>
          </b-table-column>
          <b-table-column field="createdAt" label="Created At" sortable searchable>
            <template v-slot:searchable>
              <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
                :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
                @icon-right-click="onCreatedAtCleared" range v-model="createdAtDatesSelected"
                @input="onCreatedAtSelected" append-to-body>
              </b-datepicker>
            </template>
            <template v-slot="props">{{ dateMonth(props.row.createdAt) }}</template>
          </b-table-column>
          <b-table-column field="skills" label="Skills" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.skills" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <div v-if="props.row.skills.length > 0" class="skills-inline">
                <b-tag v-for="(skill, index) in props.row.skills.slice(0, 3)" :key="`${skill}_${index}`" rounded>
                  {{ skill }}
                </b-tag>
                <b-tooltip v-if="props.row.skills.length > 3" :label="props.row.skills.join(', ')" type="is-dark" multilined append-to-body>
                  <b-tag rounded type="is-info is-light" class="see-more">See more...</b-tag>
                </b-tooltip>
              </div>
            </template>
          </b-table-column>
          <b-table-column field="isCurrentlyWorking" label="Details" searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="featuresSelected" autocomplete :data="features" open-on-focus
                field="value" icon="label" placeholder="Select Feature" @input="onFeatureChange" append-to-body>
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
          <b-table-column field="actions" v-slot="props">
            <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
              <template #trigger>
                <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
              </template>
              <b-dropdown-item aria-role="listitem" @click="$router.push(`/agency-workers/worker/${props.row.id}`)">
                Edit
              </b-dropdown-item>
              <b-dropdown-item aria-role="listitem" v-if="!props.row.approvedToWork" @click="deleteWorker(props.row)">
                Approve to work
              </b-dropdown-item>
              <b-dropdown-item aria-role="listitem" v-if="props.row.approvedToWork" @click="confirmDelete(props.row)">
                Reject to work
              </b-dropdown-item>
            </b-dropdown>
          </b-table-column>
        </template>
      </b-table>
    </div>
  </div>
</template>
<script lang="ts">

import { workerFeatures as features } from '@/constants/workerFeatures';
import { phoneMask as mask } from '@/constants/phoneMask';
import { getAgencyWorkers, updateApprovedToWork } from "@/api/agencyWorkerApi";
import { dateMonth } from '@/utils/filters';

export default {
  data() {
    return {
      mask,
      features,
      isLoading: true,
      totalItems: 0,
      createdAtDatesSelected: [],
      featuresSelected: [],
      rows: [],
      serverParams: {
        sortBy: 0,
        isDescending: false,
        pageIndex: 1,
        pageSize: 30
      }
    }
  },
  components: {
    Export: () => import("@/components/Export.vue")
  },
  methods: {
    dateMonth,
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.loadWorkers();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'fullName':
          this.serverParams.sortBy = 0;
          break;
        case 'numberId':
          this.serverParams.sortBy = 1;
          break;
        case 'requests':
          this.serverParams.sortBy = 2;
          break;
        case 'createdAt':
          this.serverParams.sortBy = 3;
          break;
        case 'skills':
          this.serverParams.sortBy = 4;
          break;
        case 'externalId':
          this.serverParams.sortBy = 5;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.loadWorkers();
    },
    onCellClick(row, column) {
      switch (column._props.field) {
        case 'actions':
          break;
        case 'requests':
          break;
        default:
          this.$router.push(`/agency-workers/worker/${row.id}`);
      }
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.loadWorkers();
      }
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.loadWorkers();
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    onFeatureChange() {
      this.serverParams.features = this.featuresSelected.map(fs => fs.id);
      this.loadWorkers();
    },
    goToApplicants(item) {
      this.$router.push({
        path: `/agency-request/${item.id}`,
        query: { tab: 'Applicants' }
      });
    },
    loadWorkers() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyWorkerProfileFilter", this.serverParams);
      getAgencyWorkers(this.serverParams)
        .then(workers => {
          this.rows = workers.items.map(w => ({ ...w, actions: null }));
          this.totalItems = workers.totalItems;
          this.isLoading = false;
        })
        .catch(() => {
          this.isLoading = false;
        });
    },
    deleteWorker(worker) {
      this.isLoading = true;
      updateApprovedToWork(worker.id)
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess(this.$t("Updated"));
          this.loadWorkers();
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
          this.loadWorkers();
        });
    },
    confirmDelete(worker) {
      this.showAlertConfirm(
        this.$t("AreYouSure"),
        this.$t("YouWantToDisableTheWorker") +
        ". " +
        this.$t("ThisWorkerWillNotBeAbleToApplyToNewRequests")
      ).then((response) => {
        if (response) {
          this.deleteWorker(worker);
        }
      })
        .catch((error) => {
          this.showAlertError(error);
        });
    },
  },
  created() {
    if (this.$store.state.agency.agencyWorkerProfileFilter) {
      this.serverParams = this.$store.state.agency.agencyWorkerProfileFilter;
      if (this.serverParams.features) {
        this.featuresSelected = this.features.filter(s => this.serverParams.features.some(sps => sps == s.id));
      }
      if (this.serverParams.createdAtFrom && this.serverParams.createdAtTo) {
        this.createdAtDatesSelected[0] = this.serverParams.createdAtFrom;
        this.createdAtDatesSelected[1] = this.serverParams.createdAtTo;
      }
    }
    this.loadWorkers();
  },
}
</script>
<style lang="scss" scoped>
.skills-inline {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  align-items: center;
}

.see-more {
  cursor: pointer;
}
</style>
