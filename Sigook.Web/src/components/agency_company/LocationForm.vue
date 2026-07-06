<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title">{{ "Location" }}</h2>

    <div class="container-flex">
      <cvn-address ref="addressComponent" v-model:model="location" :enableProvinceSettings="props.enableProvinceSettings" />
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Latitude">
          <b-input v-model="location.latitude" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Longitude">
          <b-input v-model="location.longitude" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.mainIntersection ? 'is-danger' : ''" :label="'Main Intersection'"
          :message="formErrors.mainIntersection || ''">
          <b-input type="text" v-model="mainIntersection" name="mainIntersection" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.entrance ? 'is-danger' : ''" label="Entrance"
          :message="formErrors.entrance || ''">
          <b-input type="text" v-model="entrance" name="entrance" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field>
          <b-checkbox v-model="location.isBilling">
            {{ 'Use as billing address' }}
          </b-checkbox>
        </b-field>
      </div>
    </div>
    <div class="col-12 mt-5">
      <b-button type="is-primary" @click="validateForm">
        {{ props.currentLocation ? 'Save' : 'Create' }}
      </b-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import CvnAddress from "@/components/Address.vue";
import { createAgencyCompanyLocation, updateAgencyCompanyLocation } from "@/api/agencyCompanyApi";
import { createProfileLocation, updateProfileLocation } from "@/api/companyApi";

const schema = yup.object({
  mainIntersection: yup.string().nullable().transform((v) => (v === '' ? null : v)).max(1000, 'Max 1000 characters'),
  entrance: yup.string().nullable().transform((v) => (v === '' ? null : v)).min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
});

const props = defineProps<{
  currentLocation?: any;
  currentIndex?: any;
  profileId?: string;
  enableProvinceSettings?: boolean;
}>();
const emit = defineEmits<{ (e: 'updateContent'): void }>();

const form = useStickyForm<{ mainIntersection: string; entrance: string }>({
  schema,
  initialValues: {
    mainIntersection: '',
    entrance: '',
  },
});
const { mainIntersection, entrance } = form.fields;
const formErrors = form.errors;

const addressComponent = ref<any>(null);
const isLoading = ref(false);
const location = ref<any>({});

async function validateForm() {
  form.markInteracted();
  const addressValid = await addressComponent.value.validateAddress();
  form.handleSubmit((values) => {
    if (!addressValid) {
      showAlertError('Please make sure all required fields are filled out correctly');
      return;
    }
    location.value.mainIntersection = values.mainIntersection;
    location.value.entrance = values.entrance;
    location.value.latitude = location.value.latitude === '' || location.value.latitude == null
      ? null
      : Number(location.value.latitude);
    location.value.longitude = location.value.longitude === '' || location.value.longitude == null
      ? null
      : Number(location.value.longitude);
    if (location.value.id) {
      updateLocation(location.value.id);
    } else {
      createLocation();
    }
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function createLocation() {
  isLoading.value = true;
  const request = props.profileId
    ? createAgencyCompanyLocation(props.profileId, location.value)
    : createProfileLocation(location.value);
  request
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Created');
      emit('updateContent');
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateLocation(id: any) {
  isLoading.value = true;
  const request = props.profileId
    ? updateAgencyCompanyLocation(props.profileId, id, location.value)
    : updateProfileLocation(id, location.value);
  request
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('Updated');
      emit('updateContent');
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

if (props.currentLocation) {
  location.value = Object.assign({}, props.currentLocation);
  form.hydrate({
    mainIntersection: props.currentLocation.mainIntersection || '',
    entrance: props.currentLocation.entrance || '',
  });
}
</script>
