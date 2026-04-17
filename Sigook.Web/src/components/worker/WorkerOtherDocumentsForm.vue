<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
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
      <div class="col-12">
        <b-field :type="formErrors.description ? 'is-danger' : ''"
          :message="formErrors.description || ''">
          <template #label>
            {{ "Description" }} <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="description" name="Description" />
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
import { createWorkerOtherDocuments } from '@/api/workerApi';

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
      selectedDocFile: null as any,
      fileObjects: { otherDocument: null as any },
      otherDocument: { fileName: "", description: "" } as any,
    };
  },
  methods: {
    filename,
    handleDocFileSelected(file: any) {
      if (!file) return;
      if (file.size / 1024 > 15500) {
        showAlertError('File exceeds 15MB limit');
        this.selectedDocFile = null;
        return;
      }
      this.fileObjects.otherDocument = file;
      const generatedName = generateFileName('OtherDoc', file.name);
      this.otherDocument = { fileName: generatedName, description: '' };
      this.selectedDocFile = null;
    },
    clearDocFile() {
      this.fileObjects.otherDocument = null;
      this.otherDocument = { fileName: '', description: '' };
    },
    validateAll() {
      this.markInteracted();
      this.handleSubmit((values) => {
        this.saveOtherDocument(values);
      }, () => {
        showAlertError('Please make sure all required fields are filled out correctly');
      })();
    },
    async saveOtherDocument(values: any) {
      this.isLoading = true;
      try {
        const payload = { fileName: this.otherDocument.fileName, description: values.description };
        const formData = new FormData();
        formData.append('data', JSON.stringify(payload));
        if (this.fileObjects.otherDocument) {
          const fn = payload.fileName;
          formData.append(fn, this.fileObjects.otherDocument, fn);
        }
        await createWorkerOtherDocuments((this as any).data.id, formData);
        this.$emit('closeAndUpdate', true);
      } catch (error) {
        showAlertError(error);
      } finally {
        this.isLoading = false;
      }
    },
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
