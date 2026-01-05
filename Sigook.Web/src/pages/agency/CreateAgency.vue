<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3">Create Agency</h2>
    </div>
    <form @submit.prevent="validateForm">
      <div class="container-flex">
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field label="Full Name" :type="errors.has('full name') ? 'is-danger' : ''"
            :message="errors.has('full name') ? errors.first('full name') : ''">
            <b-input type="text" v-model="agency.fullName" name="full name" v-validate="'required|max:100|min:2'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field :type="errors.has('email') ? 'is-danger' : ''" label="Email"
            :message="errors.has('email') ? errors.first('email') : ''">
            <b-input type="email" v-model="agency.email" name="email" v-validate="'required|email|max:50|min:6'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <phone-input ref="phoneComponent" :required="true" model="Phone" :defaultValue="phoneNumber"
            @formattedPhone="(phone) => phoneNumber = phone"></phone-input>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field :type="errors.has('agency type') ? 'is-danger' : ''" label="Agency Type"
            :message="errors.has('agency type') ? errors.first('agency type') : ''">
            <b-select v-model="agency.agencyType" name="agency type" v-validate="'required'" placeholder="Select agency type" expanded>
              <option v-for="type in agencyTypes" :key="type.value" :value="type.value">
                {{ type.label }}
              </option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field :type="errors.has('password') ? 'is-danger' : ''" label="Password"
            :message="errors.has('password') ? errors.first('password') : ''">
            <b-input type="password" v-model="agency.password" name="password" v-validate="'required|min:6|max:100'"
              password-reveal />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-button type="is-primary" native-type="submit">{{ $t('Create') }}</b-button>
        </div>
      </div>
    </form>
  </div>
</template>
<script>

export default {
  data() {
    return {
      isLoading: false,
      phoneNumber: "",
      agency: {},
      agencyTypes: this.$agencyTypes
    }
  },
  components: {
    phoneInput: () => import("@/components/PhoneInput")
  },
  methods: {
    async validateForm() {
      const mainFormValid = await this.$validator.validateAll();
      const phoneValid = await this.$refs.phoneComponent.validatePhone();
      if (mainFormValid && phoneValid) {
        this.createAgency();
        return;
      } else {
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      }
    },
    createAgency() {
      this.isLoading = true;
      this.agency.phonePrincipal = this.phoneNumber;
      this.$store.dispatch('agency/createAgency', this.agency)
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess("Agency created successfully");
          this.$router.push('/agency-agencies');
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    }
  }
}
</script>
