<template>
  <div class="company-detail-page">
    <b-loading v-model="isLoading"></b-loading>
    <Breadcrumbs :crumbs="crumbs" :back-to="companyBase" />

    <detail-header v-if="company" :title="lowercase(company.fullName)" :number-id="company.numberId" :meta="industry"
      :logo="company.logo?.pathFile" logo-editable :status-label="statusLabel" :status-variant="statusVariant"
      :chips="chips" :kpis="kpis" @editLogo="showUpdateLogo = true">
      <template #actions>
        <b-button @click="router.push({ path: `${companyBase}/update/${company.id}` })">Edit company</b-button>
        <b-button v-if="isClient" type="is-primary"
          @click="router.push({ path: `${requestBase}/create/${company.id}` })">
          Create request
        </b-button>
      </template>

      <b-tabs v-model="currentTab" @update:modelValue="changeTab">
        <b-tab-item label="Detail" value="Detail">
          <company-detail-tab v-if="visitedTabs.includes('Detail')" v-model:company="company" @showTab="changeTab" />
        </b-tab-item>
        <b-tab-item v-if="!requiresPayrollPermission" label="Roles" value="JobPosition">
          <template #header>Roles<span class="detail-tab-count">{{ summary?.rolesCount ?? 0 }}</span></template>
          <job-position v-if="visitedTabs.includes('JobPosition')" :company="company" />
        </b-tab-item>
        <b-tab-item v-if="!requiresPayrollPermission" label="Requests" value="Requests">
          <template #header>Requests<span class="detail-tab-count">{{ summary?.openRequestsCount ?? 0 }}</span></template>
          <requests v-if="visitedTabs.includes('Requests')" :company="company" />
        </b-tab-item>
        <b-tab-item label="Workers" value="Workers">
          <template #header>Workers<span class="detail-tab-count">{{ summary?.workersWorkingCount ?? 0 }}</span></template>
          <workers v-if="visitedTabs.includes('Workers')" :company="company" />
        </b-tab-item>
        <b-tab-item label="Locations" value="Locations">
          <template #header>Locations<span class="detail-tab-count">{{ summary?.locationsCount ?? 0 }}</span></template>
          <company-locations-grid v-if="visitedTabs.includes('Locations')" :profile-id="company.id"
            @changed="onLocationsChanged" />
        </b-tab-item>
        <b-tab-item label="Contacts" value="ContactPerson">
          <template #header>Contacts<span class="detail-tab-count">{{ summary?.contactsCount ?? 0 }}</span></template>
          <contact-person v-if="visitedTabs.includes('ContactPerson')" :company="company"
            @changed="onContactsChanged" />
        </b-tab-item>
        <b-tab-item label="Users" value="Users">
          <template #header>Users<span class="detail-tab-count">{{ summary?.usersCount ?? 0 }}</span></template>
          <users v-if="visitedTabs.includes('Users')" :company="company" />
        </b-tab-item>
        <b-tab-item v-if="showSalesTabs" label="Interactions" value="Interactions">
          <company-interactions v-if="visitedTabs.includes('Interactions')" :company="company" />
        </b-tab-item>
        <b-tab-item v-if="showSalesTabs" label="Deals" value="Deals">
          <company-deals v-if="visitedTabs.includes('Deals')" :company="company" />
        </b-tab-item>
      </b-tabs>
    </detail-header>

    <b-modal v-if="company" custom-content-class="card" v-model="showUpdateLogo" width="400px" :destroy-on-hide="true">
      <company-update-logo :logo="company.logo" @save="updateLogo" @cancel="showUpdateLogo = false" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { showAlertError } from '@/shared/utils/toast';
