<template>
  <form @submit.prevent>
    <div class="columns is-multiline">
      <div class="column is-12">
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
      </div>

      <div class="column is-12">
        <b-field
          label="Client *"
          :type="formErrors.companyProfileId ? 'is-danger' : ''"
          :message="isEditing ? '' : formErrors.companyProfileId || CLIENT_SEARCH_HINT"
        >
          <search-select
            v-if="!isEditing"
            v-model="companyProfileId"
            :options="clientOptions"
            :loading="isLoadingClients"
            :min-search-length="MINIMUM_SEARCH_LENGTH"
            remote
            placeholder="Search client…"
            @search="onClientSearch"
          />
          <p v-else class="sd-readonly">{{ interaction?.companyName }}</p>
        </b-field>
      </div>

      <div class="column is-6">
        <b-field label="Purpose">
          <b-select v-model="purpose" expanded>
            <option v-for="opt in purposeOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
          </b-select>
        </b-field>
      </div>

      <div class="column is-6">
        <b-field label="Status">
          <b-select v-model="status" expanded>
            <option v-for="opt in statusOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
          </b-select>
        </b-field>
      </div>

      <div class="column is-12">
        <b-field
          label="Description *"
          :type="formErrors.description ? 'is-danger' : ''"
          :message="formErrors.description || ''"
        >
          <b-input v-model="description" name="description" type="textarea" placeholder="What was discussed…"></b-input>
        </b-field>
      </div>
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { createCompanyInteraction, getAgencyCompaniesList, updateCompanyInteraction } from '@/api/agencyCompanyApi';
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
import type { SalesClientReference } from '@/types/sales';
import type { CatalogItem } from '@/types/common';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import SearchSelect from '@/components/SearchSelect.vue';

const MINIMUM_SEARCH_LENGTH = 3;
const CLIENT_SEARCH_HINT = `Type at least ${MINIMUM_SEARCH_LENGTH} characters to search`;

const props = defineProps<{
  interaction?: CompanyInteraction | null;
  initialClient?: SalesClientReference | null;
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

interface InteractionFormValues {
  companyProfileId: string | null;
  type: InteractionType;
  purpose: InteractionPurpose;
  status: InteractionStatus;
  description: string;
}

const validationSchema = yup.object({
  companyProfileId: yup.string().nullable().required('Client is required'),
  description: yup.string().trim().required('Description is required').max(5000, 'Max 5000 characters'),
});

const form = useStickyForm<InteractionFormValues>({
  schema: validationSchema,
  initialValues: {
    companyProfileId: null,
    type: InteractionType.Call,
    purpose: InteractionPurpose.Intro,
    status: InteractionStatus.NotStarted,
    description: '',
  },
});
const { companyProfileId, type, purpose, status, description } = form.fields;
const formErrors = form.errors;

onMounted(() => {
  if (props.interaction) {
    form.hydrate({
      companyProfileId: props.interaction.companyProfileId,
      type: props.interaction.interactionType,
      purpose: props.interaction.interactionPurpose,
      status: props.interaction.interactionStatus,
      description: props.interaction.description,
    });
    return;
  }
  if (props.initialClient) {
    form.hydrate({ companyProfileId: props.initialClient.id });
  }
});

function resetForm(): void {
  form.hydrate({ companyProfileId: props.initialClient?.id ?? null });
}

function submit(): Promise<boolean> {
  form.markInteracted();
  return new Promise<boolean>((resolve) => {
    form.handleSubmit(
      async (values) => {
        try {
          if (props.interaction) {
            await updateCompanyInteraction(props.interaction.companyProfileId, props.interaction.id, {
              description: values.description.trim(),
              interactionPurpose: values.purpose,
              interactionType: values.type,
              interactionStatus: values.status,
            });
            showAlertSuccess('Interaction updated');
          } else {
            await createCompanyInteraction(values.companyProfileId as string, {
              description: values.description.trim(),
              interactionPurpose: values.purpose,
              interactionType: values.type,
              interactionStatus: values.status,
            });
            showAlertSuccess('Interaction logged');
            resetForm();
          }
          resolve(true);
        } catch (error) {
          await showAlertError(error);
          resolve(false);
        }
      },
      () => {
        showAlertError('Please make sure all required fields are filled out correctly');
        resolve(false);
      }
    )();
  });
}

defineExpose({ submit });
</script>

<style scoped lang="scss">
@use "sass:color";
@import "../../assets/scss/variables";

.sd-readonly {
  color: $grey-font;
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
  font-weight: 600;
  background: $gray-bg;
  color: $grey-light;
  cursor: pointer;
  transition: background-color 0.15s ease, color 0.15s ease;

  &:hover {
    background: color.adjust($gray-bg, $lightness: -5%);
    color: $grey-font;
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
