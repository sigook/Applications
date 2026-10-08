<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :data="locations" :refresh="getLocations">
      <template #actions>
        <b-button icon-left="plus" @click="addLocation">Add</b-button>
      </template>
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
          <b-button outlined rounded type="is-danger" @click="deleteLocation(props.row)"
            class="mr-2" icon-left="delete" />
        </b-field>
      </b-table-column>
    </SigookGrid>

    <b-modal custom-content-class="card" v-model="showModal" width="500px">
      <address-component ref="addressComponent" v-model:model="locationBeingUpdate" @isLoading="(value) => isLoading = value" />
      <div class="columns is-multiline">
        <div class="column is-12">
          <b-checkbox v-model="locationBeingUpdate.isBilling">{{ 'Use as billing address ?' }}</b-checkbox>
        </div>
        <div class="column is-12">
          <b-button type="is-primary" @click="saveChanges">SAVE</b-button>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertConfirm, showAlertError } from "@/shared/utils/toast";
import { getAgencyLocations, createAgencyLocation, updateAgencyLocation, deleteAgencyLocation } from '@/modules/agency/profile/api';
import AddressComponent from "@/shared/ui/Address.vue";
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import type { AgencyDetail, AgencyLocationDetail } from '@/modules/agency/profile/types';

defineProps<{ agencyData?: AgencyDetail }>();

function newLocation(): AgencyLocationDetail {
  return { address: '', city: null, postalCode: '', isBilling: false };
}

const isLoading = ref(true);
const locations = ref<AgencyLocationDetail[]>([]);
const showModal = ref(false);
const locationBeingUpdate = ref<AgencyLocationDetail>(newLocation());
const addressComponent = ref<InstanceType<typeof AddressComponent> | null>(null);

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

function updateLocation(location: AgencyLocationDetail) {
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

function createLocation(location: AgencyLocationDetail) {
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

function deleteLocation(location: AgencyLocationDetail) {
  showAlertConfirm('Are you sure', 'You want to delete this location', 'Yes')
    .then(r => {
      if (!r) return;
      isLoading.value = true;
      deleteAgencyLocation(location.id)
        .then(() => {
          isLoading.value = false;
          locations.value = locations.value.filter(l => l.id !== location.id);
        }).catch(e => {
          isLoading.value = false;
          showAlertError(e.data);
        });
    });
}

function addLocation() {
  showModal.value = true;
  locationBeingUpdate.value = newLocation();
}

function editLocation(location: AgencyLocationDetail) {
  locationBeingUpdate.value = location;
  showModal.value = true;
}

function hideModal() {
  showModal.value = false;
}

function getFormattedAddress(location: AgencyLocationDetail) {
  if (!location) return "";
  return `${location.address} ${location.city.value} ${location.province.code} ${location.postalCode}`;
}

getLocations();
</script>
