<template>
  <div class="company-wrapper">
    <b-loading v-model="isLoading"></b-loading>

    <section class="company-top" v-if="company">
      <div class="hover-actions">
        <img v-if="company.logo" :src="company.logo.pathFile" alt="logo" />
        <button class="actions btn-icon-sm btn-icon-edit" type="button" @click="showUpdateLogo = true">
          Edit
        </button>
        <h2 class="capitalize fz1 fw-700">
          <span class="fw-400 fz-1" v-if="company.numberId">{{ company.numberId }} |
          </span>
          {{ lowercase(company.businessName) }}
        </h2>
      </div>

      <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
        <template #trigger>
          <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
        </template>
        <b-dropdown-item v-if="isClient" aria-role="listitem"
          @click="router.push({ path: `/agency-create-request/${company.id}` })">
          Create Order
        </b-dropdown-item>
        <b-dropdown-item aria-role="listitem"
          @click="router.push({ path: `/update-company/${company.id}` })">
          Edit Company
        </b-dropdown-item>
      </b-dropdown>
    </section>

    <b-tabs v-model="currentTab" @update:modelValue="changeTab" v-if="company">
      <b-tab-item label="Detail" value="Detail">
        <detail v-if="visitedTabs.includes('Detail')" v-model:company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Settings" value="Settings" v-if="billingAdmin.isPayrollManager">
        <settings v-if="visitedTabs.includes('Settings')" v-model:company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Users" value="Users">
        <users v-if="visitedTabs.includes('Users')" :company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Contacts" value="ContactPerson">
        <contact-person v-if="visitedTabs.includes('ContactPerson')" :company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Roles" value="JobPosition" v-if="!requiresPayrollPermission">
        <job-position v-if="visitedTabs.includes('JobPosition')" :company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Workers" value="Workers">
        <workers v-if="visitedTabs.includes('Workers')" :company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Orders" value="Requests" v-if="!requiresPayrollPermission">
        <requests v-if="visitedTabs.includes('Requests')" :company="company" class="p-2" />
      </b-tab-item>
    </b-tabs>

    <!-- update logo -->
    <company-update-logo v-if="showUpdateLogo" :logo="company.logo" v-on:save="updateLogo"
      v-on:cancel="showUpdateLogo = false" />
    <!-- update logo -->
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { showAlertError } from '@/utils/toast';
import { useBillingAdmin } from '@/composables/useBillingAdmin';
import { getAgencyCompany, updateAgencyCompanyProfileLogo } from '@/api/agencyCompanyApi';
import { lowercase } from '@/utils/filters';
import Detail from '@/components/agency_company/CompanyDetailTab.vue';
import Settings from '@/components/agency_company/CompanySettings.vue';
import Users from '@/components/agency_company/UserList.vue';
import ContactPerson from '@/components/agency_company/ContactPersonList.vue';
import JobPosition from '@/components/agency_company/JobPositionList.vue';
import Requests from '@/components/agency_company/CompanyRequests.vue';
import Workers from '@/components/agency_company/CompanyWorkers.vue';
import CompanyUpdateLogo from '@/components/agency_company/CompanyUpdateLogo.vue';

const route = useRoute();
const router = useRouter();
const billingAdmin = useBillingAdmin();

const currentTab = ref<string>('Detail');
const visitedTabs = ref<string[]>(['Detail']);
const company = ref<any>(null);
const isLoading = ref(true);
const showUpdateLogo = ref(false);

const requiresPayrollPermission = computed(() => {
  if (company.value && company.value.requiresPermissionToSeeOrders) {
    return !billingAdmin.isPayrollManager;
  }
  return false;
});

const isClient = computed(() => company.value && company.value.companyStatus === 5);

loadCompany();
if (route.query && route.query.tab) {
  currentTab.value = route.query.tab as string;
  if (!visitedTabs.value.includes(route.query.tab as string)) {
    visitedTabs.value.push(route.query.tab as string);
  }
}

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({
    path: `/agency-companies/company/${route.params.id}`,
    query: { tab: tab },
  });
}

function loadCompany() {
  getAgencyCompany(route.params.id as any)
    .then((response: any) => {
      company.value = response;
      isLoading.value = false;
    })
    .catch((error) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

function updateLogo(newLogo: any) {
  showUpdateLogo.value = false;
  isLoading.value = true;
  updateAgencyCompanyProfileLogo(company.value.id, newLogo)
    .then(() => {
      isLoading.value = false;
      company.value.logo.pathFile = company.value.logo.pathFile.replace(
        company.value.logo.fileName,
        newLogo.fileName,
      );
      company.value.logo.fileName = newLogo.fileName;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
