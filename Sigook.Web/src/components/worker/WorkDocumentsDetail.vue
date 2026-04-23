<template>
  <section>
    <div class="button-right">
      <h3 class="section-title">{{ "Documents" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil"
        @click="modalDocuments = true"></b-button>
    </div>
    <div class="worker-documents">
      <div v-if="props.worker.identificationType1File && props.worker.identificationType1">
        <span>{{ props.worker.identificationType1.value }} #</span>
        <span>
          <p class="fw-200 margin-0">{{ props.worker.identificationNumber1 }}</p>
        </span>
      </div>
      <div v-if="props.worker.identificationType1File && props.worker.identificationType1">
        <span>{{ props.worker.identificationType1.value }} ({{ "File" }}) </span>
        <span>
          <a :href="props.worker.identificationType1File.pathFile" download target="_blank">
            {{ filename(props.worker.identificationType1File.fileName) }}
            <span class="download-button"></span>
          </a>
        </span>
      </div>

      <div v-if="props.worker.identificationType2File && props.worker.identificationType2">
        <span>{{ props.worker.identificationType2.value }} #</span>
        <span>
          <p class="fw-200 margin-0">{{ props.worker.identificationNumber2 }}</p>
        </span>
      </div>

      <div v-if="props.worker.identificationType2File && props.worker.identificationType2">
        <span>{{ props.worker.identificationType2.value }} ({{ "File" }})</span>
        <span>
          <a :href="props.worker.identificationType2File.pathFile" download target="_blank">
            {{ filename(props.worker.identificationType2File.fileName) }}
            <span class="download-button"></span>
          </a>
        </span>
      </div>
      <div v-if="props.worker.havePoliceCheckBackground && props.worker.policeCheckBackGround">
        <span>{{ "Police Check/Background" }}</span>
        <span>
          <a :href="props.worker.policeCheckBackGround.pathFile" target="_blank" download>
            {{ filename(props.worker.policeCheckBackGround.fileName) }}
            <span class="download-button"></span>
          </a>
        </span>
      </div>
    </div>
    <b-modal v-model="modalDocuments" width="500px">
      <documents-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { filename } from '@/utils/filters';
import DocumentsEdit from './WorkDocumentsForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalDocuments = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalDocuments.value = false;
}
</script>
