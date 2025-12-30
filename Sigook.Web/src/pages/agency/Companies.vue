<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-2">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        {{ $t('AgencyCompanies') }}
        <span class="fw-100 fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <export :url="'/api/v2/AgencyCompanyProfile/File'" :params="serverParams" :fileName="'Companies'"
        @onDataLoading="(value) => isLoading = value">
        <template v-slot:actions>
          <b-button @click="addFile = true" icon-left="file-plus">Bulk Data</b-button>
          <b-button tag="router-link" to="/create-company" icon-left="plus">
            {{ $t('Create') }}
          </b-button>
        </template>
      </export>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="updatedAt"
        :current-page.sync="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="businessName" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.businessInfo" placeholder="Search..." :icon="isMobile ? '' : 'magnify'"
                size="is-small" @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <router-link :to="{ path: '/agency-companies/company/' + props.row.id }">
                {{ props.row.businessName }}
                <p v-for="(location, index) in props.row.locations" :key="location" v-if="index < 2">
                  <i class="fz-2 block">{{ location }}</i>
                </p>
                <p>
                  <i v-if="props.row.locations.length > 2" class="fz-2 block">See details...</i>
                </p>
              </router-link>
            </template>
          </b-table-column>
          <b-table-column field="email" label="Contact Info" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.contactInfo" placeholder="Search..." :icon="isMobile ? '' : 'magnify'"
                size="is-small" @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              {{ props.row.contactName || 'No contact name' }}
              <p><i class="fz-2 block">{{ props.row.email }}</i></p>
              <p><i class="fz-2 block">{{ props.row.phone || 'No phone' }}</i></p>
              <p><i class="fz-2 block">{{ props.row.contactRole || 'No role' }}</i></p>
              <p>
                <i v-if="!props.row.website" class="fz-2 block">No website</i>
                <a v-else :href="props.row.website" target="_blank" class="fz-2 block">{{ props.row.website }}</a>
              </p>
            </template>
          </b-table-column>
          <b-table-column field="industry" label="Industry" :visible="!isMobile" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.industry" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <b-tag size="is-medium" rounded>
                {{ props.row.industry }}
              </b-tag>
            </template>
          </b-table-column>
          <b-table-column field="salesRepresentative" label="Sales Rep" sortable searchable>
            <template v-slot:searchable>
              <b-field>
                <b-input size="is-small" icon="magnify" placeholder="Sales Rep"
                  v-model="serverParams.salesRepresentative" @keypress.native="onInputEntered"></b-input>
              </b-field>
            </template>
            <template v-slot="props">
              {{ props.row.salesRepresentative }}
            </template>
          </b-table-column>
          <b-table-column field="createdAt" label="Creation Info" sortable :searchable="!isMobile">
            <template v-slot:searchable>
              <b-field>
                <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.createdBy"
                  @keypress.native="onInputEntered"></b-input>
                <b-datepicker size="is-small" :mobile-native="false" placeholder="Created At"
                  :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                  v-model="createdAtDatesSelected" icon-right-clickable @icon-right-click="onCreatedAtCleared"
                  @input="onCreatedAtSelected"></b-datepicker>
              </b-field>
            </template>
            <template v-slot="props">
              {{ props.row.createdAt | dateMonth }}
              <p><i class="fz-2 block">{{ props.row.createdBy || 'Sigook' }}</i></p>
            </template>
          </b-table-column>
          <b-table-column field="updatedAt" label="Update Info" :visible="!isMobile" sortable searchable>
            <template v-slot:searchable>
              <b-field>
                <b-input placeholder="Updated By" size="is-small" icon="magnify" v-model="serverParams.updatedBy"
                  @keypress.native="onInputEntered"></b-input>
                <b-datepicker placeholder="Updated At" size="is-small" :mobile-native="false"
                  :icon-right="updatedAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                  v-model="updatedAtDatesSelected" icon-right-clickable @icon-right-click="onUpdatedAtCleared"
                  @input="onUpdatedAtSelected">
                </b-datepicker>
              </b-field>
            </template>
            <template v-slot="props">
              <div v-if="props.row.companyStatus === 5 && !props.row.updatedAt">
                Existing client
              </div>
              <div v-else-if="props.row.updatedAt">
                {{ props.row.updatedAt | dateMonth }}
                <p><i class="fz-2 block">{{ props.row.updatedBy }}</i></p>
              </div>
              <div v-else>
                No updates
              </div>
            </template>
          </b-table-column>
          <b-table-column field="notesCount" label="Notes" :visible="!isMobile" v-slot="props">
            <div @click="onNote(props.row, true)">
              <b-tag size="is-small" icon="note-multiple" rounded>
                <label v-if="props.row.notesCount">{{ props.row.notesCount }}</label>
              </b-tag>
            </div>
            <div v-if="props.row.showNotes" class="notes-tooltip">
              <modal-notes :can-create="false" :user-id="props.row.id" :on-get="'agency/getAgencyCompanyNote'"
                :on-create="'agency/createAgencyCompanyNote'" :on-delete="'agency/deleteAgencyCompanyNote'"
                @onUpdateNote="(val) => onUpdateNote(props.row, val.size)" @close="onNote(props.row, false)">
              </modal-notes>
            </div>
          </b-table-column>
          <b-table-column field="companyStatus" label="Status" width="250px" :searchable="!isMobile">
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
                field="value" icon="label" placeholder="Select Status" @input="onStatusSelected">
              </b-taginput>
            </template>
            <template v-slot="props">
              <b-tag size="is-medium" rounded>
                {{statuses.find(s => s.id === props.row.companyStatus).value}}
              </b-tag>
            </template>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <b-modal v-model="addFile" @close="addFile = false" width="500px">
      <bulk-data :store-action="'agency/bulkCompanies'" :error-file-name="'BulkCompaniesError'"
        :title="'Bulk Companies'" :file-label="'Companies File'" @close="addFile = false" />
    </b-modal>
  </div>
