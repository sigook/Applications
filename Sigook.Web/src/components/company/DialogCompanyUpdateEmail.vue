<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>

    <b-message type="is-warning" has-icon>
      Please note that if you change this email the worker will
      not be able to login with the previous email anymore,
      if necessary notify the worker about the change.
    </b-message>

    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field label="New Email" :type="formErrors.newEmail ? 'is-danger' : ''"
          :message="formErrors.newEmail || ''">
          <b-input type="email" v-model="newEmail" name="newEmail">
          </b-input>
        </b-field>
      </div>

      <div class="column is-12">
        <b-field label="Confirm Email" :type="formErrors.confirmEmail ? 'is-danger' : ''"
          :message="formErrors.confirmEmail || ''">
          <b-input type="email" @paste.prevent v-model="confirmEmail" name="confirmEmail">
          </b-input>
        </b-field>
      </div>

      <div class="column is-12">
        <b-button type="is-primary" @click="validateAll()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { updateAgencyCompanyEmail } from "@/api/agencyCompanyApi";

const props = defineProps<{ companyProfileId: number | string }>();
const emit = defineEmits<{ (e: 'closeModal', changed: boolean, newEmail: string): void }>();

const schema = yup.object({
  newEmail: yup.string().required('Email is required').email('Invalid email'),
  confirmEmail: yup
    .string()
    .required('Confirm email is required')
    .oneOf([yup.ref('newEmail')], 'Emails must match'),
});

const form = useStickyForm<{ newEmail: string; confirmEmail: string }>({
  schema,
  initialValues: { newEmail: '', confirmEmail: '' },
});
const { newEmail, confirmEmail } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);

function validateAll() {
  form.markInteracted();
  form.handleSubmit((values) => {
    updateEmail(values.newEmail);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

function updateEmail(email: string) {
  isLoading.value = true;
  updateAgencyCompanyEmail(String(props.companyProfileId), { newEmail: email })
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true, email);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
