<template>
  <div class="p-4">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="has-text-centered fz1 mb-4">{{ currentLocation ? 'Edit location' : 'New location' }}</h2>

    <div class="columns is-multiline">
      <div class="column is-12">
        <cvn-address ref="addressComponent" v-model:model="location"
          :enableProvinceSettings="enableProvinceSettings" />
      </div>
      <div class="column is-6">
        <b-field label="Latitude">
          <b-input v-model="location.latitude" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field label="Longitude">
          <b-input v-model="location.longitude" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field :type="formErrors.mainIntersection ? 'is-danger' : ''" :label="'Main Intersection'"
          :message="formErrors.mainIntersection || ''">
          <b-input type="text" v-model="mainIntersection" name="mainIntersection" />
        </b-field>
      </div>
      <div class="column is-6">
        <b-field :type="formErrors.entrance ? 'is-danger' : ''" label="Entrance"
          :message="formErrors.entrance || ''">
          <b-input type="text" v-model="entrance" name="entrance" />
        </b-field>
      </div>
      <div class="column is-12">
        <b-field>
          <b-checkbox v-model="location.isBilling">
            {{ 'Use as billing address' }}
          </b-checkbox>
        </b-field>
      </div>
    </div>
    <div class="mt-5">
      <b-button type="is-primary" @click="validateForm">
        {{ currentLocation ? 'Save' : 'Create' }}
      </b-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/shared/utils/toast";
import CvnAddress from "@/shared/ui/Address.vue";
import type { LocationDetailModel } from '@/shared/types/common';

const schema = yup.object({
  mainIntersection: yup.string().nullable().transform((v) => (v === '' ? null : v)).max(1000, 'Max 1000 characters'),
  entrance: yup.string().nullable().transform((v) => (v === '' ? null : v)).min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
});

const { currentLocation, enableProvinceSettings, save } = defineProps<{
  currentLocation?: LocationDetailModel | null;
  currentIndex?: number;
  save: (location: LocationDetailModel, id?: string) => Promise<unknown>;
  enableProvinceSettings?: boolean;
}>();
const emit = defineEmits<(e: 'updateContent') => void>();

const form = useStickyForm<{ mainIntersection: string; entrance: string }>({
  schema,
  initialValues: {
    mainIntersection: '',
    entrance: '',
  },
});
const { mainIntersection, entrance } = form.fields;
const formErrors = form.errors;

const addressComponent = useTemplateRef<InstanceType<typeof CvnAddress>>('addressComponent');
const isLoading = ref(false);
const location = ref<LocationDetailModel>({});

async function validateForm() {
  form.markInteracted();
  const addressValid = await addressComponent.value?.validateAddress() ?? false;
  await form.handleSubmit(async (values) => {
    if (!addressValid) {
      showAlertError('Please make sure all required fields are filled out correctly');
      return;
    }
    location.value.mainIntersection = values.mainIntersection;
    location.value.entrance = values.entrance;
    const { latitude, longitude } = location.value;
    location.value.latitude = latitude == null || String(latitude) === '' ? null : Number(latitude);
    location.value.longitude = longitude == null || String(longitude) === '' ? null : Number(longitude);
    if (location.value.id) {
      await updateLocation(location.value.id);
    } else {
      await createLocation();
    }
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

async function createLocation() {
  isLoading.value = true;
  try {
    await save(location.value);
    isLoading.value = false;
    showAlertSuccess('Created');
    emit('updateContent');
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

async function updateLocation(id: string) {
  isLoading.value = true;
  try {
    await save(location.value, id);
    isLoading.value = false;
    showAlertSuccess('Updated');
    emit('updateContent');
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

if (currentLocation) {
  location.value = Object.assign({}, currentLocation);
  form.hydrate({
    mainIntersection: currentLocation.mainIntersection || '',
    entrance: currentLocation.entrance || '',
  });
}
</script>
