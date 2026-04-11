<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title">{{ $t("Location") }}</h2>

    <div class="container-flex">
      <cvn-address :model.sync="location" :enableProvinceSettings="enableProvinceSettings" />
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Latitude" :type="errors.has('latitude') ? 'is-danger' : ''">
          <b-input v-model="location.latitude" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Longitude" :type="errors.has('longitude') ? 'is-danger' : ''">
          <b-input v-model="location.longitude" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('mainIntersection') ? 'is-danger' : ''" :label="$t('MainIntersection')"
          :message="errors.has('mainIntersection') ? errors.first('mainIntersection') : ''">
          <b-input type="text" v-model="location.mainIntersection" name="mainIntersection" v-validate="'max:1000'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('entrance') ? 'is-danger' : ''" label="Entrance"
          :message="errors.has('entrance') ? errors.first('entrance') : ''">
          <b-input type="text" v-model="location.entrance" name="entrance" v-validate="'max:100|min:2'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field>
          <b-checkbox v-model="location.isBilling">
            {{ $t('UseAsBillingAddress') }}
          </b-checkbox>
        </b-field>
      </div>
    </div>
    <div class="col-12 mt-5">
      <b-button type="is-primary" @click="validateForm">
        {{ currentLocation ? $t('Save') : $t('Create') }}
      </b-button>
    </div>
  </div>
</template>

<script lang="ts">
import CvnAddress from "@/components/Address.vue";
import { createProfileLocation } from '@/api/companyApi';
import { createAgencyCompanyLocation, updateAgencyCompanyLocation } from "@/api/agencyCompanyApi";

export default {
  components: { CvnAddress },
  props: ['currentLocation', 'currentIndex', 'profileId', 'enableProvinceSettings'],
  data() {
    return {
      isLoading: false,
      provinces: [],
      cities: [],
      location: {}
    }
  },
  methods: {
    validateForm() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          if (this.location.id) {
            this.updateLocation(this.location.id);
          } else {
            this.createLocation();
          }
          return;
        }
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    createLocation() {
      this.isLoading = true;
      createAgencyCompanyLocation(this.profileId, this.location)
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess('Created');
          this.$emit('updateContent');
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    createCompanyLocation() {
      this.isLoading = true;
      createProfileLocation(this.location)
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess('Created');
          this.$emit('updateContent');
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    updateLocation(id) {
      this.isLoading = true;
      updateAgencyCompanyLocation(this.profileId, id, this.location)
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess('Updated');
          this.$emit('updateContent');
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
  },
  created() {
    if (this.currentLocation) {
      this.location = Object.assign({}, this.currentLocation);
    }
  }
}
</script>