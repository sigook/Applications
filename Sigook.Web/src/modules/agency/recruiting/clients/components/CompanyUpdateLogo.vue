<template>
  <div class="p-4">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="has-text-centered fz1 mb-4">Logo</h2>
    <upload-image @imageSelected="(file: File) => logoFile = file"
      :edited-image="logo" :required="true" class="mx-auto my-2">
    </upload-image>
    <div class="mt-4">
      <b-button @click="emit('cancel')">Cancel</b-button>
      <b-button type="is-primary" class="ml-2" @click="saveLogo()">Save</b-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from '@/shared/utils/toast';
import { updateAgencyCompanyProfileLogo } from '@/modules/agency/recruiting/clients/api';
import UploadImage from '@/shared/ui/PreviewImage.vue';
import type { CovenantFileModel } from '@/shared/types/common';

const { profileId, logo } = defineProps<{ profileId: string; logo?: CovenantFileModel | null }>();
const emit = defineEmits<{
  (e: 'updated'): void;
  (e: 'cancel'): void;
}>();

const logoFile = ref<File | null>(null);
const isLoading = ref(false);

async function saveLogo() {
  if (!logoFile.value) {
    showAlertError('Please select an image');
    return;
  }
  isLoading.value = true;
  try {
    await updateAgencyCompanyProfileLogo(profileId, logoFile.value);
    emit('updated');
  } catch (error) {
    showAlertError(error);
  } finally {
    isLoading.value = false;
  }
}
</script>
