<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
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
      <div class="col-12">
        <b-field :type="formErrors.description ? 'is-danger' : ''"
          :message="formErrors.description || ''">
          <template #label>
            {{ "Description" }} <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="description" name="certificate description" />
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
import { createWorkerCertificates } from '@/api/workerApi';

const schema = yup.object({
  description: yup.string().required('Description is required').max(20, 'Max 20 characters'),
});

export default {
  props: ["data"],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: { description: '' },
    });
    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
    };
  },
  data() {
    return {
      isLoading: false,
      selectedCertFile: null as any,
      fileObjects: { certificate: null as any },
      certificate: { fileName: "", description: "" } as any,
      certificates: [] as any[],
    };
  },
  methods: {
    filename,
    handleCertFileSelected(file: any) {
      if (!file) return;
      if (file.size / 1024 > 15500) {
        showAlertError('File exceeds 15MB limit');
        this.selectedCertFile = null;
        return;
      }
      this.fileObjects.certificate = file;
      const generatedName = generateFileName('Certificate', file.name);
      this.certificate = { fileName: generatedName, description: '' };
      this.selectedCertFile = null;
    },
    clearCertFile() {
      this.fileObjects.certificate = null;
      this.certificate = { fileName: '', description: '' };
    },
    validateAll() {
      this.markInteracted();
      this.handleSubmit((values) => {
        this.saveCertificates(values);
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    async saveCertificates(values: any) {
      this.isLoading = true;
      try {
        const newCert = { fileName: this.certificate.fileName, description: values.description };
        const allCertificates = [...this.certificates, newCert];
        const formData = new FormData();
        formData.append('data', JSON.stringify(allCertificates));
        if (this.fileObjects.certificate) {
          const fn = newCert.fileName;
          formData.append(fn, this.fileObjects.certificate, fn);
        }
        await createWorkerCertificates((this as any).data.id, formData);
        this.$emit('closeModal', true);
      } catch (error) {
        showAlertError(error);
      } finally {
        this.isLoading = false;
      }
    },
  },
  created() {
    const data = (this as any).data;
    if (data != null) {
      for (let i = 0; i < data.certificates.length; i++) {
        this.certificates.push(data.certificates[i]);
      }
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
