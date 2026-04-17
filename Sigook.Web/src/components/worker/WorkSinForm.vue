<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field :label="'SIN/SSN#'"
          :type="formErrors.socialInsurance ? 'is-danger' : ''"
          :message="formErrors.socialInsurance || ''">
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
        <b-field :label="'Expire'" :type="formErrors.dueDate ? 'is-danger' : ''"
          :message="formErrors.dueDate || ''">
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

<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { filename } from '@/utils/filters';
import { generateFileName } from "@/utils/buildWorkerFormData";
import { createWorkerSin } from '@/api/workerApi';

const schema = yup.object({
  socialInsurance: yup.string()
    .required('SIN/SSN is required')
    .min(9, 'Min 9 characters')
    .max(15, 'Max 15 characters'),
  dueDate: yup.mixed().nullable(),
});

export default {
  props: ['data'],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        socialInsurance: '',
        dueDate: null as Date | null,
      },
    });

    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      hydrateForm: form.hydrate,
      resetForm: form.resetAll,
    };
  },
  data() {
    return {
      isLoading: false,
      selectedSinFile: null as any,
      fileObjects: {
        sinFile: null as any,
      },
      sin: {
        socialInsurance: "",
        socialInsuranceExpire: false,
        dueDate: null as Date | null,
        socialInsuranceFile: {
          fileName: "",
          description: "",
        },
      },
    };
  },
  methods: {
    filename,
    handleSinFileSelected(file: any) {
      if (!file) return;
      if (file.size / 1024 > 15500) {
        showAlertError('File exceeds 15MB limit');
        this.selectedSinFile = null;
        return;
      }
      this.fileObjects.sinFile = file;
      const generatedName = generateFileName('SIN-SSN', file.name);
      this.sin.socialInsuranceFile = { fileName: generatedName, description: '' };
      this.selectedSinFile = null;
    },
    clearSinFile() {
      this.fileObjects.sinFile = null;
      this.sin.socialInsuranceFile = { fileName: '', description: '' };
    },
    validateAll() {
      this.markInteracted();
      this.handleSubmit((values: any) => {
        if (this.sin.socialInsuranceExpire && !values.dueDate) {
          showAlertError('Please make sure all required fields are filled out correctly');
          return;
        }
        this.createWorkerSin(values);
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    async createWorkerSin(values: any) {
      this.isLoading = true;
      try {
        const payload = {
          ...this.sin,
          socialInsurance: values.socialInsurance,
          dueDate: this.sin.socialInsuranceExpire ? values.dueDate : null,
        };
        const formData = new FormData();
        formData.append('data', JSON.stringify(payload));
        if (this.fileObjects.sinFile) {
          const fn = payload.socialInsuranceFile.fileName;
          formData.append(fn, this.fileObjects.sinFile, fn);
        }
        await createWorkerSin((this as any).data.id, formData);
        this.$emit('closeModal', true);
      } catch (error) {
        showAlertError(error);
      } finally {
        this.isLoading = false;
      }
    },
  },
  created() {
    if ((this as any).data.socialInsurance !== null && (this as any).data.socialInsurance !== "") {
      this.sin = { ...(this as any).data };
      this.sin.dueDate = (this as any).data.dueDate ? new Date((this as any).data.dueDate) : null;
      this.hydrateForm({
        socialInsurance: this.sin.socialInsurance || '',
        dueDate: this.sin.dueDate,
      });
    }
  },
};
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
