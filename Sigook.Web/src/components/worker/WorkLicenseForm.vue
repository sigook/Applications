<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field>
          <template #label>
            {{ $t("File") }} <span class="has-text-danger">*</span>
          </template>
          <div v-if="licenseModal.license && licenseModal.license.fileName" class="selected-file-display">
            <b-icon icon="certificate" size="is-small"></b-icon>
            <span class="selected-file-name">{{ licenseModal.license.fileName | filename }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearLicenseFile()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedLicenseFile }">
            <b-upload v-model="selectedLicenseFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @input="handleLicenseFileSelected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedLicenseFile ? selectedLicenseFile.name : $t('AddFile') }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-8 col-lg-8 col-padding">
        <b-field :type="errors.has('license description') ? 'is-danger' : ''"
          :message="errors.has('license description') ? errors.first('license description') : ''">
          <template #label>
            {{ $t("Description") }} <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="licenseModal.license.description" name="license description"
            v-validate="'required|max:100'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field label="Number">
          <b-input type="text" v-model="licenseModal.number" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Issued">
          <b-datepicker v-model="licenseModal.issued" :focused-date="todayDate" :max-date="todayDate"
            position="is-top-left" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="errors.has('licenseExpires') ? 'is-danger' : ''"
          :message="errors.has('licenseExpires') ? errors.first('licenseExpires') : ''">
          <template #label>
            Expires <span class="has-text-danger">*</span>
          </template>
          <b-datepicker v-model="licenseModal.expires" :focused-date="todayDate" :min-date="todayDate"
            position="is-top-left" name="licenseExpires" v-validate="'required'" />
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
import toastMixin from "../../mixins/toastMixin";
import multipartUploadMixin from "../../mixins/multipartUploadMixin";
import { createWorkerLicenses } from '@/api/workerApi';

export default {
  props: ["data"],
  data() {
    return {
      todayDate: null,
      isLoading: false,
      selectedLicenseFile: null,
      fileObjects: {
        license: null
      },
      licenseModal: {
        license: {
          fileName: "",
          description: "",
        },
        number: "",
        issued: null,
        expires: null,
      },
      licenses: [],
    };
  },
  mixins: [toastMixin, multipartUploadMixin],
  methods: {
    handleLicenseFileSelected(file) {
      if (!file) return;
      if (file.size / 1024 > 15500) {
        this.showAlertError('File exceeds 15MB limit');
        this.selectedLicenseFile = null;
        return;
      }
      this.fileObjects.license = file;
      const generatedName = this.generateFileName('License', file.name);
      this.licenseModal.license = { fileName: generatedName, description: this.licenseModal.license.description || '' };
      this.selectedLicenseFile = null;
    },
    clearLicenseFile() {
      this.fileObjects.license = null;
      this.licenseModal.license = { fileName: '', description: '' };
    },
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.saveLicenses();
          return;
        }
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      });
    },
    async saveLicenses() {
      this.isLoading = true;
      try {
        const allLicenses = [...this.licenses, this.licenseModal];
        const formData = new FormData();
        formData.append('data', JSON.stringify(allLicenses));
        if (this.fileObjects.license) {
          const fn = this.licenseModal.license.fileName;
          formData.append(fn, this.fileObjects.license, fn);
        }
        await createWorkerLicenses(this.data.id, formData);
        this.$emit('closeModal', true);
      } catch (error) {
        this.showAlertError(error);
      } finally {
        this.isLoading = false;
      }
    }
  },
  created() {
    if (this.data != null) {
      for (let i = 0; i < this.data.licenses.length; i++) {
        this.licenses.push(this.data.licenses[i]);
      }
    }
    this.$store.dispatch("getCurrentDate").then((response) => {
      this.todayDate = response;
    });
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
