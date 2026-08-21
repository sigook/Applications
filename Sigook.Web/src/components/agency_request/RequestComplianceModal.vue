<template>
  <div class="p-3">
    <h3 class="title is-4 mb-2">Compliance</h3>
    <p class="has-text-grey mb-4">Requirements the worker must meet for this order.</p>

    <p v-if="!items.length" class="has-text-grey mb-4">No requirements configured.</p>

    <div v-for="(item, index) in items" :key="item.id ?? `new-${index}`" class="compliance-row">
      <span class="compliance-name">{{ item.name }}</span>
      <b-select v-model="item.documentTarget" size="is-small" class="compliance-target">
        <option v-for="documentTarget in documentTargets" :key="documentTarget" :value="documentTarget">
          {{ documentTargetLabel(documentTarget) }}
        </option>
      </b-select>
      <b-switch v-model="item.isMandatory" size="is-small">
        {{ item.isMandatory ? 'Mandatory' : 'Optional' }}
      </b-switch>
      <b-button size="is-small" type="is-text" icon-left="delete" @click="removeItem(index)"></b-button>
    </div>

    <div class="compliance-add">
      <b-input v-model="newItemName" placeholder="Add requirement" maxlength="200" :has-counter="false"
        class="compliance-new-name" @keyup.enter="addItem"></b-input>
      <b-select v-model="newItemTarget" size="is-small" class="compliance-target">
        <option v-for="documentTarget in documentTargets" :key="documentTarget" :value="documentTarget">
          {{ documentTargetLabel(documentTarget) }}
        </option>
      </b-select>
      <b-switch v-model="newItemMandatory" size="is-small">Mandatory</b-switch>
      <b-button @click="addItem">Add</b-button>
    </div>
    <p v-if="addError" class="help is-danger">{{ addError }}</p>

    <div class="compliance-actions">
      <b-button type="is-text" @click="resetToDefaults">Reset to defaults</b-button>
      <b-button type="is-primary" @click="save">Save</b-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { buildDefaultComplianceItems } from '@/constants/compliance';
import type { RequestComplianceItem } from '@/types/agency';
import {
  COMPLIANCE_DOCUMENT_TARGETS,
  COMPLIANCE_DOCUMENT_TARGET_LABELS,
  ComplianceDocumentTarget,
} from '@/types/requestApplicant';

const props = defineProps<{ items?: RequestComplianceItem[] }>();

const emit = defineEmits<{ (e: 'save', items: RequestComplianceItem[]): void }>();

const items = ref<RequestComplianceItem[]>((props.items ?? []).map((item) => ({ ...item })));
const newItemName = ref('');
const newItemMandatory = ref(true);
const newItemTarget = ref<ComplianceDocumentTarget>(ComplianceDocumentTarget.None);
const addError = ref('');

const documentTargets = COMPLIANCE_DOCUMENT_TARGETS;

function documentTargetLabel(documentTarget: ComplianceDocumentTarget): string {
  return COMPLIANCE_DOCUMENT_TARGET_LABELS[documentTarget];
}

function addItem() {
  const name = newItemName.value.trim();
  if (!name) {
    addError.value = 'Requirement name is required';
    return;
  }
  if (items.value.some((item) => item.name.toLowerCase() === name.toLowerCase())) {
    addError.value = 'That requirement already exists';
    return;
  }
  items.value.push({ name, isMandatory: newItemMandatory.value, documentTarget: newItemTarget.value });
  newItemName.value = '';
  newItemMandatory.value = true;
  newItemTarget.value = ComplianceDocumentTarget.None;
  addError.value = '';
}

function removeItem(index: number) {
  items.value.splice(index, 1);
  addError.value = '';
}

function resetToDefaults() {
  items.value = buildDefaultComplianceItems();
  addError.value = '';
}

function save() {
  emit('save', items.value.map((item) => ({ ...item })));
}
</script>

<style scoped>
.compliance-row,
.compliance-add {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.compliance-row {
  padding: 0.35rem 0;
  border-bottom: 1px solid #e0e0e0;
}

.compliance-name {
  flex: 0 0 6rem;
  min-width: 0;
  font-weight: 600;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.compliance-add {
  margin-top: 1rem;
}

.compliance-new-name {
  flex: 0 0 6rem;
  min-width: 0;
}

.compliance-target {
  flex: 1 1 auto;
  min-width: 0;
}

.compliance-target :deep(.select),
.compliance-target :deep(select) {
  width: 100%;
}

.compliance-row :deep(.switch),
.compliance-add :deep(.switch) {
  flex: 0 0 8rem;
  margin-right: 0;
  white-space: nowrap;
}

.compliance-row :deep(.button),
.compliance-add :deep(.button) {
  flex: 0 0 4rem;
}

.compliance-actions {
  display: flex;
  justify-content: space-between;
  margin-top: 1.5rem;
}
</style>
