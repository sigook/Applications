<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="profile-information">
      <basic-information id="basicinformation" :worker="worker" @updateProfile="() => updateProfile()" />
      <contact-information id="contactinformation" :worker="worker" @updateProfile="() => updateProfile()" />
      <social-insurance id="socialinsurance" :class="{ 'missing': !worker.socialInsurance }" :worker="worker"
        @updateProfile="() => updateProfile()" />
      <documents id="documents" :class="{ 'missing': !worker.identificationType1File && !worker.identificationType2File }"
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
import { defineAsyncComponent } from 'vue';
export default {
  props: ['worker'],
  data() {
    return {
      isLoading: false
    }
  },
  components: {
    basicInformation: defineAsyncComponent(() => import("./WorkBasicInformationDetail.vue")),
    socialInsurance: defineAsyncComponent(() => import("./WorkSinDetail.vue")),
    documents: defineAsyncComponent(() => import("./WorkDocumentsDetail.vue")),
    contactInformation: defineAsyncComponent(() => import('./WorkContactInformationDetail.vue')),
    resume: defineAsyncComponent(() => import('./WorkResumeDetail.vue')),
    licenses: defineAsyncComponent(() => import('./WorkLicenseDetail.vue')),
    certificates: defineAsyncComponent(() => import('./WorkCertificatesDetail.vue')),
    otherDocuments: defineAsyncComponent(() => import('./WorkerOtherDocumentsDetail.vue'))
  },
  methods: {
    updateProfile() {
      this.$emit('updateProfile');
    }
  },
  created() {
    console.log(this.worker);
  }
}
</script>
