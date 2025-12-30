<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>
    <div class="col-8 col-padding">
      <b-field label="Skills">
        <skills-form :existingSkills="data" v-if="request.canEdit" @onPressAdd="(item) => postAgencySkill(item)"
          @onDelete="(item) => deleteAgencySkill(item)" />
      </b-field>
    </div>
  </section>
</template>

<script>
import toastMixin from "@/mixins/toastMixin";
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
    SkillsForm: () => import("../FormSkillAdd")
  },
  methods: {
    getAgencySkill() {
      this.isLoading = true;
      this.$store.dispatch("agency/getAgencySkill", { requestId: this.request.id })
        .then(response => {
          this.isLoading = false;
          this.data = response;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    postAgencySkill(item) {
      let model = {
        id: null,
        skill: item.skill
      }
      this.isLoading = true;
      this.$store.dispatch("agency/postAgencySkill", { requestId: this.request.id, model: model })
        .then(response => {
          this.isLoading = false;
          this.getAgencySkill()
        }).catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    deleteAgencySkill(item) {
      this.isLoading = true;
      this.$store.dispatch("agency/deleteAgencySkill", { requestId: this.request.id, id: item.id })
        .then(() => {
          this.isLoading = false;
          this.getAgencySkill()
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    }
  },
  created() {
    this.getAgencySkill()
  }
}
</script>