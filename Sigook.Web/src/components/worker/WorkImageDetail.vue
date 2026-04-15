<template>
  <div class="hover-actions">
    <b-loading v-model="isLoading"></b-loading>
    <div class="worker-profile-image">
      <img v-if="data.profileImage" :src="data.profileImage.pathFile">
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
                @imageSelected="profileImg => this.profileImageFile = profileImg"
                :edited-image="this.data.profileImage" :required="true" @onUpload="() => subscribe('file')"
                @finishUpload="() => unsubscribe()" class="margin-10-auto">
              </upload-image>
              <div class="text-center">
                <button class="background-btn md-btn red-button btn-radius margin-top-15 margin-right uppercase"
                  @click="showEditModal = false" type="button">{{ "Cancel" }}</button>
                <button class="background-btn md-btn primary-button btn-radius margin-top-15 uppercase"
                  @click="createWorkerImage()" type="button">{{ "Save" }}</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal -->


  </div>
</template>
<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { showAlertError } from "@/utils/toast";
import { createMultipartFormData, generateFileName } from "@/utils/buildWorkerFormData";
import { usePubSub } from '@/composables/usePubSub';
import { createWorkerImage } from '@/api/workerApi';
export default {
  setup() {
    return { ...usePubSub() };
  },
  props: ['data'],
    data() {
    return {
      showEditModal: false,
      profileImage: {},
      profileImageFile: null,
      isLoading: false
    }
  },
  methods: {
    async createWorkerImage() {
      if (!this.profileImageFile) {
        showAlertError('Please select an image');
        return;
      }

      this.isLoading = true;

      try {
        // Generate unique filename with GUID (same pattern as registration)
        const generatedFileName = generateFileName('ProfileImage', this.profileImageFile.name);

        // Compress the image
        let fileToUpload;
        try {
          fileToUpload = await this.compressFile(this.profileImageFile);
        } catch {
          fileToUpload = this.profileImageFile;
        }

        // Create FormData
        const formData = new FormData();
        formData.append(generatedFileName, fileToUpload, generatedFileName);

        await createWorkerImage(this.data.id, formData);

        this.isLoading = false;
        this.showEditModal = false;
        this.$emit('updateProfile', true);
      } catch (error) {
        this.isLoading = false;
        showAlertError(error);
      }
    }
  },
  components: {
    UploadImage: defineAsyncComponent(() => import("../../components/PreviewImage.vue"))
  },
  created() {
    if (this.data != null) {
      this.profileImage = Object.assign({}, this.data.profileImage);
    }
  }

}
</script>