<template>
  <div class="p-3">
    <h2 class="text-center main-title">{{ $t('CompanyContactInformation') }}</h2>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input ref="phoneComponent" :required="true" model="Phone" :defaultValue="model.phone"
          @formattedPhone="(phone) => model.phone = phone" />
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('phoneExt') ? 'is-danger' : ''" label="Phone Ext"
          :message="errors.has('phoneExt') ? errors.first('phoneExt') : ''">
          <b-input type="text" v-model="model.phoneExt" name="phoneExt" v-validate="'max:8|min:1|numeric'" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input ref="faxComponent" :required="false" model="Fax" :defaultValue="model.fax"
          @formattedPhone="(phone) => model.fax = phone" />
      </div>

      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('faxExt') ? 'is-danger' : ''" label="Fax Ext"
          :message="errors.has('faxExt') ? errors.first('faxExt') : ''">
          <b-input type="text" v-model="model.faxExt" name="faxExt" v-validate="'max:8|min:1|numeric'" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field :type="errors.has('website') ? 'is-danger' : ''" label="Website"
          :message="errors.has('website') ? errors.first('website') : ''">
          <b-input type="text" v-model="model.website" name="website" v-validate="'required|max:50|url'"
            placeholder="www.example.com" />
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-button type="is-primary" @click="validateForm">
          {{ $t('Save') }}
        </b-button>
      </div>
    </div>
  </div>

</template>

<script>
export default {
  name: 'ContactInformationForm',
  props: ["model"],
  methods: {
    async validateForm() {
      const mainFormValid = await this.$validator.validateAll();
      const phoneValid = await this.$refs.phoneComponent.validatePhone();
      const faxValid = await this.$refs.faxComponent.validatePhone();
      if (mainFormValid && phoneValid && faxValid) {
          this.updateAgencyCompanyContactInformation();
          return;
      } else {
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      }
    },
    updateAgencyCompanyContactInformation() {
      this.$store.dispatch('agency/updateAgencyCompanyContactInformation', { profileId: this.model.id, model: this.model })
        .then(() => {
          this.$emit('save');
          this.showAlertSuccess("Updated")
        })
        .catch(error => {
          this.showAlertError(error)
        })
    },
  },
  components: {
    phoneInput: () => import("@/components/PhoneInput")
  }
}
</script>