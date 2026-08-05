<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ "Location preferences" }}</span>
      <span class="width-70 items">
        <b-taglist>
          <b-tag v-for="item in props.worker.locationPreferences" :key="'locationpref' + item.value" type="is-info is-light" size="is-medium" rounded>{{ item.value }}</b-tag>
        </b-taglist>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalLocation = true"></b-button>
    </div>
    <b-modal custom-content-class="card" v-model="modalLocation" width="500px" max-height="80vh">
      <location-preferences-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import LocationPreferencesEdit from './WorkLocationPreferencesForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalLocation = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalLocation.value = false;
}
</script>
