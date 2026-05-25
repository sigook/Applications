<template>
  <div class="profile" v-if="agency">
    <b-loading v-model="isLoading"></b-loading>

    <div class="profile-content">
      <div class="profile-top">
        <upload-image @imageSelected="(profileImg) => (agency.logo.fileName = profileImg)"
          :edited-image="agency.logo" :class="{ disabled: isDisabled }" :required="false">
        </upload-image>

        <div v-if="agency.locations">
          <h1 class="capitalize">{{ lowercase(agency.fullName) }}</h1>
          <p class="icon-before-location icon-before uppercase" v-if="agency.locations[0]">
            {{ agency.locations[0].address }}
            {{ agency.locations[0].city.value }}
            {{ agency.locations[0].city.province.code }}
            {{ agency.locations[0].postalCode }}
          </p>
        </div>
      </div>

      <b-tabs v-model="currentTab" @update:modelValue="changeTab" v-if="agency">
        <b-tab-item :label="'Business Information'" value="BusinessInformation">
          <BusinessInformation v-if="visitedTabs.includes('BusinessInformation')" v-model:agency-data="agency" />
        </b-tab-item>

        <b-tab-item :label="'Billing Information'" value="BillingInformation">
          <BillingInformation v-if="visitedTabs.includes('BillingInformation')" :agency-data="agency" />
        </b-tab-item>

        <b-tab-item :label="'Contact Information'" value="ContactInformation">
          <ContactInformation v-if="visitedTabs.includes('ContactInformation')" v-model:agency-data="agency" />
        </b-tab-item>

        <b-tab-item :label="'Account Security'" value="AccountSecurity">
          <AccountSecurity v-if="visitedTabs.includes('AccountSecurity')" :agency-data="agency" />
        </b-tab-item>

        <b-tab-item :label="'Users'" value="Users">
          <Users v-if="visitedTabs.includes('Users')" :agency-data="agency" />
        </b-tab-item>

        <b-tab-item :label="'Notifications'" value="UserNotification">
          <UserNotification v-if="visitedTabs.includes('UserNotification')" />
        </b-tab-item>
      </b-tabs>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAgencyStore } from '@/stores/agency';
import { getAgencyProfile, getPersonnelAgencies } from '@/api/agencyApi';
import { lowercase } from '@/utils/filters';
import BusinessInformation from '@/components/agency/ProfileBusiness.vue';
import BillingInformation from '@/components/agency/ProfileBilling.vue';
import ContactInformation from '@/components/agency/ProfileContact.vue';
import AccountSecurity from '@/components/agency/ProfileAccountInformation.vue';
import UploadImage from '@/components/PreviewImage.vue';
import UserNotification from '@/components/UserNotification.vue';
import Users from '@/components/agency/AgencyPersonnel.vue';

const route = useRoute();
const router = useRouter();
const agencyStore = useAgencyStore();

const isLoading = ref(false);
const currentTab = ref<string>('BusinessInformation');
const visitedTabs = ref<string[]>(['BusinessInformation']);
const isDisabled = ref(true);

const agency = computed<any>(() => agencyStore.agency);

(async () => {
  if (route.query && route.query.tab) {
    currentTab.value = route.query.tab as string;
    if (!visitedTabs.value.includes(route.query.tab as string)) {
      visitedTabs.value.push(route.query.tab as string);
    }
  }
  isLoading.value = true;
  const agencyData = await getAgencyProfile();
  agencyStore.setAgency(agencyData);
  const personnelAgencies = await getPersonnelAgencies();
  agencyStore.setPersonnelAgencies(personnelAgencies);
  isLoading.value = false;
})();

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({
    path: '/agency-profile',
    query: { tab: tab },
  });
}

</script>

<style lang="scss" scoped>
.profile {
  display: block;

  .profile-content {
    width: 100%;
    border-left: none;
    padding: 15px 20px;
  }
}

.profile-top {
  &>.disabled {
    pointer-events: none;
  }
}
</style>
