<template>
  <div class="request-skills">
    <b-loading v-model="isLoading" :is-full-page="false"></b-loading>
    <skills-form v-if="canEdit" :existingSkills="skills" @onPressAdd="(item) => addSkill(item)"
      @onDelete="(item) => removeSkill(item)" />
    <div v-else-if="skills.length" class="request-skills-list">
      <span v-for="item in skills" :key="item.id ?? item.skill" class="request-skills-tag">{{ item.skill }}</span>
    </div>
    <span v-else class="request-skills-empty">No skills</span>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { postAgencyRequestSkill, deleteAgencyRequestSkill } from "@/modules/agency/recruiting/requests/api";
import type { AgencyRequestSkillModel } from '@/modules/agency/recruiting/requests/types';
import SkillsForm from '@/shared/ui/FormSkillAdd.vue';

const { requestId, canEdit } = defineProps<{ requestId: string; canEdit?: boolean }>();
const skills = defineModel<AgencyRequestSkillModel[]>({ required: true });

const isLoading = ref(false);

async function addSkill(item: { skill: string }) {
  isLoading.value = true;
  try {
    const response = await postAgencyRequestSkill(requestId, { skill: item.skill });
    isLoading.value = false;
    skills.value = [...skills.value, { id: response.id, skill: item.skill }];
  } catch (error) {
    isLoading.value = false;
    skills.value = [...skills.value];
    showAlertError(error);
  }
}

async function removeSkill(item: AgencyRequestSkillModel) {
  if (!item.id) return;
  isLoading.value = true;
  try {
    await deleteAgencyRequestSkill(requestId, item.id);
    isLoading.value = false;
    skills.value = skills.value.filter(x => x.id !== item.id);
  } catch (error) {
    isLoading.value = false;
    skills.value = [...skills.value];
    showAlertError(error);
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.request-skills {
  position: relative;
}

.request-skills-list {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.request-skills-tag {
  padding: 3px 10px;
  border: 1px solid $border-input;
  border-radius: 999px;
  font-size: 0.8rem;
  color: $navy;
}

.request-skills-empty {
  font-size: 0.9rem;
  color: $grey-light;
}
</style>
