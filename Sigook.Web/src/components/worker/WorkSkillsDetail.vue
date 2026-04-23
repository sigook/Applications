<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ "Skills" }}</span>
      <span class="width-70 items">
        <b-taglist>
          <b-tag v-for="(item, index) in props.worker.skills" :key="'skills' + index" type="is-info is-light" size="is-medium" rounded>{{ item.skill }}</b-tag>
        </b-taglist>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalSkills = true"></b-button>
    </div>
    <b-modal v-model="modalSkills" width="500px">
      <skills-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import SkillsEdit from './WorkSkillsForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalSkills = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalSkills.value = false;
}
</script>
