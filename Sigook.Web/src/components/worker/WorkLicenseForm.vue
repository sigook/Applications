<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field>
          <template #label>
            {{ "File" }} <span class="has-text-danger">*</span>
          </template>
          <div v-if="licenseModal.license && licenseModal.license.fileName" class="selected-file-display">
            <b-icon icon="certificate" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(licenseModal.license.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearLicenseFile()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedLicenseFile }">
            <b-upload v-model="selectedLicenseFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @update:modelValue="handleLicenseFileSelected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedLicenseFile ? selectedLicenseFile.name : 'Add file' }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-8 col-lg-8 col-padding">
        <b-field :type="formErrors.description ? 'is-danger' : ''"
          :message="formErrors.description || ''">
          <template #label>
            {{ "Description" }} <span class="has-text-danger">*</span>
          </template>
          <b-input type="text" v-model="description" name="license description" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
        <b-field label="Number">
          <b-input type="text" v-model="number" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Issued">
          <b-datepicker v-model="issued" :focused-date="todayDate" :max-date="todayDate"
            position="is-top-left" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field :type="formErrors.expires ? 'is-danger' : ''"
          :message="formErrors.expires || ''">
          <template #label>
            Expires <span class="has-text-danger">*</span>
          </template>
          <b-datepicker v-model="expires" :focused-date="todayDate" :min-date="todayDate"
            position="is-top-left" name="licenseExpires" />
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
import { mapStores } from 'pinia';
import { useAppStore } from '@/stores/app';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { filename } from '@/utils/filters';
import { generateFileName } from "@/utils/buildWorkerFormData";
import { createWorkerLicenses } from '@/api/workerApi';

const schema = yup.object({
  description: yup.string().required('Description is required').max(100, 'Max 100 characters'),
  number: yup.string().nullable(),
  issued: yup.mixed().nullable(),
  expires: yup.mixed().required('Expires is required'),
});

export default {
  props: ["data"],
  setup() {
    const form = useStickyForm({
      schema,
      initialValues: {
        description: '',
        number: '',
        issued: null as Date | null,
        expires: null as Date | null,
      },
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
      todayDate: null as any,
      isLoading: false,
      selectedLicenseFile: null as any,
      fileObjects: { license: null as any },
      licenseModal: {
        license: { fileName: "", description: "" },
      } as any,
      licenses: [] as any[],
    };
  },
  computed: {
    ...mapStores(useAppStore),
  },
  methods: {
    filename,
    handleLicenseFileSelected(file: any) {
      if (!file) return;
      if (file.size / 1024 > 15500) {
        showAlertError('File exceeds 15MB limit');
        this.selectedLicenseFile = null;
        return;
      }
      this.fileObjects.license = file;
      const generatedName = generateFileName('License', file.name);
      this.licenseModal.license = { fileName: generatedName, description: '' };
      this.selectedLicenseFile = null;
    },
    clearLicenseFile() {
      this.fileObjects.license = null;
      this.licenseModal.license = { fileName: '', description: '' };
    },
    validateAll() {
      this.markInteracted();
      this.handleSubmit((values) => {
        this.saveLicenses(values);
      }, () => {
        showAlertError("Please make sure all required fields are filled out correctly");
      })();
    },
    async saveLicenses(values: any) {
      this.isLoading = true;
      try {
        const newLicense = {
          license: {
            fileName: this.licenseModal.license.fileName,
            description: values.description,
          },
          number: values.number,
          issued: values.issued,
          expires: values.expires,
        };
        const allLicenses = [...this.licenses, newLicense];
        const formData = new FormData();
        formData.append('data', JSON.stringify(allLicenses));
        if (this.fileObjects.license) {
          const fn = newLicense.license.fileName;
          formData.append(fn, this.fileObjects.license, fn);
        }
        await createWorkerLicenses((this as any).data.id, formData);
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
      for (let i = 0; i < data.licenses.length; i++) {
        this.licenses.push(data.licenses[i]);
      }
    }
    (this as any).appStore.getCurrentDate().then((response: any) => {
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
