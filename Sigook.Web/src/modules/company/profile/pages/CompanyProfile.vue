<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>

    <detail-header v-if="companyProfile" :title="lowercase(companyProfile.fullName)" :number-id="companyProfile.numberId"
      :meta="industry" :logo="companyProfile.logo?.pathFile" :kpis="kpis">
      <b-tabs v-model="currentTab" @update:modelValue="changeTab">
        <b-tab-item label="Profile" value="Profile">
          <company-profile-tab v-if="visitedTabs.includes('Profile')" v-model:company="companyProfile"
            :locations-version="locationsVersion" :contacts-version="contactsVersion"
            @contactsLoaded="contactsCount = $event" @locationsLoaded="locationsCount = $event" @showTab="changeTab" />
        </b-tab-item>
        <b-tab-item label="Locations" value="Locations">
          <template #header>
            Locations<span v-if="locationsCount !== undefined" class="detail-tab-count">{{ locationsCount }}</span>
          </template>
          <profile-location v-if="visitedTabs.includes('Locations')" @loaded="onLocationsLoaded" />
        </b-tab-item>
        <b-tab-item label="Contacts" value="Contacts">
          <template #header>
            Contacts<span v-if="contactsCount !== undefined" class="detail-tab-count">{{ contactsCount }}</span>
          </template>
          <profile-contact v-if="visitedTabs.includes('Contacts')" @loaded="onContactsLoaded" />
        </b-tab-item>
        <b-tab-item label="Users" value="CompanyUsers">
          <company-users v-if="visitedTabs.includes('CompanyUsers')" :company-data="companyProfile" />
        </b-tab-item>
        <b-tab-item label="Account security" value="AccountSecurity">
          <account-security v-if="visitedTabs.includes('AccountSecurity')" :company-data="companyProfile" />
        </b-tab-item>
        <b-tab-item label="Notifications" value="UserNotification">
          <user-notification v-if="visitedTabs.includes('UserNotification')" />
        </b-tab-item>
      </b-tabs>
    </detail-header>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { showAlertError } from '@/shared/utils/toast';
import { getCompanyProfile } from '@/modules/company/profile/api';
import { lowercase } from '@/shared/format';
import { useCompanyDetail } from '@/shared/company-profile/useCompanyDetail';
import type { CompanyProfileDetail } from '@/shared/company-profile/types';
import type { DetailKpi } from '@/shared/detail-page/types';
import DetailHeader from '@/shared/detail-page/DetailHeader.vue';
import CompanyProfileTab from '@/modules/company/profile/components/CompanyProfileTab.vue';
import ProfileLocation from '@/modules/company/profile/components/ProfileLocation.vue';
import ProfileContact from '@/modules/company/profile/components/ProfileContact.vue';
import AccountSecurity from '@/shared/company-profile/ProfileAccountInformation.vue';
import UserNotification from '@/app/layout/UserNotification.vue';
import CompanyUsers from '@/modules/company/profile/components/CompanyUsers.vue';

const tabs = ['Profile', 'Locations', 'Contacts', 'CompanyUsers', 'AccountSecurity', 'UserNotification'];

const route = useRoute();
const router = useRouter();

const isLoading = ref(false);
const companyProfile = ref<CompanyProfileDetail | null>(null);
const contactsCount = ref<number>();
const locationsCount = ref<number>();
const locationsVersion = ref(0);
const contactsVersion = ref(0);
const { industry, createdAt } = useCompanyDetail(companyProfile);

const initialTab = typeof route.query.tab === 'string' && tabs.includes(route.query.tab) ? route.query.tab : 'Profile';
const currentTab = ref<string>(initialTab);
const visitedTabs = ref<string[]>(['Profile', initialTab]);

const kpis = computed<DetailKpi[]>(() => [
  ...(locationsCount.value !== undefined ? [{ key: 'locations', label: 'Locations', value: String(locationsCount.value) }] : []),
  ...(contactsCount.value !== undefined ? [{ key: 'contacts', label: 'Contacts', value: String(contactsCount.value) }] : []),
  ...(createdAt.value ? [{ key: 'created', label: 'Member since', value: createdAt.value }] : []),
]);

function onLocationsLoaded(count: number) {
  locationsCount.value = count;
  locationsVersion.value += 1;
}

function onContactsLoaded(count: number) {
  contactsCount.value = count;
  contactsVersion.value += 1;
}

function changeTab(tab: string) {
  currentTab.value = tab;
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({ path: '/company-profile', query: { tab } });
}

function onGetProfile() {
  isLoading.value = true;
  getCompanyProfile()
    .then((data) => {
      companyProfile.value = data;
    })
    .catch((error: unknown) => showAlertError((error as { data?: unknown }).data))
    .finally(() => {
      isLoading.value = false;
    });
}

onGetProfile();
</script>
