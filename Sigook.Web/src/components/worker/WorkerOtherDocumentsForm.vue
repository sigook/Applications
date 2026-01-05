<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="form-section form-100">
      <div class="form-100">
        <div class="fz-1 fw-400">
          {{ $t("File") }}
          <span class="sign-required"></span>
          <div class="input-file-edited input-block" v-if="otherDocument.fileName">
            <span>{{ otherDocument.fileName | filename }}</span>
            <button v-if="otherDocument.fileName" @click="deleteWorkerOtherDocuments()" class="button cross-button"
              type="button" />
          </div>
          <upload-file v-else class="input-block inline-100" @fileSelected="(file) => otherDocument.fileName = file"
            :format="'document'" :name="'OtherDoc_'" :required="true" @onUpload="() => subscribe('file')"
            @finishUpload="() => unsubscribe()" />
        </div>
      </div>
      <div class="form-100">
        <label class="fz-1 fw-400 sign-required">
          {{ $t("Description") }}
        </label>
        <input type="text" class="input-border input-block" v-model="otherDocument.description" name="Description"
          v-validate="'required|max:20'" :class="{ 'is-danger': errors.has('Description') }" />
        <span v-show="errors.has('Description')" class="help is-danger no-margin">
          {{ errors.first("Description") }}
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
      otherDocument: {
        fileName: "",
        description: "",
      },
    };
  },
  mixins: [toastMixin, pubSub, updateMixin],
  methods: {
    validateAll() {
      this.$validator.validateAll().then((isValid) => {
        if (isValid) {
          this.createWorkerOtherDocuments();
          return;
        }
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      });
    },
    createWorkerOtherDocuments() {
      this.isLoading = true;
      this.$store.dispatch("worker/createWorkerOtherDocuments", {
          profileId: this.data.id,
          model: this.otherDocument,
        })
        .then(() => {
          this.isLoading = false;
          this.$emit("closeAndUpdate", true);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    deleteWorkerOtherDocuments() {
      this.deleteFile(this.otherDocument.fileName)
        .then(() => this.otherDocument.fileName = null)
    }
  },
  components: {
    UploadFile: () => import("../../components/UploadFiles"),
  },
};
</script>
