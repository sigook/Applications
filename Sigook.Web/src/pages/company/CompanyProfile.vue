<template>
  <div class="profile white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>

    <div class="profile-content">
      <div class="profile-top">
        <upload-image v-if="companyProfile && companyProfile.logo"
          @imageSelected="(profileImg) => (companyProfile.logo.fileName = profileImg)"
          :edited-image="companyProfile.logo" :class="{ disabled: isDisabled }" :required="false">
        </upload-image>
        <div v-if="companyProfile">
          <h1 class="capitalize fz2">
            {{ lowercase(companyProfile.businessName) }}
          </h1>
        </div>
      </div>

      <b-tabs v-model="currentTab" @input="changeTab" v-if="companyProfile">
        <b-tab-item :label="'Business Information'" value="BusinessInformation">
          <BusinessInformation v-if="visitedTabs.includes('BusinessInformation')" :company-data.sync="companyProfile" />
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

<script lang="ts">
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { confirmationGuard } from '@/utils/confirmationGuard';
import { getCompanyProfile, updateProfile } from '@/api/companyApi';
import { lowercase } from '@/utils/filters';

export default {
  components: {
    BusinessInformation: () => import("../../components/company/ProfileBusiness.vue"),
    ContactInformation: () => import("../../components/company/ProfileContact.vue"),
    LocationInformation: () => import("../../components/company/ProfileLocation.vue"),
    UploadImage: () => import("../../components/PreviewImage.vue"),
    AccountSecurity: () => import("../../components/agency/ProfileAccountInformation.vue"),
    UserNotification: () => import("../../components/UserNotification.vue"),
    CompanyUsers: () => import("../../components/company/CompanyUsers.vue")
  },
  data() {
    return {
      isLoading: false,
      companyProfile: null,
      currentTab: "BusinessInformation",
      visitedTabs: ["BusinessInformation"],
      isDisabled: true,
      validForm: "",
      changeForm: false,
      unsavedChanges: false
    };
  },
  methods: {
    lowercase,
    editProfile() {
      this.isDisabled = false;
    },
    validateForm() {
      this.$validator.validateAll().then((response) => {
        if (!response) {
          showAlertError("Please make sure all required fields are filled out correctly");
          return;
        }
        this.saveProfile();
      });
    },
    saveProfile() {
      this.isLoading = true;
      const id = this.companyProfile.id;
      updateProfile(id, this.companyProfile)
        .then(() => {
          this.unsavedChanges = false;
          this.isDisabled = true;
          this.isLoading = false;
          showAlertSuccess("Updated");
          this.changeForm = false;
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error.data);
        });
    },
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: "/company-profile",
        query: { tab: tab }
      });
    },
    resetForm() {
      this.isDisabled = true;
      this.changeForm = false;
      this.unsavedChanges = false;
    },
    onGetProfile() {
      this.isLoading = true;
      getCompanyProfile()
        .then((data) => {
          this.companyProfile = data;
          this.isLoading = false;
        })
        .catch((error) => {
          showAlertError(error.data);
          this.isLoading = false;
        });
    }
  },
  created() {
    if (this.$route.query && this.$route.query.tab) {
      this.currentTab = this.$route.query.tab;
      if (!this.visitedTabs.includes(this.$route.query.tab)) {
        this.visitedTabs.push(this.$route.query.tab);
      }
    }
    this.onGetProfile();
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
</style>
