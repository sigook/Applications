<template>
    <div class="p-4">
        <h2 class="has-text-centered fz1 mb-4">Move and scale</h2>
        <vue-cropper
                ref='cropper'
                :guides="true"
                :view-mode="2"
                drag-mode="crop"
                :auto-crop-area=".8"
                :min-container-width="250"
                :min-container-height="250"
                :background="true"
                :rotatable="true"
                :src="image"
                alt="Source Image"
                :img-style="{ 'width': '400px', 'height': '400px' }">

        </vue-cropper>

        <div class="options">
            <div class="rotate">
                <b-button type="is-light" size="is-small" icon-left="rotate-left" @click="rotate(-90)"></b-button>
                <b-button type="is-light" size="is-small" icon-left="rotate-right" @click="rotate(90)"></b-button>
            </div>

            <div class="actions">
                <b-button type="is-light" size="is-small" @click="closeModal">Cancel</b-button>
                <b-button type="is-primary" size="is-small" @click="uploadCroppedImage()">Crop and Upload</b-button>
            </div>

        </div>

    </div>
</template>
<script setup lang="ts">
import { ref, defineAsyncComponent, type Component } from 'vue';
import type Cropper from 'cropperjs';
import 'cropperjs/dist/cropper.css';

const VueCropper = defineAsyncComponent(() => import("vue-cropperjs").then(m => (m as { default: Component }).default));

defineProps<{ image?: string }>();
const emit = defineEmits<{
  (e: 'onCrop', file: File): void;
  (e: 'closeModal'): void;
}>();

const cropper = ref<Pick<Cropper, 'getCroppedCanvas' | 'rotate'> | null>(null);

function uploadCroppedImage() {
  cropper.value.getCroppedCanvas().toBlob((blob: Blob) => {
    const parts = blob.type.split("/");
    const extension = parts[1];
    const file = new File([blob], "uploaded_file." + extension, { type: blob.type, lastModified: Date.now() });
    emit('onCrop', file);
  });
}

function rotate(rotationAngle: number) {
  cropper.value.rotate(rotationAngle);
}

function closeModal() {
  emit('closeModal');
}
</script>
<style lang="scss" scoped>

    .options {
        padding-top: 1rem;
        display: flex;
        justify-content: space-between;
        .rotate {
            button {
                margin-right: 3px;
            }
        }

        .actions {
            button {
                margin-left: 5px;
            }
        }

        @media(max-width: 767px){
            .actions {
                display: flex;}
        }

    }

</style>
