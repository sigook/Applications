<template>
  <form class="sd-form" @submit.prevent>
    <b-field label="Deal title">
      <b-input v-model="title" placeholder="e.g. Warehouse staffing – 40 FTE"></b-input>
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
      <p v-else class="sd-readonly">{{ deal?.companyName }}</p>
    </b-field>

    <div class="sd-form__row">
      <b-field label="Value" class="sd-form__col">
        <b-input v-model="value" type="number" step="0.01" placeholder="$"></b-input>
      </b-field>

      <b-field label="Type" class="sd-form__col">
        <b-select v-model="type" expanded>
          <option v-for="opt in typeOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
        </b-select>
      </b-field>
    </div>

    <b-field label="Status">
      <b-select v-model="status" expanded>
        <option v-for="opt in statusOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
      </b-select>
    </b-field>

    <b-field label="Date">
      <b-datepicker
        ref="datePicker"
        v-model="date"
        placeholder="Pick a date"
        @active-change="onPickerActive"
      ></b-datepicker>
    </b-field>

    <b-field v-if="!isEditing" label="Document">
      <b-upload v-model="documentFile" expanded>
        <a class="button is-fullwidth sd-upload">
          <b-icon icon="paperclip" size="is-small"></b-icon>
          <span>{{ documentFile ? documentFile.name : 'Attach a document (optional)' }}</span>
        </a>
      </b-upload>
    </b-field>

    <b-field v-else label="Document">
      <a
        v-if="deal?.documentName"
        class="sd-document sd-document--link"
        href="#"
        @click.prevent="openDocument"
      >
        <b-icon icon="paperclip" size="is-small"></b-icon>
        {{ deal.documentName }}
      </a>
      <p v-else class="sd-document">No document attached</p>
    </b-field>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import type { ComponentPublicInstance } from 'vue';
import { getSalesCompanies } from '@/api/salesApi';
import { useDropdownReveal } from '@/composables/useDropdownReveal';
import { createDeal, updateDeal, getDealDocument } from '@/api/dealApi';
import {
  DealType,
  DealStatus,
  DEAL_TYPES,
  DEAL_STATUSES,
  DEAL_TYPE_LABELS,
  DEAL_STATUS_LABELS,
} from '@/types/company';
import type { Deal } from '@/types/company';
import type { AgencyCompanyListItem } from '@/types/agency';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { generateFileName } from '@/utils/fileNaming';
import SearchSelect from './SearchSelect.vue';

const props = defineProps<{ deal?: Deal | null }>();

const isEditing = computed(() => !!props.deal);

const clientOptions = computed(() => clients.value.map((c) => ({ value: c.id, label: c.fullName })));
const typeOptions = DEAL_TYPES.map((t) => ({ value: t, label: DEAL_TYPE_LABELS[t] }));
const statusOptions = DEAL_STATUSES.map((s) => ({ value: s, label: DEAL_STATUS_LABELS[s] }));

const clients = ref<AgencyCompanyListItem[]>([]);
const isLoadingClients = ref(false);

const title = ref('');
const companyProfileId = ref<string | null>(null);
const date = ref<Date | null>(null);
const value = ref<number | null>(null);
const type = ref<DealType>(DealType.Temporal);
const status = ref<DealStatus>(DealStatus.ToSend);
const documentFile = ref<File | null>(null);

const datePicker = ref<ComponentPublicInstance | null>(null);
const { reveal } = useDropdownReveal();

function onPickerActive(active: boolean): void {
  reveal(datePicker.value?.$el as HTMLElement | undefined, active);
}

async function openDocument(): Promise<void> {
  if (!props.deal) return;
  try {
    const blob = await getDealDocument(props.deal.id);
    const url = URL.createObjectURL(blob);
    window.open(url, '_blank', 'noopener');
    setTimeout(() => URL.revokeObjectURL(url), 60000);
  } catch (error) {
    await showAlertError(error);
  }
}

onMounted(() => {
  if (props.deal) {
    title.value = props.deal.title;
    companyProfileId.value = props.deal.companyProfileId;
    date.value = new Date(props.deal.date);
    value.value = props.deal.value;
    type.value = props.deal.type;
    status.value = props.deal.status;
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
  title.value = '';
  companyProfileId.value = null;
  date.value = null;
  value.value = null;
  type.value = DealType.Temporal;
  status.value = DealStatus.ToSend;
  documentFile.value = null;
}

async function submit(): Promise<boolean> {
  if (!title.value.trim()) {
    await showAlertError('Please enter a deal title');
    return false;
  }
  if (!companyProfileId.value) {
    await showAlertError('Please select a client');
    return false;
  }
  if (!date.value) {
    await showAlertError('Please pick a date');
    return false;
  }
  const amount = Number(value.value);
  if (!Number.isFinite(amount) || amount < 0) {
    await showAlertError('Please enter a valid value');
    return false;
  }
  try {
    if (props.deal) {
      await updateDeal(props.deal.id, {
        title: title.value.trim(),
        date: date.value.toISOString(),
        value: amount,
        type: type.value,
        status: status.value,
        documentId: props.deal.documentId ?? null,
      });
      showAlertSuccess('Deal updated');
    } else {
      const file = documentFile.value;
      await createDeal({
        title: title.value.trim(),
        companyProfileId: companyProfileId.value,
        date: date.value.toISOString(),
        value: amount,
        type: type.value,
        status: status.value,
        documentId: null,
        fileName: file ? generateFileName('Deal', file.name) : null,
      }, file);
      showAlertSuccess('Deal created');
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

.sd-document {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  font-size: 0.82rem;
  color: #333;
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

.sd-form__row {
  display: flex;
  gap: 0.7rem;
}

.sd-form__col {
  flex: 1;
  min-width: 0;
}
</style>
