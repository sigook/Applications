<template>
  <div class="p-4">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="has-text-centered fz1 mb-4">Add document</h2>

    <form @submit.prevent="submit">
      <div class="columns is-multiline">
        <div class="column is-12">
          <b-field :type="fileError ? 'is-danger' : ''" :message="fileError">
            <template #label>
              Document <span class="has-text-danger">*</span>
            </template>
            <div class="file is-primary" :class="{ 'has-name': !!documentFile }">
              <b-upload v-model="documentFile" class="file-label" :accept="UPLOAD_ACCEPT" name="fileCandidate"
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
          <b-field :type="errors.description ? 'is-danger' : ''" :message="errors.description">
            <template #label>
              Description <span class="has-text-danger">*</span>
            </template>
            <b-input v-model="description" name="documentDescription" placeholder="Description" />
          </b-field>
        </div>

        <div class="column is-12">
          <b-button @click="emit('close')">Cancel</b-button>
          <b-button type="is-primary" native-type="submit" class="ml-2">Add</b-button>
        </div>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { generateFileName } from '@/utils/fileNaming';
import { UPLOAD_ACCEPT, validateUploadFile } from '@/utils/fileValidation';
import { addCandidateDocument } from '@/api/agencyCandidateApi';

const props = defineProps<{ candidateId: string }>();
const emit = defineEmits<{ (e: 'created'): void; (e: 'close'): void }>();

const schema = yup.object({
  description: yup.string().required('Description is required').max(40),
});

const form = useStickyForm<{ description: string }>({
  schema,
  initialValues: { description: '' },
});
const { description } = form.fields;
const errors = form.errors;

const isLoading = ref(false);
const documentFile = ref<File | null>(null);
const fileError = ref('');

function onFileSelected(file: File | null) {
  fileError.value = validateUploadFile(file);
  if (fileError.value) documentFile.value = null;
}

function submit() {
  form.markInteracted();
  fileError.value = validateUploadFile(documentFile.value);
  form.handleSubmit(
    values => {
      if (fileError.value || !documentFile.value) {
        showAlertError('Please make sure all required fields are filled out correctly');
        return;
      }
      isLoading.value = true;
      addCandidateDocument(props.candidateId, {
        fileName: generateFileName('Candidate', documentFile.value.name),
        description: values.description,
      }, documentFile.value)
        .then(() => {
          showAlertSuccess('Document added');
          emit('created');
          emit('close');
        })
        .catch(error => {
          isLoading.value = false;
          showAlertError(error);
        });
    },
    () => showAlertError('Please make sure all required fields are filled out correctly'),
  )();
}
</script>
