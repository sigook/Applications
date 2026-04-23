<template>
  <div class="worker-account-security">
    <b-loading v-model="isLoading"></b-loading>

    <!-- Change Email Section -->
    <section>
      <h3 class="section-title">Change Email</h3>
      <div class="container-flex">
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field label="Email" :type="formErrors.userEmail ? 'is-danger' : ''"
            :message="formErrors.userEmail || ''">
            <b-input v-model="userEmail" name="email" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field label="Confirm Email" :type="formErrors.confirmNewEmail ? 'is-danger' : ''"
            :message="formErrors.confirmNewEmail || ''">
            <b-input v-model="confirmNewEmail" name="confirmNewEmail" />
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
            <b-switch v-model="item.emailNotification" @update:modelValue="saveNotification(item)">
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

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useSecurityStore } from '@/stores/security';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { getDialog } from '@/utils/buefyProgrammatic';
import { changeEmail, getEmail, deactivateAccount } from '@/api/accountApi';
import { getUserNotifications, updateUserNotification } from '@/api/userNotificationApi';

const schema = yup.object({
  userEmail: yup.string().required('Email is required').email('Invalid email'),
  confirmNewEmail: yup.string().required('Confirm Email is required').email('Invalid email')
    .oneOf([yup.ref('userEmail')], 'Emails must match'),
});

const form = useStickyForm<{ userEmail: string; confirmNewEmail: string }>({
  schema,
  initialValues: {
    userEmail: '',
    confirmNewEmail: '',
  },
});
const { userEmail, confirmNewEmail } = form.fields;
const formErrors = form.errors;

const securityStore = useSecurityStore();

const isLoading = ref(true);
const notifications = ref<any>(null);

function onChangeEmail() {
  form.markInteracted();
  form.handleSubmit((values: any) => {
    isLoading.value = true;
    changeEmail({ newEmail: values.userEmail, confirmNewEmail: values.confirmNewEmail })
      .then(() => {
        isLoading.value = false;
        showAlertSuccess("Updated");
      })
      .catch(error => {
        isLoading.value = false;
        showAlertError(error);
      });
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function onDeactivateAccount() {
  isLoading.value = true;
  deactivateAccount()
    .then(() => {
      isLoading.value = false;
      showAlertSuccess("Your account has been deactivated. You will be signed out shortly.");
      setTimeout(() => {
        securityStore.signOut();
      }, 2000);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function confirmDeactivation() {
  getDialog().confirm({
    title: 'Are you sure?',
    message: 'This action will deactivate your account. You will be signed out and will no longer be able to access the platform. Do you want to proceed?',
    confirmText: 'Yes, Deactivate',
    cancelText: 'Cancel',
    type: 'is-danger',
    hasIcon: true,
    onConfirm: () => {
      onDeactivateAccount();
    },
  });
}

function loadNotifications() {
  getUserNotifications()
    .then(response => {
      notifications.value = response;
    })
    .catch(error => {
      showAlertError(error);
    });
}

function saveNotification(item: any) {
  isLoading.value = true;
  updateUserNotification(item)
    .then(() => {
      isLoading.value = false;
    })
    .catch(error => {
      showAlertError(error);
      isLoading.value = false;
    });
}

getEmail()
  .then(response => {
    form.hydrate({
      userEmail: response.email || '',
      confirmNewEmail: '',
    });
    isLoading.value = false;
  })
  .catch(error => {
    showAlertError(error);
    isLoading.value = false;
  });
loadNotifications();
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
