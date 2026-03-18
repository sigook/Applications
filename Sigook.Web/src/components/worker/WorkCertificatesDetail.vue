<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>

    <div class="button-right">
      <h3 class="fw-700 fz-0">{{ $t('WorkerCertificates') }}</h3>
      <b-button type="is-primary" icon-right="plus" @click="modalCertificate = true">
        Add Certificate
      </b-button>
    </div>
    <div class="profile-licenses profile-experience">
      <div class="container-license hover-actions" v-for="(item, index) in localWorker.certificates"
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
              <certificate-edit :data="localWorker" @closeModal="() => closeModalEdit()" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal -->
  </section>
</template>

<script lang="ts">
import toastMixin from "../../mixins/toastMixin";
export default {
  props: ['worker'],
  mixins: [toastMixin],
  data() {
    return {
      modalCertificate: false,
      modalEdit: false,
      isLoading: false,
      localWorker: JSON.parse(JSON.stringify(this.worker))
    }
  },
  watch: {
    worker: {
      handler(newVal) {
        this.localWorker = JSON.parse(JSON.stringify(newVal));
      },
      deep: true
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalCertificate = false
    },
    confirmDelete(certificate) {
      this.showAlertConfirm(this.$t("AreYouSure"), "You want to delete this document")
        .then((response) => {
          if (response) {
            this.isLoading = true;
            this.$store.dispatch("worker/deleteWorkerCertificates", { profileId: this.localWorker.id, certificateId: certificate.id })
              .then(() => {
                this.isLoading = false;
                this.localWorker.certificates = this.localWorker.certificates.filter(d => d.id !== certificate.id);
                this.$emit('update:worker', this.localWorker);
              })
              .catch((error) => {
                this.isLoading = false;
                this.showAlertError(error);
              });
          }
        })
        .catch((error) => {
          this.showAlertError(error);
        });
    },
    deleteCertificate(certificateArr) {
      this.$store.dispatch('worker/createWorkerCertificates', { profileId: this.localWorker.id, model: certificateArr })
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
    certificateEdit: () => import("./WorkCertificatesForm.vue")
  }
}
</script>