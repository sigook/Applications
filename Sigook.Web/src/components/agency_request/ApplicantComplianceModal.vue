<template>
  <div class="modal-card" style="width: auto">
    <header class="modal-card-head">
      <div>
        <p class="modal-card-title">Compliance</p>
        <p class="is-size-7 has-text-grey">{{ name }}</p>
      </div>
    </header>
    <section class="modal-card-body" style="min-width: 760px; position: relative">
      <b-loading v-model="isLoading" :is-full-page="false" />

      <p v-if="!items.length && !isLoading" class="has-text-grey">No requirements configured for this order.</p>

      <b-message v-if="currentStatus === RequestApplicantStatus.Pending" type="is-info" size="is-small">
        Start the applicant to mark compliance items.
      </b-message>
      <b-message v-else-if="isCandidate && canEdit" type="is-info" size="is-small">
        Convert to worker to upload documents. Checks can still be marked.
      </b-message>
      <b-message v-else-if="canEdit" type="is-info" size="is-small">
        Attach the document and fill the information, then check the item to save it.
      </b-message>

      <div v-for="item in items" :key="item.id" class="compliance-item">
        <b-tooltip :label="blockedReason(item)" :active="!!blockedReason(item)" type="is-dark" position="is-right">
          <b-checkbox :model-value="checkedItems[item.id]" :disabled="!canEdit || !!blockedReason(item)"
            @update:modelValue="(value: boolean) => onItemToggled(item, value)">
            {{ item.name }}
          </b-checkbox>
        </b-tooltip>
        <b-tag v-if="item.isMandatory" type="is-warning" size="is-small">Mandatory</b-tag>
        <b-tooltip v-if="item.existingFileUrl" label="View the document currently on the profile" type="is-dark"
          position="is-top">
          <a :href="item.existingFileUrl" target="_blank" download>
            <b-icon icon="file-eye-outline" size="is-small"></b-icon>
          </a>
        </b-tooltip>

        <span v-if="item.isCompleted" class="is-size-7 has-text-grey compliance-item-right">
          {{ emailName(item.completedBy ?? '') }} — {{ dateMonth(item.completedAt ?? '') }}
        </span>
        <template v-else-if="showUploadControls(item)">
          <b-input v-if="isIdentification(item)" v-model="drafts[item.id].identificationNumber" size="is-small"
            placeholder="Identification number" maxlength="15" :has-counter="false"
            class="compliance-item-right compliance-number" />
          <b-select v-if="isIdentification(item)" v-model="drafts[item.id].identificationTypeId" size="is-small"
            placeholder="Type" class="compliance-type">
            <option v-for="type in identificationTypes" :key="type.id" :value="type.id">{{ type.value }}</option>
          </b-select>
          <b-input v-if="isSocialInsurance(item)" v-model="drafts[item.id].socialInsuranceNumber" size="is-small"
            placeholder="Social insurance number" maxlength="15" :has-counter="false"
            class="compliance-item-right compliance-number" />
          <b-field class="file mb-0"
            :class="{ 'compliance-item-right': !isIdentification(item) && !isSocialInsurance(item), 'has-name': !!drafts[item.id].file }">
            <b-upload :model-value="null" :accept="UPLOAD_ACCEPT" class="file-label"
              @update:modelValue="(file: File | null) => onFileSelected(item, file)">
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload" size="is-small"></b-icon>
                <span class="file-label">Attach</span>
              </span>
              <span v-if="drafts[item.id].file" class="file-name compliance-file-name">
                {{ drafts[item.id].file?.name }}
              </span>
            </b-upload>
          </b-field>
          <b-button v-if="drafts[item.id].file" type="is-text" size="is-small" icon-right="close"
            @click="clearFile(item)" />
        </template>
      </div>
    </section>
    <footer class="modal-card-foot is-justify-content-space-between">
      <b-button @click="emit('close')">Close</b-button>
      <div class="buttons mb-0">
        <b-button v-if="canCancel" type="is-danger" outlined :loading="isChangingStatus" @click="confirmCancel">
          Cancel applicant
        </b-button>
        <b-button v-if="currentStatus === RequestApplicantStatus.Pending" type="is-primary"
          :loading="isChangingStatus" @click="start">
          Start
        </b-button>
        <b-button v-if="currentStatus === RequestApplicantStatus.Cancelled" type="is-primary"
          :loading="isChangingStatus" @click="reopen">
          Reopen
        </b-button>
        <b-tooltip v-if="currentStatus === RequestApplicantStatus.InProgress" :label="confirmBlockedReason"
          :active="!canConfirm" type="is-dark" position="is-top">
          <b-button type="is-primary" :disabled="!canConfirm" :loading="isConfirming" @click="confirm">
            Confirm applicant
          </b-button>
        </b-tooltip>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { getDialog } from '@/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { emailName, dateMonth } from '@/utils/filters';
