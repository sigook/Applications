<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field :type="formErrors.socialInsurance ? 'is-danger' : ''"
          :message="formErrors.socialInsurance || ''">
          <template #label>
            SIN/SSN# <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="socialInsurance" name="social insurance #">
          </b-input>
        </b-field>
      </div>
      <div class="col-12">
        <b-field :label="'SIN/SSN (Upload file)'">
          <div v-if="sin.socialInsuranceFile && sin.socialInsuranceFile.fileName" class="selected-file-display">
            <b-icon icon="file-document" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(sin.socialInsuranceFile.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearSinFile()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedSinFile }">
            <b-upload v-model="selectedSinFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @update:modelValue="handleSinFileSelected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedSinFile ? selectedSinFile.name : 'Add file' }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-12">
        <b-field :label="'Expire'" class="has-text-weight-normal">
          <b-switch v-model="sin.socialInsuranceExpire" :true-value="true" :false-value="false">
            {{ sin.socialInsuranceExpire ? "Yes" : "No" }}
          </b-switch>
        </b-field>
      </div>
      <div class="col-12" v-if="sin.socialInsuranceExpire === true">
        <b-field :type="formErrors.dueDate ? 'is-danger' : ''"
          :message="formErrors.dueDate || ''">
          <template #label>
            Expire <span class="has-text-danger">*</span>
          </template>
          <b-datepicker v-model="dueDate" name="due date" append-to-body position="is-top-right">
          </b-datepicker>
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
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { filename } from '@/utils/filters';
import { generateFileName } from "@/utils/buildWorkerFormData";
import { createWorkerSin } from '@/api/workerApi';

interface SinForm {
  socialInsurance: string;
  dueDate: Date | null;
}

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const schema = yup.object({
  socialInsurance: yup.string()
    .required('SIN/SSN is required')
    .min(9, 'Min 9 characters')
    .max(15, 'Max 15 characters'),
  dueDate: yup.mixed().nullable(),
});

const form = useStickyForm<SinForm>({
  schema,
  initialValues: {
    socialInsurance: '',
    dueDate: null,
  },
});
const { socialInsurance, dueDate } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const selectedSinFile = ref<any>(null);
const fileObjects = reactive<{ sinFile: any }>({ sinFile: null });
const sin = reactive<any>({
  socialInsurance: "",
  socialInsuranceExpire: false,
  dueDate: null as Date | null,
  socialInsuranceFile: {
    fileName: "",
    description: "",
  },
});

function handleSinFileSelected(file: any) {
  if (!file) return;
  if (file.size / 1024 > 15500) {
    showAlertError('File exceeds 15MB limit');
    selectedSinFile.value = null;
    return;
  }
  fileObjects.sinFile = file;
  const generatedName = generateFileName('SIN-SSN', file.name);
  sin.socialInsuranceFile = { fileName: generatedName, description: '' };
  selectedSinFile.value = null;
}

function clearSinFile() {
  fileObjects.sinFile = null;
  sin.socialInsuranceFile = { fileName: '', description: '' };
}

async function saveWorkerSin(values: any) {
  isLoading.value = true;
  try {
    const payload = {
      ...sin,
      socialInsurance: values.socialInsurance,
      dueDate: sin.socialInsuranceExpire ? values.dueDate : null,
    };
    const formData = new FormData();
    formData.append('data', JSON.stringify(payload));
    if (fileObjects.sinFile) {
      const fn = payload.socialInsuranceFile.fileName;
      formData.append(fn, fileObjects.sinFile, fn);
    }
    await createWorkerSin(props.data.id, formData);
    emit('closeModal', true);
  } catch (error) {
    showAlertError(error);
  } finally {
    isLoading.value = false;
  }
}

function validateAll() {
  form.markInteracted();
  form.handleSubmit((values: any) => {
    if (sin.socialInsuranceExpire && !values.dueDate) {
      showAlertError('Please make sure all required fields are filled out correctly');
      return;
    }
    saveWorkerSin(values);
  }, () => {
    showAlertError('Please make sure all required fields are filled out correctly');
  })();
}

if (props.data && props.data.socialInsurance !== null && props.data.socialInsurance !== "") {
  Object.assign(sin, props.data);
  sin.dueDate = props.data.dueDate ? new Date(props.data.dueDate) : null;
  form.hydrate({
    socialInsurance: sin.socialInsurance || '',
    dueDate: sin.dueDate,
  });
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
