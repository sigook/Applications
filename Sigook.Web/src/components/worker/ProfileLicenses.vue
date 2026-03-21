<template>
  <div>
    <div class="profile-worker">
      <licenses :class="{ 'missing': worker.licenses.length === 0 }" :worker.sync="worker"
        @updateProfile="() => updateProfile()" />
      <certificates :class="{ 'missing': worker.certificates.length === 0 }" :worker.sync="worker"
        @updateProfile="() => updateProfile()" />
      <otherDocuments :class="{ 'missing': worker.otherDocuments.length === 0 }" :worker="worker" :justWhmis="true">
      </otherDocuments>
    </div>
  </div>
</template>

<script lang="ts">
import toastMixin from "../../mixins/toastMixin";

export default {
  props: ['worker'],
  mixins: [
    toastMixin
  ],
  components: {
    licenses: () => import("../../components/worker/WorkLicenseDetail.vue"),
    certificates: () => import("../../components/worker/WorkCertificatesDetail.vue"),
    otherDocuments: () => import("../../components/worker/WorkerOtherDocumentsDetail.vue")
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
  },
}
</script>