import { generateFileName } from '@/utils/fileNaming';
import { UPLOAD_ACCEPT, validateUploadFile } from '@/utils/fileValidation';
import { getIdentificationTypes } from '@/api/catalogApi';
import {
  getApplicantComplianceItems,
  completeApplicantComplianceItem,
  uncompleteApplicantComplianceItem,
  changeApplicantStatus,
} from '@/api/agencyRequestApi';
import type { ApplicantComplianceItem, CompleteApplicantComplianceItemModel } from '@/types/agency';
import type { IdentificationType } from '@/types/common';
import { ComplianceDocumentTarget, RequestApplicantStatus } from '@/types/requestApplicant';

interface ComplianceDraft {
  identificationNumber: string;
  identificationTypeId: string | null;
  socialInsuranceNumber: string;
  file: File | null;
}

const props = defineProps<{
  requestId: string;
  applicantId: string;
  name: string;
  status: RequestApplicantStatus;
  workerProfileId?: string | null;
}>();
const emit = defineEmits<{ (e: 'updated'): void; (e: 'close'): void }>();

const isLoading = ref(false);
const isConfirming = ref(false);
const isChangingStatus = ref(false);
const currentStatus = ref<RequestApplicantStatus>(props.status);
const items = ref<ApplicantComplianceItem[]>([]);
const identificationTypes = ref<IdentificationType[]>([]);
const drafts = ref<Record<string, ComplianceDraft>>({});
const checkedItems = ref<Record<string, boolean>>({});

const isCandidate = computed(() => !props.workerProfileId);
const canEdit = computed(() => currentStatus.value === RequestApplicantStatus.InProgress);
const canCancel = computed(() =>
  currentStatus.value === RequestApplicantStatus.Pending || currentStatus.value === RequestApplicantStatus.InProgress);
const mandatoryCompleted = computed(() => items.value.every(item => !item.isMandatory || item.isCompleted));
const canConfirm = computed(() => canEdit.value && !isCandidate.value && mandatoryCompleted.value);

const confirmBlockedReason = computed(() => {
  if (isCandidate.value) return 'Convert the candidate to a worker first';
  if (!mandatoryCompleted.value) return 'Complete all mandatory items first';
  return '';
});

function emptyDraft(): ComplianceDraft {
  return { identificationNumber: '', identificationTypeId: null, socialInsuranceNumber: '', file: null };
}

function isIdentification(item: ApplicantComplianceItem): boolean {
  return item.documentTarget === ComplianceDocumentTarget.Identification1
    || item.documentTarget === ComplianceDocumentTarget.Identification2;
}

function isSocialInsurance(item: ApplicantComplianceItem): boolean {
  return item.documentTarget === ComplianceDocumentTarget.SocialInsurance;
}

function showUploadControls(item: ApplicantComplianceItem): boolean {
  return canEdit.value && !item.isCompleted && item.canUpload;
}

function loadItems() {
  isLoading.value = true;
  getApplicantComplianceItems(props.requestId, props.applicantId)
    .then(response => {
      items.value = response;
      for (const item of response) {
        drafts.value[item.id] ??= emptyDraft();
        checkedItems.value[item.id] = item.isCompleted;
      }
      if (response.some(isIdentification) && !identificationTypes.value.length) {
        loadIdentificationTypes();
      }
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isLoading.value = false;
    });
}

function loadIdentificationTypes() {
  getIdentificationTypes()
    .then(types => {
      identificationTypes.value = types;
    })
    .catch(error => showAlertError(error));
}

function onFileSelected(item: ApplicantComplianceItem, file: File | null) {
  if (!file) return;
  const error = validateUploadFile(file);
  if (error) {
    showAlertError(error);
    return;
  }
  drafts.value[item.id].file = file;
}

