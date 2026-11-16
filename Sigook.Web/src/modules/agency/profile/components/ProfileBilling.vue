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
import { ref, useTemplateRef } from 'vue';
import { showAlertConfirm, showAlertError } from "@/shared/utils/toast";
import { getAgencyLocations, createAgencyLocation, updateAgencyLocation, deleteAgencyLocation } from '@/modules/agency/profile/api';
import AddressComponent from "@/shared/ui/Address.vue";
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import type { AgencyDetail, AgencyLocationDetail } from '@/modules/agency/profile/types';

const { agencyData } = defineProps<{ agencyData: AgencyDetail }>();
const emit = defineEmits<(e: 'changed', locations: AgencyLocationDetail[]) => void>();

function newLocation(): AgencyLocationDetail {
  return { address: '', city: null, postalCode: '', isBilling: false };
}

const isLoading = ref(true);
const locations = ref<AgencyLocationDetail[]>([]);
const showModal = ref(false);
const locationBeingUpdate = ref<AgencyLocationDetail>(newLocation());
const addressComponent = useTemplateRef<InstanceType<typeof AddressComponent>>('addressComponent');

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

async function updateLocation(location: AgencyLocationDetail) {
  if (!location.id) return;
  isLoading.value = true;
  try {
    await updateAgencyLocation(agencyData.id, location.id, location);
    isLoading.value = false;
    hideModal();
    location.formattedAddress = getFormattedAddress(location);
    emit('changed', locations.value);
  } catch (error) {
    isLoading.value = false;
    showAlertError((error as { data?: unknown }).data);
  }
}

async function createLocation(location: AgencyLocationDetail) {
  isLoading.value = true;
  try {
    const r = await createAgencyLocation(agencyData.id, location);
    location.id = r.id;
    location.formattedAddress = getFormattedAddress(location);
    locations.value.push(location);
    emit('changed', locations.value);
    isLoading.value = false;
    hideModal();
  } catch (error) {
    isLoading.value = false;
    showAlertError((error as { data?: unknown }).data);
  }
}

async function getLocations() {
  isLoading.value = true;
  locations.value = await getAgencyLocations(agencyData.id);
  emit('changed', locations.value);
  isLoading.value = false;
}

async function deleteLocation(location: AgencyLocationDetail) {
  const r = await showAlertConfirm('Are you sure', 'You want to delete this location', 'Yes');
  if (!r || !location.id) return;
  isLoading.value = true;
  try {
    await deleteAgencyLocation(agencyData.id, location.id);
    isLoading.value = false;
    locations.value = locations.value.filter(l => l.id !== location.id);
    emit('changed', locations.value);
  } catch (e) {
    isLoading.value = false;
    showAlertError((e as { data?: unknown }).data);
  }
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
  return `${location.address} ${location.city?.value ?? ''} ${location.province?.code ?? ''} ${location.postalCode}`;
}

void getLocations();
</script>
