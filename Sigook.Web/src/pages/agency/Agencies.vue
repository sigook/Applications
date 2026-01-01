<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Agencies
        <span class="fw-100 fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <b-field grouped position="is-right">
        <b-button tag="router-link" to="/create-agency" icon-left="plus">
          {{ $t('Create') }}
        </b-button>
      </b-field>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" default-sort="fullName"
        :current-page.sync="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="fullName" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.fullName" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <router-link :to="{ path: '/agency-detail/' + props.row.id }">
                <span class="d-block">{{ props.row.fullName }}</span>
                <p v-for="(location, index) in props.row.locations" :key="location" v-if="index < 2">
                  <i class="fz-2 block">{{ location }}</i>
                </p>
                <p v-if="props.row.locations && props.row.locations.length > 2">
                  <i class="fz-2 block">See details...</i>
                </p>
              </router-link>
            </template>
          </b-table-column>
          <b-table-column field="email" label="Email" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.email" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span class="d-block">{{ props.row.email }}</span>
            </template>
          </b-table-column>
          <b-table-column field="agencyType" label="Type" sortable searchable>
            <template v-slot:searchable>
              <b-taginput size="is-small" v-model="agencyTypesSelected" autocomplete :data="$agencyTypes" open-on-focus
                field="label" icon="label" placeholder="Select Type" @input="onAgencyTypeSelected">
              </b-taginput>
            </template>
            <template v-slot="props">
              <b-tag size="is-medium" rounded>
                {{ props.row.agencyType | agencyType }}
              </b-tag>
            </template>
          </b-table-column>
        </template>
      </b-table>
    </div>
  </div>
</template>
<script>

export default {
  data() {
    return {
      isLoading: true,
      totalItems: 0,
      rows: [],
      agencyTypesSelected: [],
      serverParams: {
        sortBy: 0,
        isDescending: false,
        pageIndex: 1,
        pageSize: 30
      }
    }
  },
  created() {
    if (this.$store.state.agency.agencyListFilter) {
      this.serverParams = this.$store.state.agency.agencyListFilter;
    }
    this.getAgencies();
  },
  methods: {
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getAgencies();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'fullName':
          this.serverParams.sortBy = 0;
          break;
        case 'email':
          this.serverParams.sortBy = 1;
          break;
        case 'agencyType':
          this.serverParams.sortBy = 2;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.getAgencies();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.getAgencies();
      }
    },
    onAgencyTypeSelected() {
      this.serverParams.agencyTypes = this.agencyTypesSelected.map(t => t.value);
      this.getAgencies();
    },
    getAgencies() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyListFilter", this.serverParams);
      this.$store.dispatch('agency/getAgencies', this.serverParams)
        .then(agencies => {
          this.rows = agencies.items;
          this.totalItems = agencies.totalItems;
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    }
  }
}
</script>
