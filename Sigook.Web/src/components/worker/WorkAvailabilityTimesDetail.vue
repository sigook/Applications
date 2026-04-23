<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ "Available Time" }}</span>
      <span class="width-70 items">
        <b-taglist>
          <b-tag v-for="item in props.worker.availabilityTimes" :key="item.value" type="is-info is-light" size="is-medium" rounded>{{ item.value }}</b-tag>
        </b-taglist>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalAvailability = true"></b-button>
    </div>
    <b-modal v-model="modalAvailability" width="500px">
      <availability-times-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import AvailabilityTimesEdit from './WorkAvailabilityTimesForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalAvailability = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalAvailability.value = false;
}
</script>
