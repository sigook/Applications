<template>
  <div>
    <b-taginput size="is-small" ref="workSkillFormTagInput" v-model="localSkills" autocomplete :data="filteredSkills"
      open-on-focus field="skill" icon="label" placeholder="Select or Add Skill" allow-new @typing="getFilteredSkills"
      @add="addSkill" @keydown.esc="closeTagInput" @remove="deleteSkill">
    </b-taginput>
  </div>
</template>
<script lang="ts">
import { getSkills } from "@/api/catalogApi";
export default {
  props: ['existingSkills'],
  data() {
    return {
      showInput: false,
      skills: [],
      filteredSkills: [],
      localSkills: []
    }
  },
  watch: {
    existingSkills: {
      handler(newVal) {
        this.localSkills = [...(newVal || [])];
      },
      immediate: true
    },
    localSkills(newVal) {
      this.$emit('update:existingSkills', newVal);
    }
  },
  methods: {
    addSkill(skill) {
      if (!skill.skill) {
        skill = { skill };
      }
      this.$emit('onPressAdd', skill);
    },
    deleteSkill(skill) {
      this.$emit('onDelete', skill);
    },
    getFilteredSkills(text) {
      this.filteredSkills = this.skills.filter((option) => option.skill.toLowerCase().includes(text.toLowerCase()));
    },
    closeTagInput() {
      this.showInput = false
    }
  },
  created() {
    getSkills()
      .then(response => {
        this.skills = response;
        this.filteredSkills = this.skills;
      });
  }
}
</script>