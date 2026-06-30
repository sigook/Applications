<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.identificationType1 ? 'is-danger' : ''"
          :message="formErrors.identificationType1 || ''">
          <template #label>
            Identification type <span class="has-text-danger">*</span>
          </template>
          <b-select v-model="identificationType1" name="identificationType1" expanded>
            <option :value="null" disabled>{{ "Select" }}</option>
            <option v-for="(type, index) in identificationTypes" :value="type"
              :disabled="type === identificationType2" v-bind:key="'identificationType1' + index">
              {{ type.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Identification Number'"
          :type="formErrors.identificationNumber1 ? 'is-danger' : ''"
          :message="formErrors.identificationNumber1 || ''">
          <b-input type="text" v-model="identificationNumber1" name="identificationNumber1" expanded />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :label="'Identification file'">
          <div v-if="worker.identificationType1File && worker.identificationType1File.fileName"
            class="selected-file-display">
            <b-icon icon="file-document" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(worker.identificationType1File.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearFile1()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedFile1 }">
            <b-upload v-model="selectedFile1" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @update:modelValue="handleFile1Selected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedFile1 ? selectedFile1.name : 'Add file' }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.identificationType2 ? 'is-danger' : ''"
          :message="formErrors.identificationType2 || ''">
          <template #label>
            Identification type <span class="has-text-danger">*</span>
          </template>
          <b-select v-model="identificationType2" name="identificationType2" expanded>
            <option :value="null" disabled>{{ "Select" }}</option>
            <option v-for="(type, index) in identificationTypes" :value="type"
              :disabled="type === identificationType1" v-bind:key="'identificationType2' + index">
              {{ type.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :label="'Identification Number'"
          :type="formErrors.identificationNumber2 ? 'is-danger' : ''"
          :message="formErrors.identificationNumber2 || ''">
          <b-input type="text" v-model="identificationNumber2" name="identificationNumber2" expanded />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :label="'Identification file'">
          <div v-if="worker.identificationType2File && worker.identificationType2File.fileName"
            class="selected-file-display">
            <b-icon icon="file-document" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(worker.identificationType2File.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearFile2()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedFile2 }">
            <b-upload v-model="selectedFile2" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @update:modelValue="handleFile2Selected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedFile2 ? selectedFile2.name : 'Add file' }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field :label="'Got Police Check/Background?'">
          <b-switch v-model="worker.havePoliceCheckBackground" :true-value="true" :false-value="false">
            {{ worker.havePoliceCheckBackground ? "Yes" : "No" }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateAll()" :disabled="isLoading">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { filename } from '@/utils/filters';
import { generateFileName } from "@/utils/buildWorkerFormData";
import { getIdentificationTypes } from "@/api/catalogApi";
import { createWorkerDocuments } from '@/api/workerApi';

interface DocsForm {
  identificationType1: any;
  identificationNumber1: string;
  identificationType2: any;
  identificationNumber2: string;
}

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const schema = yup.object({
  identificationType1: yup.mixed().required('Identification type is required'),
  identificationNumber1: yup.string().nullable().transform((v) => (v === '' ? null : v)).min(5, 'Min 5 characters').max(15, 'Max 15 characters'),
  identificationType2: yup.mixed().required('Identification type is required'),
  identificationNumber2: yup.string().nullable().transform((v) => (v === '' ? null : v)).min(5, 'Min 5 characters').max(15, 'Max 15 characters'),
});

const form = useStickyForm<DocsForm>({
  schema,
  initialValues: {
    identificationType1: null,
    identificationNumber1: '',
    identificationType2: null,
    identificationNumber2: '',
  },
});
const { identificationType1, identificationNumber1, identificationType2, identificationNumber2 } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const worker = ref<any>({});
const identificationTypes = ref<any[]>([]);
const selectedFile1 = ref<any>(null);
const selectedFile2 = ref<any>(null);
const fileObjects = reactive<{ identificationType1: any; identificationType2: any }>({
  identificationType1: null,
  identificationType2: null,
});

function handleFile1Selected(file: any) {
  if (!file) return;
  if (file.size / 1024 > 15500) {
    showAlertError('File exceeds 15MB limit');
    selectedFile1.value = null;
    return;
  }
  fileObjects.identificationType1 = file;
  const generatedName = generateFileName('Document', file.name);
  worker.value.identificationType1File = { fileName: generatedName, description: '' };
  selectedFile1.value = null;
}

function handleFile2Selected(file: any) {
  if (!file) return;
  if (file.size / 1024 > 15500) {
    showAlertError('File exceeds 15MB limit');
    selectedFile2.value = null;
    return;
  }
  fileObjects.identificationType2 = file;
  const generatedName = generateFileName('Document', file.name);
  worker.value.identificationType2File = { fileName: generatedName, description: '' };
  selectedFile2.value = null;
}

function clearFile1() {
  fileObjects.identificationType1 = null;
  worker.value.identificationType1File = null;
}

function clearFile2() {
  fileObjects.identificationType2 = null;
  worker.value.identificationType2File = null;
}

async function saveDocuments(values: any) {
  isLoading.value = true;
  try {
    const payload = {
      identificationType1: values.identificationType1,
      identificationNumber1: values.identificationNumber1,
      identificationType1File: worker.value.identificationType1File,
      identificationType2: values.identificationType2,
      identificationNumber2: values.identificationNumber2,
      identificationType2File: worker.value.identificationType2File,
      havePoliceCheckBackground: worker.value.havePoliceCheckBackground,
    };
    const formData = new FormData();
    formData.append('data', JSON.stringify(payload));
    if (fileObjects.identificationType1) {
      const fn = worker.value.identificationType1File.fileName;
      formData.append(fn, fileObjects.identificationType1, fn);
    }
    if (fileObjects.identificationType2) {
      const fn = worker.value.identificationType2File.fileName;
      formData.append(fn, fileObjects.identificationType2, fn);
    }
    await createWorkerDocuments(worker.value.id, formData);
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
    saveDocuments(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

(async () => {
  identificationTypes.value = await getIdentificationTypes();
  if (props.data != null) {
    worker.value = { ...props.data };
    form.hydrate({
      identificationType1: props.data.identificationType1 || null,
      identificationNumber1: props.data.identificationNumber1 || '',
      identificationType2: props.data.identificationType2 || null,
      identificationNumber2: props.data.identificationNumber2 || '',
    });
  }
})();
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
