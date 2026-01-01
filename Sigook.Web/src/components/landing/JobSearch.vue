<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="d-lg-flex gap-lg-4 align-items-end justify-content-between">
      <div class="flex-fill">
        <label class="font-weight-bold w-100">Job Title</label>
        <input list="skills" type="text" v-model="job"
          class="control-rounded color-white b-color-white bg-transparent w-100"
          placeholder="Type the job you are looking for" />
        <datalist id="skills">
          <option v-for="skill in skills" :key="skill.skill">{{ skill.skill }}</option>
        </datalist>
      </div>
      <div class="flex-fill pt-3 pt-lg-0 ml-lg-4">
        <label class="font-weight-bold w-100">Location</label>
        <input type="text" v-model="location" class="control-rounded color-white b-color-white bg-transparent w-100"
          placeholder="Type City" />
      </div>
      <div class="ml-lg-4 pt-3 pt-lg-0 d-flex align-items-end justify-content-center">
        <button type="button" class="button-rounded single-line-button color-white bg-grey-light"
          @click="searchJob">SEARCH JOBS</button>
      </div>
    </div>
  </div>
</template>

<script>

export default {
  data() {
    return {
      isLoading: false,
      skills: [],
      job: null,
      location: null
    }
  },
  async created() {
    this.isLoading = true;
    this.skills = await this.$store.dispatch('getSkills');
    this.isLoading = false;
  },
  methods: {
    searchJob() {
      const query = { jobTitle: this.job, location: this.location };
      this.$emit("onSearch", query);
      this.$router.push({
        path: '/jobSeekers',
        query: query
      });
    }
  }
}

</script>