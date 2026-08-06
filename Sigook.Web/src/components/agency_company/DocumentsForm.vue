<template>
  <div class="p-4">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="has-text-centered fz1 mb-4">Document</h2>

    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field :type="fileError ? 'is-danger' : ''" :message="fileError">
          <template #label>
            Document <span class="has-text-danger">*</span>
          </template>
          <div class="file is-primary" :class="{ 'has-name': !!documentFile }">
            <b-upload v-model="documentFile" class="file-label" :accept="UPLOAD_ACCEPT" name="fileCompany"
              @update:modelValue="onFileSelected">
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">Click to upload</span>
              </span>
              <span class="file-name" v-if="documentFile">{{ documentFile.name }}</span>
            </b-upload>
          </div>
        </b-field>
      </div>

      <div class="column is-12">
        <b-field :type="formErrors.description ? 'is-danger' : ''" :message="formErrors.description">
          <template #label>
            Description <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="description" name="Description" autocomplete="nope" />
        </b-field>
      </div>

      <div class="column is-12">
        <b-field label="Document Type">
          <b-select v-model="documentType" placeholder="Select Document Type" expanded>
            <option value="1">Contract</option>
          </b-select>
        </b-field>
      </div>

      <div class="column is-12">
        <b-button type="is-primary" @click="validateForm">
          Save
        </b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { generateFileName } from "@/utils/fileNaming";
import { UPLOAD_ACCEPT, validateUploadFile } from "@/utils/fileValidation";
import { createAgencyCompanyDocument } from "@/api/agencyCompanyApi";
import { useStickyForm } from '@/composables/useStickyForm';

const schema = yup.object({
  description: yup.string().required('Description is required').min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
});

const props = defineProps<{ profileId: string }>();
const emit = defineEmits<{ (e: 'onCreateDocument'): void }>();

const form = useStickyForm<{ description: string }>({
  schema,
  initialValues: { description: '' },
});
const { description } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const documentFile = ref<File | null>(null);
const documentType = ref<string | null>(null);
const fileError = ref('');

function onFileSelected(file: File | null) {
  fileError.value = validateUploadFile(file);
  if (fileError.value) documentFile.value = null;
}

async function validateForm() {
  form.markInteracted();
  const { valid } = await form.validate();
  fileError.value = validateUploadFile(documentFile.value);
  if (!valid || fileError.value) {
    showAlertError("Please make sure all required fields are filled out correctly");
    return;
  }
  submitDocument();
}

function submitDocument() {
  if (!documentFile.value) return;
  isLoading.value = true;
  const model = {
    fileName: generateFileName('Company', documentFile.value.name),
    description: description.value,
    documentType: documentType.value ?? undefined,
  };
  createAgencyCompanyDocument(props.profileId, model, documentFile.value)
    .then(() => {
      isLoading.value = false;
      emit("onCreateDocument");
      showAlertSuccess("Created");
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
