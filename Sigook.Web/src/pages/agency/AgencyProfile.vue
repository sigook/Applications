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

      <b-tabs v-model="currentTab" @input="changeTab" v-if="agency">
        <b-tab-item :label="$t('Business Information')" value="BusinessInformation">
          <BusinessInformation v-if="visitedTabs.includes('BusinessInformation')" v-model:agency-data="agency" />
        </b-tab-item>

        <b-tab-item :label="$t('Billing Information')" value="BillingInformation">
          <BillingInformation v-if="visitedTabs.includes('BillingInformation')" :agency-data="agency" />
        </b-tab-item>

        <b-tab-item :label="$t('Contact Information')" value="ContactInformation">
          <ContactInformation v-if="visitedTabs.includes('ContactInformation')" v-model:agency-data="agency" />
        </b-tab-item>

        <b-tab-item :label="$t('Account Security')" value="AccountSecurity">
          <AccountSecurity v-if="visitedTabs.includes('AccountSecurity')" :agency-data="agency" />
        </b-tab-item>

        <b-tab-item :label="$t('Users')" value="Users">
          <Users v-if="visitedTabs.includes('Users')" :agency-data="agency" />
        </b-tab-item>

        <b-tab-item :label="$t('User Notification')" value="UserNotification">
          <UserNotification v-if="visitedTabs.includes('UserNotification')" />
        </b-tab-item>
      </b-tabs>
    </div>
  </div>
</template>

<script lang="ts">
import { confirmationGuard } from '@/utils/confirmationGuard';
import switchLocaleMixin from "../../mixins/switchLocaleMixin";
import { getAgencyProfile, getPersonnelAgencies, updateAgency } from "@/api/agencyApi";
import { lowercase } from '@/utils/filters';

export default {
  data() {
    return {
      isLoading: false,
      currentTab: "BusinessInformation",
      visitedTabs: ["BusinessInformation"],
      isDisabled: true,
      lang: this.$validator.dictionary.locale,
      unsavedChanges: false,
    };
  },
  components: {
    BusinessInformation: () => import("@/components/agency/ProfileBusiness.vue"),
    BillingInformation: () => import("@/components/agency/ProfileBilling.vue"),
    ContactInformation: () => import("@/components/agency/ProfileContact.vue"),
    AccountSecurity: () => import("@/components/agency/ProfileAccountInformation.vue"),
    UploadImage: () => import("@/components/PreviewImage.vue"),
    UserNotification: () => import("../../components/UserNotification.vue"),
    Users: () => import("../../components/agency/AgencyPersonnel.vue"),
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
    this.$store.commit("agency/setAgency", agency);
    const personnelAgencies = await getPersonnelAgencies();
    this.$store.commit("agency/setPersonnelAgencies", personnelAgencies);
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
          this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
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
          this.showAlertSuccess(this.$t("Updated"));
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: "/agency-profile",
        query: { tab: tab },
      });
    },
  },
  computed: {
    agency() {
      return this.$store.state.agency.agency;
    }
  },
  mixins: [switchLocaleMixin],
  beforeRouteLeave: confirmationGuard,
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
