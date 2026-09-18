<template>
  <div style="position: relative">
    <b-loading v-model="isLoading" :is-full-page="false" />

    <p v-if="!items.length && !isLoading" class="has-text-grey">No requirements configured for this request.</p>

    <b-message v-if="status === RequestApplicantStatus.Pending" type="is-info" size="is-small">
      Start the applicant to mark compliance items.
    </b-message>
    <b-message v-else-if="isCandidate && canEdit" type="is-info" size="is-small">
      Convert to worker to upload documents. Checks can still be marked.
    </b-message>
    <b-message v-else-if="canEdit" type="is-info" size="is-small">
      Attach the document and fill the information, then check the item to save it. Items with a document already on
      file can be checked as is, or replaced with a new one.
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
        <a :href="item.existingFileUrl" target="_blank" download class="compliance-existing">
          <b-icon icon="file-check-outline" size="is-small"></b-icon>
          <span class="is-size-7">On file</span>
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
              <span class="file-label">{{ item.existingFileUrl ? 'Replace' : 'Attach' }}</span>
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
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { showAlertError } from '@/utils/toast';
import { emailName, dateMonth } from '@/utils/filters';
import { generateFileName } from '@/utils/fileNaming';
import { UPLOAD_ACCEPT, validateUploadFile } from '@/utils/fileValidation';
import { getIdentificationTypes } from '@/api/catalogApi';
import {
  getApplicantComplianceItems,
  completeApplicantComplianceItem,
  uncompleteApplicantComplianceItem,
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
  status: RequestApplicantStatus;
  workerProfileId?: string | null;
}>();
const emit = defineEmits<{
  (e: 'updated'): void;
  (e: 'loaded', value: { mandatoryCompleted: boolean; itemsCount: number; completedCount: number }): void;
}>();

const isLoading = ref(false);
const items = ref<ApplicantComplianceItem[]>([]);
const identificationTypes = ref<IdentificationType[]>([]);
const drafts = ref<Record<string, ComplianceDraft>>({});
const checkedItems = ref<Record<string, boolean>>({});

const isCandidate = computed(() => !props.workerProfileId);
const canEdit = computed(() => props.status === RequestApplicantStatus.InProgress);

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
      emit('loaded', {
        mandatoryCompleted: response.every(item => !item.isMandatory || item.isCompleted),
        itemsCount: response.length,
        completedCount: response.filter(item => item.isCompleted).length,
      });
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
  if (item.existingFileUrl && !draft.file) return '';
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

watch(() => props.applicantId, () => loadItems());

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

.compliance-existing {
  display: inline-flex;
  align-items: center;
  gap: 0.15rem;
}
</style>
