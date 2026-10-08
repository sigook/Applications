<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field label="Skills">
          <b-taginput ref="workSkillFormTagInput" v-model="selectedSkills" autocomplete :data="filteredSkills"
            open-on-focus field="skill" icon="label" placeholder="Select or Add Skill" :create-tag="createTag" allow-new
            @typing="getFilteredSkills" append-to-body>
          </b-taginput>
        </b-field>
      </div>
      <div class="column is-12 mt-5">
        <b-button type="is-primary" @click="saveWorkerSkills()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { getSkills } from "@/shared/api/catalogApi";
import { createWorkerSkills } from '@/shared/worker-profile/api';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import type { Skill } from '@/shared/types/common';

const props = defineProps<{ data?: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const skills = ref<Skill[]>([]);
const selectedSkills = ref<Skill[]>([]);
const filteredSkills = ref<Skill[]>([]);

function saveWorkerSkills() {
  isLoading.value = true;
  const skillsToInsert = selectedSkills.value.map(skill => skill.skill);
  createWorkerSkills(props.data.id, skillsToInsert)
    .then(() => {
      isLoading.value = false;
      emit('closeModal', true);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function createTag(skill: string | Skill): Skill {
  if (typeof skill === 'string') {
    const tag: Skill = { skill };
    skills.value.push(tag);
    return tag;
  }
  return skill;
}

function loadSkills() {
  isLoading.value = true;
  getSkills().then(response => {
    isLoading.value = false;
    skills.value = response;
    filteredSkills.value = skills.value;
  });
}

function getFilteredSkills(text: string) {
  filteredSkills.value = skills.value.filter((option) => option.skill.toLowerCase().includes(text.toLowerCase()));
}

if (props.data != null) {
  selectedSkills.value = props.data.skills;
  loadSkills();
}
</script>
