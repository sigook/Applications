<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field :label="$t('Resume')">
          <div v-if="resume && resume.fileName" class="selected-file-display">
            <b-icon icon="file-document" size="is-small"></b-icon>
            <span class="selected-file-name">{{ resume.fileName | filename }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearResumeFile()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedResumeFile }">
            <b-upload v-model="selectedResumeFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @input="handleResumeFileSelected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedResumeFile ? selectedResumeFile.name : $t('AddFile') }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="validateAll()" :disabled="isLoading">
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
  props: ['data'],
  data() {
    return {
      isLoading: false,
      selectedResumeFile: null,
      fileObjects: {
        resume: null
      },
      resume: {
        fileName: '',
        description: ''
      }
    };
  },
  mixins: [toastMixin, multipartUploadMixin],
  created() {
    if (this.data != null && this.data.resume) {
      this.resume = { ...this.data.resume };
    }
  },
  methods: {
    handleResumeFileSelected(file) {
      if (!file) return;
      if (file.size / 1024 > 15500) {
        this.showAlertError('File exceeds 15MB limit');
        this.selectedResumeFile = null;
        return;
      }
      this.fileObjects.resume = file;
      const generatedName = this.generateFileName('Resume', file.name);
      this.resume = { fileName: generatedName, description: '' };
      this.selectedResumeFile = null;
    },
    clearResumeFile() {
      this.fileObjects.resume = null;
      this.resume = { fileName: '', description: '' };
    },
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.saveResume();
          return;
        }
        this.showAlertError(this.$t('PleaseVerifyThatTheFieldsAreCorrect'));
      });
    },
    async saveResume() {
      this.isLoading = true;
      try {
        const formData = new FormData();
        formData.append('data', JSON.stringify(this.resume));
        if (this.fileObjects.resume) {
          const fn = this.resume.fileName;
          formData.append(fn, this.fileObjects.resume, fn);
        }
        await this.$store.dispatch('worker/createWorkerResume', { profileId: this.data.id, formData });
        this.$emit('closeModal', true);
      } catch (error) {
        this.showAlertError(error);
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
