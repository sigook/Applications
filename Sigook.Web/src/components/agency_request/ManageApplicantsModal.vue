<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="p-3">
      <div class="container-flex">
        <div class="col-12 col-padding">
          <b-field label="Worker/Candidate" :type="formErrors.applicant ? 'is-danger' : ''"
            :message="formErrors.applicant || 'Type at least 3 characters to search'">
            <b-autocomplete v-model="applicant" :data="applicants" placeholder="Search by name, email or ID..."
              name="applicant" append-to-body :loading="isLoadingList" @typing="onSearchInput" @select="selectApplicant"
              :custom-formatter="(option) => `#${option.numberId} | ${option.name} | ${option.email || 'No Email' } | ${option.type}`">
              <template v-slot="props">
                <div class="d-flex justify-content-between align-items-center">
                  <div>
                    <strong>#{{ props.option.numberId }}</strong>
                    <span class="ms-2">{{ props.option.name }}</span>
                    <small class="ms-2 color-gray-light" v-if="props.option.email">{{ props.option.email }}</small>
                  </div>
                  <span class="tag-sm-gray ms-2">{{ props.option.type }}</span>
                </div>
              </template>
            </b-autocomplete>
          </b-field>
        </div>
        <div class="col-12 mt-5">
          <b-button type="is-primary" @click="saveApplicant">Add Applicant</b-button>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRoute } from 'vue-router';
import * as yup from 'yup';
import { showAlertError } from "@/utils/toast";
import { searchAgencyRequestApplicants } from "@/api/agencyRequestApi";
import { useStickyForm } from '@/composables/useStickyForm';

const emit = defineEmits<{ (e: 'updateApplicants', value: { model: any }): void }>();

const schema = yup.object({
  applicant: yup.string().required('Applicant is required'),
});

const form = useStickyForm<{ applicant: string }>({
  schema,
  initialValues: { applicant: '' },
});
const { applicant } = form.fields;
const formErrors = form.errors;

const route = useRoute();

const isLoading = ref(false);
const isLoadingList = ref(false);
const requestId = route.params.id;
const applicants = ref<any[]>([]);
const model = reactive<any>({
  workerProfileId: null,
  candidateId: null,
  comments: null
});

function onSearchInput(text: string) {
  if (text.length >= 3) {
    searchApplicants(text);
  } else {
    applicants.value = [];
  }
}

function searchApplicants(text: string) {
  isLoadingList.value = true;
  searchAgencyRequestApplicants(requestId, text)
    .then(response => {
      isLoadingList.value = false;
      applicants.value = response;
    })
    .catch(error => {
      isLoadingList.value = false;
      showAlertError(error);
    });
}

function selectApplicant(item: any) {
  if (!item) return;
  model.workerProfileId = item.workerProfileId || null;
  model.candidateId = item.candidateId || null;
}

async function saveApplicant() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) return;
  emit('updateApplicants', { model: { ...model } });
}
</script>
