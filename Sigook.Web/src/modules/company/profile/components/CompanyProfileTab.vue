<template>
  <detail-layout :sections="sections" aria-label="Company profile sections" rail-first>
    <detail-card :id="anchor('business')" title="Business">
      <template #actions>
        <b-button type="is-ghost" size="is-small" class="detail-link" @click="showBusinessModal = true">Edit</b-button>
      </template>
      <detail-fields :fields="fields" />
    </detail-card>

    <template #rail>
      <company-locations-card :key="locationsVersion" :fetch-locations="getProfileLocations"
        @loaded="emit('locationsLoaded', $event)" @showAll="emit('showTab', 'Locations')" />
      <company-main-contact-card :key="contactsVersion" :fetch-contacts="getContactPeople"
        @loaded="emit('contactsLoaded', $event)" @showAll="emit('showTab', 'Contacts')" />
    </template>
  </detail-layout>

  <b-modal custom-content-class="card" v-model="showBusinessModal" width="800px" :destroy-on-hide="true">
    <profile-business :company-data="company" @update:companyData="emit('update:company', $event)"
      @saved="showBusinessModal = false" />
  </b-modal>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import type { CompanyProfileDetail } from '@/shared/company-profile/types';
import type { DetailFact, DetailSection } from '@/shared/detail-page/types';
import { companyIndustryLabel, companyWebsiteUrl } from '@/shared/company-profile/useCompanyDetail';
import DetailLayout from '@/shared/detail-page/DetailLayout.vue';
import DetailCard from '@/shared/detail-page/DetailCard.vue';
import DetailFields from '@/shared/detail-page/DetailFields.vue';
import ProfileBusiness from '@/modules/company/profile/components/ProfileBusiness.vue';
import CompanyLocationsCard from '@/shared/company-profile/CompanyLocationsCard.vue';
import CompanyMainContactCard from '@/shared/company-profile/CompanyMainContactCard.vue';
import { getContactPeople, getProfileLocations } from '@/modules/company/profile/api';

const props = defineProps<{ company: CompanyProfileDetail; locationsVersion?: number; contactsVersion?: number }>();
const emit = defineEmits<{
  (e: 'update:company', value: CompanyProfileDetail): void;
  (e: 'contactsLoaded', count: number): void;
  (e: 'locationsLoaded', count: number): void;
  (e: 'showTab', tab: string): void;
}>();

const showBusinessModal = ref(false);

function anchor(id: string) {
  return `company-profile-section-${id}`;
}

function withExt(value?: string | null, ext?: number | null) {
  if (!value) return '—';
  return ext ? `${value} ext ${ext}` : value;
}

const sections = computed<DetailSection[]>(() => [
  { id: anchor('business'), label: 'Business' },
]);

const fields = computed<DetailFact[]>(() => [
  { label: 'Company name', value: props.company.fullName || '—' },
  { label: 'Industry', value: companyIndustryLabel(props.company) || '—' },
  { label: 'Website', value: props.company.website || '—', href: companyWebsiteUrl(props.company.website) || undefined },
  { label: 'Phone', value: withExt(props.company.phone, props.company.phoneExt) },
  { label: 'Fax', value: withExt(props.company.fax, props.company.faxExt) },
  { label: 'Email', value: props.company.email || '—' },
]);
</script>
