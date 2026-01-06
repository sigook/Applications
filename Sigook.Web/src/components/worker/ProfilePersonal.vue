<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="profile-information">
      <basic-information id="basicinformation" :worker="worker" @updateProfile="() => updateProfile()" />
      <contact-information id="contactinformation" :worker="worker" @updateProfile="() => updateProfile()" />
      <social-insurance id="socialinsurance" :class="{ 'missing': !worker.socialInsurance }" :worker="worker"
        @updateProfile="() => updateProfile()" />
      <documents id="documents" :class="{ 'missing': !worker.identificationType1File || !worker.identificationType2File }"
        :worker="worker" @updateProfile="() => updateProfile()" />
      <resume id="resume" :class="{ 'missing': !worker.resume }" :worker="worker"
        @updateProfile="() => updateProfile()" />
    </div>
  </div>
</template>

<script>
export default {
  props: ['worker'],
  inject: ['$validator'],
  data() {
    return {
      isLoading: false
    }
  },
  components: {
    basicInformation: () => import("./WorkBasicInformationDetail"),
    socialInsurance: () => import("./WorkSinDetail"),
    documents: () => import("./WorkDocumentsDetail"),
    contactInformation: () => import('./WorkContactInformationDetail'),
    resume: () => import('./WorkResumeDetail')
  },
  methods: {
    updateProfile() {
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
  }
}
</script>
