<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-4">
        <b-field :label="'Do you have any health problems / allergies?'" class="has-text-weight-normal">
          <b-switch v-model="worker.haveAnyHealthProblem">
            {{ worker.haveAnyHealthProblem ? 'Yes' : 'No' }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-4" v-if="worker.haveAnyHealthProblem">
        <b-field :label="`${'Which'} ?`" :type="formErrors.healthProblem ? 'is-danger' : ''"
          :message="formErrors.healthProblem || ''">
          <b-input type="text" v-model="healthProblem" name="health problem">
          </b-input>
        </b-field>
      </div>
      <div class="col-4" v-if="worker.haveAnyHealthProblem">
        <b-field :label="'Other allergies'" class="has-text-weight-normal">
          <b-input type="text" v-model="worker.otherHealthProblem">
          </b-input>
        </b-field>
      </div>
      <div class="col-12">
        <h1 class="fw-700">{{ 'In case of emergency notify' }}</h1>
      </div>
      <div class="col-6">
        <b-field :label="'Name'" :type="formErrors.contactEmergencyName ? 'is-danger' : ''"
          :message="formErrors.contactEmergencyName || ''">
          <b-input type="text" v-model="contactEmergencyName" name="contact emergency">
          </b-input>
        </b-field>
      </div>
      <div class="col-6">
        <b-field :label="'Last Name'" class="has-text-weight-normal"
          :type="formErrors.contactEmergencyLastName ? 'is-danger' : ''"
          :message="formErrors.contactEmergencyLastName || ''">
          <b-input type="text" v-model="contactEmergencyLastName" name="contact emergency lastname" expanded>
          </b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <phone-input ref="emergencyPhoneComponent" :required="true" model="Contact Emergency Phone"
          :defaultValue="worker.contactEmergencyPhone"
          @formattedPhone="(phone) => worker.contactEmergencyPhone = phone" />
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
import { defineAsyncComponent } from 'vue';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { createWorkerEmergencyInformation } from '@/api/workerApi';

const schema = yup.object({
  healthProblem: yup.string().nullable().transform(v => v === '' ? null : v)
    .min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  contactEmergencyName: yup.string()
    .required('Name is required')
    .min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  contactEmergencyLastName: yup.string()
    .required('Last Name is required')
    .min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
});

export default {
  props: ['data'],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        healthProblem: '',
        contactEmergencyName: '',
        contactEmergencyLastName: '',
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
      worker: {} as any,
      isLoading: false,
    };
  },
  methods: {
    async validateAll() {
      this.markInteracted();
      const phoneComponent = this.$refs.emergencyPhoneComponent as any;
      let phoneValid = true;
      if (phoneComponent && typeof phoneComponent.validatePhone === 'function') {
        phoneValid = await phoneComponent.validatePhone();
      }
      if (this.worker.haveAnyHealthProblem && !(this as any).healthProblem) {
        showAlertError('Please make sure all required fields are filled out correctly');
        return;
      }
      this.handleSubmit((values: any) => {
        if (!phoneValid) {
          showAlertError('Please make sure all required fields are filled out correctly');
          return;
        }
        this.createWorkerEmergencyInformation(values);
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    createWorkerEmergencyInformation(values: any) {
      this.isLoading = true;
      const payload = {
        ...this.worker,
        healthProblem: this.worker.haveAnyHealthProblem ? values.healthProblem : null,
        contactEmergencyName: values.contactEmergencyName,
        contactEmergencyLastName: values.contactEmergencyLastName,
      };
      createWorkerEmergencyInformation((this as any).data.id, payload)
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
      this.hydrateForm({
        healthProblem: this.worker.healthProblem || '',
        contactEmergencyName: this.worker.contactEmergencyName || '',
        contactEmergencyLastName: this.worker.contactEmergencyLastName || '',
      });
    }
  },
  components: {
    phoneInput: defineAsyncComponent(() => import("../PhoneInput.vue")),
  },
};
</script>
