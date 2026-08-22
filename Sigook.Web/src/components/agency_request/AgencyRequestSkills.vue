<template>
  <section class="columns is-multiline">
    <b-loading v-model="isLoading"></b-loading>
    <div class="column is-8-mobile is-8">
      <b-field label="Skills">
        <skills-form :existingSkills="data" v-if="props.request.canEdit" @onPressAdd="(item) => addSkill(item)"
          @onDelete="(item) => removeSkill(item)" />
      </b-field>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getAgencyRequestSkill, postAgencyRequestSkill, deleteAgencyRequestSkill } from "@/api/agencyRequestApi";
import SkillsForm from '../FormSkillAdd.vue';

const props = defineProps<{ request: any }>();

const isLoading = ref(false);
const data = ref<any[]>([]);

function loadSkills() {
  isLoading.value = true;
  getAgencyRequestSkill(props.request.id)
    .then(response => {
      isLoading.value = false;
      data.value = response;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function addSkill(item: any) {
  isLoading.value = true;
  postAgencyRequestSkill(props.request.id, { skill: item.skill })
    .then(() => {
      isLoading.value = false;
      loadSkills();
    }).catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function removeSkill(item: any) {
  isLoading.value = true;
  deleteAgencyRequestSkill(props.request.id, item.id)
    .then(() => {
      isLoading.value = false;
      loadSkills();
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

loadSkills();
</script>
