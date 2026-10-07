<template>
  <div class="p-4">
    <h2 class="has-text-centered fz1 mb-4">Logo</h2>
    <upload-image v-if="newLogo" @imageSelected="profileImg => newLogo = { fileName: profileImg }"
      :edited-image="props.logo" :required="false" @onUpload="() => subscribe('file')"
      @finishUpload="() => unsubscribe()" class="mx-auto my-2">
    </upload-image>
    <div class="mt-4">
      <b-button @click="cancelUpdate">Cancel</b-button>
      <b-button type="is-primary" class="ml-2" @click="updateLogo()">Save</b-button>
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
