<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-6">
        <b-field :label="'Identification type'"
          :type="errors.has('identificationType1') ? 'is-danger' : ''"
          :message="errors.has('identificationType1') ? errors.first('identificationType1') : ''">
          <b-select v-model="worker.identificationType1" name="identificationType1" v-validate="'required'" expanded>
            <option value="" disabled>{{ "Select" }}</option>
            <option v-for="(type, index) in identificationTypes" :value="type"
              :disabled="type === worker.identificationType2" v-bind:key="'identificationType1' + index">
              {{ type.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-6">
        <b-field :label="'Identification Number'"
          :type="errors.has('identificationNumber1') ? 'is-danger' : ''"
          :message="errors.has('identificationNumber1') ? errors.first('identificationNumber1') : ''">
          <b-input type="text" v-model="worker.identificationNumber1" name="identificationNumber1"
            v-validate="'max:15|min:5'" expanded />
        </b-field>
      </div>
      <div class="col-12">
        <b-field :label="'Identification file'">
          <div v-if="worker.identificationType1File && worker.identificationType1File.fileName"
            class="selected-file-display">
            <b-icon icon="file-document" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(worker.identificationType1File.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearFile1()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedFile1 }">
            <b-upload v-model="selectedFile1" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @input="handleFile1Selected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedFile1 ? selectedFile1.name : 'Add file' }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-6">
        <b-field :label="'Identification type'"
          :type="errors.has('identificationType2') ? 'is-danger' : ''"
          :message="errors.has('identificationType2') ? errors.first('identificationType2') : ''">
          <b-select v-model="worker.identificationType2" name="identificationType2" v-validate="'required'" expanded>
            <option value="" disabled>{{ "Select" }}</option>
            <option v-for="(type, index) in identificationTypes" :value="type"
              :disabled="type === worker.identificationType1" v-bind:key="'identificationType2' + index">
              {{ type.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-6">
        <b-field :label="'Identification Number'"
          :type="errors.has('identificationNumber2') ? 'is-danger' : ''"
          :message="errors.has('identificationNumber2') ? errors.first('identificationNumber2') : ''">
          <b-input type="text" v-model="worker.identificationNumber2" name="identificationNumber2"
            v-validate="'max:15|min:5'" expanded />
        </b-field>
      </div>
      <div class="col-12">
        <b-field :label="'Identification file'">
          <div v-if="worker.identificationType2File && worker.identificationType2File.fileName"
            class="selected-file-display">
            <b-icon icon="file-document" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(worker.identificationType2File.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearFile2()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedFile2 }">
            <b-upload v-model="selectedFile2" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @input="handleFile2Selected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedFile2 ? selectedFile2.name : 'Add file' }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-12">
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

<script lang="ts">
import { showAlertError } from "@/utils/toast";
import { filename } from '@/utils/filters';
import { createMultipartFormData, generateFileName } from "@/utils/buildWorkerFormData";
import { getIdentificationTypes } from "@/api/catalogApi";
import { createWorkerDocuments } from '@/api/workerApi';

export default {
  props: ["data"],
  data() {
    return {
      isLoading: false,
      worker: {},
      identificationTypes: [],
      selectedFile1: null,
      selectedFile2: null,
      fileObjects: {
        identificationType1: null,
        identificationType2: null
      }
    };
  },
  async created() {
    this.identificationTypes = await getIdentificationTypes();
    if (this.data != null) {
      this.worker = { ...this.data };
    }
  },
  methods: {
    filename,
    handleFile1Selected(file) {
      if (!file) return;
      if (file.size / 1024 > 15500) {
        showAlertError('File exceeds 15MB limit');
        this.selectedFile1 = null;
        return;
      }
      this.fileObjects.identificationType1 = file;
      const generatedName = generateFileName('Document', file.name);
      this.worker.identificationType1File = { fileName: generatedName, description: '' };
      this.selectedFile1 = null;
    },
    handleFile2Selected(file) {
      if (!file) return;
      if (file.size / 1024 > 15500) {
        showAlertError('File exceeds 15MB limit');
        this.selectedFile2 = null;
        return;
      }
      this.fileObjects.identificationType2 = file;
      const generatedName = generateFileName('Document', file.name);
      this.worker.identificationType2File = { fileName: generatedName, description: '' };
      this.selectedFile2 = null;
    },
    clearFile1() {
      this.fileObjects.identificationType1 = null;
      this.worker.identificationType1File = null;
    },
    clearFile2() {
      this.fileObjects.identificationType2 = null;
      this.worker.identificationType2File = null;
    },
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.saveDocuments();
          return;
        }
        showAlertError("Please make sure all required fields are filled out correctly");
      });
    },
    async saveDocuments() {
      this.isLoading = true;
      try {
        const payload = {
          identificationType1: this.worker.identificationType1,
          identificationNumber1: this.worker.identificationNumber1,
          identificationType1File: this.worker.identificationType1File,
          identificationType2: this.worker.identificationType2,
          identificationNumber2: this.worker.identificationNumber2,
          identificationType2File: this.worker.identificationType2File,
          havePoliceCheckBackground: this.worker.havePoliceCheckBackground
        };
        const formData = new FormData();
        formData.append('data', JSON.stringify(payload));
        if (this.fileObjects.identificationType1) {
          const fn = this.worker.identificationType1File.fileName;
          formData.append(fn, this.fileObjects.identificationType1, fn);
        }
        if (this.fileObjects.identificationType2) {
          const fn = this.worker.identificationType2File.fileName;
          formData.append(fn, this.fileObjects.identificationType2, fn);
        }
        await createWorkerDocuments(this.worker.id, formData);
        this.$emit('closeModal', true);
      } catch (error) {
        showAlertError(error);
      } finally {
        this.isLoading = false;
      }
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
