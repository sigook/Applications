<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field>
          <template #label>
            {{ $t("File") }} <span class="has-text-danger">*</span>
          </template>
          <div v-if="otherDocument && otherDocument.fileName" class="selected-file-display">
            <b-icon icon="file-document" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(otherDocument.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearDocFile()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedDocFile }">
            <b-upload v-model="selectedDocFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @input="handleDocFileSelected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedDocFile ? selectedDocFile.name : $t('AddFile') }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-12">
        <b-field :type="errors.has('Description') ? 'is-danger' : ''"
          :message="errors.has('Description') ? errors.first('Description') : ''">
          <template #label>
            {{ $t("Description") }} <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="otherDocument.description" name="Description"
            v-validate="'required|max:20'" />
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateAll()">
          {{ $t("Save") }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { filename } from '@/utils/filters';
import toastMixin from "../../mixins/toastMixin";
import multipartUploadMixin from "../../mixins/multipartUploadMixin";
import { createWorkerOtherDocuments } from '@/api/workerApi';

export default {
  props: ["data"],
  data() {
    return {
      isLoading: false,
      selectedDocFile: null,
      fileObjects: {
        otherDocument: null
      },
      otherDocument: {
        fileName: "",
        description: "",
      },
    };
  },
  mixins: [toastMixin, multipartUploadMixin],
  methods: {
    filename,
    handleDocFileSelected(file) {
      if (!file) return;
      if (file.size / 1024 > 15500) {
        this.showAlertError('File exceeds 15MB limit');
        this.selectedDocFile = null;
        return;
      }
      this.fileObjects.otherDocument = file;
      const generatedName = this.generateFileName('OtherDoc', file.name);
      this.otherDocument = { fileName: generatedName, description: this.otherDocument.description || '' };
      this.selectedDocFile = null;
    },
    clearDocFile() {
      this.fileObjects.otherDocument = null;
      this.otherDocument = { fileName: '', description: '' };
    },
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.saveOtherDocument();
          return;
        }
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      });
    },
    async saveOtherDocument() {
      this.isLoading = true;
      try {
        const formData = new FormData();
        formData.append('data', JSON.stringify(this.otherDocument));
        if (this.fileObjects.otherDocument) {
          const fn = this.otherDocument.fileName;
          formData.append(fn, this.fileObjects.otherDocument, fn);
        }
        await createWorkerOtherDocuments(this.data.id, formData);
        this.$emit('closeAndUpdate', true);
      } catch (error) {
        this.showAlertError(error);
      } finally {
        this.isLoading = false;
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
