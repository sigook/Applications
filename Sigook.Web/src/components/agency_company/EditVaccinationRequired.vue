<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field :label="$t('VaccinationRequired')">
          <b-switch v-model="model.required" :value="true" :false-value="false">
            {{ model.required ? $t('Yes') : $t('No') }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field label="Comments" :message="errors.first('vaccinationComments')"
          :type="errors.has('vaccinationComments') ? 'is-danger' : ''">
          <b-input type="textarea" v-model="model.comments" name="vaccinationComments" v-validate="'max:5000'">
          </b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding text-right">
        <b-button type="is-primary" @click="updateCompanyVaccinationRequired">
          {{ $t('Save') }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "EditVaccinationRequired",
  data() {
    return {
      model: {},
      isLoading: false
    }
  },
  props: ["companyProfileId", "vaccinationRequired", "vaccinationComments"],
  methods: {
    updateCompanyVaccinationRequired() {
      this.isLoading = true
      this.$validator.validateAll().then(result => {
        if (result) {
          this.$store.dispatch('agency/updateCompanyVaccinationRequired', {
            companyProfileId: this.companyProfileId,
            model: this.model
          }).then(() => {
            this.isLoading = false
            this.$emit('updated', this.model);
          }).catch(error => {
            this.isLoading = false
            this.showAlertError(error)
          });
        }
      })
    },
  },
  created() {
    this.model.required = this.vaccinationRequired;
    this.model.comments = this.vaccinationComments;
  }
}
</script>