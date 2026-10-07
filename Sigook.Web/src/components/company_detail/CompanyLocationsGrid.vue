<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :data="locations" :refresh="load">
      <template #actions>
        <b-button icon-left="plus" @click="openForm(null)">Add location</b-button>
      </template>
      <b-table-column field="address" label="Address" v-slot="props" searchable :custom-search="matchesLocation">
        {{ props.row.address }}
        <span v-if="props.row.isBilling" class="location-billing">Billing</span>
      </b-table-column>
      <b-table-column field="city" label="City" v-slot="props">
        {{ props.row.city?.value }}
      </b-table-column>
      <b-table-column field="province" label="Province" v-slot="props">
        {{ props.row.city?.province?.code }}
      </b-table-column>
      <b-table-column field="postalCode" label="Postal code" v-slot="props">
        {{ props.row.postalCode }}
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
          <template #trigger>
            <b-button icon-right="dots-vertical" type="is-text" aria-label="Location actions" />
          </template>
          <b-dropdown-item aria-role="listitem" @click="openForm(props.row)">Edit</b-dropdown-item>
          <b-dropdown-item aria-role="listitem" :disabled="!mapsUrl(props.row)" @click="openMap(props.row)">
            Open in maps
          </b-dropdown-item>
          <b-dropdown-item v-if="isAdmin" aria-role="listitem" @click="configureTax(props.row)">
            Configure tax
          </b-dropdown-item>
          <b-dropdown-item aria-role="listitem" class="has-text-danger" @click="onDelete(props.row)">
            Delete
          </b-dropdown-item>
        </b-dropdown>
      </b-table-column>
    </SigookGrid>

    <b-modal custom-content-class="card" v-model="showModal" width="800px">
      <location-form :current-location="currentLocation" :profile-id="profileId" :enable-province-settings="true"
        @updateContent="onSaved" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/utils/toast';
import { deleteAgencyCompanyLocation, getAgencyCompanyLocation } from '@/api/agencyCompanyApi';
import { getLocationTax, upsertLocationTax } from '@/api/locationApi';
import { getDialog } from '@/utils/buefyProgrammatic';
import { formatLocation, mapsUrl, matchesLocation } from '@/utils/requestDetail';
import { useAdmin } from '@/composables/useAdmin';
import type { AgencyCompanyLocationModel } from '@/types/agency';
import SigookGrid from '@/components/SigookGrid.vue';
import LocationForm from '@/components/agency_company/LocationForm.vue';

const props = defineProps<{ profileId: string }>();
const emit = defineEmits<{ (e: 'changed', count: number): void }>();

const { isAdmin } = useAdmin();

const locations = ref<AgencyCompanyLocationModel[]>([]);
const isLoading = ref(false);
const showModal = ref(false);
const currentLocation = ref<AgencyCompanyLocationModel | null>(null);

async function load() {
  isLoading.value = true;
  try {
    locations.value = await getAgencyCompanyLocation(props.profileId);
    emit('changed', locations.value.length);
  } catch (error) {
    showAlertError(error);
  } finally {
    isLoading.value = false;
  }
}

function openForm(location: AgencyCompanyLocationModel | null) {
  currentLocation.value = location;
  showModal.value = true;
}

function openMap(location: AgencyCompanyLocationModel) {
  const url = mapsUrl(location);
  if (url) window.open(url, '_blank', 'noopener');
}

async function onSaved() {
  await load();
  showModal.value = false;
  currentLocation.value = null;
}

function onDelete(location: AgencyCompanyLocationModel) {
  if (!location.id) return;
  const id = location.id;
  showAlertConfirm('Are you sure', 'You want to delete this location')
    .then(async (confirmed) => {
      if (!confirmed) return;
      await deleteAgencyCompanyLocation(props.profileId, id);
      await load();
      showAlertSuccess('Deleted');
    })
    .catch(showAlertError);
}

async function configureTax(location: AgencyCompanyLocationModel) {
  if (!location.id) return;
  const id = location.id;
  let current = 0;
  try {
    const tax = await getLocationTax(id);
    current = tax?.tax1 ?? 0;
  } catch (error) {
    showAlertError(error);
    return;
  }
  getDialog().prompt({
    message: `Tax (%) for ${formatLocation(location)}`,
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
    onConfirm: async (value: string, dialog: { close: () => void }) => {
      const parsed = parseFloat(value);
      if (isNaN(parsed) || parsed < 0 || parsed > 100) {
        showAlertError('Tax must be a number between 0 and 100');
        return;
      }
      try {
        await upsertLocationTax(id, { tax1: parsed });
        showAlertSuccess('Saved');
        dialog.close();
      } catch (error) {
        showAlertError(error);
      }
    },
  });
}

load();
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.location-billing {
  margin-left: 8px;
  padding: 1px 8px;
  border-radius: 999px;
  background: rgba($blue, 0.1);
  font-size: 0.75rem;
  font-weight: 600;
  color: $blue-dark;
}
</style>