function clearFile(item: ApplicantComplianceItem) {
  drafts.value[item.id].file = null;
}

function blockedReason(item: ApplicantComplianceItem): string {
  if (item.isCompleted || !item.canUpload || !item.isMandatory) return '';
  const draft = drafts.value[item.id];
  if (!draft) return '';
  if (!draft.file) return 'Attach the document to complete this item';
  if (isIdentification(item) && (!draft.identificationNumber || !draft.identificationTypeId)) {
    return 'Enter the identification number and type to complete this item';
  }
  if (isSocialInsurance(item) && !draft.socialInsuranceNumber) {
    return 'Enter the social insurance number to complete this item';
  }
  return '';
}

function buildModel(item: ApplicantComplianceItem): CompleteApplicantComplianceItemModel {
  const model: CompleteApplicantComplianceItemModel = {};
  if (!item.canUpload) return model;
  const draft = drafts.value[item.id];
  if (draft.file) model.fileName = generateFileName('Document', draft.file.name);
  if (isIdentification(item)) {
    if (draft.identificationNumber) model.identificationNumber = draft.identificationNumber;
    if (draft.identificationTypeId) model.identificationTypeId = draft.identificationTypeId;
  }
  if (isSocialInsurance(item) && draft.socialInsuranceNumber) model.socialInsuranceNumber = draft.socialInsuranceNumber;
  return model;
}

function onItemToggled(item: ApplicantComplianceItem, checked: boolean) {
  checkedItems.value[item.id] = checked;
  if (!checked) {
    uncompleteItem(item);
    return;
  }
  completeItem(item, buildModel(item), drafts.value[item.id].file);
}

function completeItem(item: ApplicantComplianceItem, model: CompleteApplicantComplianceItemModel, file?: File | null) {
  isLoading.value = true;
  completeApplicantComplianceItem(props.requestId, props.applicantId, item.id, model, file)
    .then(() => {
      drafts.value[item.id] = emptyDraft();
      emit('updated');
      loadItems();
    })
    .catch(error => {
      isLoading.value = false;
      checkedItems.value[item.id] = item.isCompleted;
      showAlertError(error);
    });
}

function uncompleteItem(item: ApplicantComplianceItem) {
  isLoading.value = true;
  uncompleteApplicantComplianceItem(props.requestId, props.applicantId, item.id)
    .then(() => {
      emit('updated');
      loadItems();
    })
    .catch(error => {
      isLoading.value = false;
      checkedItems.value[item.id] = item.isCompleted;
      showAlertError(error);
    });
}

function start() {
  changeStatus(RequestApplicantStatus.InProgress, 'Applicant started');
}

function reopen() {
  changeStatus(RequestApplicantStatus.InProgress, 'Applicant reopened');
}

function confirmCancel() {
  getDialog().confirm({
    title: 'Cancel applicant',
    message: `Cancel <strong>${props.name}</strong>? Their compliance checks are kept and the applicant can be reopened later.`,
    confirmText: 'Cancel applicant',
    cancelText: 'Keep',
    type: 'is-danger',
    hasIcon: true,
    onConfirm: () => changeStatus(RequestApplicantStatus.Cancelled, 'Applicant cancelled', true),
  });
}

function changeStatus(status: RequestApplicantStatus, successMessage: string, closeAfter = false) {
  isChangingStatus.value = true;
  changeApplicantStatus(props.requestId, props.applicantId, { status })
    .then(() => {
      currentStatus.value = status;
      showAlertSuccess(successMessage);
      emit('updated');
      if (closeAfter) emit('close');
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isChangingStatus.value = false;
    });
}

function confirm() {
  isConfirming.value = true;
  changeApplicantStatus(props.requestId, props.applicantId, { status: RequestApplicantStatus.Confirmed })
    .then(() => {
      showAlertSuccess('Applicant confirmed');
      emit('updated');
      emit('close');
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isConfirming.value = false;
    });
}

loadItems();
</script>

<style scoped>
.compliance-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.45rem 0;
  border-bottom: 1px solid #e0e0e0;
}

.compliance-item-right {
  margin-left: auto;
}

.compliance-number {
  width: 160px;
  flex-shrink: 0;
}

.compliance-type :deep(select) {
  max-width: 150px;
}

.compliance-file-name {
  max-width: 140px;
}
</style>
