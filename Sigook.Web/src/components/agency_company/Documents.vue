<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>

    <div class="is-flex is-align-items-center is-justify-content-space-between">
      <span class="has-text-weight-bold">Documents</span>
      <button class="show-notes-btn" @click="onShowDocuments()">
        <img src="../../assets/images/right-arrow.svg" :class="{ open: showDocuments }" />
      </button>
    </div>
    <span class="line-gray"></span>

    <transition name="fade">
      <div v-if="showDocuments && data" class="mb-5">
        <div class="profile-licenses profile-experience">
          <div v-for="(document, index) in data.items" :key="document.id" class="container-license hover-actions">
            <div v-if="document.canDownload" class="is-flex is-align-items-center is-justify-content-space-between">
              <a :href="document.pathFile" target="_blank" download>
                <p class="has-text-weight-normal">
                  {{ filename(document.fileName) }}
                  <span class="download-button"></span>
                </p>
              </a>
              <div class="actions has-text-right">
                <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
                  <button class="btn-icon-sm btn-icon-delete bg-transparent" type="button"
                    @click="onDeleteDocument(document.id, Number(index))">
                    {{ "Delete" }}
                  </button>
                </b-tooltip>
              </div>
            </div>
            <p v-else class="has-text-weight-normal">{{ filename(document.fileName) }}</p>
            <div class="fz-1">
              <p>
                <strong class="has-text-weight-normal">{{ document.description }}</strong>
              </p>
            </div>
          </div>
        </div>

        <b-button type="is-primary" size="is-small" outlined rounded @click="showModal = true">Add</b-button>

        <pagination :total-pages="data.totalPages" :index-page="data.pageIndex" :size-page="size"
          @changePage="(index) => loadDocuments(index)">
        </pagination>
      </div>
    </transition>

    <transition name="modal">
      <div v-if="showModal" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light small-container modal-overflow h-auto border-radius">
              <button @click="showModal = false" type="button" class="cross-icon">
                close
              </button>
              <documents-form :profile-id="profileId" @onCreateDocument="(item) => onCreateDocument(item)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertConfirm, showAlertError, showAlertSuccess } from "@/utils/toast";
import { filename } from "@/utils/filters";
import { getAgencyCompanyDocument, deleteAgencyCompanyDocument } from "@/api/agencyCompanyApi";
import DocumentsForm from "../../components/agency_company/DocumentsForm.vue";
import Pagination from "../../components/Paginator.vue";

const route = useRoute();

const showDocuments = ref(false);
const isLoading = ref(false);
const showModal = ref(false);
const data = ref<any>(null);
const profileId = route.params.id;
const size = 10;
const currentPage = 1;

function onShowDocuments() {
  if (!showDocuments.value) {
    showDocuments.value = true;
    loadDocuments(currentPage);
  } else {
    showDocuments.value = false;
  }
}

function loadDocuments(index: number) {
  isLoading.value = true;
  getAgencyCompanyDocument(profileId as string, { size, page: index })
    .then((response) => {
      isLoading.value = false;
      data.value = response;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onCreateDocument(_item?: any) {
  showModal.value = false;
  loadDocuments(currentPage);
}

function onDeleteDocument(id: any, index: number) {
  showAlertConfirm("Are you sure", "You want to delete this document")
    .then((response) => {
      if (response) {
        isLoading.value = true;
        deleteAgencyCompanyDocument(profileId as string, id)
          .then(() => {
            isLoading.value = false;
            showAlertSuccess("Deleted");
            data.value.items.splice(index, 1);
          })
          .catch((error) => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    })
    .catch((error) => {
      showAlertError(error);
    });
}
</script>
