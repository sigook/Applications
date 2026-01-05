<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="form-section form-100">
      <div class="form-100">
        <div class="fz-1 fw-400">
          {{ $t("File") }}
          <span class="sign-required"></span>
          <div class="input-file-edited input-block" v-if="certificate && certificate.fileName">
            <span>
              {{ certificate.fileName | filename }}
            </span>
            <button v-if="certificate" @click="deleteWorkerCertificates()" class="button cross-button" type="button" />
          </div>
          <upload-file v-else class="input-block inline-100"
            @fileSelected="(file) => (this.certificate = { fileName: file })" :format="'document'" :name="'Certificate_'"
            :required="true" @onUpload="() => subscribe('file')" @finishUpload="() => unsubscribe()" />
        </div>
      </div>
      <div class="form-100">
        <label class="fz-1 fw-400 sign-required">
          {{ $t("Description") }}
        </label>
        <input type="text" class="input-border input-block" v-model="certificate.description"
          name="certificate description" v-validate="'required|max:20'"
          :class="{ 'is-danger': errors.has('certificate description') }" />

        <span v-show="errors.has('certificate description')" class="help is-danger no-margin">
          {{ errors.first("certificate description") }}
        </span>
      </div>
    </div>
    <button class="background-btn md-btn primary-button btn-radius margin-top-15 uppercase" @click="validateAll()"
      type="button">
      {{ $t("Save") }}
    </button>
  </div>
</template>

<script>
import toastMixin from "../../mixins/toastMixin";
import pubSub from "@/mixins/pubSub";
import updateMixin from "../../mixins/uploadFiles";

export default {
  props: ["data"],
  data() {
    return {
      todayDate: null,
      isLoading: false,
      certificate: {
        fileName: "",
        description: "",
      },
      certificates: [],
    };
  },
  mixins: [toastMixin, pubSub, updateMixin],
  methods: {
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.createWorkerCertificates();
          return;
        }
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      });
    },
    createWorkerCertificates() {
      this.isLoading = true;
      this.certificates.push(this.certificate);
      this.$store
        .dispatch("worker/createWorkerCertificates", {
          profileId: this.data.id,
          model: this.certificates,
        })
        .then(() => {
          this.isLoading = false;
          this.$emit("closeModal", true);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    deleteWorkerCertificates() {
      this.deleteFile(this.certificate.fileName)
        .then(() => this.certificate.fileName = null)
    }
  },
  components: {
    UploadFile: () => import("../../components/UploadFiles"),
  },
  created() {
    if (this.data != null) {
      for (let i = 0; i < this.data.certificates.length; i++) {
        this.certificates.push(this.data.certificates[i]);
      }
    }

    this.$store.dispatch("getCurrentDate").then((response) => {
      this.todayDate = response;
    });
  },
};
</script>