import { useAdmin } from '@/shared/composables/useAdmin';
import { useSalesAccess } from '@/shared/composables/useSalesAccess';
import { useModuleBase } from '@/modules/agency/shared/useModuleBase';
import { useCompanyDetail } from '@/shared/company-profile/useCompanyDetail';
import Breadcrumbs from '@/shared/ui/Breadcrumbs.vue';
import type { PageBreadcrumb } from '@/shared/types/common';
import type { AgencyCompanyProfileDetail } from '@/modules/agency/recruiting/clients/types';
import type { DetailChip, DetailKpi } from '@/shared/detail-page/types';
import { getAgencyCompany, updateAgencyCompanyProfileLogo } from '@/modules/agency/recruiting/clients/api';
import { lowercase } from '@/shared/format';
import { CompanyStatus } from '@/shared/constants/enums';
import DetailHeader from '@/shared/detail-page/DetailHeader.vue';
import CompanyDetailTab from '@/modules/agency/recruiting/clients/components/CompanyDetailTab.vue';
import CompanyLocationsGrid from '@/modules/agency/recruiting/clients/components/CompanyLocationsGrid.vue';
import Users from '@/modules/agency/recruiting/clients/components/UserList.vue';
import ContactPerson from '@/modules/agency/recruiting/clients/components/ContactPersonList.vue';
import JobPosition from '@/modules/agency/recruiting/clients/components/JobPositionList.vue';
import Requests from '@/modules/agency/recruiting/clients/components/CompanyRequests.vue';
import Workers from '@/modules/agency/recruiting/clients/components/CompanyWorkers.vue';
import CompanyInteractions from '@/modules/agency/sales/clients/components/CompanyInteractions.vue';
import CompanyDeals from '@/modules/agency/sales/clients/components/CompanyDeals.vue';
import CompanyUpdateLogo from '@/modules/agency/recruiting/clients/components/CompanyUpdateLogo.vue';

const route = useRoute();
const router = useRouter();
const { isSalesView, requestBase, companyBase, moduleCrumbs } = useModuleBase();
const crumbs = computed<PageBreadcrumb[]>(() => [...moduleCrumbs.value, { label: 'Clients', to: companyBase.value }]);
const { isAdmin } = useAdmin();
const { hasSalesAccess } = useSalesAccess();
const showSalesTabs = computed(() => isSalesView.value && hasSalesAccess.value);

const company = ref<AgencyCompanyProfileDetail | null>(null);
const isLoading = ref(true);
const showUpdateLogo = ref(false);
const { statusLabel, statusVariant, industry, vaccinationChip, createdAt } = useCompanyDetail(company);

const initialTab = typeof route.query.tab === 'string' && route.query.tab !== 'Settings' ? route.query.tab : 'Detail';
const currentTab = ref<string>(initialTab);
const visitedTabs = ref<string[]>(['Detail', initialTab]);

const summary = computed(() => company.value?.summary);

const requiresPayrollPermission = computed(() => !!company.value?.requiresPermissionToSeeRequests && !isAdmin.value);

const isClient = computed(() => (company.value?.companyStatus as unknown as CompanyStatus) === CompanyStatus.Client);

const chips = computed<DetailChip[]>(() => [
  ...vaccinationChip.value,
  ...(company.value?.requiresPermissionToSeeRequests ? [{ label: 'Needs permission to see requests' }] : []),
]);

const kpis = computed<DetailKpi[]>(() => {
  const current = company.value;
  const counts = current?.summary;
  if (!current || !counts) return [];
  return [
    ...(requiresPayrollPermission.value ? [] : [{
      key: 'requests',
      label: 'Open requests',
      value: String(counts.openRequestsCount),
      hint: counts.asapRequestsCount ? `· ${counts.asapRequestsCount} ASAP` : undefined,
    }]),
    { key: 'workers', label: 'Workers', value: String(counts.workersWorkingCount), hint: 'assigned' },
    ...(requiresPayrollPermission.value ? [] : [{ key: 'roles', label: 'Roles', value: String(counts.rolesCount) }]),
    {
      key: 'overtime',
      label: 'Overtime after',
      value: `${current.overtimeStartsAfter} h`,
      hint: current.paidHolidays ? '· paid holidays' : undefined,
    },
    ...(counts.salesRepresentativeName
      ? [{ key: 'sales', label: 'Sales owner', value: counts.salesRepresentativeName }]
      : []),
    { key: 'created', label: 'Created', value: createdAt.value },
  ];
});

function changeTab(tab: string) {
  currentTab.value = tab;
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({ path: `${companyBase.value}/${route.params.id}`, query: { tab } });
}

function onContactsChanged(count: number) {
  if (company.value?.summary) company.value.summary.contactsCount = count;
}

function onLocationsChanged(count: number) {
  if (company.value?.summary) company.value.summary.locationsCount = count;
}

function loadCompany() {
  getAgencyCompany(route.params.id as string)
    .then((response) => {
      company.value = response;
    })
    .catch(showAlertError)
    .finally(() => {
      isLoading.value = false;
    });
}

function updateLogo(logo: File) {
  const current = company.value;
  if (!current) return;
  showUpdateLogo.value = false;
  isLoading.value = true;
  updateAgencyCompanyProfileLogo(current.id, logo)
    .then(() => loadCompany())
    .catch((error: unknown) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

loadCompany();
</script>

<style>
.logged-content:has(.company-detail-page) {
  overflow-y: auto !important;
}
</style>
