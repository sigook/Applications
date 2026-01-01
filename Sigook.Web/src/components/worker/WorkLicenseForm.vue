<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="form-section form-100">
      <div class="form-100">
        <div class="fz-1 fw-400">
          {{ $t("File") }}
          <span class="sign-required"></span>
          <div class="input-file-edited input-block" v-if="licenseModal.license && licenseModal.license.fileName">
            <span>
              {{ licenseModal.license.fileName | filename }}
            </span>
            <button v-if="licenseModal.license" @click="deleteWorkerLicenses()" class="button cross-button"
              type="button" />
          </div>
          <upload-file v-else class="input-block inline-100"
            @fileSelected="(file) => (licenseModal.license = { fileName: file })" :format="'document'" :name="'License_'"
            :required="true" @onUpload="() => subscribe('file')" @finishUpload="() => unsubscribe()" />
        </div>
      </div>
      <div class="form-50 sm-form-100">
        <label class="fz-1 fw-400 sign-required">{{ $t("Description") }}
        </label>
        <input type="text" class="input-border input-block" v-model="licenseModal.license.description"
          name="license description" v-validate="'required|max:15'"
          :class="{ 'is-danger': errors.has('license description') }" />
        <span v-show="errors.has('license description')" class="help is-danger no-margin">
          {{ errors.first("license description") }}
        </span>
      </div>
      <div class="form-50 sm-form-100">
        <label class="fz-1 fw-400">Number</label>
        <input type="text" class="input-border input-block" v-model="licenseModal.number" />
      </div>
      <div class="form-50 sm-form-100">
        <label class="fz-1 fw-400">Issued</label>
        <b-datepicker class="input-block" v-model="licenseModal.issued" :focused-date="todayDate" :max-date="todayDate"
          position="is-top-left">
        </b-datepicker>
      </div>
      <div class="form-50 sm-form-100">
        <label class="fz-1 fw-400 sign-required">Expires</label>
        <b-datepicker class="input-block" v-model="licenseModal.expires" :focused-date="todayDate" :min-date="todayDate"
          position="is-top-left" v-validate="'required'" :name="'licenseExpires'"
          :class="{ 'is-danger': errors.has('licenseExpires') }">
        </b-datepicker>
        <span v-show="errors.has('licenseExpires')" class="help is-danger no-margin">
          {{ errors.first("licenseExpires") }}
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
import updateMixin from "../../mixins/uploadFiles";
import pubSub from "@/mixins/pubSub";

export default {
  props: ["data"],
  data() {
    return {
      todayDate: null,
      isLoading: false,
      licenseModal: {
        license: {
          fileName: "",
          description: "",
        },
        number: "",
        issued: null,
        expires: null,
      },
      licenses: [],
    };
  },
  mixins: [toastMixin, pubSub, updateMixin],
  methods: {
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.createWorkerLicenses();
          return;
        }
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      });
    },
    createWorkerLicenses() {
      this.isLoading = true;
      this.licenses.push(this.licenseModal);
      this.$store
        .dispatch("worker/createWorkerLicenses", {
          profileId: this.data.id,
          model: this.licenses,
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
    deleteWorkerLicenses() {
      this.deleteFile(this.licenseModal.license.fileName)
        .then(() => this.licenseModal.license.fileName = null)
    }
  },
  components: {
    UploadFile: () => import("../../components/UploadFiles"),
  },
  created() {
    if (this.data != null) {
      for (let i = 0; i < this.data.licenses.length; i++) {
        this.licenses.push(this.data.licenses[i]);
      }
    }
    this.$store.dispatch("getCurrentDate").then((response) => {
      this.todayDate = response;
    });
  },
};
</script>
