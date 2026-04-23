<template>
  <div class="hover-actions">
    <b-loading v-model="isLoading"></b-loading>
    <div class="worker-profile-image">
      <img v-if="props.data.profileImage" :src="props.data.profileImage.pathFile">
      <button class="actions btn-icon-sm btn-icon-edit" type="button" @click="showEditModal = true">Edit</button>
    </div>
    <!-- custom modal -->
    <transition name="modal">
      <div v-if="showEditModal" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light small-container worker-profile-modal">
              <span class="fz1 fw-700 ">Profile Photo</span>
              <button @click="showEditModal = false" type="button" class="cross-icon">
                {{ 'Close' }}
              </button>
              <upload-image v-if="profileImage"
                @imageSelected="(profileImg: File) => profileImageFile = profileImg"
                :edited-image="props.data.profileImage" :required="true" @onUpload="() => pubSub.subscribe('file')"
                @finishUpload="() => pubSub.unsubscribe()" class="margin-10-auto">
              </upload-image>
              <div class="text-center">
                <button class="background-btn md-btn red-button btn-radius margin-top-15 margin-right uppercase"
                  @click="showEditModal = false" type="button">{{ "Cancel" }}</button>
                <button class="background-btn md-btn primary-button btn-radius margin-top-15 uppercase"
                  @click="createWorkerImageHandler()" type="button">{{ "Save" }}</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal -->


  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from '@/utils/toast';
import { generateFileName } from '@/utils/buildWorkerFormData';
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
