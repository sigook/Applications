<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>
    <div class="button-right">
      <div>
        <h3 class="fw-700 fz-0">{{ props.justWhmis ? 'WHMIS and Health and Safety Training' : 'Other documents' }} </h3>
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
        <div class="button-right">
          <a :href="item.pathFile" target="_blank" download>
            <h4 class="fw-400">
              {{ filename(item.fileName) }}
              <span class="download-button"></span>
            </h4>
          </a>
          <div class="actions text-right">
            <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
              <button class="btn-icon-sm btn-icon-delete" type="button" @click="confirmDelete(item)">
                {{ "Delete" }}
              </button>
            </b-tooltip>
          </div>
        </div>
        <div class="fz-1">
          <p>
            <strong class="fw-400">{{ item.description }}</strong>
          </p>
        </div>
      </div>
    </div>

    <!-- custom CREATE modal -->
    <transition name="modal">
      <div v-if="modalDocuments" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light overflow-initial">
              <span class="fz1 fw-700">New Document</span>
              <button @click="modalDocuments = false" type="button" class="cross-icon">
                {{ "Close" }}
              </button>
              <documents-form :data="props.worker" @closeAndUpdate="() => closeAndUpdate()" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal -->
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
