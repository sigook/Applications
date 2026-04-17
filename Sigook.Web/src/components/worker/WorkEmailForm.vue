<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <b-message type="is-warning" has-icon>
      Please note that if you change this email the worker will
      not be able to login with the previous email anymore,
      if necessary notify the worker about the change.
    </b-message>
    <div class="container-flex">
      <div class="col-12">
        <b-field label="New Email" :type="formErrors.newEmail ? 'is-danger' : ''"
          :message="formErrors.newEmail || ''">
          <b-input type="email" v-model="newEmail" name="newEmail">
          </b-input>
        </b-field>
      </div>
      <div class="col-12">
        <b-field label="Confirm Email" :type="formErrors.confirmEmail ? 'is-danger' : ''"
          :message="formErrors.confirmEmail || ''">
          <b-input type="email" @paste.prevent v-model="confirmEmail" name="confirmEmail">
          </b-input>
        </b-field>
      </div>
      <div class="col-12 mt-5">
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
import { updateAgencyWorkerEmail } from "@/api/agencyWorkerApi";

const schema = yup.object({
  newEmail: yup.string()
    .required('Email is required')
    .email('Invalid email'),
  confirmEmail: yup.string()
    .required('Confirm Email is required')
    .email('Invalid email')
    .oneOf([yup.ref('newEmail')], 'Emails must match'),
});

export default {
  props: ['data'],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        newEmail: '',
        confirmEmail: '',
      },
    });

    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrateForm: form.hydrate,
      resetForm: form.resetAll,
    };
  },
  data() {
    return {
      isLoading: false,
      worker: {} as any,
    };
  },
  methods: {
    validateAll() {
      this.markInteracted();
      this.handleSubmit((values: any) => {
        this.updateWorkerEmail(values);
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    updateWorkerEmail(values: any) {
      this.isLoading = true;
      updateAgencyWorkerEmail(this.worker.id, { newEmail: values.newEmail })
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
  },
  created() {
    if ((this as any).data != null) {
      this.worker = Object.assign({}, (this as any).data);
    }
  },
};
</script>
