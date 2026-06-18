<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>
    <div class="d-flex align-items-center justify-content-between">
      <div>
        <h3 class="fw-bold fz-0">{{ props.justWhmis ? 'WHMIS and Health and Safety Training' : 'Other documents' }} </h3>
        <i class="fz-2" v-if="props.justWhmis">Complete the training following both links below and uplaod your
          certificates</i>
      </div>
      <b-button type="is-primary" icon-right="plus" @click="modalDocuments = true">
        Add Document
      </b-button>
    </div>
    <div v-if="props.justWhmis">
      <p v-if="!props.worker.location.isUSA">
        <a href="https://aixsafety.com/wp-content/uploads/articulate_uploads/WMS3May2024AixSafety23/story.html"
          target="_blank">WHIMS</a>
      </p>
      <p v-if="!props.worker.location.isUSA">
        <a href="https://www.labour.gov.on.ca/english/hs/elearn/worker/foursteps.php" target="_blank">
          HS BOOKLET
        </a>
      </p>
    </div>
    <div class="profile-licenses profile-experience">
      <div class="container-license hover-actions" v-for="(item, index) in props.worker.otherDocuments"
        v-bind:key="'docs' + index">
        <div class="d-flex align-items-center justify-content-between">
          <a :href="item.pathFile" target="_blank" download>
            <h4 class="fw-normal">
              {{ filename(item.fileName) }}
              <span class="download-button"></span>
            </h4>
          </a>
          <div class="actions text-end">
            <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
              <button class="btn-icon-sm btn-icon-delete" type="button" @click="confirmDelete(item)">
                {{ "Delete" }}
              </button>
            </b-tooltip>
          </div>
        </div>
        <div class="fz-1">
          <p>
            <strong class="fw-normal">{{ item.description }}</strong>
          </p>
        </div>
      </div>
    </div>

    <b-modal v-model="modalDocuments" width="500px">
      <documents-form :data="props.worker" @closeAndUpdate="() => closeAndUpdate()" />
    </b-modal>
  </section>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertConfirm, showAlertError } from '@/utils/toast';
import { filename } from '@/utils/filters';
import { deleteWorkerOtherDocuments } from '@/api/workerApi';
import DocumentsForm from './WorkerOtherDocumentsForm.vue';

const props = defineProps<{ worker?: any; justWhmis?: boolean }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const isLoading = ref(false);
const modalDocuments = ref(false);

function closeAndUpdate() {
  modalDocuments.value = false;
  emit('updateProfile', true);
}

function confirmDelete(document: any) {
  showAlertConfirm('Are you sure', 'You want to delete this document').then((response) => {
    if (response) {
      isLoading.value = true;
      deleteWorkerOtherDocuments(props.worker.id, document.id)
        .then(() => {
          isLoading.value = false;
          emit('updateProfile', true);
        })
        .catch((error) => {
          isLoading.value = false;
          showAlertError(error);
        });
    }
  });
}

</script>
