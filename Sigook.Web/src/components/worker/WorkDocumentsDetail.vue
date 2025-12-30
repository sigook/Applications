<template>
  <section>
    <div class="button-right">
      <h3 class="section-title">{{ $t("WorkerDocuments") }}</h3>
      <button class="actions btn-icon-sm btn-icon-edit" type="button" @click="modalDocuments = true">
        Edit
      </button>
    </div>
    <div class="worker-documents">
      <div v-if="worker.identificationType1File && worker.identificationType1">
        <span>{{ worker.identificationType1.value }} #</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.identificationNumber1 }}</p>
        </span>
      </div>
      <div v-if="worker.identificationType1File && worker.identificationType1">
        <span>{{ worker.identificationType1.value }} ({{ $t("File") }}) </span>
        <span>
          <a :href="worker.identificationType1File.pathFile" download target="_blank">
            {{ worker.identificationType1File.fileName | filename }}
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
        <span>{{ worker.identificationType2.value }} ({{ $t("File") }})</span>
        <span>
          <a :href="worker.identificationType2File.pathFile" download target="_blank">
            {{ worker.identificationType2File.fileName | filename }}
            <span class="download-button"></span>
          </a>
        </span>
      </div>
      <div v-if="worker.havePoliceCheckBackground && worker.policeCheckBackGround">
        <span>{{ $t("WorkerPoliceCheckBackground") }}</span>
        <span>
          <a :href="worker.policeCheckBackGround.pathFile" target="_blank" download>
            {{ worker.policeCheckBackGround.fileName | filename }}
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

<script>
export default {
  props: ["worker"],
  data() {
    return {
      modalDocuments: false,
    };
  },
  methods: {
    closeModalEdit() {
      this.$emit("updateProfile", true);
      this.modalDocuments = false;
    },
  },
  components: {
    documentsEdit: () => import("./WorkDocumentsForm"),
  },
};
</script>
