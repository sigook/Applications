<template>
  <detail-layout :sections="sections" aria-label="Company sections" rail-first>
    <company-info-card :id="anchor('company')" :company="company" @update:company="emit('update:company', $event)" />

    <detail-card :id="anchor('about')" title="About">
      <p v-if="company.about" class="detail-rich-text">{{ company.about }}</p>
      <span v-else class="detail-empty">No description</span>
    </detail-card>

    <detail-card :id="anchor('internal')" title="Internal info" internal>
      <template #badge>
        <span class="detail-tag">Agency only</span>
      </template>
      <div v-if="company.internalInfo" class="detail-rich-text" v-html="company.internalInfo"></div>
      <span v-else class="detail-empty">No internal info</span>
    </detail-card>

    <company-documents-card :id="anchor('documents')" :profile-id="company.id" @changed="onDocumentsChanged" />

    <company-invoicing-card :id="anchor('invoicing')" :profile-id="company.id" />

    <company-settings-card v-if="isAdmin" :id="anchor('settings')" :company="company"
      @update:company="emit('update:company', $event)" />

    <template #rail>
      <company-locations-card :key="company.summary?.locationsCount" :fetch-locations="fetchLocations" @loaded="onLocationsChanged"
        @showAll="emit('showTab', 'Locations')" />
      <company-main-contact-card :key="company.summary?.contactsCount" :fetch-contacts="fetchContacts" @showAll="emit('showTab', 'ContactPerson')" />
      <company-notes-card :profile-id="company.id" :created-at="createdAt" />
    </template>
  </detail-layout>
</template>

<script setup lang="ts">
import { computed, toRef } from 'vue';
import { useAdmin } from '@/composables/useAdmin';
import { useCompanyDetail } from '@/composables/useCompanyDetail';
import { getAgencyCompanyContactPerson, getAgencyCompanyLocation } from '@/api/agencyCompanyApi';
import type { AgencyCompanyProfileDetail } from '@/types/company';
import type { DetailSection } from '@/types/detailPage';
import DetailLayout from '@/components/detail_page/DetailLayout.vue';
import DetailCard from '@/components/detail_page/DetailCard.vue';
import CompanyInfoCard from './CompanyInfoCard.vue';
import CompanyDocumentsCard from './CompanyDocumentsCard.vue';
import CompanyInvoicingCard from './CompanyInvoicingCard.vue';
import CompanySettingsCard from './CompanySettingsCard.vue';
import CompanyLocationsCard from './CompanyLocationsCard.vue';
import CompanyMainContactCard from './CompanyMainContactCard.vue';
import CompanyNotesCard from './CompanyNotesCard.vue';

const props = defineProps<{ company: AgencyCompanyProfileDetail }>();
const emit = defineEmits<{
  (e: 'update:company', value: AgencyCompanyProfileDetail): void;
  (e: 'showTab', tab: string): void;
}>();

const { isAdmin } = useAdmin();
const { createdAt } = useCompanyDetail(toRef(props, 'company'));

const fetchLocations = () => getAgencyCompanyLocation(props.company.id);
const fetchContacts = () => getAgencyCompanyContactPerson(props.company.id);

function anchor(id: string) {
  return `company-section-${id}`;
}

const sections = computed<DetailSection[]>(() => [
  { id: anchor('company'), label: 'Company' },
  { id: anchor('about'), label: 'About' },
  { id: anchor('internal'), label: 'Internal', hint: 'agency only' },
  { id: anchor('documents'), label: 'Documents', count: props.company.summary?.documentsCount },
  { id: anchor('invoicing'), label: 'Invoicing' },
  ...(isAdmin.value ? [{ id: anchor('settings'), label: 'Settings' }] : []),
]);

function onDocumentsChanged(delta: number) {
  if (!props.company.summary) return;
  emit('update:company', {
    ...props.company,
    summary: { ...props.company.summary, documentsCount: props.company.summary.documentsCount + delta },
  });
}

function onLocationsChanged(count: number) {
  if (!props.company.summary || props.company.summary.locationsCount === count) return;
  emit('update:company', { ...props.company, summary: { ...props.company.summary, locationsCount: count } });
}
</script>
