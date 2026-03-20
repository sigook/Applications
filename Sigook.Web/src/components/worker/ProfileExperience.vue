<template>
  <form>
    <b-loading v-model="isLoading"></b-loading>
    <div class="tab-actions">
      <b-button type="is-primary" icon-right="plus" @click="modalWorkExperience = true">
        {{ $t("AddExperience") }}
      </b-button>
    </div>
    <div>
      <work-experience-detail
        v-for="(item, index) in worker.jobExperiences"
        :key="'workExp' + index"
        :item="item"
        :workerId="worker.id"
        @getWorker="() => updateExperience()" />

      <b-modal v-model="modalWorkExperience" width="800px">
        <work-experience :workerId="worker.id" @updateExperience="() => updateExperience()" />
      </b-modal>
    </div>

  </form>
</template>

<script lang="ts">
import utilsWorkerMixin from '@/mixins/utilsWorkerMixin';
export default {
  data() {
    return {
      disableStartDate: null,
      modalWorkExperience: false,
      isLoading: false
    }
  },
  mixins: [utilsWorkerMixin],
  methods: {
    disableEndDate(index) {
      let disabledDates = this.$store.state.currentDate;

      if (this.worker.jobExperiences[index].startDate) {
        disabledDates = this.worker.jobExperiences[index].startDate
      }

      return disabledDates;
    },
    updateExperience() {
      this.modalWorkExperience = false;
      this.isLoading = true;
      this.$store.dispatch('worker/getProfile', this.worker.id)
        .then(() => {
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    }
  },
  components: {
    workExperience: () => import("./WorkExperienceForm.vue"),
    workExperienceDetail: () => import("../../components/worker/WorkExperienceDetail.vue")
  },
  created() {
    this.$store.dispatch('getCurrentDate').then(response => {
      this.disableStartDate = response;
    })
  },
  computed: {
    worker() {
      return this.$store.state.worker.workerProfile;
    }
  }
}
</script>

<style scoped>
.tab-actions {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 12px;
}
</style>
