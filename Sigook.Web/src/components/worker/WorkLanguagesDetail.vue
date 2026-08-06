<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ "Languages" }}</span>
      <span class="width-70 items">
        <b-taglist>
          <b-tag v-for="item in props.worker.languages" :key="'languages' + item.value" type="is-info is-light" size="is-medium" rounded>{{ item.value }}</b-tag>
        </b-taglist>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalLanguages = true"></b-button>
    </div>
    <b-modal custom-content-class="card" v-model="modalLanguages" width="500px">
      <languages-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import LanguagesEdit from './WorkLanguagesForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalLanguages = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalLanguages.value = false;
}
</script>
