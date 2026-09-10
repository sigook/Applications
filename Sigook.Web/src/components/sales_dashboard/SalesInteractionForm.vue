<template>
  <form class="sd-form" @submit.prevent>
    <b-field label="Type">
      <div class="sd-choices">
        <b-button
          v-for="option in INTERACTION_TYPES"
          :key="option"
          class="sd-choice"
          :class="{ 'is-active': type === option }"
          @click="type = option"
        >
          {{ INTERACTION_TYPE_LABELS[option] }}
        </b-button>
      </div>
    </b-field>

    <b-field label="Client">
      <search-select
        v-if="!isEditing"
        v-model="companyProfileId"
        :options="clientOptions"
        :loading="isLoadingClients"
        :min-search-length="MINIMUM_SEARCH_LENGTH"
        remote
        clearable
        placeholder="Search client…"
        @search="onClientSearch"
      />
      <p v-else class="sd-readonly">{{ interaction?.companyName }}</p>
    </b-field>

    <b-field label="Purpose">
      <b-select v-model="purpose" expanded>
        <option v-for="opt in purposeOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
      </b-select>
    </b-field>

    <b-field label="Status">
      <b-select v-model="status" expanded>
        <option v-for="opt in statusOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
      </b-select>
    </b-field>

    <b-field label="Description">
      <b-input v-model="description" type="textarea" placeholder="What was discussed…"></b-input>
    </b-field>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { getAgencyCompaniesList } from '@/api/agencyCompanyApi';
import { createCompanyInteraction, updateCompanyInteraction } from '@/api/companyApi';
import {
  InteractionType,
  InteractionPurpose,
  InteractionStatus,
  INTERACTION_TYPES,
  INTERACTION_PURPOSES,
  INTERACTION_STATUSES,
  INTERACTION_TYPE_LABELS,
  INTERACTION_PURPOSE_LABELS,
  INTERACTION_STATUS_LABELS,
} from '@/types/company';
import type { CompanyInteraction } from '@/types/company';
import type { AgencyCompanyListItem } from '@/types/agency';
import type { CatalogItem } from '@/types/common';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import SearchSelect from './SearchSelect.vue';

const MINIMUM_SEARCH_LENGTH = 3;

const props = defineProps<{
  interaction?: CompanyInteraction | null;
  initialClient?: AgencyCompanyListItem | null;
}>();

const isEditing = computed(() => !!props.interaction);

const clientOptions = computed(() => {
  const options = clients.value.map((c) => ({ value: c.id, label: c.value }));
  if (
    props.initialClient &&
    !clientSearchTerm.value.trim() &&
    !options.some((o) => o.value === props.initialClient?.id)
  ) {
    options.unshift({ value: props.initialClient.id, label: props.initialClient.fullName });
  }
  return options;
});
const purposeOptions = INTERACTION_PURPOSES.map((p) => ({ value: p, label: INTERACTION_PURPOSE_LABELS[p] }));
const statusOptions = INTERACTION_STATUSES.map((s) => ({ value: s, label: INTERACTION_STATUS_LABELS[s] }));

const clients = ref<CatalogItem[]>([]);
const isLoadingClients = ref(false);
const clientSearchTerm = ref('');

function loadClients(term: string): void {
  isLoadingClients.value = true;
  getAgencyCompaniesList(term || undefined)
    .then((result) => {
      clients.value = result;
    })
    .catch((error) => showAlertError(error))
    .finally(() => {
      isLoadingClients.value = false;
    });
}

function onClientSearch(term: string): void {
  clientSearchTerm.value = term;
  const normalized = term.trim();
  if (normalized.length < MINIMUM_SEARCH_LENGTH) {
    clients.value = [];
    return;
  }
  loadClients(normalized);
}

const type = ref<InteractionType>(InteractionType.Call);
const companyProfileId = ref<string | null>(null);
const purpose = ref<InteractionPurpose>(InteractionPurpose.Intro);
const status = ref<InteractionStatus>(InteractionStatus.NotStarted);
const description = ref('');

onMounted(() => {
  if (props.interaction) {
    type.value = props.interaction.interactionType;
    companyProfileId.value = props.interaction.companyProfileId;
    purpose.value = props.interaction.interactionPurpose;
    status.value = props.interaction.interactionStatus;
    description.value = props.interaction.description;
    return;
  }
  if (props.initialClient) {
    companyProfileId.value = props.initialClient.id;
  }
});

function resetForm(): void {
  type.value = InteractionType.Call;
  companyProfileId.value = props.initialClient?.id ?? null;
  purpose.value = InteractionPurpose.Intro;
  status.value = InteractionStatus.NotStarted;
  description.value = '';
}

async function submit(): Promise<boolean> {
  if (!companyProfileId.value) {
    await showAlertError('Please select a client');
    return false;
  }
  if (!description.value.trim()) {
    await showAlertError('Please enter a description');
    return false;
  }
  try {
    if (props.interaction) {
      await updateCompanyInteraction(props.interaction.id, {
        description: description.value.trim(),
        interactionPurpose: purpose.value,
        interactionType: type.value,
        interactionStatus: status.value,
      });
      showAlertSuccess('Interaction updated');
    } else {
      await createCompanyInteraction({
        companyProfileId: companyProfileId.value,
        description: description.value.trim(),
        interactionPurpose: purpose.value,
        interactionType: type.value,
        interactionStatus: status.value,
      });
      showAlertSuccess('Interaction logged');
      resetForm();
    }
    return true;
  } catch (error) {
    await showAlertError(error);
    return false;
  }
}

defineExpose({ submit });
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-form {
  display: flex;
  flex-direction: column;
  gap: 0.2rem;

  :deep(.label) {
    font-size: 0.75rem;
    font-weight: 600;
    color: #777;
    margin-bottom: 0.35rem;
  }

  :deep(.input),
  :deep(.textarea),
  :deep(.select select) {
    font-size: 0.82rem;
    border-color: $gray-border;
    box-shadow: none;
    color: #333;

    &:focus,
    &:active {
      border-color: $primary;
      box-shadow: 0 0 0 2px rgba($primary, 0.15);
    }
  }

  :deep(.textarea) {
    min-height: 5.5rem;
  }
}

.sd-readonly {
  font-size: 0.82rem;
  color: #333;
  padding: 0.35rem 0;
  font-weight: 600;
}

.sd-choices {
  display: flex;
  gap: 0.4rem;
  flex-wrap: wrap;
}

.sd-choice {
  height: auto;
  border: 0;
  border-radius: 7px;
  padding: 0.4rem 0.85rem;
  font-size: 0.78rem;
  font-weight: 600;
  background: #eef0f3;
  color: #666;
  cursor: pointer;
  transition: background-color 0.15s ease, color 0.15s ease;

  &:hover {
    background: #e4e7eb;
    color: #555;
  }

  &.is-active {
    background: $primary;
    color: $white;

    &:hover {
      background: $primary;
      color: $white;
    }
  }

  &:focus-visible {
    outline: 2px solid rgba($primary, 0.5);
    outline-offset: 1px;
  }
}
</style>
