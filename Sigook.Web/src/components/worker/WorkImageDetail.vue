<template>
  <div class="hover-actions">
    <b-loading v-model="isLoading"></b-loading>
    <div class="worker-profile-image">
      <img v-if="props.data.profileImage" :src="props.data.profileImage.pathFile">
      <button class="actions btn-icon-sm btn-icon-edit" type="button" @click="showEditModal = true">Edit</button>
    </div>
    <b-modal custom-content-class="card" v-model="showEditModal" width="400px" :destroy-on-hide="true">
      <div class="p-4">
        <h2 class="has-text-centered fz1 mb-4">Profile Photo</h2>
        <upload-image v-if="profileImage"
          @imageSelected="(profileImg: File) => profileImageFile = profileImg"
          :edited-image="props.data.profileImage" :required="true" @onUpload="() => pubSub.subscribe('file')"
          @finishUpload="() => pubSub.unsubscribe()" class="mx-auto my-2">
        </upload-image>
        <div class="mt-4">
          <b-button @click="showEditModal = false">Cancel</b-button>
          <b-button type="is-primary" class="ml-2" @click="createWorkerImageHandler()">Save</b-button>
        </div>
      </div>
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from '@/utils/toast';
import { generateFileName } from '@/utils/fileNaming';
import { compressFile } from '@/utils/compressFile';
import { usePubSub } from '@/composables/usePubSub';
import { createWorkerImage } from '@/api/workerApi';
import UploadImage from '../../components/PreviewImage.vue';

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const pubSub = usePubSub();

const showEditModal = ref(false);
const profileImage = ref<any>({});
const profileImageFile = ref<File | null>(null);
const isLoading = ref(false);

async function createWorkerImageHandler() {
  if (!profileImageFile.value) {
    showAlertError('Please select an image');
    return;
  }

  isLoading.value = true;

  try {
    // Generate unique filename with GUID (same pattern as registration)
    const generatedFileName = generateFileName('ProfileImage', profileImageFile.value.name);

    // Compress the image
    let fileToUpload: Blob | File;
    try {
      fileToUpload = await compressFile(profileImageFile.value);
    } catch {
      fileToUpload = profileImageFile.value;
    }

    // Create FormData
    const formData = new FormData();
    formData.append(generatedFileName, fileToUpload, generatedFileName);

    await createWorkerImage(props.data.id, formData);

    isLoading.value = false;
    showEditModal.value = false;
    emit('updateProfile', true);
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

if (props.data != null) {
  profileImage.value = Object.assign({}, props.data.profileImage);
}
</script>

<style lang="scss" scoped>
.worker-profile-image {
  text-align: center;

  img {
    border-radius: 50%;
    width: 100px;
    height: 100px;
    object-fit: cover;
  }

  .btn-icon-edit {
    margin-right: -10px;
  }

  @media (max-width: 767px) {
    position: relative;
  }
}
</style>
