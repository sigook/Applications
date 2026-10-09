<template>
  <detail-card title="Documents">
    <template #actions>
      <b-button type="is-ghost" size="is-small" class="detail-link" icon-left="plus" @click="showModal = true">
        Add document
      </b-button>
    </template>

    <span v-if="isLoading && !data" class="detail-empty">Loading…</span>
    <span v-else-if="!data?.items.length" class="detail-empty">No documents</span>
    <ul v-else class="company-documents">
      <li v-for="(document, index) in data.items" :key="document.id" class="company-document">
        <b-icon icon="file-document-outline" class="company-document-icon" />
        <div class="company-document-text">
          <a v-if="document.canDownload" :href="document.pathFile" target="_blank" download>
            {{ filename(document.fileName) }}
          </a>
          <span v-else>{{ filename(document.fileName) }}</span>
          <span v-if="document.description" class="detail-muted">{{ document.description }}</span>
        </div>
        <b-button v-if="document.canDownload" type="is-ghost" icon-left="delete-outline"
          class="detail-icon-button is-destructive" aria-label="Delete document" @click="onDelete(document.id, index)" />
      </li>
    </ul>

    <b-pagination v-if="data && data.totalItems > size" v-model="currentPage" :total="data.totalItems" :per-page="size"
      size="is-small" rounded @change="load" />

    <b-modal custom-content-class="card" v-model="showModal" width="500px" :destroy-on-hide="true">
      <documents-form :profile-id="profileId" @onCreateDocument="onCreated" />
    </b-modal>
  </detail-card>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { filename } from '@/shared/format';
import { deleteAgencyCompanyDocument, getAgencyCompanyDocument } from '@/modules/agency/recruiting/clients/api';
import type { CompanyProfileDocumentModel } from '@/modules/agency/recruiting/clients/types';
import type { PaginatedList } from '@/shared/types/common';
import DetailCard from '@/shared/detail-page/DetailCard.vue';
import DocumentsForm from '@/modules/agency/recruiting/clients/components/DocumentsForm.vue';

const props = defineProps<{ profileId: string }>();
const emit = defineEmits<{ (e: 'changed', delta: number): void }>();

const size = 10;
const currentPage = ref(1);
const isLoading = ref(false);
const showModal = ref(false);
const data = ref<PaginatedList<CompanyProfileDocumentModel> | null>(null);

function load(page: number) {
  isLoading.value = true;
  getAgencyCompanyDocument(props.profileId, { size, page })
    .then((response) => {
      data.value = response;
    })
    .catch(showAlertError)
    .finally(() => {
      isLoading.value = false;
    });
}

function onCreated() {
  showModal.value = false;
  emit('changed', 1);
  load(currentPage.value);
}

function onDelete(id: string | undefined, index: number) {
  if (!id) return;
  showAlertConfirm('Are you sure', 'You want to delete this document')
    .then((confirmed) => {
      if (!confirmed) return;
      isLoading.value = true;
      return deleteAgencyCompanyDocument(props.profileId, id).then(() => {
        data.value?.items.splice(index, 1);
        emit('changed', -1);
        showAlertSuccess('Deleted');
      });
    })
    .catch(showAlertError)
    .finally(() => {
      isLoading.value = false;
    });
}

load(currentPage.value);
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.company-documents {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 10px;
  margin: 0;
  padding: 0;
  list-style: none;
}

.company-document {
  display: flex;
  align-items: center;
  gap: 12px;
  min-width: 0;
  padding: 10px 12px;
  border: 1px solid $gray-border;
  border-radius: 8px;
}

.company-document-icon {
  flex-shrink: 0;
  color: $blue;
}

.company-document-text {
  display: flex;
  flex: 1;
  flex-direction: column;
  gap: 1px;
  min-width: 0;
  font-size: 0.9rem;
  overflow-wrap: anywhere;

  a {
    font-weight: 600;
    color: $blue;
  }
}
</style>
