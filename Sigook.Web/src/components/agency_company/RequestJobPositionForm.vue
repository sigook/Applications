<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title"> Role </h2>
    <div class="container-flex">
      <div class="col-12">
        <b-field label="Position" :type="formErrors.jobPosition ? 'is-danger' : ''"
          :message="formErrors.jobPosition">
          <b-autocomplete :data="filteredPositions" placeholder="Position" v-model="jobPosition" field="value"
            open-on-focus name="positions">
            <template v-slot="props">
              <span class="fz-0">{{ props.option.value }}</span>
              <span v-if="props.option.industry" class="fz-2 d-block">Industry: {{ props.option.industry }}</span>
            </template>
          </b-autocomplete>
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
import { ref, computed } from 'vue';
import * as yup from 'yup';
import { showAlertError } from "@/utils/toast";
import { getJobPositions } from "@/api/catalogApi";
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
const jobPositionList = ref<any[]>([]);

const filteredPositions = computed(() => {
  const q = (jobPosition.value || '').toLowerCase();
  return jobPositionList.value.filter((jpl: any) => jpl.value.toLowerCase().includes(q));
});

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

(async () => {
  isLoading.value = true;
  jobPositionList.value = await getJobPositions();
  isLoading.value = false;
})();
</script>
