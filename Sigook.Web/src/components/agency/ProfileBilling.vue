<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="addLocation">Add</b-button>
    </b-field>
    <b-table :data="locations" narrowed hoverable paginated pagination-rounded>
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="formattedAddress" label="Address" v-slot="props">
          {{ props.row.formattedAddress }}
        </b-table-column>
        <b-table-column field="isBilling" label="Company Use As Billing Address" v-slot="props">
          {{ props.row.isBilling ? 'Yes' : 'No' }}
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-field>
            <b-button outlined rounded type="is-primary" @click="editLocation(props.row)" class="mr-2"
              icon-left="pencil" />
            <b-button outlined rounded type="is-danger" @click="deleteLocation(props.row, props.index)"
              class="mr-2" icon-left="delete" />
          </b-field>
        </b-table-column>
      </template>
    </b-table>

    <b-modal v-model="showModal" width="500px">
      <address-component ref="addressComponent" :model="locationBeingUpdate" @isLoading="(value) => isLoading = value" />
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
      isLoading: true,
      locations: [],
      showModal: false,
      locationBeingUpdate: {},
    }
  },
  props: ['agencyData'],
  methods: {
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
      this.$store.dispatch("agency/updateAgencyLocation", { id: location.id, model: location })
        .then(() => {
          this.isLoading = false;
          this.hideModal();
          location.formattedAddress = this.getFormattedAddress(location);
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    createLocation(location) {
      this.isLoading = true;
      this.$store.dispatch("agency/createAgencyLocation", location).then(r => {
        location.id = r.id;
        location.formattedAddress = this.getFormattedAddress(location);
        this.locations.push(location);
        this.isLoading = false;
        this.hideModal();
      }).catch(error => {
        this.isLoading = false;
        this.showAlertError(error.data);
      });
    },
    getLocations() {
      this.isLoading = true;
      this.$store.dispatch("agency/getAgencyLocation", this.profileId).then(r => {
        this.locations = r;
        this.isLoading = false;
      });
    },
    deleteLocation(location, index) {
      this.showAlertConfirm("Are you sure you want to delete this location?", '', "Yes")
        .then(r => {
          if (!r) return;
          this.isLoading = true;
          this.$store.dispatch("agency/deleteAgencyLocation", location.id)
            .then(() => {
              this.isLoading = false;
              this.locations.splice(index, 1);
            }).catch(e => {
              this.isLoading = false;
              this.showAlertError(e.data);
            });
        });
    },
    addLocation() {
      this.showModal = true;
      this.locationBeingUpdate = {};
    },
    editLocation(location) {
      this.locationBeingUpdate = location;
      this.showModal = true;
    },
    hideModal() {
      this.showModal = false;
    },
    getFormattedAddress(location) {
      if (!location) return "";
      return `${location.address} ${location.city.value} ${location.province.code} ${location.postalCode}`;
    }
  },
  created() {
    this.getLocations();
  }
}
</script>