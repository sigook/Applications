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
        <span v-if="props.row.isBilling" class="billing-address">{{ 'Billing Address' }}</span>
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
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertConfirm, showAlertError, showAlertSuccess } from "@/utils/toast";
import { getAgencyCompanyLocation, deleteAgencyCompanyLocation } from "@/api/agencyCompanyApi";
import LocationForm from "./LocationForm.vue";

const route = useRoute();

const pageIndex = ref(1);
const pageSize = 8;
const profileId = route.params.id;
const data = ref<any[]>([]);
const showModal = ref(false);
const currentLocation = ref<any>(null);
const isLoading = ref(false);

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

function onDeleteLocation(id: any, index: number) {
  showAlertConfirm("Are you sure", "You want to delete this location")
    .then((response) => {
      if (response) {
        isLoading.value = true;
        deleteAgencyCompanyLocation(profileId, id)
          .then(() => {
            isLoading.value = false;
            showAlertSuccess('Deleted');
            data.value.splice(index, 1);
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
