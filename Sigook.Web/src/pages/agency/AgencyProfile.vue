<template>
  <div class="profile white-container-mobile" v-if="agency">
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

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { mapStores } from 'pinia';
import { useAgencyStore } from '@/stores/agency';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { confirmationGuard } from '@/utils/confirmationGuard';
import { getAgencyProfile, getPersonnelAgencies, updateAgency } from "@/api/agencyApi";
import { lowercase } from '@/utils/filters';

export default {
  data() {
    return {
      isLoading: false,
      currentTab: "BusinessInformation",
      visitedTabs: ["BusinessInformation"],
      isDisabled: true,
      unsavedChanges: false
    };
  },
  components: {
    BusinessInformation: defineAsyncComponent(() => import("@/components/agency/ProfileBusiness.vue")),
    BillingInformation: defineAsyncComponent(() => import("@/components/agency/ProfileBilling.vue")),
    ContactInformation: defineAsyncComponent(() => import("@/components/agency/ProfileContact.vue")),
    AccountSecurity: defineAsyncComponent(() => import("@/components/agency/ProfileAccountInformation.vue")),
    UploadImage: defineAsyncComponent(() => import("@/components/PreviewImage.vue")),
    UserNotification: defineAsyncComponent(() => import("../../components/UserNotification.vue")),
    Users: defineAsyncComponent(() => import("../../components/agency/AgencyPersonnel.vue"))
  },
  async created() {
    if (this.$route.query && this.$route.query.tab) {
      this.currentTab = this.$route.query.tab;
      if (!this.visitedTabs.includes(this.$route.query.tab)) {
        this.visitedTabs.push(this.$route.query.tab);
      }
    }
    this.isLoading = true;
    const agency = await getAgencyProfile();
    this.agencyStore.setAgency(agency);
    const personnelAgencies = await getPersonnelAgencies();
    this.agencyStore.setPersonnelAgencies(personnelAgencies);
    this.isLoading = false;
  },
  methods: {
    lowercase,
    editProfile() {
      this.isDisabled = false;
    },
    validateForm() {
      let isValid = true;
      this.$validator.validateAll().then((response) => {
        if (!response || !isValid) {
          showAlertError("Please make sure all required fields are filled out correctly");
          return;
        }
        this.saveProfile();
      });
    },
    saveProfile() {
      this.isLoading = true;
      updateAgency(this.agency)
        .then(() => {
          this.isDisabled = true;
          this.unsavedChanges = false;
          this.isLoading = false;
          showAlertSuccess("Updated");
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: "/agency-profile",
        query: { tab: tab }
      });
    }
  },
  computed: {
    ...mapStores(useAgencyStore),
    agency() {
      return this.agencyStore.agency;
    }
  },
  beforeRouteLeave: confirmationGuard
};
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
