<template>
  <div class="container-image">
    <b-loading v-model="isLoading"></b-loading>
    <div class="update-image" :class="{ 'is-danger': errors.has('file') }">
      <input type="file" id="file" @change="validateImage" accept="image/*" title="logo" name="file"
        v-validate="rules" :ref="'file'" />

      <label for="file">{{ "Upload Picture" }}</label>
      <img v-if="localImage" :src="localImage" alt="logo" style="z-index: 1;">
      <default-image v-if="showDefault && !localImage" :name="name" class="img-100"></default-image>

    </div>
    <span v-show="errors.has('file')" class="help is-danger no-margin">{{ errors.first('file') }}</span>

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
import { showAlertError } from "@/utils/toast";
import { compressFile } from '@/utils/compressFile';

export default {
  inject: ['$validator'],
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
      hasImage: false
    }
  },
  components: {
    cropImage: () => import("./CropImage.vue")
  },
  computed: {
    rules() {
      let oRules = {
        required: false,
        size: 10000,
        ext: ['jpeg', 'jpg', 'png', 'gif', 'svg']
      }
      if (this.required) {
        oRules.required = !this.hasImage;
      }
      return oRules;
    }
  },
  methods: {
    compressFile,
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
      const input = this.$refs['file'];
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
      Promise.all([
        this.$validator.validate('file')
          .then(isValid => {
            if (isValid) {
              this.validateDimensions(evt);
            } else {
              this.localImage = null;
            }
          })
      ]);
    }
  }
}
</script>