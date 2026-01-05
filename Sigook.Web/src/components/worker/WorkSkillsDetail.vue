<template>
  <section class="hover-transform">
    <div class="button-right">
      <h3 class="section-title">{{ $t("WorkerSkills") }}</h3>
      <button class="actions btn-icon-sm btn-icon-edit" type="button" @click="modalSkills = true">Edit</button>
    </div>
    <div class="worker-documents">
      <div class="worker-skills margin-top-15">
        <div>
          <span class="icon-star fw-200" v-for="(item, index) in worker.skills"
            v-bind:key="'skills' + index">{{ item.skill }}</span>
        </div>
      </div>
    </div>
    <b-modal v-model="modalSkills" width="500px">
      <skills-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
</template>

<script>
export default {
  props: ['worker'],
  data() {
    return {
      modalSkills: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalSkills = false
    }
  },
  components: {
    skillsEdit: () => import("./WorkSkillsForm")
  }
}
</script>