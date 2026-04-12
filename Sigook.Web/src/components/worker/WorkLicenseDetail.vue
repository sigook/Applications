<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>
    <div class="button-right">
      <h3 class="fw-700 fz-0">{{ $t("WorkerLicenses") }}</h3>
      <b-button type="is-primary" icon-right="plus" @click="modalLicense = true">
        Add License
      </b-button>
    </div>
    <div class="profile-licenses profile-experience">
      <div class="container-license hover-actions" v-for="(item, index) in localWorker.licenses"
        v-bind:key="'licences' + index">
        <div class="button-right">
          <a :href="item.license.pathFile" target="_blank" download>
            <h4 class="fw-400">
              {{ filename(item.license.fileName) }}
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
          <span v-if="item.issued">Issued: {{ dateMonth(item.issued) }} | </span>
          <span v-if="item.expires">Expire: {{dateMonth(item.expires) }}</span>
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
              <license-edit :data="localWorker" @closeModal="() => closeModalEdit()" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal -->
  </section>
</template>

<script lang="ts">
import { filename, dateMonth } from '@/utils/filters';
import toastMixin from "../../mixins/toastMixin";
import { deleteWorkerLicenses } from '@/api/workerApi';
export default {
  props: ["worker"],
  data() {
    return {
      modalLicense: false,
      modalEdit: false,
      isLoading: false,
      localWorker: JSON.parse(JSON.stringify(this.worker)),
    };
  },
  watch: {
    worker: {
      handler(newVal) {
        this.localWorker = JSON.parse(JSON.stringify(newVal));
      },
      deep: true
    }
  },
  mixins: [toastMixin],
  methods: {
    filename,
    dateMonth,
    closeModalEdit() {
      this.$emit("updateProfile", true);
      this.modalLicense = false;
    },
    confirmDelete(license) {
      this.showAlertConfirm("Are you sure", "You want to delete this document")
        .then((response) => {
          if (response) {
            this.isLoading = true;
            deleteWorkerLicenses(this.localWorker.id, license.id)
              .then(() => {
                this.isLoading = false;
                this.localWorker.licenses = this.localWorker.licenses.filter(d => d.license.id !== license.id);
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
  },
  components: {
    licenseEdit: () => import("./WorkLicenseForm.vue"),
  },
};
</script>
