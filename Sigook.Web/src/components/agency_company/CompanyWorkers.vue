<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="fullName"
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
          <b-table-column field="numberId" width="100" label="ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span :class="props.row.isSubcontractor ? 'Blue' : ''">{{ props.row.numberId }}</span>
            </template>
          </b-table-column>
          <b-table-column field="fullName" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.fullName" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
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
              <b-input v-model="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered" v-cleave="mask"></b-input>
            </template>
            <template v-slot="props">{{ props.row.mobileNumber }}</template>
          </b-table-column>
          <b-table-column field="requestsNumberId" label="Request ID" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.requestId" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
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
                @input="onCreatedAtSelected">
              </b-datepicker>
            </template>
            <template v-slot="props">{{ props.row.createdAt | dateMonth }}</template>
          </b-table-column>
          <b-table-column field="skills" width="800px" label="Skills" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.skills" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
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
          <b-table-column field="isCurrentlyWorking" width="250px" label="Details" searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="featuresSelected" autocomplete :data="features" open-on-focus
                field="value" icon="label" placeholder="Select Details" @input="onFeatureChange">
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
<script>
import workerFeaturesMixin from "@/mixins/workerFeaturesMixin";
import phoneMaskMixin from "@/mixins/phoneMaskMixin"

export default {
  props: ['company'],
  mixins: [workerFeaturesMixin, phoneMaskMixin],
  data() {
    const companyProfileId = this.company.id;
    return {
      isLoading: false,
      totalItems: 0,
      createdAtDatesSelected: [],
      featuresSelected: [],
      rows: [],
      serverParams: {
        sortBy: 0,
        isDescending: false,
        pageIndex: 1,
        pageSize: 30,
        companyProfileId: companyProfileId
      }
    }
  },
  methods: {
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getAgencyCompanyWorkers();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'fullName':
          this.serverParams.sortBy = 0;
          break;
        case 'numberId':
          this.serverParams.sortBy = 1;
          break;
        case 'requestsNumberId':
          this.serverParams.sortBy = 2;
          break;
        case 'createdAt':
          this.serverParams.sortBy = 3;
          break;
        case 'skills':
          this.serverParams.sortBy = 4;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.getWorkers();
    },
    onCellClick(row) {
      this.$router.push(`/agency-workers/worker/${row.id}`);
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.getAgencyCompanyWorkers();
      }
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.getAgencyCompanyWorkers();
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    onFeatureChange() {
      this.serverParams.features = this.featuresSelected.map(fs => fs.id);
      this.getAgencyCompanyWorkers();
    },
    getAgencyCompanyWorkers() {
      this.isLoading = true;
      this.$store.dispatch('agency/getWorkers', this.serverParams)
        .then(response => {
          this.rows = response.items;
          this.totalItems = response.totalItems;
          this.isLoading = false;
        }).catch((error) => {
          this.showAlertError(error);
          this.isLoading = false;
        })
    },
  },
  created() {
    this.getAgencyCompanyWorkers();
  }
}
</script>