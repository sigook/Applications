<template>
  <div class="worker-account-security">
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
          <b-button type="is-primary" @click="onChangeEmail">Save</b-button>
        </div>
      </div>
    </section>

    <!-- Notifications Section -->
    <section v-if="notifications">
      <h3 class="section-title">Notifications</h3>
      <div v-for="item in notifications" :key="'notification' + item.id">
        <h4 class="notification-title">{{ item.title }}</h4>
        <div class="container-flex">
          <div class="col-sm-12 col-md-8 col-lg-8 col-padding">
            <span>{{ item.description }}</span>
          </div>
          <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
            <b-switch v-model="item.emailNotification" @input="saveNotification(item)">
              {{ item.emailNotification ? 'Yes' : 'No' }}
            </b-switch>
          </div>
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

<script lang="ts">
import { mapStores } from 'pinia';
import { useSecurityStore } from '@/stores/security';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { changeEmail, getEmail, deactivateAccount } from '@/api/accountApi';
import { getUserNotifications, updateUserNotification } from '@/api/userNotificationApi';

export default {
  data() {
    return {
      userEmail: null,
      confirmNewEmail: '',
      isLoading: true,
      notifications: null
    }
  },
  computed: {
    ...mapStores(useSecurityStore),
  },
  methods: {
    onChangeEmail() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.isLoading = true;
          changeEmail({ newEmail: this.userEmail, confirmNewEmail: this.confirmNewEmail })
            .then(() => {
              this.isLoading = false;
              showAlertSuccess("Updated");
            })
            .catch(error => {
              this.isLoading = false;
              showAlertError(error);
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
          this.onDeactivateAccount();
        }
      });
    },
    loadNotifications() {
      getUserNotifications()
        .then(response => {
          this.notifications = response;
        })
        .catch(error => {
          showAlertError(error);
        })
    },
    saveNotification(item) {
      this.isLoading = true;
      updateUserNotification(item)
        .then(() => {
          this.isLoading = false;
        })
        .catch(error => {
          showAlertError(error);
          this.isLoading = false;
        })
    },
    onDeactivateAccount() {
      this.isLoading = true;
      deactivateAccount()
        .then(() => {
          this.isLoading = false;
          showAlertSuccess("Your account has been deactivated. You will be signed out shortly.");
          setTimeout(() => {
            this.securityStore.signOut();
          }, 2000);
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    }
  },
  created() {
    getEmail()
      .then(response => {
        this.userEmail = response.email;
        this.isLoading = false;
      })
      .catch(error => {
        showAlertError(error);
        this.isLoading = false;
      });
    this.loadNotifications();
  }
}
</script>

<style lang="scss">
.worker-account-security {
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

    li {
      color: #666;
      margin-bottom: 8px;
    }
  }

  .notification-title {
    font-weight: 600;
    font-size: 15px;
    margin: 15px 0 5px;
  }
}
</style>
