<template>
  <detail-card title="Company">
    <template #actions>
      <b-button type="is-ghost" size="is-small" class="detail-link" @click="showContactModal = true">Edit</b-button>
    </template>
    <detail-fields :fields="fields" />
    <p v-if="company.vaccinationRequired && company.vaccinationRequiredComments" class="company-info-note">
      {{ company.vaccinationRequiredComments }}
    </p>

    <b-modal custom-content-class="card" v-model="showContactModal" width="800px">
      <contact-information-form :model="company" @update:model="emit('update:company', $event)"
        @save="showContactModal = false" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="showEmailModal" width="500px">
      <dialog-company-update-email :company-profile-id="company.id" @closeModal="onEmailUpdated" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="showVaccinationModal" width="500px">
      <edit-vaccination-required :company-profile-id="company.id" :vaccination-required="company.vaccinationRequired"
        :vaccination-comments="company.vaccinationRequiredComments" @updated="onVaccinationUpdated" />
    </b-modal>
  </detail-card>
</template>

<script setup lang="ts" generic="T extends CompanyProfileDetail">
import { computed, ref } from 'vue';
import type { CompanyProfileDetail } from '@/types/company';
import type { DetailFact } from '@/types/detailPage';
import { companyIndustryLabel, companyWebsiteUrl } from '@/composables/useCompanyDetail';
import DetailCard from '@/components/detail_page/DetailCard.vue';
import DetailFields from '@/components/detail_page/DetailFields.vue';
import ContactInformationForm from '@/components/agency_company/ContactInformationForm.vue';
import DialogCompanyUpdateEmail from '@/components/company/DialogCompanyUpdateEmail.vue';
import EditVaccinationRequired from '@/components/agency_company/EditVaccinationRequired.vue';

const props = defineProps<{ company: T }>();
const emit = defineEmits<{ (e: 'update:company', value: T): void }>();

const showContactModal = ref(false);
const showEmailModal = ref(false);
const showVaccinationModal = ref(false);

function withExt(value?: string | null, ext?: number | null) {
  if (!value) return '—';
  return ext ? `${value} ext ${ext}` : value;
}

const fields = computed<DetailFact[]>(() => [
  { label: 'Phone', value: withExt(props.company.phone, props.company.phoneExt) },
  { label: 'Fax', value: withExt(props.company.fax, props.company.faxExt) },
  { label: 'Website', value: props.company.website || '—', href: companyWebsiteUrl(props.company.website) || undefined },
  { label: 'Email', value: props.company.email || '—', linkLabel: 'Change', onAction: () => { showEmailModal.value = true; } },
  { label: 'Industry', value: companyIndustryLabel(props.company) || '—' },
  {
    label: 'Vaccination',
    value: props.company.vaccinationRequired ? 'Required' : 'Not required',
    onAction: () => { showVaccinationModal.value = true; },
  },
]);

function onEmailUpdated(_changed: boolean, newEmail: string) {
  showEmailModal.value = false;
  if (newEmail) emit('update:company', { ...props.company, email: newEmail });
}

function onVaccinationUpdated(model: { required: boolean; comments: string | null }) {
  showVaccinationModal.value = false;
  emit('update:company', {
    ...props.company,
    vaccinationRequired: model.required,
    vaccinationRequiredComments: model.comments ?? '',
  });
}
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.company-info-note {
  margin: 0;
  padding: 10px 12px;
  border-radius: 8px;
  background: rgba($accent, 0.12);
  font-size: 0.85rem;
  color: $accent-text;
}
</style>
