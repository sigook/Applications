<template>
  <form class="sd-form" @submit.prevent>
    <b-field label="Type">
      <div class="sd-choices">
        <button
          v-for="option in INTERACTION_TYPES"
          :key="option"
          type="button"
          class="sd-choice"
          :class="{ 'is-active': type === option }"
          @click="type = option"
        >
          {{ INTERACTION_TYPE_LABELS[option] }}
        </button>
      </div>
    </b-field>

    <b-field label="Client">
      <search-select
        v-if="!isEditing"
        v-model="companyProfileId"
        :options="clientOptions"
        :loading="isLoadingClients"
        clearable
        placeholder="Search client…"
      />
      <p v-else class="sd-readonly">{{ interaction?.companyName }}</p>
    </b-field>

    <b-field label="Purpose">
      <search-select :model-value="purpose" :options="purposeOptions" placeholder="Search purpose…" @update:model-value="setPurpose" />
    </b-field>

    <b-field label="Status">
      <search-select :model-value="status" :options="statusOptions" placeholder="Search status…" @update:model-value="setStatus" />
    </b-field>

    <b-field label="Description">
      <b-input v-model="description" type="textarea" placeholder="What was discussed…"></b-input>
    </b-field>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { getSalesCompanies } from '@/api/salesApi';
import { createCompanyInteraction, updateCompanyInteraction } from '@/api/companyInteractionApi';
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
} from '@/types/companyInteraction';
import type { CompanyInteraction } from '@/types/companyInteraction';
import type { AgencyCompanyListItem } from '@/types/agency';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import SearchSelect from './SearchSelect.vue';

const props = defineProps<{ interaction?: CompanyInteraction | null }>();

const isEditing = computed(() => !!props.interaction);

const clientOptions = computed(() => clients.value.map((c) => ({ value: c.id, label: c.fullName })));
const purposeOptions = INTERACTION_PURPOSES.map((p) => ({ value: p, label: INTERACTION_PURPOSE_LABELS[p] }));
const statusOptions = INTERACTION_STATUSES.map((s) => ({ value: s, label: INTERACTION_STATUS_LABELS[s] }));

function setPurpose(value: InteractionPurpose | null): void {
  if (value !== null) purpose.value = value;
}

function setStatus(value: InteractionStatus | null): void {
  if (value !== null) status.value = value;
}

const clients = ref<AgencyCompanyListItem[]>([]);
const isLoadingClients = ref(false);

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
  isLoadingClients.value = true;
  getSalesCompanies({ pageSize: 100 })
    .then((result) => {
      clients.value = result.items;
    })
    .catch((error) => showAlertError(error))
    .finally(() => {
      isLoadingClients.value = false;
    });
});

function resetForm(): void {
  type.value = InteractionType.Call;
  companyProfileId.value = null;
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
      box-shadow: 0 0 0 2px rgba(33, 183, 255, 0.15);
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
  border: 0;
  border-radius: 7px;
  padding: 0.4rem 0.85rem;
  font-size: 0.78rem;
  font-weight: 600;
  background: #eef0f3;
  color: #666;
  cursor: pointer;
  transition: background-color 0.15s ease, color 0.15s ease;

  &.is-active {
    background: $primary;
    color: $white;
  }

  &:focus-visible {
    outline: 2px solid rgba(33, 183, 255, 0.5);
    outline-offset: 1px;
  }
}
</style>
