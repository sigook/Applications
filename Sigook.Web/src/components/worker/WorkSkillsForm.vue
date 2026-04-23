<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12">
        <b-field label="Skills">
          <b-taginput ref="workSkillFormTagInput" v-model="selectedSkills" autocomplete :data="filteredSkills"
            open-on-focus field="skill" icon="label" placeholder="Select or Add Skill" :create-tag="createTag" allow-new
            @typing="getFilteredSkills" append-to-body>
          </b-taginput>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="saveWorkerSkills()">
          {{ "Save" }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getSkills } from "@/api/catalogApi";
import { createWorkerSkills } from '@/api/workerApi';

const props = defineProps<{ data?: any }>();
const emit = defineEmits<{ (e: 'closeModal', value: boolean): void }>();

const isLoading = ref(false);
const skills = ref<any[]>([]);
const selectedSkills = ref<any[]>([]);
const filteredSkills = ref<any[]>([]);

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

function createTag(skill: any) {
  if (!skill.skill) {
    skill = { skill };
    skills.value.push(skill);
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
