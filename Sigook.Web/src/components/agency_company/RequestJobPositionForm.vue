<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title"> Role </h2>
    <div class="container-flex">
      <div class="col-12">
        <b-field label="Position" :type="formErrors.jobPosition ? 'is-danger' : ''"
          :message="formErrors.jobPosition">
          <b-input placeholder="Position" v-model="jobPosition" name="positions" />
        </b-field>
      </div>
      <div class="col-12">
        <b-field :type="formErrors.message ? 'is-danger' : ''" label="Message"
          :message="formErrors.message">
          <b-input type="textarea" v-model="message" name="message" />
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateForm">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { showAlertError } from "@/utils/toast";
import { petitionAgencyCompanyJobPosition } from "@/api/agencyCompanyApi";
import { useStickyForm } from '@/composables/useStickyForm';

const schema = yup.object({
  jobPosition: yup.string().required('Position is required'),
  message: yup.string().max(1000, 'Max 1000 characters').nullable().transform(v => v || null),
});

const props = defineProps<{ profileId: any }>();
const emit = defineEmits<{ (e: 'closeModal'): void }>();

const form = useStickyForm<{ jobPosition: string; message: string }>({
  schema,
  initialValues: { jobPosition: '', message: '' },
});
const { jobPosition, message } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);

async function validateForm() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) {
    showAlertError('Please make sure all required fields are filled out correctly');
    return;
  }
  requestAgencyJobPosition();
}

function requestAgencyJobPosition() {
  isLoading.value = true;
  const model = {
    id: null,
    jobPosition: jobPosition.value,
    message: message.value,
  };
  petitionAgencyCompanyJobPosition(props.profileId, model)
    .then(() => {
      isLoading.value = false;
      emit('closeModal');
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

</script>
