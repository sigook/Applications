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
        <b-button type="is-primary" @click="createWorkerSkills()">
          {{ $t("Save") }}
        </b-button>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: ['data'],
  data() {
    return {
      isLoading: false,
      skills: [],
      selectedSkills: [],
      filteredSkills: [],
    }
  },
  methods: {
    createWorkerSkills() {
      this.isLoading = true;
      const skillsToInsert = this.selectedSkills.map(skill => skill.skill);
      this.$store.dispatch('worker/createWorkerSkills', { profileId: this.data.id, model: skillsToInsert })
        .then(() => {
          this.isLoading = false;
          this.$emit('closeModal', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    createTag(skill) {
      if (!skill.skill) {
        skill = { skill };
        this.skills.push(skill);
      }
      return skill;
    },
    getSkills() {
      this.isLoading = true;
      this.$store.dispatch('getSkills').then(response => {
        this.isLoading = false;
        this.skills = response;
        this.filteredSkills = this.skills;
      });
    },
    getFilteredSkills(text) {
      this.filteredSkills = this.skills.filter((option) => option.skill.toLowerCase().includes(text.toLowerCase()));
    }
  },
  created() {
    if (this.data != null) {
      this.selectedSkills = this.data.skills;
      this.getSkills();
    }
  }
}
</script>