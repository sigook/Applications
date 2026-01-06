<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message type="is-info" has-icon>
      The user will receive an email to confirm and create a password. If you don't receive an email,
      please check your spam or junk mail folder.
    </b-message>
    <div class="container-flex">
      <div class="col-6">
        <b-field :type="errors.has('name') ? 'is-danger' : ''" label="Name"
          :message="errors.has('name') ? errors.first('name') : ''">
          <b-input type="text" v-model="user.name" :name="'name'" v-validate="'required|max:20|min:2'" />
        </b-field>
      </div>
      <div class="col-6">
        <b-field :type="errors.has('email') ? 'is-danger' : ''" label="Email"
          :message="errors.has('email') ? errors.first('email') : ''">
          <b-input type="email" v-model="user.email" :name="'email'" v-validate="'required|max:50|email|min:6'" />
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateForm">{{ $t('Create') }}</b-button>
      </div>
    </div>
  </div>
</template>


<script>

export default {
  name: "CompanyUsersForm",
  data() {
    return {
      isLoading: false,
      user: {
        name: null,
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
      this.$store.dispatch('agency/createAgencyPersonnel', this.user)
        .then(() => {
          this.isLoading = false;
          this.$emit("updateUsers")
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    }
  }
}
</script>