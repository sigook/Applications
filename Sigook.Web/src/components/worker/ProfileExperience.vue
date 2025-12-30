<template>
  <form>
    <b-loading v-model="isLoading"></b-loading>
    <section>
      <div class="button-right">
        <h3 class="fw-700 fz-0 color-accent">{{ $t('WorkerWorkExperience') }}</h3>
        <button type="button" class="outline-btn md-btn orange-button btn-radius"
          @click="modalWorkExperience = true">Add experience +</button>
      </div>
      <div class="profile-information profile-experience" v-for="(item, index) in worker.jobExperiences"
        :key="'workExp' + index">
        <work-experience-detail :item="item" :workerId="worker.id" @getWorker="() => updateExperience()" />
      </div>

      <b-modal v-model="modalWorkExperience" width="800px">
        <work-experience :workerId="worker.id" @updateExperience="() => updateExperience()" />
      </b-modal>
    </section>

  </form>
</template>

<script>
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
    workExperience: () => import("./WorkExperienceForm"),
    workExperienceDetail: () => import("../../components/worker/WorkExperienceDetail")
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
