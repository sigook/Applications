<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Email" :type="formErrors.email ? 'is-danger' : ''"
          :message="formErrors.email">
          <b-input v-model="email" name="email" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Confirm Email" :type="formErrors.confirmNewEmail ? 'is-danger' : ''"
          :message="formErrors.confirmNewEmail">
          <b-input v-model="confirmNewEmail" name="confirmNewEmail" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-button type="is-primary" @click="onChangeEmail">Save</b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { changeEmail, getEmail } from '@/api/accountApi';
import { useStickyForm } from '@/composables/useStickyForm';

const schema = yup.object({
  email: yup.string().required('Email is required').email('Invalid email'),
  confirmNewEmail: yup.string().required('Confirm Email is required').email('Invalid email')
    .oneOf([yup.ref('email')], 'Emails must match'),
});

const form = useStickyForm<{ email: string; confirmNewEmail: string }>({
  schema,
  initialValues: { email: '', confirmNewEmail: '' },
});
const { email, confirmNewEmail } = form.fields;
const formErrors = form.errors;

const isLoading = ref(true);

async function onChangeEmail() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) return;
  isLoading.value = true;
  changeEmail({ newEmail: email.value, confirmNewEmail: confirmNewEmail.value })
    .then(() => {
      isLoading.value = false;
      showAlertSuccess("Updated");
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

getEmail()
  .then(response => {
    form.setFieldValue('email', response.email);
    isLoading.value = false;
  })
  .catch(error => {
    showAlertError(error);
    isLoading.value = false;
  });
</script>
