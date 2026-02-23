<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ $t("WorkerSkills") }}</span>
      <span class="width-70 items">
        <b-taglist>
          <b-tag v-for="(item, index) in worker.skills" :key="'skills' + index" type="is-info is-light" size="is-medium" rounded>{{ item.skill }}</b-tag>
        </b-taglist>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalSkills = true"></b-button>
    </div>
    <b-modal v-model="modalSkills" width="500px">
      <skills-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
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