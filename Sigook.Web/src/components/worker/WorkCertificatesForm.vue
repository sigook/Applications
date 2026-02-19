<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field>
          <template #label>
            {{ $t("File") }} <span class="has-text-danger">*</span>
          </template>
          <div v-if="certificate && certificate.fileName" class="selected-file-display">
            <b-icon icon="certificate" size="is-small"></b-icon>
            <span class="selected-file-name">{{ certificate.fileName | filename }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearCertFile()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedCertFile }">
            <b-upload v-model="selectedCertFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @input="handleCertFileSelected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedCertFile ? selectedCertFile.name : $t('AddFile') }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-12">
        <b-field :type="errors.has('certificate description') ? 'is-danger' : ''"
          :message="errors.has('certificate description') ? errors.first('certificate description') : ''">
          <template #label>
            {{ $t("Description") }} <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="certificate.description" name="certificate description"
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

<script>
import toastMixin from "../../mixins/toastMixin";
import multipartUploadMixin from "../../mixins/multipartUploadMixin";

export default {
  props: ["data"],
  data() {
    return {
      isLoading: false,
      selectedCertFile: null,
      fileObjects: {
        certificate: null
      },
      certificate: {
        fileName: "",
        description: "",
      },
      certificates: [],
    };
  },
  mixins: [toastMixin, multipartUploadMixin],
  methods: {
    handleCertFileSelected(file) {
      if (!file) return;
      if (file.size / 1024 > 15500) {
        this.showAlertError('File exceeds 15MB limit');
        this.selectedCertFile = null;
        return;
      }
      this.fileObjects.certificate = file;
      const generatedName = this.generateFileName('Certificate', file.name);
      this.certificate = { fileName: generatedName, description: this.certificate.description || '' };
      this.selectedCertFile = null;
    },
    clearCertFile() {
      this.fileObjects.certificate = null;
      this.certificate = { fileName: '', description: '' };
    },
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.saveCertificates();
          return;
        }
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      });
    },
    async saveCertificates() {
      this.isLoading = true;
      try {
        const allCertificates = [...this.certificates, this.certificate];
        const formData = new FormData();
        formData.append('data', JSON.stringify(allCertificates));
        if (this.fileObjects.certificate) {
          const fn = this.certificate.fileName;
          formData.append(fn, this.fileObjects.certificate, fn);
        }
        await this.$store.dispatch('worker/createWorkerCertificates', { profileId: this.data.id, formData });
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
      for (let i = 0; i < this.data.certificates.length; i++) {
        this.certificates.push(this.data.certificates[i]);
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
