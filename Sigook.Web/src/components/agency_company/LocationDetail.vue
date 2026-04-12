<template>
  <div>
    <div class="container-flex space-between mb-4">
      <h3 class="fw-700">Location</h3>
      <button @click="showModal = true" class="fw-700 fz-1 sm-btn outline-btn orange-button btn-radius">Add</button>
    </div>
    <b-table :data="data" narrowed hoverable paginated :per-page="pageSize" v-model:current-page="pageIndex"
      pagination-rounded @cellclick="onCellClick">
      <b-table-column field="id" v-slot="props" searchable :custom-search="onSearchLocation">
        <span>
          {{ props.row.address }}
          {{ props.row.city.value }}
          {{ props.row.city.province.code }},
          {{ props.row.postalCode }}
        </span>
        <span v-if="props.row.isBilling" class="billing-address">{{ $t('AgencyBillingAddress') }}</span>
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <b-button type="is-danger" outlined rounded icon-right="delete"
          @click="onDeleteLocation(props.row.id, props.row.index)"></b-button>
      </b-table-column>
    </b-table>
    <div class="locations-aside-map" v-if="data && data[0]">
      <iframe v-if="data[0].latitude && data[0].longitude"
        :src="'https://www.google.com/maps/embed/v1/place?key=AIzaSyDj0QAxxsRhSUXsZ-pSKlRh62vsK362xqs&q=' + data[0].latitude + ',' + data[0].longitude + '&zoom=13'"
        allowfullscreen width="100%" height="400px" frameborder="0"
        style="border:0; height: calc(100% + 110px); margin-top: -110px;"></iframe>
    </div>

    <b-modal v-model="showModal" width="800px">
      <location-form :current-location="currentLocation" :profile-id="profileId"
        @updateContent="onUpdateModal" :enableProvinceSettings="true"></location-form>
    </b-modal>
  </div>
</template>
<script lang="ts">
import { getAgencyCompanyLocation, deleteAgencyCompanyLocation } from "@/api/agencyCompanyApi";
export default {
  data() {
    return {
      pageIndex: 1,
      pageSize: 8,
      profileId: this.$route.params.id,
      data: [],
      showModal: false,
      currentLocation: null,
    }
  },
  methods: {
    onCellClick(row, column) {
      switch (column._props.field) {
        case 'id':
          this.openEditModal(row);
          break;
      }
    },
    onSearchLocation(row, searchTerm) {
      const lowerSearchTerm = searchTerm.toLowerCase();
      return (
        row.address.toLowerCase().includes(lowerSearchTerm) ||
        row.city.value.toLowerCase().includes(lowerSearchTerm) ||
        row.city.province.code.toLowerCase().includes(lowerSearchTerm) ||
        row.postalCode.toLowerCase().includes(lowerSearchTerm)
      );
    },
    async loadCompanyLocations() {
      this.data = await getAgencyCompanyLocation(this.profileId);
      this.data = this.data.map(d => ({ ...d, actions: null }));
    },
    async onUpdateModal() {
      await this.loadCompanyLocations();
      this.closeModal();
    },
    closeModal() {
      this.currentLocation = null;
      this.showModal = false;
    },
    openEditModal(item) {
      this.currentLocation = item;
      this.showModal = true;
    },
    onDeleteLocation(id, index) {
      this.showAlertConfirm("Are you sure", "You want to delete this location")
        .then((response) => {
          if (response) {
            this.isLoading = true;
            deleteAgencyCompanyLocation(this.profileId, id)
              .then(() => {
                this.isLoading = false;
                this.showAlertSuccess('Deleted')
                this.data.splice(index, 1);
              })
              .catch((error) => {
                this.isLoading = false;
                this.showAlertError(error)
              })
          }
        }).catch((error) => {
          this.showAlertError(error)
        })
    }
  },
  async created() {
    await this.loadCompanyLocations();
  },
  components: {
    LocationForm: () => import("./LocationForm.vue")
  }
}
</script>