</template>
<script>

export default {
  components: {
    Export: () => import("@/components/Export"),
    ModalNotes: () => import("@/components/notes/ModalNotes"),
    BulkData: () => import("@/components/agency/BulkData"),
  },
  data() {
    return {
      isLoading: true,
      totalItems: 0,
      statuses: [],
      statusesSelected: [],
      createdAtDatesSelected: [],
      updatedAtDatesSelected: [],
      rows: [],
      addFile: false,
      serverParams: {
        sortBy: 3,
        isDescending: true,
        pageIndex: 1,
        pageSize: 30
      }
    }
  },
  created() {
    this.statuses = this.$route.meta.companyStatuses;
    if (this.$store.state.agency.agencyCompanyProfileFilter) {
      this.serverParams = this.$store.state.agency.agencyCompanyProfileFilter;
      if (this.serverParams.companyStatuses) {
        this.statusesSelected = this.statuses.filter(s => this.serverParams.companyStatuses.some(sps => sps == s.id));
      }
      if (this.serverParams.createdAtFrom && this.serverParams.createdAtTo) {
        this.createdAtDatesSelected[0] = this.serverParams.createdAtFrom;
        this.createdAtDatesSelected[1] = this.serverParams.createdAtTo;
      }
    }
    this.getCompanies();
  },
  methods: {
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getCompanies();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'businessName':
          this.serverParams.sortBy = 0;
          break;
        case 'industry':
          this.serverParams.sortBy = 1;
          break;
        case 'createdAt':
          this.serverParams.sortBy = 2;
          break;
        case 'updatedAt':
          this.serverParams.sortBy = 3;
          break;
        case 'salesRepresentative':
          this.serverParams.sortBy = 4;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.getCompanies();
    },
    onStatusSelected() {
      this.serverParams.companyStatuses = this.statusesSelected.map(ss => ss.id);
      this.getCompanies();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.getCompanies();
      }
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.getCompanies();
    },
    onUpdatedAtCleared() {
      this.updatedAtDatesSelected = [];
      this.onUpdatedAtSelected();
    },
    onUpdatedAtSelected() {
      this.serverParams.updatedAtFrom = this.updatedAtDatesSelected[0];
      this.serverParams.updatedAtTo = this.updatedAtDatesSelected[1];
      this.getCompanies();
    },
    onCellClick(row, column, rowIndex) {
      switch (column._props.field) {
        case 'notesCount':
        case 'email':
          break;
        default:
          this.$router.push({ path: `/agency-companies/company/${row.id}` });
      }
    },
    onNote(row, status) {
      const index = this.rows.findIndex(r => r.id === row.id);
      this.$set(this.rows[index], "showNotes", status);
    },
    getCompanies() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyCompanyProfileFilter", this.serverParams);
      this.$store.dispatch('agency/getCompanies', this.serverParams)
        .then(companies => {
          this.rows = companies.items;
          this.totalItems = companies.totalItems;
          this.isLoading = false;
        })
        .catch(() => {
          this.isLoading = false;
        });
    }
  },
  computed: {
    isMobile() {
      return this.$store.state.isMobile;
    }
  }
}
</script>
<style lang="scss">
tr {
  cursor: pointer;
}
</style>
