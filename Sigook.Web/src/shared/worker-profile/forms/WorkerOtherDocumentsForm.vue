<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field>
          <template #label>
            {{ "File" }} <span class="has-text-danger">*</span>
          </template>
          <div v-if="otherDocument && otherDocument.fileName" class="selected-file-display">
            <b-icon icon="file-document" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(otherDocument.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearDocFile()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedDocFile }">
            <b-upload v-model="selectedDocFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @update:modelValue="handleDocFileSelected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedDocFile ? selectedDocFile.name : 'Add file' }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="column is-12">
        <b-field :type="formErrors.description ? 'is-danger' : ''"
          :message="formErrors.description || ''">
          <template #label>
            {{ "Description" }} <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="description" name="Description" />
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="validateAll()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { showAlertError } from "@/shared/utils/toast";
import { filename } from '@/shared/format';
import { generateFileName } from "@/shared/utils/fileNaming";
import { createWorkerOtherDocuments } from '@/shared/worker-profile/api';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import type { CovenantFileModel } from '@/shared/types/common';

const props = defineProps<{ data?: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'closeAndUpdate', value: boolean): void }>();

const schema = yup.object({
  description: yup.string().required('Description is required').max(20, 'Max 20 characters'),
});

const form = useStickyForm<{ description: string }>({
  schema,
  initialValues: { description: '' },
});
const { description } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const selectedDocFile = ref<File | null>(null);
const fileObjects = reactive<{ otherDocument: File | null }>({ otherDocument: null });
const otherDocument = ref<CovenantFileModel>({ fileName: "", description: "" });

function handleDocFileSelected(file: File | null) {
  if (!file) return;
  if (file.size / 1024 > 15500) {
    showAlertError('File exceeds 15MB limit');
    selectedDocFile.value = null;
    return;
  }
  fileObjects.otherDocument = file;
  const generatedName = generateFileName('OtherDoc', file.name);
  otherDocument.value = { fileName: generatedName, description: '' };
  selectedDocFile.value = null;
}

function clearDocFile() {
  fileObjects.otherDocument = null;
  otherDocument.value = { fileName: '', description: '' };
}

async function saveOtherDocument(values: { description: string }) {
  isLoading.value = true;
  try {
    const payload: CovenantFileModel = { fileName: otherDocument.value.fileName, description: values.description };
    const formData = new FormData();
    formData.append('data', JSON.stringify(payload));
    if (fileObjects.otherDocument) {
      const fn = payload.fileName;
      formData.append(fn, fileObjects.otherDocument, fn);
    }
    await createWorkerOtherDocuments(props.data.id, formData);
    emit('closeAndUpdate', true);
  } catch (error) {
    showAlertError(error);
  } finally {
    isLoading.value = false;
  }
}

function validateAll() {
  form.markInteracted();
  form.handleSubmit((values) => {
    saveOtherDocument(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}
</script>

<style scoped>
.selected-file-display {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px;
  background: #f5f5f5;
  border-radius: 4px;
}
.selected-file-name {
  flex: 1;
  font-size: 0.875rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
