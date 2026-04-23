<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ "Email" }}</span>
      <span class="width-70">
        <p class="fw-200 margin-0">{{ props.worker.email }}</p>
      </span>
      <button class="actions btn-icon-sm btn-icon-edit button-top-m8" type="button"
        @click="modalEmail = true">Edit</button>
    </div>
    <b-modal v-model="modalEmail" width="500px">
      <email-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import EmailEdit from './WorkEmailForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalEmail = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalEmail.value = false;
}
</script>
