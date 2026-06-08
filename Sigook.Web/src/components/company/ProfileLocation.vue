<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="addLocation">Add</b-button>
    </b-field>
    <b-table sticky-header height="var(--grid-height)" :data="locations" narrowed hoverable paginated pagination-size="is-small" pagination-rounded :per-page="pageSize"
      v-model:current-page="pageIndex">
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
      <AddressComponent ref="addressComponent" v-model:model="locationBeingUpdate"
        :enableProvinceSettings="true"
        @isLoading="(value: boolean) => isLoading = value" />
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
import AddressComponent from "@/components/Address.vue";
import {
  getProfileLocations,
  createProfileLocation,
  updateProfileLocation,
  deleteProfileLocation
} from "@/api/companyApi";

defineProps<{ companyData?: any }>();

const locations = ref<any[]>([]);
const isLoading = ref(false);
const showModal = ref(false);
const locationBeingUpdate = ref<any>({});
const pageSize = ref(30);
const pageIndex = ref(1);
const addressComponent = ref<any>(null);

function addLocation() {
  locationBeingUpdate.value = {};
  showModal.value = true;
}

function editLocation(location: any) {
  locationBeingUpdate.value = location;
  showModal.value = true;
}

function closeModal() {
  showModal.value = false;
  locationBeingUpdate.value = {};
}

function deleteLocation(id: any) {
  showAlertConfirm("Are you sure you want to delete this location?", '', "Yes").then(r => {
    if (!r) return;
    isLoading.value = true;
    deleteProfileLocation(id)
      .then(() => {
        isLoading.value = false;
        getLocations();
      }).catch(e => {
        isLoading.value = false;
        showAlertError(e.data);
      });
  });
}

async function saveChanges() {
  const addressValid = await addressComponent.value?.validateAddress();
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
  updateProfileLocation(location.id, location)
    .then(() => {
      isLoading.value = false;
      closeModal();
      getLocations();
    }).catch((error: any) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function createLocation(location: any) {
  isLoading.value = true;
  createProfileLocation(location)
    .then(() => {
      isLoading.value = false;
      closeModal();
      getLocations();
    }).catch((error: any) => {
      isLoading.value = false;
      showAlertError(error.data);
    });
}

function getLocations() {
  isLoading.value = true;
  getProfileLocations()
    .then((r: any) => {
      locations.value = r;
      isLoading.value = false;
    });
}

function onSearchLocation(row: any, searchTerm: string) {
  const lowerSearchTerm = searchTerm.toLowerCase();
  return (
    row.address.toLowerCase().includes(lowerSearchTerm) ||
    row.city.value.toLowerCase().includes(lowerSearchTerm) ||
    row.city.province.code.toLowerCase().includes(lowerSearchTerm) ||
    row.postalCode.toLowerCase().includes(lowerSearchTerm)
  );
}

getLocations();
</script>
