<template>
  <section>
    <div class="d-flex align-items-center justify-content-between">
      <h3 class="section-title">{{ "Account" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalEmail = true"></b-button>
    </div>
    <div class="worker-documents">
      <div>
        <span>{{ 'Email' }}</span>
        <span>
          <p class="fw-light m-0">{{ props.worker.email }}</p>
        </span>
      </div>
    </div>
    <b-modal v-model="modalEmail" width="500px">
      <email-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
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
