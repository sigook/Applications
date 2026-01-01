<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>

    <div class="button-right">
      <h3 class="fw-700 fz-0">{{ $t('WorkerCertificates') }}</h3>
      <button type="button" class="outline-btn md-btn orange-button btn-radius" @click="modalCertificate = true">
        Add Certificate +
      </button>
    </div>
    <div class="profile-licenses profile-experience">
      <div class="container-license hover-actions" v-for="(item, index) in worker.certificates"
        v-bind:key="'certificates' + index">

        <div class="button-right">
          <a :href="item.pathFile" target="_blank" download>
            <h4 class="fw-400">{{ item.fileName | filename }} <span class="download-button"></span></h4>
          </a>
          <div class="actions text-right">
            <b-tooltip label="Delete" type="is-dark" position="is-top">
              <button class="btn-icon-sm btn-icon-delete bg-transparent" type="button" @click="confirmDelete(item)">
                {{ $t("Delete") }}
              </button>
            </b-tooltip>
          </div>
        </div>
        <div class="fz-1">
          <p>
            <strong class="fw-400">{{ item.description }}</strong>
          </p>
        </div>
      </div>
    </div>
    <!-- custom CREATE modal -->
    <transition name="modal">
      <div v-if="modalCertificate" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light overflow-initial">
              <span class="fz1 fw-700">{{ $t("WorkerCertificates") }}</span>
              <button @click="modalCertificate = false" type="button" class="cross-icon">
                {{ $t('Close') }}
              </button>
              <certificate-edit :data="worker" @closeModal="() => closeModalEdit()" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal -->
  </section>
</template>

<script>
import toastMixin from "../../mixins/toastMixin";
export default {
  props: ['worker'],
  mixins: [toastMixin],
  data() {
    return {
      modalCertificate: false,
      modalEdit: false,
      isLoading: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalCertificate = false
    },
    confirmDelete(certificate) {
      let vm = this;
      this.showAlertConfirm(this.$t("AreYouSure"), "You want to delete this document")
        .then(response => {
          if (response) {
            vm.isLoading = true;
            vm.$store.dispatch("worker/deleteWorkerCertificates", { profileId: this.worker.id, certificateId: certificate.id })
              .then(() => {
                vm.isLoading = false;
                vm.worker.certificates = vm.worker.certificates.filter(d => d.id !== certificate.id);
              })
              .catch((error) => {
                vm.isLoading = false;
                this.showAlertError(error);
              });
          }
        })
        .catch(error => {
          this.showAlertError(error);
        });
    },
    deleteCertificate(certificateArr) {
      this.$store.dispatch('worker/createWorkerCertificates', { profileId: this.worker.id, model: certificateArr })
        .then(() => {
          this.isLoading = false;
          this.$emit('updateProfile', true);
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    }
  },
  components: {
    certificateEdit: () => import("./WorkCertificatesForm")
  }
}
</script>