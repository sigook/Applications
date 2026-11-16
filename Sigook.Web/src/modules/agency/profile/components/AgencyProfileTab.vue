<template>
  <detail-layout :sections="[]" aria-label="Agency profile sections" rail-first>
    <detail-card title="Business">
      <template #actions>
        <b-button type="is-ghost" size="is-small" class="detail-link" @click="showBusinessModal = true">Edit</b-button>
      </template>
      <detail-fields :fields="fields" />
    </detail-card>

    <template #rail>
      <company-locations-card :key="`${agency.id}-${locationsVersion}`" :fetch-locations="fetchLocations"
        @showAll="emit('showTab', 'Locations')" />
      <company-main-contact-card :key="contactsKey" :fetch-contacts="fetchContacts"
        @showAll="emit('showTab', 'Contacts')" />
    </template>
  </detail-layout>

  <b-modal custom-content-class="card" v-model="showBusinessModal" width="800px" :destroy-on-hide="true">
    <profile-business :agency-data="agency" @update:agencyData="emit('update:agency', $event)"
      @saved="showBusinessModal = false" />
  </b-modal>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import type { AgencyDetail, AgencyLocationDetail } from '@/modules/agency/profile/types';
import type { CompanyContactSummary } from '@/shared/company-profile/types';
import type { DetailFact } from '@/shared/detail-page/types';
import { getAgencyLocations } from '@/modules/agency/profile/api';
import { companyWebsiteUrl } from '@/shared/company-profile/useCompanyDetail';
import DetailLayout from '@/shared/detail-page/DetailLayout.vue';
import DetailCard from '@/shared/detail-page/DetailCard.vue';
import DetailFields from '@/shared/detail-page/DetailFields.vue';
import CompanyLocationsCard from '@/shared/company-profile/CompanyLocationsCard.vue';
import CompanyMainContactCard from '@/shared/company-profile/CompanyMainContactCard.vue';
import ProfileBusiness from '@/modules/agency/profile/components/ProfileBusiness.vue';

const { agency, locationsVersion } = defineProps<{ agency: AgencyDetail; locationsVersion?: number }>();
const emit = defineEmits<{
  (e: 'update:agency', value: AgencyDetail): void;
  (e: 'showTab', tab: string): void;
}>();

const showBusinessModal = ref(false);

const contacts = computed<CompanyContactSummary[]>(() => (agency.contactInformation ?? []).map((contact) => ({
  firstName: contact.firstName ?? '',
  middleName: contact.middleName,
  lastName: contact.lastName ?? '',
  position: contact.position,
  mobileNumber: contact.mobileNumber,
  email: contact.email,
})));

const contactsKey = computed(() => `${agency.id}-${contacts.value.map((contact) => contact.email).join('|')}`);

function fetchLocations(): Promise<AgencyLocationDetail[]> {
  return getAgencyLocations(agency.id);
}

function fetchContacts(): Promise<CompanyContactSummary[]> {
  return Promise.resolve(contacts.value);
}

const fields = computed<DetailFact[]>(() => [
  { label: 'Agency name', value: agency.fullName || '—' },
  { label: 'HST number', value: agency.hstNumber || '—' },
  { label: 'BN/EIN', value: agency.businessNumber || '—' },
  {
    label: 'Phone',
    value: agency.phonePrincipal || '—',
    copy: { kind: 'phone', value: agency.phonePrincipal, ext: agency.phonePrincipalExt },
  },
  { label: 'Web page', value: agency.webPage || '—', href: companyWebsiteUrl(agency.webPage) || undefined },
  { label: 'WSIB groups', value: (agency.wsibGroup ?? []).map((group) => group.value).join(', ') || '—' },
]);
</script>
