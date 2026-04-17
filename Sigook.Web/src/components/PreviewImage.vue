<template>
  <div class="container-image">
    <b-loading v-model="isLoading"></b-loading>
    <div class="update-image" :class="{ 'is-danger': !!fileError }">
      <input type="file" id="file" @change="validateImage" accept="image/*" title="logo" name="file"
        :ref="'file'" />

      <label for="file">{{ "Upload Picture" }}</label>
      <img v-if="localImage" :src="localImage" alt="logo" style="z-index: 1;">
      <default-image v-if="showDefault && !localImage" :name="name" class="img-100"></default-image>

    </div>
    <span v-show="fileError" class="help is-danger no-margin">{{ fileError }}</span>

    <!-- custom modal -->
    <transition name="modal">
      <div v-if="modalValidation" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container small-container">
              <button @click="modalValidation = false" class="cross-icon" type="button">{{ 'Close' }}</button>
              <crop-image :image="cropImage" @onCrop="response => showImage(response)"
                @closeModal="() => modalValidation = false"></crop-image>
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
import { compressFile } from '@/utils/compressFile';

const ALLOWED_EXTENSIONS = ['jpeg', 'jpg', 'png', 'gif', 'svg'];
const MAX_SIZE_KB = 10000;

export default {
  props: [
    'editedImage',
    'required',
    'name',
    'showDefault'
  ],
  data() {
    return {
      pathImage: null,
      isLoading: false,
      selected: false,
      localImage: this.editedImage ? this.editedImage.pathFile : null,
      modalValidation: false,
      cropImage: '',
      hasImage: false,
      fileError: '' as string,
    }
  },
  components: {
    cropImage: defineAsyncComponent(() => import("./CropImage.vue"))
  },
  methods: {
    compressFile,
    validateFile(file: File): boolean {
      this.fileError = '';
      if (!file) {
        if (this.required && !this.hasImage) {
          this.fileError = 'Image is required';
          return false;
        }
        return true;
      }
      const ext = file.name.split('.').pop()?.toLowerCase() || '';
      if (!ALLOWED_EXTENSIONS.includes(ext)) {
        this.fileError = `Allowed formats: ${ALLOWED_EXTENSIONS.join(', ')}`;
        return false;
      }
      if (file.size / 1024 > MAX_SIZE_KB) {
        this.fileError = `File exceeds ${MAX_SIZE_KB / 1000}MB limit`;
        return false;
      }
      return true;
    },
    showCrop(evt) {
      if ((document as any).documentMode || /Edge/.test(navigator.userAgent)) {
        this.showImage(evt.target.files[0])
      } else {
        const file = evt.target.files[0];
        this.cropImage = URL.createObjectURL(file);
        this.modalValidation = true;
      }
    },
    async showImage(output) {
      this.isLoading = true;
      this.hasImage = true;
      this.localImage = URL.createObjectURL(output);
      this.modalValidation = false;
      this.$emit('onUpload', true);

      try {
        const compressedImage = await this.compressFile(output);
        this.cleanInput();
        this.$emit('finishUpload', true);
        this.$emit('imageSelected', compressedImage);
        this.isLoading = false;
      } catch (e) {
        showAlertError(e);
        this.localImage = null;
        this.cleanInput();
        this.$emit('finishUpload', true);
        this.isLoading = false;
      }
    },
    cleanInput() {
      const input = this.$refs['file'] as HTMLInputElement;
      input.type = 'text';
      input.type = 'file';
    },
    validateDimensions(evt) {
      let reader = new FileReader();
      reader.readAsDataURL(evt.target.files[0]);
      reader.onload = (e) => {
        let image = new Image();
        image.src = (e.target as FileReader).result as string;
        image.onload = () => {
          let height = image.height;
          let width = image.width;
          if (height < 150 || width < 150) {
            showAlertError('This file es too small, please select a photo with height and width of at least 150 pixels');
            this.localImage = null;
          } else {
            this.showCrop(evt);
          }
        };
      };

    },
    validateImage(evt) {
      const file = evt.target.files[0];
      if (this.validateFile(file)) {
        this.validateDimensions(evt);
      } else {
        this.localImage = null;
      }
    }
  }
}
</script>
