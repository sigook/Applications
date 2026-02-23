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
      <licenses v-if="worker && worker.licenses" id="licenses" :class="{ 'missing': worker.licenses.length === 0 }" :worker="worker"
        @updateProfile="() => updateProfile()" />
      <certificates v-if="worker && worker.certificates" id="certificates" :class="{ 'missing': worker.certificates.length === 0 }" :worker="worker"
        @updateProfile="() => updateProfile()" />
      <other-documents v-if="worker && worker.otherDocuments" id="otherdocuments" :class="{ 'missing': worker.otherDocuments.length === 0 }" :worker="worker"
        :justWhmis="true" />
    </div>
  </div>
</template>

<script>
import worker from "../../store/modules/worker";

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
    resume: () => import('./WorkResumeDetail'),
    licenses: () => import('./WorkLicenseDetail'),
    certificates: () => import('./WorkCertificatesDetail'),
    otherDocuments: () => import('./WorkerOtherDocumentsDetail')
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
