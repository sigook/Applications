<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ "Can you Lift up to" }}</span>
      <span class="width-70">
        <b-tag v-if="props.worker.lift" type="is-info is-light" size="is-medium" rounded>{{ props.worker.lift.value }}</b-tag>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalLift = true"></b-button>
    </div>
    <b-modal v-model="modalLift" width="500px">
      <lift-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import LiftEdit from './WorkLiftForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalLift = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalLift.value = false;
}
</script>
