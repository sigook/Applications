<template>
  <div>
    <b-taginput size="is-small" ref="workSkillFormTagInput" v-model="localSkills" autocomplete :data="filteredSkills"
      open-on-focus field="skill" icon="label" placeholder="Select or Add Skill" allow-new @typing="getFilteredSkills"
      @add="addSkill" @keydown.esc="closeTagInput" @remove="deleteSkill">
    </b-taginput>
  </div>
</template>
<script setup lang="ts">
import { ref, watch } from 'vue';
import { getSkills } from "@/api/catalogApi";

const props = defineProps<{ existingSkills?: any[] }>();
const emit = defineEmits<{
  (e: 'update:existingSkills', v: any[]): void;
  (e: 'onPressAdd', skill: any): void;
  (e: 'onDelete', skill: any): void;
}>();

const showInput = ref(false);
const skills = ref<any[]>([]);
const filteredSkills = ref<any[]>([]);
const localSkills = ref<any[]>([]);

watch(() => props.existingSkills, (newVal) => {
  localSkills.value = [...(newVal || [])];
}, { immediate: true });

watch(localSkills, (newVal) => {
  emit('update:existingSkills', newVal);
});

function addSkill(skill: any) {
  if (!skill.skill) {
    skill = { skill };
  }
  emit('onPressAdd', skill);
}

function deleteSkill(skill: any) {
  emit('onDelete', skill);
}

function getFilteredSkills(text: string) {
  filteredSkills.value = skills.value.filter((option) => option.skill.toLowerCase().includes(text.toLowerCase()));
}

function closeTagInput() {
  showInput.value = false;
}

getSkills().then(response => {
  skills.value = response;
  filteredSkills.value = skills.value;
});
</script>

<style scoped lang="scss">
// Long skill names (e.g. "Forklift / Scissors Lift Operator - …") otherwise
// stay on one line and force the Skills column — and the whole table — wider
// than the viewport. Let tag text wrap so the column can shrink to fit.
:deep(.taginput .taginput-container) {
  max-width: 100%;
}

:deep(.taginput .tag) {
  height: auto;
  min-height: 2em;
  white-space: normal;
  word-break: break-word;
  text-align: left;
}
</style>
