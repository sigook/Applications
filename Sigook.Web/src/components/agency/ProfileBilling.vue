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
      <address-component ref="addressComponent" v-model:model="locationBeingUpdate" @isLoading="(value) => isLoading = value" />
      <div class="container-flex">
        <div class="col-12 col-padding">
          <b-checkbox v-model="locationBeingUpdate.isBilling">{{ 'Use as billing address ?' }}</b-checkbox>
        </div>
        <div class="col-12 col-padding">
          <b-button type="is-primary" @click="saveChanges">SAVE</b-button>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertConfirm, showAlertError } from "@/utils/toast";
import {
  getAgencyLocations,
  createAgencyLocation,
  updateAgencyLocation,
  deleteAgencyLocation
} from "@/api/agencyApi";
import AddressComponent from "@/components/Address.vue";

defineProps<{ agencyData?: any }>();

const isLoading = ref(true);
const locations = ref<any[]>([]);
const showModal = ref(false);
const locationBeingUpdate = ref<any>({});
const addressComponent = ref<any>(null);

async function saveChanges() {
  const addressValid = await addressComponent.value.validateAddress();
  if (addressValid) {
    if (locationBeingUpdate.value.id) {
      updateLocation(locationBeingUpdate.value);
    } else {
      createLocation(locationBeingUpdate.value);
    }
  }
}

function updateLocation(location: any) {
  isLoading.value = true;
  updateAgencyLocation(location.id, location)
    .then(() => {
      isLoading.value = false;
      hideModal();
      location.formattedAddress = getFormattedAddress(location);
    }).catch(error => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function createLocation(location: any) {
  isLoading.value = true;
  createAgencyLocation(location).then(r => {
    location.id = r.id;
    location.formattedAddress = getFormattedAddress(location);
    locations.value.push(location);
    isLoading.value = false;
    hideModal();
  }).catch(error => {
    isLoading.value = false;
    showAlertError(error.data);
  });
}

function getLocations() {
  isLoading.value = true;
  getAgencyLocations().then(r => {
    locations.value = r;
    isLoading.value = false;
  });
}

function deleteLocation(location: any, index: number) {
  showAlertConfirm("Are you sure you want to delete this location?", '', "Yes")
    .then(r => {
      if (!r) return;
      isLoading.value = true;
      deleteAgencyLocation(location.id)
        .then(() => {
          isLoading.value = false;
          locations.value.splice(index, 1);
        }).catch(e => {
          isLoading.value = false;
          showAlertError(e.data);
        });
    });
}

function addLocation() {
  showModal.value = true;
  locationBeingUpdate.value = {};
}

function editLocation(location: any) {
  locationBeingUpdate.value = location;
  showModal.value = true;
}

function hideModal() {
  showModal.value = false;
}

function getFormattedAddress(location: any) {
  if (!location) return "";
  return `${location.address} ${location.city.value} ${location.province.code} ${location.postalCode}`;
}

getLocations();
</script>
