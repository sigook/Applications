<template>
  <div class="container-image">
    <b-loading v-model="isLoading"></b-loading>
    <div class="update-image" :class="{ 'is-danger': !!fileError }">
      <input type="file" id="file" @change="validateImage" accept="image/*" title="logo" name="file"
        ref="fileInput" />

      <label for="file">{{ "Upload Picture" }}</label>
      <img v-if="localImage" :src="localImage" alt="logo" style="z-index: 1;">
      <default-image v-if="showDefault && !localImage" :name="name" class="img-100"></default-image>

    </div>
    <span v-show="fileError" class="help is-danger no-margin">{{ fileError }}</span>

    <Teleport to="body">
      <b-modal custom-content-class="card" v-model="modalValidation" width="450px" :destroy-on-hide="true">
        <crop-image :image="cropImage" @onCrop="response => showImage(response)"
          @closeModal="() => modalValidation = false"></crop-image>
      </b-modal>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { compressFile } from '@/shared/utils/compressFile';
import CropImage from "@/shared/ui/CropImage.vue";
import DefaultImage from "@/shared/ui/DefaultImage.vue";
import type { CovenantFileModel } from '@/shared/types/common';

const ALLOWED_EXTENSIONS = ['jpeg', 'jpg', 'png', 'gif', 'svg'];
const MAX_SIZE_KB = 10000;

const props = defineProps<{
  editedImage?: CovenantFileModel | null;
  required?: boolean;
  name?: string;
  showDefault?: boolean;
}>();

const emit = defineEmits<{
  (e: 'onUpload', v: boolean): void;
  (e: 'finishUpload', v: boolean): void;
  (e: 'imageSelected', file: File): void;
}>();

const isLoading = ref(false);
const localImage = ref<string | null>(props.editedImage ? props.editedImage.pathFile : null);
const modalValidation = ref(false);
const cropImage = ref<string>('');
const hasImage = ref(false);
const fileError = ref('');
const fileInput = ref<HTMLInputElement | null>(null);

function validateFile(file: File): boolean {
  fileError.value = '';
  if (!file) {
    if (props.required && !hasImage.value) {
      fileError.value = 'Image is required';
      return false;
    }
    return true;
  }
  const ext = file.name.split('.').pop()?.toLowerCase() || '';
  if (!ALLOWED_EXTENSIONS.includes(ext)) {
    fileError.value = `Allowed formats: ${ALLOWED_EXTENSIONS.join(', ')}`;
    return false;
  }
  if (file.size / 1024 > MAX_SIZE_KB) {
    fileError.value = `File exceeds ${MAX_SIZE_KB / 1000}MB limit`;
    return false;
  }
  return true;
}

function fileFromEvent(evt: Event): File | undefined {
  return (evt.target as HTMLInputElement).files?.[0];
}

function showCrop(evt: Event) {
  if ((document as Document & { documentMode?: number }).documentMode || /Edge/.test(navigator.userAgent)) {
    showImage(fileFromEvent(evt));
  } else {
    const file = fileFromEvent(evt);
    cropImage.value = URL.createObjectURL(file);
    modalValidation.value = true;
  }
}

async function showImage(output: File) {
  isLoading.value = true;
  hasImage.value = true;
  localImage.value = URL.createObjectURL(output);
  modalValidation.value = false;
  emit('onUpload', true);

  try {
    const compressedImage = await compressFile(output);
    cleanInput();
    emit('finishUpload', true);
    emit('imageSelected', compressedImage as File);
    isLoading.value = false;
  } catch (e) {
    showAlertError(e);
    localImage.value = null;
    cleanInput();
    emit('finishUpload', true);
    isLoading.value = false;
  }
}

function cleanInput() {
  const input = fileInput.value;
  if (!input) return;
  input.type = 'text';
  input.type = 'file';
}

function validateDimensions(evt: Event) {
  const reader = new FileReader();
  reader.readAsDataURL(fileFromEvent(evt));
  reader.onload = (e) => {
    const image = new Image();
    image.src = (e.target as FileReader).result as string;
    image.onload = () => {
      const height = image.height;
      const width = image.width;
      if (height < 150 || width < 150) {
        showAlertError('This file es too small, please select a photo with height and width of at least 150 pixels');
        localImage.value = null;
      } else {
        showCrop(evt);
      }
    };
  };
}

function validateImage(evt: Event) {
  const file = fileFromEvent(evt);
  if (validateFile(file)) {
    validateDimensions(evt);
  } else {
    localImage.value = null;
  }
}
</script>
