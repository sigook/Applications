<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field :label="'Vaccination Required'">
          <b-switch v-model="required" :value="true" :false-value="false">
            {{ required ? 'Yes' : 'No' }}
          </b-switch>
        </b-field>
      </div>
      <div class="column is-12">
        <b-field label="Comments" :message="formErrors.comments || ''"
          :type="formErrors.comments ? 'is-danger' : ''">
          <b-input type="textarea" v-model="comments" name="vaccinationComments">
          </b-input>
        </b-field>
      </div>
      <div class="column is-12 has-text-right">
        <b-button type="is-primary" @click="saveVaccinationRequired">
          {{ 'Save' }}
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
import { updateCompanyVaccinationRequired } from "@/api/agencyCompanyApi";

const schema = yup.object({
  comments: yup.string().nullable().transform((v) => (v === '' ? null : v)).max(5000, 'Max 5000 characters'),
});

const props = defineProps<{
  companyProfileId: any;
  vaccinationRequired?: boolean | null;
  vaccinationComments?: string | null;
}>();
const emit = defineEmits<{ (e: 'updated', value: { required: boolean; comments: string | null }): void }>();

const form = useStickyForm<{ required: boolean; comments: string | null }>({
  schema,
  initialValues: {
    required: false,
    comments: '',
  },
});
const { required, comments } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);

function saveVaccinationRequired() {
  form.markInteracted();
  form.handleSubmit((values) => {
    isLoading.value = true;
    updateCompanyVaccinationRequired(props.companyProfileId, {
      vaccinationRequired: required.value,
      vaccinationRequiredComments: values.comments,
    }).then(() => {
      isLoading.value = false;
      emit('updated', { required: required.value, comments: values.comments });
    }).catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

form.hydrate({
  required: !!props.vaccinationRequired,
  comments: props.vaccinationComments || '',
});
</script>
