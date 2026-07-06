<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title">Document</h2>
    <div class="form container-flex">
      <div class="col-12 col-padding">
        <div class="fz-1 fw-normal">
          Document
          <span class="sign-required"></span>
          <div class="input-file-edited input-block" v-if="fileName">
            <span>{{ filename(fileName) }}</span>
            <button v-if="fileName" @click="deleteDocument()" class="button cross-button" type="button" />
          </div>
          <upload-file v-else id="documentButton" class="input-block inline-100"
            @fileSelected="(file) => addDocument(file)" :format="'document'" :name="'Company_'" :required="true"
            @onUpload="() => subscribe('file')" @finishUpload="() => unsubscribe()" />
        </div>
      </div>
      <div class="col-12 col-padding">
        <label class="fz-1 fw-normal sign-required d-block">Description</label>
        <input type="text" v-model="description" class="input-border input-block" name="Description"
          :class="{ 'is-danger': formErrors.description }"
          autocomplete="nope" />
        <span v-show="formErrors.description" class="help is-danger no-margin">
          {{ formErrors.description }}
        </span>
      </div>
      <div class="col-12 col-padding">
        <label class="fz-1 fw-normal d-block">Document Type</label>
        <b-select v-model="documentType" placeholder="Select Document Type" expanded>
          <option value="1">Contract</option>
        </b-select>
      </div>
    </div>

    <div class="text-end pe-3">
      <b-button type="is-primary" rounded class="mt-3 mb-3" @click="validateForm" :disabled="disableButton">
        Save
      </b-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { usePubSub } from "@/composables/usePubSub";
import { deleteFile } from "@/utils/fileUpload";
import { filename } from "@/utils/filters";
import { createAgencyCompanyDocument } from "@/api/agencyCompanyApi";
import { useStickyForm } from '@/composables/useStickyForm';
import UploadFile from "../../components/UploadFiles.vue";

const schema = yup.object({
  description: yup.string().required('Description is required').min(2, 'Min 2 characters').max(100, 'Max 100 characters'),
});

const props = defineProps<{ profileId: any }>();
const emit = defineEmits<{ (e: 'onCreateDocument', value: any): void }>();

const { subscribe, unsubscribe } = usePubSub();

const form = useStickyForm<{ description: string }>({
  schema,
  initialValues: { description: '' },
});
const { description } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const disableButton = ref(false);
const fileName = ref<string | null>(null);
const documentType = ref<string | null>(null);

async function validateForm() {
  form.markInteracted();
  const { valid } = await form.validate();
  if (!valid) {
    showAlertError("Please make sure all required fields are filled out correctly");
    return;
  }
  submitDocument();
}

function submitDocument() {
  isLoading.value = true;
  const model = {
    fileName: fileName.value,
    description: description.value,
    documentType: documentType.value,
  };
  createAgencyCompanyDocument(props.profileId, model)
    .then((response) => {
      isLoading.value = false;
      const newDocument = {
        id: response.id,
        fileName: fileName.value,
        description: description.value,
        pathFile: response.pathFile,
      };
      emit("onCreateDocument", newDocument);
      showAlertSuccess("Created");
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function addDocument(file: any) {
  fileName.value = file;
}

function deleteDocument() {
  isLoading.value = true;
  deleteFile(fileName.value)
    .then(() => { fileName.value = null; })
    .finally(() => { isLoading.value = false; });
}

</script>
