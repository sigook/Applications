<template>
  <section>
    <div class="button-right">
      <h3 class="section-title">{{ "Documents" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil"
        @click="modalDocuments = true"></b-button>
    </div>
    <div class="worker-documents">
      <div v-if="worker.identificationType1File && worker.identificationType1">
        <span>{{ worker.identificationType1.value }} #</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.identificationNumber1 }}</p>
        </span>
      </div>
      <div v-if="worker.identificationType1File && worker.identificationType1">
        <span>{{ worker.identificationType1.value }} ({{ "File" }}) </span>
        <span>
          <a :href="worker.identificationType1File.pathFile" download target="_blank">
            {{ filename(worker.identificationType1File.fileName) }}
            <span class="download-button"></span>
          </a>
        </span>
      </div>

      <div v-if="worker.identificationType2File && worker.identificationType2">
        <span>{{ worker.identificationType2.value }} #</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.identificationNumber2 }}</p>
        </span>
      </div>

      <div v-if="worker.identificationType2File && worker.identificationType2">
        <span>{{ worker.identificationType2.value }} ({{ "File" }})</span>
        <span>
          <a :href="worker.identificationType2File.pathFile" download target="_blank">
            {{ filename(worker.identificationType2File.fileName) }}
            <span class="download-button"></span>
          </a>
        </span>
      </div>
      <div v-if="worker.havePoliceCheckBackground && worker.policeCheckBackGround">
        <span>{{ "Police Check/Background" }}</span>
        <span>
          <a :href="worker.policeCheckBackGround.pathFile" target="_blank" download>
            {{ filename(worker.policeCheckBackGround.fileName) }}
            <span class="download-button"></span>
          </a>
        </span>
      </div>
    </div>
    <b-modal v-model="modalDocuments" width="500px">
      <documents-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { filename } from '@/utils/filters';

export default {
  props: ["worker"],
  data() {
    return {
      modalDocuments: false,
    };
  },
  methods: {
    filename,
    closeModalEdit() {
      this.$emit("updateProfile", true);
      this.modalDocuments = false;
    },
  },
  components: {
    documentsEdit: defineAsyncComponent(() => import("./WorkDocumentsForm.vue")),
  },
};
</script>
