<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field :label="'Vaccination Required'">
          <b-switch v-model="required" :value="true" :false-value="false">
            {{ required ? 'Yes' : 'No' }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
        <b-field label="Comments" :message="formErrors.comments || ''"
          :type="formErrors.comments ? 'is-danger' : ''">
          <b-input type="textarea" v-model="comments" name="vaccinationComments">
          </b-input>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-12 col-lg-12 col-padding text-right">
        <b-button type="is-primary" @click="saveVaccinationRequired">
          {{ 'Save' }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { updateCompanyVaccinationRequired } from "@/api/agencyCompanyApi";

const schema = yup.object({
  comments: yup.string().nullable().transform((v) => (v === '' ? null : v)).max(5000, 'Max 5000 characters'),
});

export default {
  name: "EditVaccinationRequired",
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        required: false,
        comments: '' as string | null,
      },
    });
    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrateForm: form.hydrate,
    };
  },
  data() {
    return {
      isLoading: false,
    };
  },
  props: ["companyProfileId", "vaccinationRequired", "vaccinationComments"],
  methods: {
    saveVaccinationRequired() {
      this.markInteracted();
      this.handleSubmit((values) => {
        this.isLoading = true;
        updateCompanyVaccinationRequired(this.companyProfileId, {
          vaccinationRequired: this.required,
          vaccinationRequiredComments: values.comments
        }).then(() => {
          this.isLoading = false;
          this.$emit('updated', { required: this.required, comments: values.comments });
        }).catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    }
  },
  created() {
    this.hydrateForm({
      required: !!this.vaccinationRequired,
      comments: this.vaccinationComments || '',
    });
  }
}
</script>
