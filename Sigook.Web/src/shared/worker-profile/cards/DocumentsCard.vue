<template>
  <profile-card title="Documents">
    <template #actions>
      <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
        <template #trigger>
          <b-button type="is-ghost" size="is-small" class="profile-link" icon-left="plus" icon-right="menu-down">Add</b-button>
        </template>
        <b-dropdown-item aria-role="listitem" @click="open('identification')">Identification</b-dropdown-item>
        <b-dropdown-item aria-role="listitem" @click="open('resume')">Resume</b-dropdown-item>
        <b-dropdown-item aria-role="listitem" @click="open('license')">License</b-dropdown-item>
        <b-dropdown-item aria-role="listitem" @click="open('certificate')">Certificate</b-dropdown-item>
        <b-dropdown-item aria-role="listitem" @click="open('other')">Other document</b-dropdown-item>
      </b-dropdown>
    </template>

    <div class="documents-scroll">
      <table class="documents-table">
        <thead>
          <tr>
            <th>Type</th>
            <th>Document</th>
            <th>Number</th>
            <th>Expires</th>
            <th>Status</th>
            <th><span class="is-sr-only">Actions</span></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in rows" :key="row.key">
            <td class="documents-type">{{ row.type }}</td>
            <td>
              <a v-if="row.file?.pathFile" :href="row.file.pathFile" target="_blank" download>{{ row.name }}</a>
              <span v-else class="profile-empty">{{ row.name }}</span>
            </td>
            <td :class="{ 'profile-empty': !row.number }">{{ row.number || '—' }}</td>
            <td :class="{ 'profile-empty': !row.expires }">{{ row.expires ? dateMonth(row.expires) : '—' }}</td>
            <td><span class="profile-status" :class="`is-${row.status.tone}`">{{ row.status.label }}</span></td>
            <td class="documents-actions">
              <b-button v-if="!row.file" type="is-ghost" size="is-small" class="profile-link" @click="open(formForKind(row.kind))">Upload</b-button>
              <b-button v-else-if="row.kind === 'identification' || row.kind === 'resume' || row.kind === 'policeCheck'"
                type="is-ghost" size="is-small" class="profile-icon-button" icon-left="pencil" aria-label="Replace document"
                @click="open(formForKind(row.kind))" />
              <b-button v-if="row.deletableId" type="is-ghost" size="is-small" class="profile-icon-button is-destructive"
                icon-left="delete-outline" aria-label="Delete document" @click="confirmDelete(row)" />
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <b-modal custom-content-class="card" v-model="isModalOpen" width="500px">
      <documents-form v-if="activeForm === 'identification'" :data="props.worker" @closeModal="onSaved" />
      <resume-form v-else-if="activeForm === 'resume'" :data="props.worker" @closeModal="onSaved" />
      <license-form v-else-if="activeForm === 'license'" :data="props.worker" @closeModal="onSaved" />
      <certificate-form v-else-if="activeForm === 'certificate'" :data="props.worker" @closeModal="onSaved" />
      <other-documents-form v-else-if="activeForm === 'other'" :data="props.worker" @closeAndUpdate="onSaved" />
    </b-modal>
  </profile-card>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { showAlertConfirm, showAlertError } from '@/shared/utils/toast';
import { dateMonth, filename } from '@/shared/format';
import { deleteWorkerCertificates, deleteWorkerLicenses, deleteWorkerOtherDocuments } from '@/shared/worker-profile/api';
import { expiryStatus } from '@/shared/worker-profile/useWorkerProfileStatus';
import type { CovenantFileModel } from '@/shared/types/common';
import type { WorkerDocumentKind, WorkerDocumentRow, WorkerExpiryStatus, WorkerProfileDetail } from '@/shared/worker-profile/types';
import ProfileCard from '@/shared/worker-profile/cards/ProfileCard.vue';
import DocumentsForm from '@/shared/worker-profile/forms/WorkDocumentsForm.vue';
import ResumeForm from '@/shared/worker-profile/forms/WorkResumeForm.vue';
import LicenseForm from '@/shared/worker-profile/forms/WorkLicenseForm.vue';
import CertificateForm from '@/shared/worker-profile/forms/WorkCertificatesForm.vue';
import OtherDocumentsForm from '@/shared/worker-profile/forms/WorkerOtherDocumentsForm.vue';

type DocumentForm = 'identification' | 'resume' | 'license' | 'certificate' | 'other';

const props = defineProps<{ worker: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'updateProfile'): void; (e: 'loading', value: boolean): void }>();

const isModalOpen = ref(false);
const activeForm = ref<DocumentForm | null>(null);

const onFile: WorkerExpiryStatus = { label: 'On file', tone: 'success' };
const missing: WorkerExpiryStatus = { label: 'Missing', tone: 'danger' };

