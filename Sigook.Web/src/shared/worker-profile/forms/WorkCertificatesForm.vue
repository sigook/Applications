<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field>
          <template #label>
            {{ "File" }} <span class="has-text-danger">*</span>
          </template>
          <div v-if="certificate && certificate.fileName" class="selected-file-display">
            <b-icon icon="certificate" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(certificate.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearCertFile()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedCertFile }">
            <b-upload v-model="selectedCertFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @update:modelValue="handleCertFileSelected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedCertFile ? selectedCertFile.name : 'Add file' }}</span>
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
          <b-input type="text" v-model="description" name="certificate description" />
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
import { createWorkerCertificates } from '@/shared/worker-profile/api';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import type { CovenantFileModel } from '@/shared/types/common';

const props = defineProps<{ data?: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

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
const selectedCertFile = ref<File | null>(null);
const fileObjects = reactive<{ certificate: File | null }>({ certificate: null });
const certificate = ref<CovenantFileModel>({ fileName: "", description: "" });
const certificates = ref<CovenantFileModel[]>([]);

function handleCertFileSelected(file: File | null) {
  if (!file) return;
  if (file.size / 1024 > 15500) {
    showAlertError('File exceeds 15MB limit');
    selectedCertFile.value = null;
    return;
  }
  fileObjects.certificate = file;
  const generatedName = generateFileName('Certificate', file.name);
  certificate.value = { fileName: generatedName, description: '' };
  selectedCertFile.value = null;
}

function clearCertFile() {
  fileObjects.certificate = null;
  certificate.value = { fileName: '', description: '' };
}

async function saveCertificates(values: { description: string }) {
  isLoading.value = true;
  try {
    const newCert: CovenantFileModel = { fileName: certificate.value.fileName, description: values.description };
    const allCertificates = [...certificates.value, newCert];
    const formData = new FormData();
    formData.append('data', JSON.stringify(allCertificates));
    if (fileObjects.certificate) {
      const fn = newCert.fileName;
      formData.append(fn, fileObjects.certificate, fn);
    }
    await createWorkerCertificates(props.data.id, formData);
    emit('closeModal', true);
  } catch (error) {
    showAlertError(error);
  } finally {
    isLoading.value = false;
  }
}

function validateAll() {
  form.markInteracted();
  form.handleSubmit((values) => {
    saveCertificates(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

if (props.data != null) {
  for (let i = 0; i < props.data.certificates.length; i++) {
    certificates.value.push(props.data.certificates[i]);
  }
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
