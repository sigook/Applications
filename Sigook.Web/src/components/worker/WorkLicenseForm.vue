<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field>
          <template #label>
            {{ "File" }} <span class="has-text-danger">*</span>
          </template>
          <div v-if="licenseModal.license && licenseModal.license.fileName" class="selected-file-display">
            <b-icon icon="certificate" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(licenseModal.license.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearLicenseFile()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedLicenseFile }">
            <b-upload v-model="selectedLicenseFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @update:modelValue="handleLicenseFileSelected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedLicenseFile ? selectedLicenseFile.name : 'Add file' }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-8 col-lg-8 col-padding">
        <b-field :type="formErrors.description ? 'is-danger' : ''"
          :message="formErrors.description || ''">
          <template #label>
            {{ "Description" }} <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="description" name="license description" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field label="Number">
          <b-input type="text" v-model="number" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Issued">
          <b-datepicker v-model="issued" :focused-date="todayDate" :max-date="todayDate"
            position="is-top-left" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.expires ? 'is-danger' : ''"
          :message="formErrors.expires || ''">
          <template #label>
            Expires <span class="has-text-danger">*</span>
          </template>
          <b-datepicker v-model="expires" :focused-date="todayDate" :min-date="todayDate"
            position="is-top-left" name="licenseExpires" />
        </b-field>
      </div>
      <div class="col-12 mt-5">
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
import { useAppStore } from '@/stores/app';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { filename } from '@/utils/filters';
import { generateFileName } from "@/utils/buildWorkerFormData";
import { createWorkerLicenses } from '@/api/workerApi';

interface LicenseForm {
  description: string;
  number: string;
  issued: Date | null;
  expires: Date | null;
}

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const schema = yup.object({
  description: yup.string().required('Description is required').max(100, 'Max 100 characters'),
  number: yup.string().nullable(),
  issued: yup.mixed().nullable(),
  expires: yup.mixed().required('Expires is required'),
});

const form = useStickyForm<LicenseForm>({
  schema,
  initialValues: {
    description: '',
    number: '',
    issued: null,
    expires: null,
  },
});
const { description, number, issued, expires } = form.fields;
const formErrors = form.errors;

const appStore = useAppStore();

const todayDate = ref<any>(null);
const isLoading = ref(false);
const selectedLicenseFile = ref<any>(null);
const fileObjects = reactive<{ license: any }>({ license: null });
const licenseModal = ref<any>({
  license: { fileName: "", description: "" },
});
const licenses = ref<any[]>([]);

function handleLicenseFileSelected(file: any) {
  if (!file) return;
  if (file.size / 1024 > 15500) {
    showAlertError('File exceeds 15MB limit');
    selectedLicenseFile.value = null;
    return;
  }
  fileObjects.license = file;
  const generatedName = generateFileName('License', file.name);
  licenseModal.value.license = { fileName: generatedName, description: '' };
  selectedLicenseFile.value = null;
}

function clearLicenseFile() {
  fileObjects.license = null;
  licenseModal.value.license = { fileName: '', description: '' };
}

async function saveLicenses(values: any) {
  isLoading.value = true;
  try {
    const newLicense = {
      license: {
        fileName: licenseModal.value.license.fileName,
        description: values.description,
      },
      number: values.number,
      issued: values.issued,
      expires: values.expires,
    };
    const allLicenses = [...licenses.value, newLicense];
    const formData = new FormData();
    formData.append('data', JSON.stringify(allLicenses));
    if (fileObjects.license) {
      const fn = newLicense.license.fileName;
      formData.append(fn, fileObjects.license, fn);
    }
    await createWorkerLicenses(props.data.id, formData);
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
    saveLicenses(values);
  }, () => {
    showAlertError("Please make sure all required fields are filled out correctly");
  })();
}

if (props.data != null) {
  for (let i = 0; i < props.data.licenses.length; i++) {
    licenses.value.push(props.data.licenses[i]);
  }
}
appStore.getCurrentDate().then((response: any) => {
  todayDate.value = response;
});
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
