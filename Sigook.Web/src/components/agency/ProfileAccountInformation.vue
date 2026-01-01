<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Email" :type="errors.has('email') ? 'is-danger' : ''"
          :message="errors.has('email') ? errors.first('email') : ''">
          <b-input v-model="userEmail" v-validate="'required|email'" name="email" ref="email" data-vv-as="Email" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Confirm Email" :type="errors.has('confirmNewEmail') ? 'is-danger' : ''"
          :message="errors.has('confirmNewEmail') ? errors.first('confirmNewEmail') : ''">
          <b-input v-model="confirmNewEmail" name="confirmNewEmail" v-validate="'required|email|confirmed:email'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-button type="is-primary" @click="changeEmail">Save</b-button>
      </div>
    </div>
  </div>
</template>

<script>

export default {
  data() {
    return {
      userEmail: null,
      confirmNewEmail: '',
      isLoading: true
    }
  },
  methods: {
    changeEmail() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.$store.dispatch("changeEmail", { newEmail: this.userEmail, confirmNewEmail: this.confirmNewEmail })
            .then(() => {
              this.isLoading = false;
              this.showAlertSuccess("Updated");
            })
            .catch(error => {
              this.isLoading = false;
              this.showAlertError(error);
            })
        }
      })
    }
  },
  created() {
    this.$store.dispatch('getEmail')
      .then(response => {
        this.userEmail = response.email;
        this.isLoading = false;
      })
      .catch(error => {
        this.showAlertError(error);
        this.isLoading = false;
      })

  }
}
</script>