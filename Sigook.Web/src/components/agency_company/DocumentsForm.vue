<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title">Document</h2>
    <div class="form">
      <div class="form-100">
        <div class="fz-1 fw-400">
          Document
          <span class="sign-required"></span>
          <div class="input-file-edited input-block" v-if="model.fileName">
            <span>{{ model.fileName | filename }}</span>
            <button v-if="model.fileName" @click="deleteDocument()" class="button cross-button" type="button" />
          </div>
          <upload-file v-else id="documentButton" class="input-block inline-100"
            @fileSelected="(file) => addDocument(file)" :format="'document'" :name="'Company_'" :required="true"
            @onUpload="() => subscribe('file')" @finishUpload="() => unsubscribe()" />
        </div>
      </div>
      <div class="form-100">
        <label class="fz-1 fw-400 sign-required">Description</label>
        <input type="text" v-model="model.description" class="input-border input-block" :name="'Description'"
          v-validate="{ required: true, max: 100, min: 2 }" :class="{ 'is-danger': errors.has('Description') }"
          autocomplete="nope" />
        <span v-show="errors.has('Description')" class="help is-danger no-margin">
          {{ errors.first("Description") }}
        </span>
      </div>
      <div class="form-100">
        <label class="fz-1 fw-400">Document Type</label>
        <b-select v-model="model.documentType" placeholder="Select Document Type" expanded>
          <option value="1">Contract</option>
        </b-select>
      </div>
    </div>

    <div class="text-right pr-3">
      <button class="background-btn md-btn orange-button btn-radius margin-top-15 margin-bottom-10" type="button"
        @click="validateForm" :disabled="disableButton">
        {{ $t("Save") }}
      </button>
    </div>
  </div>
</template>

<script>
import pubSub from "@/mixins/pubSub";
import updateMixin from "../../mixins/uploadFiles";

export default {
  props: ["profileId"],
  data() {
    return {
      isLoading: false,
      disableButton: false,
      model: {
        fileName: null,
        description: null,
      },
    };
  },
  components: {
    UploadFile: () => import("../../components/UploadFiles"),
  },
  mixins: [pubSub, updateMixin],
  methods: {
    validateForm() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.createAgencyCompanyDocument();
          return;
        }
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      });
    },
    createAgencyCompanyDocument() {
      this.isLoading = true;
      this.$store.dispatch("agency/createAgencyCompanyDocument", {
          profileId: this.profileId,
          model: this.model,
        })
        .then((response) => {
          this.isLoading = false;
          let newDocument = {
            id: response.id,
            fileName: this.model.fileName,
            description: this.model.description,
            pathFile: response.pathFile,
          };
          this.$emit("onCreateDocument", newDocument);
          this.showAlertSuccess("Created");
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    addDocument(file) {
      this.model.fileName = file;
    },
    deleteDocument() {
      this.deleteFile(this.model.fileName).then(() => this.model.fileName = null)
    }
  },
};
</script>
