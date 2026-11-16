<template>
  <detail-card title="Invoicing">
    <div class="company-invoicing-block">
      <span class="detail-muted">Invoice notes · printed on every invoice</span>
      <QuillEditor theme="snow" content-type="html" v-model:content="notes" :toolbar="toolbar" />
      <div class="company-invoicing-row">
        <span class="detail-muted" :class="{ 'is-over': notesLength > maxNotes }">{{ notesLength }} / {{ maxNotes }}</span>
        <b-button size="is-small" :loading="isSavingNotes" @click="saveNotes">Save notes</b-button>
      </div>
    </div>

    <div class="company-invoicing-block is-separated">
      <span class="detail-muted">Recipients</span>
      <div v-if="recipients.length" class="company-recipients">
        <span v-for="(recipient, index) in recipients" :key="recipient.id ?? recipient.email" class="company-recipient">
          <span>{{ recipient.name }} ·</span>
          <copy-link kind="email" :value="recipient.email" />
          <b-button type="is-ghost" size="is-small" icon-left="close" class="detail-icon-button is-destructive"
            :aria-label="`Remove ${recipient.email}`" @click="removeRecipient(recipient, index)" />
        </span>
      </div>
      <span v-else class="detail-empty">No recipients</span>
      <div class="columns is-variable is-2 company-recipient-form">
        <b-field class="column" :type="formErrors.name ? 'is-danger' : ''" :message="formErrors.name || ''">
          <b-input v-model="name" placeholder="Name" aria-label="Recipient name" size="is-small" />
        </b-field>
        <b-field class="column" :type="formErrors.email ? 'is-danger' : ''" :message="formErrors.email || ''">
          <b-input v-model="email" placeholder="Email" aria-label="Recipient email" size="is-small" />
        </b-field>
        <div class="column is-narrow">
          <b-button size="is-small" icon-left="plus" :loading="isSavingRecipient" @click="addRecipient">Add</b-button>
        </div>
      </div>
    </div>
  </detail-card>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/shared/composables/useStickyForm';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { deleteCompanyInvoiceRecipient, getCompanyInvoiceRecipients, getInvoiceNotes, postCompanyInvoiceRecipient, postInvoiceNotes } from '@/modules/agency/recruiting/clients/api';
import type { InvoiceRecipientModel } from '@/modules/agency/recruiting/clients/types';
import DetailCard from '@/shared/detail-page/DetailCard.vue';
import CopyLink from '@/shared/ui/CopyLink.vue';

const { profileId } = defineProps<{ profileId: string }>();

const maxNotes = 500;
const toolbar = [
  ['bold', 'italic', 'underline'],
  [{ list: 'ordered' }, { list: 'bullet' }],
  ['clean'],
];

const recipientSchema = yup.object({
  name: yup.string().required('Name is required').min(3, 'Min 3 characters').max(50, 'Max 50 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6).max(50),
});

const form = useStickyForm<{ name: string; email: string }>({
  schema: recipientSchema,
  initialValues: { name: '', email: '' },
});
const { name, email } = form.fields;
const formErrors = form.errors;

const notes = ref('');
const recipients = ref<InvoiceRecipientModel[]>([]);
const isSavingNotes = ref(false);
const isSavingRecipient = ref(false);

const notesLength = computed(() => (notes.value || '').replace(/(<([^>]+)>)/gi, '').length);

async function saveNotes() {
  if (notesLength.value > maxNotes) {
    showAlertError(`Notes can't be greater than ${maxNotes} characters.`);
    return;
  }
  isSavingNotes.value = true;
  try {
    await postInvoiceNotes(profileId, { htmlNotes: notes.value });
    showAlertSuccess('Updated');
  } catch (error) {
    showAlertError(error);
  } finally {
    isSavingNotes.value = false;
  }
}

function addRecipient() {
  form.markInteracted(['name', 'email']);
  void form.handleSubmit(async (values) => {
    isSavingRecipient.value = true;
    try {
      const response = await postCompanyInvoiceRecipient(profileId, values);
      recipients.value.push({ id: response.id, name: values.name, email: values.email });
      form.resetAll();
    } catch (error) {
      showAlertError(error);
    } finally {
      isSavingRecipient.value = false;
    }
  })();
}

async function removeRecipient(recipient: InvoiceRecipientModel, index: number) {
  if (!recipient.id) return;
  const id = recipient.id;
  try {
    if (!(await showAlertConfirm('Are you sure', `Remove ${recipient.email} from invoice recipients`))) return;
    await deleteCompanyInvoiceRecipient(profileId, id);
    recipients.value.splice(index, 1);
  } catch (error) {
    showAlertError(error);
  }
}

async function loadInvoiceNotes() {
  try {
    const response = await getInvoiceNotes(profileId);
    notes.value = response.htmlNotes ?? '';
  } catch (error) {
    showAlertError(error);
  }
}

async function loadRecipients() {
  try {
    recipients.value = await getCompanyInvoiceRecipients(profileId);
  } catch (error) {
    showAlertError(error);
  }
}

void loadInvoiceNotes();
void loadRecipients();
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.company-invoicing-block {
  display: flex;
  flex-direction: column;
  gap: 8px;

  &.is-separated {
    padding-top: 14px;
    border-top: 1px solid $gray-bg;
  }
}

.company-invoicing-row {
  display: flex;
  align-items: center;
  justify-content: space-between;

  .is-over {
    color: $danger-hover;
  }
}

.company-recipients {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.company-recipient {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 2px 4px 2px 12px;
  border: 1px solid $border-input;
  border-radius: 999px;
  font-size: 0.8rem;
  color: $navy;
}

.company-recipient-form {
  margin-bottom: 0;
}
</style>
