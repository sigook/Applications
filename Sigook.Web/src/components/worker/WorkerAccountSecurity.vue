<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>

    <!-- Change Email Section -->
    <section>
      <h3 class="section-title">Change Email</h3>
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
            <b-input v-model="confirmNewEmail" name="confirmNewEmail"
              v-validate="'required|email|confirmed:email'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-button type="is-primary" @click="changeEmail">Save</b-button>
        </div>
      </div>
    </section>

    <!-- Account Deactivation Section -->
    <section class="account-deactivation">
      <h3 class="section-title">Deactivate Account</h3>
      <p class="deactivation-description">
        Deactivating your account will prevent you from accessing the platform.
        Your data will be retained as required by law, but you will no longer be able to sign in or apply to jobs.
      </p>
      <ul class="deactivation-consequences">
        <li>You will be signed out immediately</li>
        <li>You will no longer be able to sign in to your account</li>
        <li>You will not be able to apply to new job requests</li>
      </ul>
      <b-button type="is-danger" outlined @click="confirmDeactivation">
        Deactivate My Account
      </b-button>
    </section>
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
          this.isLoading = true;
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
    },
    confirmDeactivation() {
      this.$buefy.dialog.confirm({
        title: 'Are you sure?',
        message: 'This action will deactivate your account. You will be signed out and will no longer be able to access the platform. Do you want to proceed?',
        confirmText: 'Yes, Deactivate',
        cancelText: 'Cancel',
        type: 'is-danger',
        hasIcon: true,
        onConfirm: () => {
          this.deactivateAccount();
        }
      });
    },
    deactivateAccount() {
      this.isLoading = true;
      this.$store.dispatch('deactivateAccount')
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess("Your account has been deactivated. You will be signed out shortly.");
          setTimeout(() => {
            this.$store.dispatch('signOut');
          }, 2000);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
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

<style scoped>
.account-deactivation {
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid #ddd;
}

.deactivation-description {
  color: #666;
  margin-bottom: 15px;
}

.deactivation-consequences {
  margin-bottom: 20px;
  padding-left: 20px;
}

.deactivation-consequences li {
  color: #666;
  margin-bottom: 8px;
}
</style>
