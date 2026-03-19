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
      <licenses v-if="worker && worker.licenses" id="licenses" :class="{ 'missing': worker.licenses.length === 0 }" :worker="worker" @update:worker="$emit('update:worker', $event)"
        @updateProfile="() => updateProfile()" />
      <certificates v-if="worker && worker.certificates" id="certificates" :class="{ 'missing': worker.certificates.length === 0 }" :worker="worker" @update:worker="$emit('update:worker', $event)"
        @updateProfile="() => updateProfile()" />
      <other-documents v-if="worker && worker.otherDocuments" id="otherdocuments" :class="{ 'missing': worker.otherDocuments.length === 0 }" :worker="worker"
        :justWhmis="true" />
    </div>
  </div>
</template>

<script lang="ts">
export default {
  props: ['worker'],
  inject: ['$validator'],
  data() {
    return {
      isLoading: false
    }
  },
  components: {
    basicInformation: () => import("./WorkBasicInformationDetail.vue"),
    socialInsurance: () => import("./WorkSinDetail.vue"),
    documents: () => import("./WorkDocumentsDetail.vue"),
    contactInformation: () => import('./WorkContactInformationDetail.vue'),
    resume: () => import('./WorkResumeDetail.vue'),
    licenses: () => import('./WorkLicenseDetail.vue'),
    certificates: () => import('./WorkCertificatesDetail.vue'),
    otherDocuments: () => import('./WorkerOtherDocumentsDetail.vue')
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
