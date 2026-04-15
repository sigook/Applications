<template>
  <div class="p-3">
    <div class="white-container-mobile">
      <h2 class="text-center main-title">Documents</h2>
      <div class="tooltip-form tooltip-form-multiline">
        <div class="form-100">
          <div class="fz-1 fw-400">
            Document
            <span class="sign-required"></span>
            <div class="input-file-edited input-block" v-if="newDocument.fileName">
              <span>{{ filename(newDocument.fileName) }}</span>
              <button v-if="newDocument.fileName" @click="deleteDocument" class="button cross-button" type="button" />
            </div>
            <upload-file v-else id="documentButton" class="input-block inline-100"
              @fileSelected="(file) => addDocument(file)" :format="'document'" :name="'Candidate_'" :required="true"
              @onUpload="() => subscribe('file')" @finishUpload="() => unsubscribe()" />
          </div>
        </div>
        <div class="form-100">
          <input type="text" class="input-border input-block" placeholder="Description"
            v-model="newDocument.description" name="documentDescription" v-validate="'required|max:40'" />
          <span v-show="errors.has('documentDescription')" class="help is-danger no-margin">
            {{ errors.first("documentDescription") }}
          </span>
        </div>
        <button type="button" class="sm-btn fz-1 background-btn orange-button" :disabled="isDisabled"
          @click="submitDocument()">
          Add
        </button>
      </div>
      <div class="relative">
        <b-loading v-model="isLoading"></b-loading>
        <ul v-if="documents && documents.items && documents.items.length > 0" class="tooltip-list">
          <li v-for="(document, index) in documents.items" :key="'document' + index" class="margin-0">
            <a :href="document.pathFile" target="_blank">
              {{ document.description }}
            </a>
            <button class="btn-icon-sm btn-icon-delete" type="button"
              @click="onDeleteDocument(document.id, index)"></button>
          </li>
        </ul>
        <div class="padding-5 color-gray-light" v-else>
          Documents
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { filename } from '@/utils/filters';
import toast from "../../mixins/toastMixin";
import { usePubSub } from "@/composables/usePubSub";
import { deleteFile } from "@/utils/fileUpload";
import { getCandidateDocuments, addCandidateDocument, deleteCandidateDocument } from "@/api/agencyCandidateApi";

export default {
  setup() {
    return { ...usePubSub() };
  },
  props: ["candidateId"],
  mixins: [toast],
  data() {
    return {
      showInput: true,
      isDisabled: false,
      isLoading: false,
      documents: null,
      newDocument: {
        fileName: "",
        description: "",
      },
    };
  },
  components: {
    UploadFile: () => import("../../components/UploadFiles.vue"),
  },
  created() {
    this.loadDocuments();
  },
  methods: {
    filename,
    loadDocuments() {
      this.isLoading = true;
      getCandidateDocuments(this.candidateId)
        .then((response) => {
          this.isLoading = false;
          this.documents = response;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    addDocument(file) {
      this.newDocument.fileName = file;
    },
    deleteDocument() {
      this.isLoading = true;
      deleteFile(this.newDocument.fileName)
        .then(() => { this.newDocument.fileName = null; })
        .finally(() => { this.isLoading = false; });
    },
    cleanInput() {
      this.newDocument = {
        fileName: "",
        description: "",
      };
    },
    submitDocument() {
      this.$validator.validateAll().then((result) => {
        if (result && this.newDocument.fileName) {
          this.isLoading = true;
          addCandidateDocument(this.candidateId, this.newDocument)
            .then(() => {
              this.isLoading = false;
              this.loadDocuments();
              this.cleanInput();
            })
            .catch((error) => {
              this.isLoading = false;
              this.showAlertError(error);
            });
          return;
        }
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      });
    },
    onDeleteDocument(id, index) {
      this.isLoading = true;
      deleteCandidateDocument(this.candidateId, id)
        .then(() => {
          this.isLoading = false;
          this.showAlertSuccess("Deleted");
          this.documents.items.splice(index, 1);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
  },
};
</script>