function fileLabel(file: CovenantFileModel | null, fallback: string): string {
  return file?.description || (file?.fileName ? filename(file.fileName) : fallback);
}

const rows = computed<WorkerDocumentRow[]>(() => {
  const w = props.worker;
  const result: WorkerDocumentRow[] = [];

  if (w.identificationType1 || w.identificationType1File) {
    result.push({
      key: 'id-1', kind: 'identification', type: 'Identification', name: w.identificationType1?.value ?? 'Identification',
      number: w.identificationNumber1, file: w.identificationType1File, expires: null,
      status: w.identificationType1File ? onFile : missing, deletableId: null,
    });
  } else {
    result.push({
      key: 'id-1', kind: 'identification', type: 'Identification', name: 'Not uploaded',
      number: null, file: null, expires: null, status: missing, deletableId: null,
    });
  }

  if (w.identificationType2 || w.identificationType2File) {
    result.push({
      key: 'id-2', kind: 'identification', type: 'Identification', name: w.identificationType2?.value ?? 'Identification',
      number: w.identificationNumber2, file: w.identificationType2File, expires: null,
      status: w.identificationType2File ? onFile : missing, deletableId: null,
    });
  }

  if (w.havePoliceCheckBackground) {
    result.push({
      key: 'police', kind: 'policeCheck', type: 'Background', name: 'Police check',
      number: null, file: w.policeCheckBackGround, expires: null,
      status: w.policeCheckBackGround ? onFile : missing, deletableId: null,
    });
  }

  result.push({
    key: 'resume', kind: 'resume', type: 'Resume', name: w.resume ? 'Resume' : 'Not uploaded',
    number: null, file: w.resume, expires: null, status: w.resume ? onFile : missing, deletableId: null,
  });

  w.licenses.forEach((item, index) => {
    result.push({
      key: `license-${index}`, kind: 'license', type: 'License', name: fileLabel(item.license, 'License'),
      number: item.number, file: item.license, expires: item.expires,
      status: expiryStatus(item.expires), deletableId: item.license.id ?? null,
    });
  });

  w.certificates.forEach((item, index) => {
    result.push({
      key: `certificate-${index}`, kind: 'certificate', type: 'Certificate', name: fileLabel(item, 'Certificate'),
      number: null, file: item, expires: null, status: onFile, deletableId: item.id ?? null,
    });
  });

  w.otherDocuments.forEach((item, index) => {
    result.push({
      key: `other-${index}`, kind: 'other', type: 'Other', name: fileLabel(item, 'Document'),
      number: null, file: item, expires: null, status: onFile, deletableId: item.id ?? null,
    });
  });

  return result;
});

function formForKind(kind: WorkerDocumentKind): DocumentForm {
  switch (kind) {
    case 'resume':
      return 'resume';
    case 'license':
      return 'license';
    case 'certificate':
      return 'certificate';
    case 'other':
      return 'other';
    default:
      return 'identification';
  }
}

function open(form: DocumentForm) {
  activeForm.value = form;
  isModalOpen.value = true;
}

function onSaved() {
  isModalOpen.value = false;
  emit('updateProfile');
}

function deleteRequest(row: WorkerDocumentRow, id: string): Promise<void> {
  if (row.kind === 'license') {
    return deleteWorkerLicenses(props.worker.id, id);
  }
  if (row.kind === 'certificate') {
    return deleteWorkerCertificates(props.worker.id, id);
  }
  return deleteWorkerOtherDocuments(props.worker.id, id);
}

function confirmDelete(row: WorkerDocumentRow) {
  const id = row.deletableId;
  if (!id) {
    return;
  }
  showAlertConfirm('Are you sure?', 'You want to delete this document').then((response) => {
    if (!response) {
      return;
    }
    emit('loading', true);
    deleteRequest(row, id)
      .then(() => emit('updateProfile'))
      .catch((error) => {
        emit('loading', false);
        showAlertError(error);
      });
  });
}
</script>

<style lang="scss" scoped>
@import '@/assets/scss/worker-profile';

.documents-scroll {
  overflow-x: auto;
}

.documents-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
  color: $navy;

  th {
    padding: 6px 12px 8px 0;
    text-align: left;
    font-size: 0.75rem;
    font-weight: 600;
    color: $grey-font;
    white-space: nowrap;
  }

  td {
    padding: 10px 12px 10px 0;
    border-top: 1px solid $gray-border;
    vertical-align: middle;
  }

  a {
    color: $blue;
    overflow-wrap: anywhere;
  }
}

.documents-type {
  white-space: nowrap;
  color: $grey-font;
}

.documents-actions {
  white-space: nowrap;
  text-align: right;
}
</style>
