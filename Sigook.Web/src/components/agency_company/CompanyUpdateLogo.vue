<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light small-container update-logo-modal">
              <span class="fz1 fw-bold ">Logo</span>
              <button @click="cancelUpdate" type="button" class="cross-icon">{{ 'Close' }}</button>

              <upload-image v-if="newLogo" @imageSelected="profileImg => newLogo = { fileName: profileImg }"
                :edited-image="props.logo" :required="false" @onUpload="() => subscribe('file')"
                @finishUpload="() => unsubscribe()" class="mx-auto my-2">
              </upload-image>
              <div class="text-center">
                <b-button type="is-danger" rounded class="mt-3 me-2" @click="cancelUpdate">Cancel</b-button>
                <b-button type="is-primary" rounded class="mt-3" @click="updateLogo()">Save</b-button>
              </div>
            </div>
          </div>
        </div>
      </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import UploadImage from "@/components/PreviewImage.vue";
import { usePubSub } from "@/composables/usePubSub";

const props = defineProps<{ logo?: any }>();
const emit = defineEmits<{
  (e: 'save', value: any): void;
  (e: 'cancel'): void;
}>();

const { subscribe, unsubscribe } = usePubSub();

const newLogo = ref<any>({});
const isLoading = ref(false);

function updateLogo() {
  if (!newLogo.value || props.logo.fileName === newLogo.value.fileName) {
    emit("cancel");
  } else {
    emit("save", newLogo.value);
  }
}

function cancelUpdate() {
  emit("cancel");
}

if (props.logo != null) {
  newLogo.value = Object.assign({}, props.logo);
}
</script>
<style lang="scss">
.update-logo-modal {

  .fz1 {
    margin-bottom: 20px;
    display: inline-block;
    position: relative;
    top: -5px;

  }

  .update-image label {
    z-index: 3;
    color: white;

  }
}
</style>
