<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field :label="'SIN/SSN#'"
          :type="errors.has('social insurance #') ? 'is-danger' : ''"
          :message="errors.has('social insurance #') ? errors.first('social insurance #') : ''">
          <b-input type="text" v-model="sin.socialInsurance" name="social insurance #"
            v-validate="'required|max:15|min:9'">
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
              @input="handleSinFileSelected" class="file-label" rounded>
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
        <b-field :label="'Expire'" :type="errors.has('due date') ? 'is-danger' : ''"
          :message="errors.has('due date') ? errors.first('due date') : ''">
          <b-datepicker v-model="sin.dueDate" name="due date" v-validate="'required'" append-to-body position="is-top-right">
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
import { showAlertError } from "@/utils/toast";
import { filename } from '@/utils/filters';
import { createMultipartFormData, generateFileName } from "@/utils/buildWorkerFormData";
import { createWorkerSin } from '@/api/workerApi';

export default {
  props: ['data'],
  data() {
    return {
      isLoading: false,
      selectedSinFile: null,
      fileObjects: {
        sinFile: null
      },
      sin: {
        socialInsurance: "",
        socialInsuranceExpire: false,
        dueDate: null,
        socialInsuranceFile: {
          fileName: "",
          description: ""
        }
      }
    };
  },
  methods: {
    filename,
    handleSinFileSelected(file) {
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
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.createWorkerSin();
          return;
        }
        showAlertError('Please make sure all required fields are filled out correctly');
      });
    },
    async createWorkerSin() {
      this.isLoading = true;
      try {
        const formData = new FormData();
        formData.append('data', JSON.stringify(this.sin));
        if (this.fileObjects.sinFile) {
          const fn = this.sin.socialInsuranceFile.fileName;
          formData.append(fn, this.fileObjects.sinFile, fn);
        }
        await createWorkerSin(this.data.id, formData);
        this.$emit('closeModal', true);
      } catch (error) {
        showAlertError(error);
      } finally {
        this.isLoading = false;
      }
    }
  },
  created() {
    if (this.data.socialInsurance !== null && this.data.socialInsurance !== "") {
      this.sin = { ...this.data };
      this.sin.dueDate = this.data.dueDate ? new Date(this.sin.dueDate) : null;
    }
  }
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
