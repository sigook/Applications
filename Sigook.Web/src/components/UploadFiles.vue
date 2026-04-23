<template>
  <div class="inline">
    <b-loading v-model="isLoading"></b-loading>
    <input type="file" class="input-70 input-border" @change="validateFile($event)"
      accept=".pdf,.doc,.docx,.xls,.xlsx,image/*" title="logo" :name="'file' + name + format"
      :class="{ 'is-danger': errorMessage }" :disabled="disabled"
      :ref="fileRef" :id="id" />
    <span v-show="errorMessage" class="help is-danger no-margin">
      {{ errorMessage }}
    </span>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { uploadFile } from "@/utils/fileUpload";

const ALLOWED_EXTENSIONS = ['pdf', 'jpeg', 'jpg', 'png', 'gif', 'doc', 'docx', 'xls', 'xlsx'];
const MAX_SIZE_KB = 15500;

const props = defineProps<{
  name?: string;
  format?: string;
  required?: boolean;
  id?: string;
  disabled?: boolean;
}>();

const emit = defineEmits<{
  (e: 'onUpload', v: boolean): void;
  (e: 'finishUpload', v: boolean): void;
  (e: 'fileSelected', v: string | null): void;
}>();

const pathFile = ref<string | null>(null);
const isLoading = ref(false);
const selected = ref(false);
const errorMessage = ref('');
const fileRefEl = ref<HTMLInputElement | null>(null);

function fileRef(el: any) {
  fileRefEl.value = el as HTMLInputElement | null;
}

function onSelectFile(evt: Event) {
  isLoading.value = true;
  emit('onUpload', true);
  uploadFile(evt, props.format, props.name)
    .then((response: string) => {
      pathFile.value = response;
      cleanInput();
      emit('finishUpload', true);
      emit('fileSelected', pathFile.value);
      isLoading.value = false;
    })
    .catch((e: any) => {
      showAlertError(e);
      cleanInput();
      emit('finishUpload', true);
      isLoading.value = false;
    });
}

function cleanInput() {
  const input = fileRefEl.value;
  if (!input) return;
  input.type = 'text';
  input.type = 'file';
}

function validateFile(evt: Event) {
  selected.value = true;
  errorMessage.value = '';
  const target = evt.target as HTMLInputElement;
  const file = target.files && target.files[0];

  if (!file) {
    if (props.required) errorMessage.value = 'File is required';
    return;
  }

  const ext = file.name.split('.').pop()?.toLowerCase() || '';
  if (!ALLOWED_EXTENSIONS.includes(ext)) {
    errorMessage.value = `File extension not allowed. Allowed: ${ALLOWED_EXTENSIONS.join(', ')}`;
    return;
  }

  const sizeKb = file.size / 1024;
  if (sizeKb > MAX_SIZE_KB) {
    errorMessage.value = `File size must be less than ${MAX_SIZE_KB} KB`;
    return;
  }

  onSelectFile(evt);
}
</script>

<style lang="scss" scoped>
.inline {
  display: inline-block;
  width: calc(100% - 32px);

  input {
    width: 100%;
  }
}
</style>
