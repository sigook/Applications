<template>
  <div class="p-4">
    <h2 class="has-text-centered fz1 mb-4">Logo</h2>
    <upload-image @imageSelected="(file) => newLogo = file"
      :edited-image="props.logo" :required="false" @onUpload="() => subscribe('file')"
      @finishUpload="() => unsubscribe()" class="mx-auto my-2">
    </upload-image>
    <div class="mt-4">
      <b-button @click="cancelUpdate">Cancel</b-button>
      <b-button type="is-primary" class="ml-2" :disabled="isLoadingFiles" @click="updateLogo()">Save</b-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import UploadImage from "@/shared/ui/PreviewImage.vue";
import { usePubSub } from "@/shared/composables/usePubSub";
import type { CovenantFileModel } from '@/shared/types/common';

const props = defineProps<{ logo?: CovenantFileModel | null }>();
const emit = defineEmits<{
  (e: 'save', value: File): void;
  (e: 'cancel'): void;
}>();

const { subscribe, unsubscribe, isLoadingFiles } = usePubSub();

const newLogo = ref<File | null>(null);

function updateLogo() {
  if (!newLogo.value) {
    emit("cancel");
  } else {
    emit("save", newLogo.value);
  }
}

function cancelUpdate() {
  emit("cancel");
}
</script>
