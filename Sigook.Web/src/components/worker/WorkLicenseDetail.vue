<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>
    <div class="button-right">
      <h3 class="fw-700 fz-0">{{ $t("WorkerLicenses") }}</h3>
      <button type="button" class="outline-btn md-btn orange-button btn-radius" @click="modalLicense = true">
        Add License +
      </button>
    </div>
    <div class="profile-licenses profile-experience">
      <div class="container-license hover-actions" v-for="(item, index) in worker.licenses"
        v-bind:key="'licences' + index">
        <div class="button-right">
          <a :href="item.license.pathFile" target="_blank" download>
            <h4 class="fw-400">
              {{ item.license.fileName | filename }}
              <span class="download-button"></span>
            </h4>
          </a>
          <div class="actions text-right">
            <b-tooltip label="Delete" type="is-dark" position="is-top">
              <button class="btn-icon-sm btn-icon-delete" type="button" @click="confirmDelete(item.license)">
                {{ $t("Delete") }}
              </button>
            </b-tooltip>
          </div>
        </div>

        <div class="fz-1">
          <p>
            <strong class="fw-400">{{ item.license.description }}</strong>
            <strong class="fw-400" v-if="item.number">
              # {{ item.number }}</strong>
          </p>
          <span v-if="item.issued">Issued: {{ item.issued | dateMonth }} | </span>
          <span v-if="item.expires">Expire: {{item.expires | dateMonth }}</span>
        </div>
      </div>
    </div>
    <!-- custom CREATE modal -->
    <transition name="modal">
      <div v-if="modalLicense" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light overflow-initial">
              <span class="fz1 fw-700">{{ $t("WorkerLicenses") }}</span>
              <button @click="modalLicense = false" type="button" class="cross-icon">
                {{ $t("Close") }}
              </button>
              <license-edit :data="worker" @closeModal="() => closeModalEdit()" />
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
  props: ["worker"],
  data() {
    return {
      modalLicense: false,
      modalEdit: false,
      isLoading: false,
    };
  },
  mixins: [toastMixin],
  methods: {
    closeModalEdit() {
      this.$emit("updateProfile", true);
      this.modalLicense = false;
    },
    confirmDelete(license) {
      let vm = this;
      this.showAlertConfirm("Are you sure", "You want to delete this document")
        .then((response) => {
          if (response) {
            vm.isLoading = true;
            vm.$store.dispatch("worker/deleteWorkerLicenses", { profileId: this.worker.id, licenseId: license.id })
              .then(() => {
                vm.isLoading = false;
                vm.worker.licenses = vm.worker.licenses.filter(d => d.license.id !== license.id);
              })
              .catch((error) => {
                vm.isLoading = false;
                this.showAlertError(error);
              });
          }
        })
        .catch((error) => {
          this.showAlertError(error);
        });
    },
  },
  components: {
    licenseEdit: () => import("./WorkLicenseForm"),
  },
};
</script>
