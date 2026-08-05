<template>
  <div class="p-3">
    <div>
      <h2 class="has-text-centered main-title">Documents</h2>
      <div class="tooltip-form tooltip-form-multiline">
        <div class="">
          <div class="fz-1 has-text-weight-normal">
            Document
            <span class="sign-required"></span>
            <div class="input-file-edited input-block" v-if="newDocument.fileName">
              <span>{{ filename(newDocument.fileName) }}</span>
              <button v-if="newDocument.fileName" @click="deleteDocument" class="button cross-button" type="button" />
            </div>
            <UploadFile v-else id="documentButton" class="input-block inline-100"
              @fileSelected="(file: string) => addDocument(file)" :format="'document'" :name="'Candidate_'" :required="true"
              @onUpload="() => pubSub.subscribe('file')" @finishUpload="() => pubSub.unsubscribe()" />
          </div>
        </div>
        <div class="">
          <input type="text" class="input-border input-block" placeholder="Description"
            v-model="description" name="documentDescription" />
          <span v-show="errors.description" class="help is-danger no-margin">
            {{ errors.description }}
          </span>
        </div>
        <b-button type="is-primary" size="is-small" :disabled="isDisabled" @click="submitDocument">
          Add
        </b-button>
      </div>
      <div class="relative">
        <b-loading v-model="isLoading"></b-loading>
        <ul v-if="documents && documents.items && documents.items.length > 0" class="tooltip-list">
          <li v-for="(document, index) in documents.items" :key="'document' + index" class="m-0">
            <a :href="document.pathFile" target="_blank">
              {{ document.description }}
            </a>
            <button class="btn-icon-sm btn-icon-delete" type="button"
              @click="onDeleteDocument(document.id, Number(index))"></button>
          </li>
        </ul>
        <div class="padding-5 color-gray-light" v-else>
          Documents
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useForm } from 'vee-validate';
import * as yup from 'yup';
import UploadFile from "../../components/UploadFiles.vue";
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { filename } from '@/utils/filters';
import { usePubSub } from "@/composables/usePubSub";
import { deleteFile } from "@/utils/fileUpload";
import { getCandidateDocuments, addCandidateDocument, deleteCandidateDocument } from "@/api/agencyCandidateApi";

const props = defineProps<{ candidateId: number | string }>();

const pubSub = usePubSub();

const schema = yup.object({
  description: yup.string().required('Description is required').max(40),
});
const { handleSubmit, errors, defineField, resetForm } = useForm({
  validationSchema: schema,
});
const [description] = defineField('description');

const isDisabled = ref(false);
const isLoading = ref(false);
const documents = ref<any>(null);
const newDocument = reactive<{ fileName: string | null }>({ fileName: '' });

function loadDocuments() {
  isLoading.value = true;
  getCandidateDocuments(String(props.candidateId))
    .then((response: any) => {
      isLoading.value = false;
      documents.value = response;
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function addDocument(file: string) {
  newDocument.fileName = file;
}

function deleteDocument() {
  isLoading.value = true;
  deleteFile(newDocument.fileName as string)
    .then(() => { newDocument.fileName = null; })
    .finally(() => { isLoading.value = false; });
}

function cleanInput() {
  newDocument.fileName = "";
  resetForm();
}

function submitDocument() {
  handleSubmit((values: any) => {
    if (!newDocument.fileName) {
      showAlertError("Please make sure all required fields are filled out correctly");
      return;
    }
    isLoading.value = true;
    addCandidateDocument(String(props.candidateId), {
      fileName: newDocument.fileName,
      description: values.description,
    })
      .then(() => {
        isLoading.value = false;
        loadDocuments();
        cleanInput();
      })
      .catch((error: unknown) => {
        isLoading.value = false;
        showAlertError(error);
      });
  })();
}

function onDeleteDocument(id: number, index: number) {
  isLoading.value = true;
  deleteCandidateDocument(String(props.candidateId), String(id))
    .then(() => {
      isLoading.value = false;
      showAlertSuccess("Deleted");
      documents.value.items.splice(index, 1);
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

loadDocuments();
</script>

<style scoped lang="scss">
.tooltip-list {
  font-size: 16px;
  text-align: left;
  padding-bottom: 6px;

  li {
    padding: 5px;
    border-bottom: 1px solid #dbdcdb;
    justify-content: space-between;
    display: flex;
    align-items: center;
    transition: 0.4s all ease;

    &:hover {
      background: #f1f1f1;
    }

    &:last-child {
      border-bottom: 0;
    }
  }
}
</style>
