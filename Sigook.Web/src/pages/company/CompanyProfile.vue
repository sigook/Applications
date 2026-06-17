<template>
  <div class="profile">
    <b-loading v-model="isLoading"></b-loading>

    <div class="profile-content">
      <div class="profile-top">
        <UploadImage v-if="companyProfile && companyProfile.logo"
          @imageSelected="(profileImg) => (companyProfile.logo.fileName = profileImg)"
          :edited-image="companyProfile.logo" :class="{ disabled: isDisabled }" :required="false">
        </UploadImage>
        <div v-if="companyProfile">
          <h1 class="text-capitalize fz2">
            {{ lowercase(companyProfile.businessName) }}
          </h1>
        </div>
      </div>

      <b-tabs v-model="currentTab" @update:modelValue="changeTab" v-if="companyProfile">
        <b-tab-item :label="'Business Information'" value="BusinessInformation">
          <BusinessInformation v-if="visitedTabs.includes('BusinessInformation')" v-model:company-data="companyProfile" />
        </b-tab-item>

        <b-tab-item :label="'Contact Information'" value="ContactInformation">
          <ContactInformation v-if="visitedTabs.includes('ContactInformation')" :company-data="companyProfile" />
        </b-tab-item>

        <b-tab-item :label="'Location Information'" value="LocationInformation">
          <LocationInformation v-if="visitedTabs.includes('LocationInformation')" :company-data="companyProfile" />
        </b-tab-item>

        <b-tab-item :label="'Users'" value="CompanyUsers">
          <CompanyUsers v-if="visitedTabs.includes('CompanyUsers')" :company-data="companyProfile" />
        </b-tab-item>

        <b-tab-item :label="'Account Security'" value="AccountSecurity">
          <AccountSecurity v-if="visitedTabs.includes('AccountSecurity')" :company-data="companyProfile" />
        </b-tab-item>

        <b-tab-item :label="'Notifications'" value="UserNotification">
          <UserNotification v-if="visitedTabs.includes('UserNotification')" />
        </b-tab-item>
      </b-tabs>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { showAlertError } from '@/utils/toast';
import { getCompanyProfile } from '@/api/companyApi';
import { lowercase } from '@/utils/filters';
import BusinessInformation from '../../components/company/ProfileBusiness.vue';
import ContactInformation from '../../components/company/ProfileContact.vue';
import LocationInformation from '../../components/company/ProfileLocation.vue';
import UploadImage from '../../components/PreviewImage.vue';
import AccountSecurity from '../../components/agency/ProfileAccountInformation.vue';
import UserNotification from '../../components/UserNotification.vue';
import CompanyUsers from '../../components/company/CompanyUsers.vue';

const route = useRoute();
const router = useRouter();

const isLoading = ref(false);
const companyProfile = ref<any>(null);
const currentTab = ref<string>('BusinessInformation');
const visitedTabs = ref<string[]>(['BusinessInformation']);
const isDisabled = ref(true);

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({
    path: '/company-profile',
    query: { tab: tab },
  });
}

function onGetProfile() {
  isLoading.value = true;
  getCompanyProfile()
    .then((data: any) => {
      companyProfile.value = data;
      isLoading.value = false;
    })
    .catch((error: any) => {
      showAlertError(error.data);
      isLoading.value = false;
    });
}

if (route.query && route.query.tab) {
  const tab = route.query.tab as string;
  currentTab.value = tab;
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
}
onGetProfile();
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
</style>
