<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>

    <b-message type="is-warning" has-icon>
      Please note that if you change this email the worker will
      not be able to login with the previous email anymore,
      if necessary notify the worker about the change.
    </b-message>

    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field label="New Email" :type="formErrors.newEmail ? 'is-danger' : ''"
          :message="formErrors.newEmail || ''">
          <b-input type="email" v-model="newEmail" name="newEmail">
          </b-input>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field label="Confirm Email" :type="formErrors.confirmEmail ? 'is-danger' : ''"
          :message="formErrors.confirmEmail || ''">
          <b-input type="email" @paste.prevent v-model="confirmEmail" name="confirmEmail">
          </b-input>
        </b-field>
      </div>

      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-button type="is-primary" @click="validateAll()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { updateAgencyCompanyEmail } from "@/api/agencyCompanyApi";

const schema = yup.object({
  newEmail: yup.string().required('Email is required').email('Invalid email'),
  confirmEmail: yup
    .string()
    .required('Confirm email is required')
    .oneOf([yup.ref('newEmail')], 'Emails must match'),
});

export default {
  name: "DialogCompanyUpdateEmail",
  props: ['companyProfileId'],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: { newEmail: '', confirmEmail: '' },
    });
    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
    };
  },
  data() {
    return {
      isLoading: false,
    }
  },
  methods: {
    validateAll() {
      this.markInteracted();
      this.handleSubmit((values) => {
        this.updateEmail(values.newEmail);
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    updateEmail(newEmail: string) {
      this.isLoading = true;
      updateAgencyCompanyEmail(this.companyProfileId, { newEmail }).then(() => {
        this.isLoading = false;
        this.$emit('closeModal', true, newEmail);
      })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    }
  }
}

</script>
