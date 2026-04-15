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
            v-model="description" name="documentDescription" />
          <span v-show="errors.description" class="help is-danger no-margin">
            {{ errors.description }}
          </span>
        </div>
        <button type="button" class="sm-btn fz-1 background-btn orange-button" :disabled="isDisabled"
          @click="submitDocument">
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
import { defineAsyncComponent } from 'vue';
import { useForm } from 'vee-validate';
import * as yup from 'yup';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { filename } from '@/utils/filters';
import { usePubSub } from "@/composables/usePubSub";
import { deleteFile } from "@/utils/fileUpload";
import { getCandidateDocuments, addCandidateDocument, deleteCandidateDocument } from "@/api/agencyCandidateApi";

export default {
  props: ["candidateId"],
  components: {
    UploadFile: defineAsyncComponent(() => import("../../components/UploadFiles.vue")),
  },
  setup() {
    const schema = yup.object({
      description: yup.string().required('Description is required').max(40),
    });
    const { handleSubmit, errors, defineField, resetForm } = useForm({
      validationSchema: schema,
    });
    const [description] = defineField('description');
    return { ...usePubSub(), description, errors, handleSubmit, resetForm };
  },
  data() {
    return {
      isDisabled: false,
      isLoading: false,
      documents: null as any,
      newDocument: {
        fileName: "" as string | null,
      },
    };
  },
  created() {
    this.loadDocuments();
  },
  methods: {
    filename,
    loadDocuments() {
      this.isLoading = true;
      getCandidateDocuments(this.candidateId)
        .then((response: any) => {
          this.isLoading = false;
          this.documents = response;
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    addDocument(file: string) {
      this.newDocument.fileName = file;
    },
    deleteDocument() {
      this.isLoading = true;
      deleteFile(this.newDocument.fileName as string)
        .then(() => { this.newDocument.fileName = null; })
        .finally(() => { this.isLoading = false; });
    },
    cleanInput() {
      this.newDocument.fileName = "";
      (this as any).resetForm();
    },
    submitDocument() {
      (this as any).handleSubmit((values: any) => {
        if (!this.newDocument.fileName) {
          showAlertError("Please make sure all required fields are filled out correctly");
          return;
        }
        this.isLoading = true;
        addCandidateDocument(this.candidateId, {
          fileName: this.newDocument.fileName,
          description: values.description,
        })
          .then(() => {
            this.isLoading = false;
            this.loadDocuments();
            this.cleanInput();
          })
          .catch((error: any) => {
            this.isLoading = false;
            showAlertError(error);
          });
      })();
    },
    onDeleteDocument(id: number, index: number) {
      this.isLoading = true;
      deleteCandidateDocument(this.candidateId, id)
        .then(() => {
          this.isLoading = false;
          showAlertSuccess("Deleted");
          this.documents.items.splice(index, 1);
        })
        .catch((error: any) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
  },
};
</script>
