<template>
  <div>
    <div class="container-flex justify-content-between mb-4">
      <h3 class="fw-bold">Location</h3>
      <b-button type="is-primary" size="is-small" outlined rounded @click="showModal = true">Add</b-button>
    </div>
    <div class="location-table-wrap">
    <b-table :data="data" narrowed hoverable paginated pagination-size="is-small" :per-page="pageSize" v-model:current-page="pageIndex"
      pagination-rounded @cellclick="onCellClick">
      <b-table-column field="id" v-slot="props" searchable :custom-search="onSearchLocation">
        <span>
          {{ props.row.address }}
          {{ props.row.city.value }}
          {{ props.row.city.province.code }},
          {{ props.row.postalCode }}
        </span>
        <span v-if="props.row.isBilling" class="billing-address">{{ 'Billing Address' }}</span>
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
          <template #trigger>
            <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
          </template>
          <b-dropdown-item aria-role="listitem" @click="openEditModal(props.row)">
            Edit
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" :disabled="!props.row.latitude || !props.row.longitude"
            @click="openMapModal(props.row)">
            View map
          </b-dropdown-item>
          <b-dropdown-item v-if="isAdmin" aria-role="listitem" @click="configureTax(props.row)">
            Configure tax
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" class="has-text-danger" @click="onDeleteLocation(props.row.id)">
            Delete
          </b-dropdown-item>
        </b-dropdown>
      </b-table-column>
    </b-table>
    </div>

    <b-modal v-model="showModal" width="800px">
      <location-form :current-location="currentLocation" :profile-id="profileId"
        @updateContent="onUpdateModal" :enableProvinceSettings="true"></location-form>
    </b-modal>

    <b-modal v-model="showMapModal" width="600px">
      <div class="card p-4" v-if="mapLocation">
        <h4 class="fw-bold mb-2">{{ locationLabel(mapLocation) }}</h4>
        <p class="fz-1 op5 mb-3">Lat: {{ mapLocation.latitude }} | Lng: {{ mapLocation.longitude }}</p>
        <div class="location-map-modal">
          <iframe :src="mapEmbedSrc(mapLocation)" allowfullscreen width="100%" height="400px" frameborder="0"
            style="border:0; height: 400px;"></iframe>
        </div>
        <p class="mt-3">
          <a :href="mapSearchUrl(mapLocation)" target="_blank" rel="noopener noreferrer">
            Open in Google Maps
          </a>
        </p>
      </div>
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertConfirm, showAlertError, showAlertSuccess } from "@/utils/toast";
import { getAgencyCompanyLocation, deleteAgencyCompanyLocation } from "@/api/agencyCompanyApi";
import { getLocationTax, upsertLocationTax } from "@/api/locationApi";
import { getDialog } from "@/utils/buefyProgrammatic";
import { useAdmin } from "@/composables/useAdmin";
import LocationForm from "./LocationForm.vue";

const route = useRoute();
const { isAdmin } = useAdmin();

const pageIndex = ref(1);
const pageSize = 8;
const profileId = route.params.id as string;
const data = ref<any[]>([]);
const showModal = ref(false);
const currentLocation = ref<any>(null);
const showMapModal = ref(false);
const mapLocation = ref<any>(null);
const isLoading = ref(false);

const MAPS_KEY = 'AIzaSyDj0QAxxsRhSUXsZ-pSKlRh62vsK362xqs';

function onCellClick(row: any, column: any) {
  switch (column.field) {
    case 'id':
      openEditModal(row);
      break;
  }
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

function locationLabel(loc: any) {
  return `${loc.address} ${loc.city.value} ${loc.city.province.code}, ${loc.postalCode}`;
}

function mapEmbedSrc(loc: any) {
  return `https://www.google.com/maps/embed/v1/place?key=${MAPS_KEY}&q=${loc.latitude},${loc.longitude}&zoom=13`;
}

function mapSearchUrl(loc: any) {
  return `https://www.google.com/maps/search/?api=1&query=${loc.latitude},${loc.longitude}`;
}

function openMapModal(loc: any) {
  mapLocation.value = loc;
  showMapModal.value = true;
}

async function configureTax(row: any) {
  let current = 0;
  try {
    const tax = await getLocationTax(row.id);
    current = tax?.tax1 ?? 0;
  } catch (error) {
    showAlertError(error);
    return;
  }
  getDialog().prompt({
    message: `Tax (%) for ${locationLabel(row)}`,
    inputAttrs: {
      type: 'number',
      step: '0.01',
      min: '0',
      max: '100',
      value: String(current),
      placeholder: 'e.g. 10 for 10%',
    },
    closeOnConfirm: false,
    confirmText: 'Save',
    onConfirm: async (value: string, dialog: any) => {
      const parsed = parseFloat(value);
      if (isNaN(parsed) || parsed < 0 || parsed > 100) {
        showAlertError('Tax must be a number between 0 and 100');
        return;
      }
      try {
        await upsertLocationTax(row.id, { tax1: parsed });
        showAlertSuccess('Saved');
        dialog.close();
      } catch (error) {
        showAlertError(error);
      }
    },
  });
}

async function loadCompanyLocations() {
  const response = await getAgencyCompanyLocation(profileId);
  data.value = response.map((d: any) => ({ ...d, actions: null }));
}

async function onUpdateModal() {
  await loadCompanyLocations();
  closeModal();
}

function closeModal() {
  currentLocation.value = null;
  showModal.value = false;
}

function openEditModal(item: any) {
  currentLocation.value = item;
  showModal.value = true;
}

function onDeleteLocation(id: any) {
  showAlertConfirm("Are you sure", "You want to delete this location")
    .then((response) => {
      if (response) {
        isLoading.value = true;
        deleteAgencyCompanyLocation(profileId, id)
          .then(async () => {
            await loadCompanyLocations();
            isLoading.value = false;
            showAlertSuccess('Deleted');
          })
          .catch((error) => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    }).catch((error) => {
      showAlertError(error);
    });
}

(async () => {
  await loadCompanyLocations();
})();
</script>

<style scoped>
/* Own scroll for the location list. Dropped buefy's sticky-header prop on
   purpose so the global `:has(.table-wrapper.has-sticky-header)` cascade
   doesn't stretch every ancestor of this sidebar widget. The thead is made
   sticky by hand here, and overflow-x is clipped to kill the horizontal bar. */
.location-table-wrap {
  max-height: 360px;
  overflow-y: auto;
  overflow-x: hidden;
}

.location-table-wrap :deep(.table) {
  width: 100%;
  min-width: 0;
}

.location-table-wrap :deep(thead th) {
  position: sticky;
  top: 0;
  z-index: 2;
  background: #fff;
}
</style>
