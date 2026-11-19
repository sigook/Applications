<template>
  <div class="hover-actions">
    <b-loading v-model="isLoading"></b-loading>
    <div class="worker-profile-image">
      <img v-if="data.profileImage" :src="data.profileImage.pathFile">
      <button class="actions btn-icon-sm" type="button" @click="showEditModal = true" title="Edit">
        <b-icon icon="pencil" size="is-small"></b-icon>
      </button>
    </div>
    <b-modal custom-content-class="card" v-model="showEditModal" width="400px" :destroy-on-hide="true">
      <div class="p-4">
        <h2 class="has-text-centered fz1 mb-4">Profile Photo</h2>
        <upload-image @imageSelected="(file: File) => profileImageFile = file"
          :edited-image="data.profileImage" :required="true" class="mx-auto my-2">
        </upload-image>
        <div class="mt-4">
          <b-button @click="showEditModal = false">Cancel</b-button>
          <b-button type="is-primary" class="ml-2" @click="saveImage()">Save</b-button>
        </div>
      </div>
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from '@/shared/utils/toast';
import { createWorkerImage } from '@/shared/worker-profile/api';
import UploadImage from '@/shared/ui/PreviewImage.vue';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

const { data } = defineProps<{ data: WorkerProfileDetail }>();
const emit = defineEmits<(e: 'updateProfile') => void>();

const showEditModal = ref(false);
const profileImageFile = ref<File | null>(null);
const isLoading = ref(false);

async function saveImage() {
  if (!profileImageFile.value) {
    showAlertError('Please select an image');
    return;
  }
  isLoading.value = true;
  try {
    await createWorkerImage(data.id, profileImageFile.value);
    showEditModal.value = false;
    emit('updateProfile');
  } catch (error) {
    showAlertError(error);
  } finally {
    isLoading.value = false;
  }
}
</script>

<style lang="scss" scoped>
.worker-profile-image {
  text-align: center;

  img {
    border-radius: 50%;
    width: 72px;
    height: 72px;
    object-fit: cover;
  }

  .btn-icon-sm {
    margin-right: -10px;
  }

  @media (max-width: 767px) {
    position: relative;
  }
}
</style>
