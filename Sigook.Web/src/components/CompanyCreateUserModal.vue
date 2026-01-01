<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message type="is-info" has-icon>
      The user will receive an email to confirm and create a password. If you don't receive an email,
      please check your spam or junk mail folder.
    </b-message>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('name') ? 'is-danger' : ''" label="Name"
          :message="errors.has('name') ? errors.first('name') : ''">
          <b-input type="text" v-model="user.name" :name="'name'" v-validate="'required|max:20|min:2'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('lastname') ? 'is-danger' : ''" label="Last Name"
          :message="errors.has('lastname') ? errors.first('lastname') : ''">
          <b-input type="text" v-model="user.lastname" :name="'lastname'" v-validate="'required|max:20|min:2'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('position') ? 'is-danger' : ''" label="Position"
          :message="errors.has('position') ? errors.first('position') : ''">
          <b-input type="text" v-model="user.position" :name="'position'" v-validate="'max:100|min:2'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <phone-input :required="false" :model="'Mobile'"
          @formattedPhone="(phone) => user.mobileNumber = phone"></phone-input>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('email') ? 'is-danger' : ''" label="Email"
          :message="errors.has('email') ? errors.first('email') : ''">
          <b-input type="email" v-model="user.email" :name="'email'"
            v-validate="'required|max:50|email|min:6'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding mt-5">
        <b-button type="is-primary" @click="validateForm">{{ $t('Create') }}</b-button>
      </div>
    </div>
  </div>
</template>


<script>

export default {
  props: ['companyId'],
  components: {
    PhoneInput: () => import("@/components/PhoneInput")
  },
  data() {
    return {
      isLoading: false,
      user: {
        name: null,
        lastname: null,
        mobileNumber: null,
        position: null,
        email: null
      }
    }
  },
  methods: {
    validateForm() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.createUser()
          return;
        }
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    createUser() {
      this.isLoading = true;
      const action = this.companyId ?
        this.$store.dispatch('agency/createCompanyProfileUser', { user: this.user, companyId: this.companyId }) :
        this.$store.dispatch('company/createCompanyUser', this.user);
      action
        .then(() => {
          this.isLoading = false;
          this.$emit("updateUsers")
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        })
    }
  }
}
</script>