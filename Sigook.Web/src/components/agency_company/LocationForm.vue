<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title">{{ "Location" }}</h2>

    <div class="container-flex">
      <cvn-address ref="addressComponent" v-model:model="location" :enableProvinceSettings="enableProvinceSettings" />
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
        {{ currentLocation ? 'Save' : 'Create' }}
      </b-button>
    </div>
  </div>
</template>

<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import CvnAddress from "@/components/Address.vue";
import { createProfileLocation } from '@/api/companyApi';
import { createAgencyCompanyLocation, updateAgencyCompanyLocation } from "@/api/agencyCompanyApi";

const schema = yup.object({
  mainIntersection: yup.string().nullable().transform((v) => (v === '' ? null : v)).max(1000, 'Max 1000 characters'),
  entrance: yup.string().nullable().transform((v) => (v === '' ? null : v)).min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
});

export default {
  components: { CvnAddress },
  props: ['currentLocation', 'currentIndex', 'profileId', 'enableProvinceSettings'],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        mainIntersection: '',
        entrance: '',
      },
    });
    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrateForm: form.hydrate,
    };
  },
  data() {
    return {
      isLoading: false,
      provinces: [] as any[],
      cities: [] as any[],
      location: {} as any,
    };
  },
  methods: {
    async validateForm() {
      this.markInteracted();
      const addressValid = await (this.$refs.addressComponent as any).validateAddress();
      this.handleSubmit((values) => {
        if (!addressValid) {
          showAlertError('Please make sure all required fields are filled out correctly');
          return;
        }
        this.location.mainIntersection = values.mainIntersection;
        this.location.entrance = values.entrance;
        if (this.location.id) {
          this.updateLocation(this.location.id);
        } else {
          this.createLocation();
        }
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    createLocation() {
      this.isLoading = true;
      createAgencyCompanyLocation((this as any).profileId, this.location)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess('Created');
          this.$emit('updateContent');
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    createCompanyLocation() {
      this.isLoading = true;
      createProfileLocation(this.location)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess('Created');
          this.$emit('updateContent');
        }).catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    updateLocation(id: any) {
      this.isLoading = true;
      updateAgencyCompanyLocation((this as any).profileId, id, this.location)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess('Updated');
          this.$emit('updateContent');
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
  },
  created() {
    const currentLocation = (this as any).currentLocation;
    if (currentLocation) {
      this.location = Object.assign({}, currentLocation);
      this.hydrateForm({
        mainIntersection: currentLocation.mainIntersection || '',
        entrance: currentLocation.entrance || '',
      });
    }
  },
};
</script>
