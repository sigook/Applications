<template>
  <div class="p-4">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="has-text-centered fz1 mb-4">Documents</h2>

    <div class="has-text-right mb-4">
      <b-button type="is-primary" size="is-small" icon-left="plus" @click="showForm = true">
        Add Document
      </b-button>
    </div>

    <nav v-if="items.length" class="panel">
      <div v-for="(document, index) in items" :key="document.id"
        class="panel-block is-justify-content-space-between">
        <a :href="document.pathFile" target="_blank"
          class="is-flex is-align-items-center is-flex-grow-1">
          <b-icon icon="file-document-outline" size="is-small" class="mr-2" />
          <span>{{ document.description }}</span>
        </a>
        <b-button type="is-text" size="is-small" icon-right="delete" class="has-text-danger"
          @click="onDeleteDocument(document.id, Number(index))"></b-button>
      </div>
    </nav>
    <p v-else class="op3">No documents yet</p>

    <div class="mt-4">
      <b-button @click="emit('close')">Close</b-button>
    </div>

    <b-modal custom-content-class="card" v-model="showForm" width="500px" :destroy-on-hide="true">
      <documents-form :candidate-id="String(candidateId)" @created="loadDocuments" @close="showForm = false" />
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref, computed } from 'vue';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { getCandidateDocuments, deleteCandidateDocument } from "@/api/agencyCandidateApi";
import type { PaginatedList } from "@/types/common";
import type { CandidateDocument } from "@/types/candidate";
import DocumentsForm from "@/components/candidate/DocumentsForm.vue";

const props = defineProps<{ candidateId: number | string }>();
const emit = defineEmits<{ (e: 'close'): void }>();

const isLoading = ref(false);
const documents = ref<PaginatedList<CandidateDocument> | null>(null);
const showForm = ref(false);

const items = computed(() => documents.value?.items ?? []);

function loadDocuments() {
  isLoading.value = true;
  getCandidateDocuments(String(props.candidateId))
    .then((response) => {
      isLoading.value = false;
      documents.value = response;
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onDeleteDocument(id: string, index: number) {
  isLoading.value = true;
  deleteCandidateDocument(String(props.candidateId), id)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess("Deleted");
      documents.value?.items.splice(index, 1);
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

loadDocuments();
</script>
