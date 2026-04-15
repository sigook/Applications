<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>
    <div class="col-8 col-padding">
      <b-field label="Skills">
        <skills-form :existingSkills="data" v-if="request.canEdit" @onPressAdd="(item) => addSkill(item)"
          @onDelete="(item) => removeSkill(item)" />
      </b-field>
    </div>
  </section>
</template>

<script lang="ts">
import toastMixin from "@/mixins/toastMixin";
import { getAgencyRequestSkill, postAgencyRequestSkill, deleteAgencyRequestSkill } from "@/api/agencyRequestApi";

export default {
  props: ['request'],
  data() {
    return {
      isLoading: false,
      data: []
    }
  },
  mixins: [toastMixin],
  components: {
    SkillsForm: () => import("../FormSkillAdd.vue")
  },
  methods: {
    loadSkills() {
      this.isLoading = true;
      getAgencyRequestSkill(this.request.id)
        .then(response => {
          this.isLoading = false;
          this.data = response;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    addSkill(item) {
      this.isLoading = true;
      postAgencyRequestSkill(this.request.id, { skill: item.skill })
        .then(() => {
          this.isLoading = false;
          this.loadSkills()
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    removeSkill(item) {
      this.isLoading = true;
      deleteAgencyRequestSkill(this.request.id, item.id)
        .then(() => {
          this.isLoading = false;
          this.loadSkills()
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    }
  },
  created() {
    this.loadSkills()
  }
}
</script>