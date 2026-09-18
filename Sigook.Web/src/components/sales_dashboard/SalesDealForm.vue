<template>
  <form @submit.prevent>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field label="Deal title *" :type="formErrors.title ? 'is-danger' : ''" :message="formErrors.title || ''">
          <b-input v-model="title" name="title" placeholder="e.g. Warehouse staffing – 40 FTE"></b-input>
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
          <p v-else class="sd-readonly">{{ deal?.companyName }}</p>
        </b-field>
      </div>

      <div class="column is-6">
        <b-field label="Value" :type="formErrors.value ? 'is-danger' : ''" :message="formErrors.value || ''">
          <b-numberinput v-model="value" name="value" :min="0" :max="1000000" :step="0.01" :controls="false" />
        </b-field>
      </div>

      <div class="column is-6">
        <b-field label="Type">
          <b-select v-model="type" expanded>
            <option v-for="opt in typeOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
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

      <div class="column is-6">
        <b-field label="Date *" :type="formErrors.date ? 'is-danger' : ''" :message="formErrors.date || ''">
          <b-datepicker
            ref="datePicker"
            v-model="date"
            name="date"
            placeholder="Pick a date"
            expanded
            @active-change="onPickerActive"
          ></b-datepicker>
        </b-field>
      </div>

      <div class="column is-12">
        <b-field label="Document" :type="fileError ? 'is-danger' : ''" :message="fileError">
          <div class="file is-primary" :class="{ 'has-name': !!documentFile }">
            <b-upload
              v-model="documentFile"
              class="file-label"
              :accept="UPLOAD_ACCEPT"
              name="dealDocument"
              @update:modelValue="onFileSelected"
            >
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload"></b-icon>
                <span class="file-label">{{ hasDocument ? 'Click to replace' : 'Click to upload' }}</span>
              </span>
              <span class="file-name" v-if="documentFile">{{ documentFile.name }}</span>
            </b-upload>
          </div>
        </b-field>

        <a
          v-if="isEditing && !documentFile && deal?.documentPath"
          class="sd-document sd-document--link"
          :href="deal.documentPath"
          target="_blank"
          rel="noopener"
        >
          <b-icon icon="paperclip" size="is-small"></b-icon>
          {{ deal.documentName }}
        </a>
        <p v-else-if="isEditing && !documentFile" class="sd-document">No document attached</p>
      </div>
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import type { ComponentPublicInstance } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { useDropdownReveal } from '@/composables/useDropdownReveal';
import { getAgencyCompaniesList } from '@/api/agencyCompanyApi';
import { createDeal, updateDeal } from '@/api/companyApi';
import {
  DealType,
  DealStatus,
  DEAL_TYPES,
  DEAL_STATUSES,
  DEAL_TYPE_LABELS,
  DEAL_STATUS_LABELS,
} from '@/types/company';
import type { Deal } from '@/types/company';
import type { CatalogItem } from '@/types/common';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { generateFileName } from '@/utils/fileNaming';
import { UPLOAD_ACCEPT, validateUploadFile } from '@/utils/fileValidation';
import SearchSelect from './SearchSelect.vue';

const MINIMUM_SEARCH_LENGTH = 3;
const CLIENT_SEARCH_HINT = `Type at least ${MINIMUM_SEARCH_LENGTH} characters to search`;

const props = defineProps<{ deal?: Deal | null }>();

const isEditing = computed(() => !!props.deal);
const hasDocument = computed(() => !!props.deal?.documentId);

const clientOptions = computed(() => clients.value.map((c) => ({ value: c.id, label: c.value })));
const typeOptions = DEAL_TYPES.map((t) => ({ value: t, label: DEAL_TYPE_LABELS[t] }));
const statusOptions = DEAL_STATUSES.map((s) => ({ value: s, label: DEAL_STATUS_LABELS[s] }));

const clients = ref<CatalogItem[]>([]);
const isLoadingClients = ref(false);

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
  const normalized = term.trim();
  if (normalized.length < MINIMUM_SEARCH_LENGTH) {
    clients.value = [];
    return;
  }
  loadClients(normalized);
}

interface DealFormValues {
  title: string;
  companyProfileId: string | null;
  date: Date | null;
  value: number | null;
  type: DealType;
  status: DealStatus;
}

const validationSchema = yup.object({
  title: yup.string().trim().required('Deal title is required').max(200, 'Max 200 characters'),
  companyProfileId: yup.string().nullable().required('Client is required'),
  date: yup.date().nullable().required('Date is required').typeError('Please pick a valid date'),
  value: yup
    .number()
    .nullable()
    .transform((v, original) => (original === '' || original === null ? null : v))
    .min(0, 'Value must be 0 or greater')
    .typeError('Please enter a valid value'),
});

const form = useStickyForm<DealFormValues>({
  schema: validationSchema,
  initialValues: {
    title: '',
    companyProfileId: null,
    date: null,
    value: null,
    type: DealType.Temporal,
    status: DealStatus.ToSend,
  },
});
const { title, companyProfileId, date, value, type, status } = form.fields;
const formErrors = form.errors;

const documentFile = ref<File | null>(null);
const fileError = ref('');

function onFileSelected(file: File | null): void {
  fileError.value = file ? validateUploadFile(file) : '';
  if (fileError.value) documentFile.value = null;
}

const datePicker = ref<ComponentPublicInstance | null>(null);
const { reveal } = useDropdownReveal();

function onPickerActive(active: boolean): void {
  reveal(datePicker.value?.$el as HTMLElement | undefined, active);
}

onMounted(() => {
  if (!props.deal) return;
  form.hydrate({
    title: props.deal.title,
    companyProfileId: props.deal.companyProfileId,
    date: new Date(props.deal.date),
    value: props.deal.value,
    type: props.deal.type,
    status: props.deal.status,
  });
});

function resetForm(): void {
  form.resetAll();
  documentFile.value = null;
  fileError.value = '';
}

function submit(): Promise<boolean> {
  form.markInteracted();
  return new Promise<boolean>((resolve) => {
    form.handleSubmit(
      async (values) => {
        const amount = Number(values.value ?? 0);
        const dealDate = new Date(values.date as Date);
        try {
          const file = documentFile.value;
          if (props.deal) {
            await updateDeal(props.deal.id, {
              title: values.title.trim(),
              date: dealDate.toISOString(),
              value: amount,
              type: values.type,
              status: values.status,
              documentId: props.deal.documentId ?? null,
              fileName: file ? generateFileName('Deal', file.name) : null,
            }, file);
            showAlertSuccess('Deal updated');
          } else {
            await createDeal({
              title: values.title.trim(),
              companyProfileId: values.companyProfileId as string,
              date: dealDate.toISOString(),
              value: amount,
              type: values.type,
              status: values.status,
              documentId: null,
              fileName: file ? generateFileName('Deal', file.name) : null,
            }, file);
            showAlertSuccess('Deal created');
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
@import "../../assets/scss/variables";

.sd-readonly {
  color: $grey-font;
  padding: 0.35rem 0;
  font-weight: 600;
}

.sd-document {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  color: $grey-font;
  padding: 0.35rem 0;
  word-break: break-all;
}

.sd-document--link {
  color: $primary;
  text-decoration: underline;

  &:hover {
    color: $primary;
    filter: brightness(0.9);
  }
}

</style>
