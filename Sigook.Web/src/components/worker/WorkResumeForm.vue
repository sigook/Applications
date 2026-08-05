<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field :label="'Resume'">
          <div v-if="resume && resume.fileName" class="selected-file-display">
            <b-icon icon="file-document" size="is-small"></b-icon>
            <span class="selected-file-name">{{ filename(resume.fileName) }}</span>
            <b-button type="is-danger" size="is-small" icon-left="delete" outlined @click="clearResumeFile()"></b-button>
          </div>
          <b-field v-else class="file is-primary" :class="{ 'has-name': !!selectedResumeFile }">
            <b-upload v-model="selectedResumeFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
              @update:modelValue="handleResumeFileSelected" class="file-label" rounded>
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ selectedResumeFile ? selectedResumeFile.name : 'Add file' }}</span>
              </span>
            </b-upload>
          </b-field>
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="validateAll()" :disabled="isLoading">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from "@/utils/toast";
import { filename } from '@/utils/filters';
import { generateFileName } from "@/utils/buildWorkerFormData";
import { createWorkerResume } from '@/api/workerApi';

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const selectedResumeFile = ref<any>(null);
const fileObjects = reactive<{ resume: any }>({ resume: null });
const resume = ref<any>({ fileName: '', description: '' });

if (props.data != null && props.data.resume) {
  resume.value = { ...props.data.resume };
}

function handleResumeFileSelected(file: any) {
  if (!file) return;
  if (file.size / 1024 > 15500) {
    showAlertError('File exceeds 15MB limit');
    selectedResumeFile.value = null;
    return;
  }
  fileObjects.resume = file;
  const generatedName = generateFileName('Resume', file.name);
  resume.value = { fileName: generatedName, description: '' };
  selectedResumeFile.value = null;
}

function clearResumeFile() {
  fileObjects.resume = null;
  resume.value = { fileName: '', description: '' };
}

async function saveResume() {
  isLoading.value = true;
  try {
    const formData = new FormData();
    formData.append('data', JSON.stringify(resume.value));
    if (fileObjects.resume) {
      const fn = resume.value.fileName;
      formData.append(fn, fileObjects.resume, fn);
    }
    await createWorkerResume(props.data.id, formData);
    emit('closeModal', true);
  } catch (error) {
    showAlertError(error);
  } finally {
    isLoading.value = false;
  }
}

function validateAll() {
  saveResume();
}
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
