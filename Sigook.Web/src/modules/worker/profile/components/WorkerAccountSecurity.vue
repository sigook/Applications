<template>
  <div class="worker-account">
    <b-loading v-model="isLoading"></b-loading>

    <profile-card class="worker-card" title="Login email">
      <template #actions>
        <b-button type="is-ghost" size="is-small" class="profile-link" @click="openEmailModal">Change</b-button>
      </template>
      <dl class="profile-fields">
        <div class="profile-field">
          <dt class="profile-field-label">Email you sign in with</dt>
          <dd>{{ currentEmail || '—' }}</dd>
        </div>
      </dl>
    </profile-card>

    <profile-card v-if="notifications" class="worker-card" title="Notifications">
      <ul class="account-notifications">
        <li v-for="item in notifications" :key="'notification' + item.id" class="account-notification">
          <div class="account-notification-text">
            <strong>{{ item.title }}</strong>
            <span>{{ item.description }}</span>
          </div>
          <b-switch v-model="item.emailNotification" @update:modelValue="saveNotification(item)">
            {{ item.emailNotification ? 'Yes' : 'No' }}
          </b-switch>
        </li>
      </ul>
    </profile-card>

    <profile-card class="worker-card" title="Deactivate account">
      <p class="account-text">
        Deactivating your account will prevent you from accessing the platform.
        Your data will be retained as required by law, but you will no longer be able to sign in or apply to jobs.
      </p>
      <ul class="account-consequences">
        <li>You will be signed out immediately</li>
        <li>You will no longer be able to sign in to your account</li>
        <li>You will not be able to apply to new job requests</li>
      </ul>
      <div>
        <b-button type="is-danger" outlined @click="confirmDeactivation">Deactivate my account</b-button>
      </div>
    </profile-card>

    <b-modal custom-content-class="card" v-model="isEmailModalOpen" width="500px">
      <div class="p-4">
        <h2 class="has-text-centered fz1 mb-4">Change login email</h2>
        <div class="columns is-multiline">
          <div class="column is-12">
            <b-field label="New email" :type="formErrors.userEmail ? 'is-danger' : ''" :message="formErrors.userEmail || ''">
              <b-input v-model="userEmail" type="email" name="email" />
            </b-field>
          </div>
          <div class="column is-12">
            <b-field label="Confirm email" :type="formErrors.confirmNewEmail ? 'is-danger' : ''"
              :message="formErrors.confirmNewEmail || ''">
              <b-input v-model="confirmNewEmail" type="email" name="confirmNewEmail" @paste.prevent />
            </b-field>
          </div>
          <div class="column is-12">
            <b-button type="is-primary" @click="onChangeEmail">Save</b-button>
          </div>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useSecurityStore } from '@/app/stores/security';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { changeEmail, getEmail, deactivateAccount } from '@/shared/api/accountApi';
import { getUserNotifications, updateUserNotification } from '@/shared/api/userNotificationApi';
import type { UserNotificationItem } from '@/shared/types/common';
import ProfileCard from '@/shared/worker-profile/cards/ProfileCard.vue';

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
const isEmailModalOpen = ref(false);
const currentEmail = ref('');
const notifications = ref<UserNotificationItem[] | null>(null);

function openEmailModal() {
  form.hydrate({ userEmail: currentEmail.value, confirmNewEmail: '' });
  isEmailModalOpen.value = true;
}

function onChangeEmail() {
  form.markInteracted();
  form.handleSubmit((values) => {
    isLoading.value = true;
    changeEmail({ newEmail: values.userEmail, confirmNewEmail: values.confirmNewEmail })
      .then(() => {
        currentEmail.value = values.userEmail;
        isEmailModalOpen.value = false;
        isLoading.value = false;
        showAlertSuccess('Updated');
      })
      .catch((error) => {
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
      showAlertSuccess('Your account has been deactivated. You will be signed out shortly.');
      setTimeout(() => {
        securityStore.signOut().then(() => window.location.assign('/'));
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
    .then((response) => {
      notifications.value = response;
    })
    .catch((error) => {
      showAlertError(error);
    });
}

function saveNotification(item: UserNotificationItem) {
  isLoading.value = true;
  updateUserNotification(item)
    .then(() => {
      isLoading.value = false;
    })
    .catch((error) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

getEmail()
  .then((response) => {
    currentEmail.value = response.email || '';
    isLoading.value = false;
  })
  .catch((error) => {
    showAlertError(error);
    isLoading.value = false;
  });
loadNotifications();
</script>

<style lang="scss" scoped>
@import '@/assets/scss/worker-profile-layout';
@import '@/assets/scss/worker-profile';

.worker-account {
  display: flex;
  flex-direction: column;
  gap: 20px;
  max-width: 820px;
}

.account-notifications {
  margin: 0;
  padding: 0;
  list-style: none;
}

.account-notification {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
  padding: 12px 0;
  border-top: 1px solid $gray-border;

  &:first-child {
    border-top: 0;
    padding-top: 0;
  }
}

.account-notification-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
  font-size: 0.9rem;
  color: $navy;

  span {
    font-size: 0.85rem;
    color: $grey-font;
  }
}

.account-text {
  margin: 0;
  font-size: 0.9rem;
  line-height: 1.5;
  color: $navy;
}

.account-consequences {
  margin: 0;
  padding-left: 20px;
  list-style: disc;
  font-size: 0.9rem;
  color: $grey-font;

  li {
    margin-bottom: 6px;
  }
}
</style>
