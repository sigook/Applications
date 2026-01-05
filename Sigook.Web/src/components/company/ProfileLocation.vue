<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="addLocation">Add</b-button>
    </b-field>
    <b-table :data="locations" narrowed hoverable paginated pagination-rounded :per-page="pageSize"
      :current-page.sync="pageIndex">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="formattedAddress" label="Address" v-slot="props" searchable
          :custom-search="onSearchLocation">
          {{ props.row.formattedAddress }}
        </b-table-column>
        <b-table-column field="isBilling" label="Company Use As Billing Address" v-slot="props">
          {{ props.row.isBilling ? 'Yes' : 'No' }}
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-field>
            <b-button outlined rounded type="is-primary" @click="editLocation(props.row)" class="mr-2"
              icon-left="pencil" />
            <b-button outlined rounded type="is-danger" @click="deleteLocation(props.row.id)"
              class="mr-2" icon-left="delete" />
          </b-field>
        </b-table-column>
      </template>
    </b-table>
    <b-modal v-model="showModal" width="500px">
      <address-component ref="addressComponent" :model="locationBeingUpdate"
        :enableProvinceSettings="true"
        @isLoading="(value) => isLoading = value" />
      <div class="container-flex">
        <div class="col-12 col-padding">
          <b-checkbox v-model="locationBeingUpdate.isBilling">{{ $t('CompanyUseAsBillingAddress') }}</b-checkbox>
        </div>
        <div class="col-12 col-padding">
          <b-button type="is-primary" @click="saveChanges">SAVE</b-button>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script>
import AddressComponent from "@/components/Address";

export default {
  components: { AddressComponent },
  data() {
    return {
      locations: [],
      isLoading: false,
      showModal: false,
      locationBeingUpdate: {},
      pageSize: 30,
      pageIndex: 1,
    }
  },
  props: ['companyData'],
  methods: {
    addLocation() {
      this.locationBeingUpdate = {};
      this.showModal = true;
    },
    editLocation(location) {
      this.locationBeingUpdate = location;
      this.showModal = true;
    },
    closeModal() {
      this.showModal = false;
      this.locationBeingUpdate = {};
    },
    deleteLocation(id) {
      this.showAlertConfirm("Are you sure you want to delete this location?", '', "Yes").then(r => {
        if (!r) return;
        this.isLoading = true;
        this.$store.dispatch("company/deleteProfileLocation", { id })
          .then(() => {
            this.isLoading = false;
            this.getLocations();
          }).catch(e => {
            this.isLoading = false;
            this.showAlertError(e.data);
          });
      });
    },
    async saveChanges() {
      const addressValid = await this.$refs.addressComponent.validateAddress();
      if (addressValid) {
        if (this.locationBeingUpdate.id) {
          this.updateLocation(this.locationBeingUpdate);
        } else {
          this.createLocation(this.locationBeingUpdate);
        }
      }
    },
    updateLocation(location) {
      this.isLoading = true;
      this.$store.dispatch("company/updateProfileLocation", { id: location.id, model: location })
        .then(() => {
          this.isLoading = false;
          this.closeModal();
          this.getLocations();
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    createLocation(location) {
      this.isLoading = true;
      this.$store.dispatch("company/createProfileLocation", location)
        .then(() => {
          this.isLoading = false;
          this.closeModal();
          this.getLocations();
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    getLocations() {
      this.isLoading = true;
      this.$store.dispatch("company/getProfileLocations")
        .then(r => {
          this.locations = r;
          this.isLoading = false;
        });
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
  },
  created() {
    this.getLocations();
  }
}
</script>