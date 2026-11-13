<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :data="locations" :refresh="getLocations">
      <template #actions>
        <b-button icon-left="plus" @click="addLocation">Add location</b-button>
      </template>
      <b-table-column field="formattedAddress" label="Address" v-slot="props" searchable
        :custom-search="matchesLocation">
        {{ props.row.formattedAddress || formatLocation(props.row) }}
      </b-table-column>
      <b-table-column field="isBilling" label="Company Use As Billing Address" v-slot="props">
        {{ props.row.isBilling ? 'Yes' : 'No' }}
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <div class="buttons is-flex-wrap-nowrap">
          <b-button type="is-info" outlined rounded icon-right="pencil" title="Edit" aria-label="Edit location"
            @click="editLocation(props.row)" />
          <b-button type="is-danger" outlined rounded icon-right="delete" aria-label="Delete location"
            @click="deleteLocation(props.row.id)" />
        </div>
      </b-table-column>
    </SigookGrid>
    <b-modal custom-content-class="card" v-model="showModal" width="500px">
      <div class="p-4">
        <h2 class="has-text-centered fz1 mb-4">{{ locationBeingUpdate.id ? 'Edit location' : 'New location' }}</h2>
        <AddressComponent ref="addressComponent" v-model:model="locationBeingUpdate"
          :enableProvinceSettings="true"
          @isLoading="(value: boolean) => isLoading = value" />
        <div class="columns is-multiline">
          <div class="column is-12">
            <b-checkbox v-model="locationBeingUpdate.isBilling">{{ 'Use as billing address ?' }}</b-checkbox>
          </div>
          <div class="column is-12">
            <b-button type="is-primary" @click="saveChanges">SAVE</b-button>
          </div>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { showAlertConfirm, showAlertError } from "@/shared/utils/toast";
import AddressComponent from "@/shared/ui/Address.vue";
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import { formatLocation, matchesLocation } from '@/shared/request-detail/requestDetail';
import type { CompanyProfileLocationDetail } from '@/shared/company-profile/types';
import { getProfileLocations, createProfileLocation, updateProfileLocation, deleteProfileLocation } from '@/modules/company/profile/api';

const emit = defineEmits<(e: 'loaded', count: number) => void>();

function newLocation(): CompanyProfileLocationDetail {
  return {
    address: '',
    postalCode: '',
    isBilling: false,
    latitude: null,
    longitude: null,
    entrance: '',
    mainIntersection: '',
  };
}

const locations = ref<CompanyProfileLocationDetail[]>([]);
const isLoading = ref(false);
const showModal = ref(false);
const locationBeingUpdate = ref<CompanyProfileLocationDetail>(newLocation());
const addressComponent = useTemplateRef<InstanceType<typeof AddressComponent>>('addressComponent');

function addLocation() {
  locationBeingUpdate.value = newLocation();
  showModal.value = true;
}

function editLocation(location: CompanyProfileLocationDetail) {
  locationBeingUpdate.value = location;
  showModal.value = true;
}

function closeModal() {
  showModal.value = false;
  locationBeingUpdate.value = newLocation();
}

async function deleteLocation(id: string) {
  if (!(await showAlertConfirm('Are you sure', 'You want to delete this location', 'Yes'))) return;
  isLoading.value = true;
  try {
    await deleteProfileLocation(id);
    isLoading.value = false;
    await getLocations();
  } catch (error) {
    isLoading.value = false;
    showAlertError((error as { data?: unknown }).data);
  }
}

async function saveChanges() {
  const addressValid = await addressComponent.value?.validateAddress();
  if (addressValid) {
    if (locationBeingUpdate.value.id) {
      await updateLocation(locationBeingUpdate.value);
    } else {
      await createLocation(locationBeingUpdate.value);
    }
  }
}

async function updateLocation(location: CompanyProfileLocationDetail) {
  if (!location.id) return;
  isLoading.value = true;
  try {
    await updateProfileLocation(location.id, location);
    isLoading.value = false;
    closeModal();
    await getLocations();
  } catch (error) {
    isLoading.value = false;
    showAlertError((error as { data?: unknown }).data);
  }
}

async function createLocation(location: CompanyProfileLocationDetail) {
  isLoading.value = true;
  try {
    await createProfileLocation(location);
    isLoading.value = false;
    closeModal();
    await getLocations();
  } catch (error) {
    isLoading.value = false;
    showAlertError((error as { data?: unknown }).data);
  }
}

async function getLocations() {
  isLoading.value = true;
  try {
    const response = await getProfileLocations();
    locations.value = response;
    emit('loaded', response.length);
  } catch (error) {
    showAlertError((error as { data?: unknown }).data);
  } finally {
    isLoading.value = false;
  }
}

void getLocations();
</script>
