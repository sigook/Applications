<template>
  <div class="profile white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <nav class="profile-menu">
      <div class="scroll">
        <a v-for="(tab, index) in tabs" :key="'tabCompany' + index"
          v-bind:class="['tab-button', { active: currentTab === tab }]" v-on:click="changeTab(tab)">
          {{ $t(titleTab(tab)) }}
        </a>
      </div>

      <div class="options-profile">
        <p @click="dropdownOptions = !dropdownOptions" :class="{ 'dropdown-open': dropdownOptions }">
          <span>▶</span> {{ $t("Options") }}
        </p>
        <transition name="fade">
          <div class="dropdown-rol" v-if="dropdownOptions">
            <b class="fz-1">{{ $t("ChangeLanguage") }}:</b>
            <div class="navbar-languages">
              <span v-on:click="switchLocale('en')" :class="lang === 'en' ? 'active' : ''">EN</span>
              <span v-on:click="switchLocale('fr')" :class="lang === 'fr' ? 'active' : ''">FR</span>
              <span v-on:click="switchLocale('es')" :class="lang === 'es' ? 'active' : ''">ES</span>
            </div>
          </div>
        </transition>
      </div>
    </nav>

    <div class="profile-content">
      <div class="profile-top">
        <upload-image v-if="companyProfile && companyProfile.logo"
          @imageSelected="(profileImg) => (companyProfile.logo.fileName = profileImg)"
          :edited-image="companyProfile.logo" :class="{ disabled: isDisabled }" :required="false">
        </upload-image>
        <div v-if="companyProfile">
          <h1 class="capitalize fz2">
            {{ companyProfile.businessName | lowercase }}
          </h1>
        </div>
      </div>
      <component v-bind:is="currentTabComponent" v-if="companyProfile" class="tab" :company-data="companyProfile">
      </component>
    </div>
  </div>
</template>

<script>
import confirmationAlert from "@/mixins/confirmationAlert";
import switchLocaleMixin from "@/mixins/switchLocaleMixin";

export default {
  components: {
    BusinessInformation: () => import("../../components/company/ProfileBusiness"),
    ContactInformation: () => import("../../components/company/ProfileContact"),
    LocationInformation: () => import("../../components/company/ProfileLocation"),
    UploadImage: () => import("../../components/PreviewImage"),
    AccountSecurity: () => import("../../components/agency/ProfileAccountInformation"),
    UserNotification: () => import("../../components/UserNotification"),
    CompanyUsers: () => import("../../components/company/CompanyUsers"),
  },
  data() {
    return {
      isLoading: false,
      currentTab: "BusinessInformation",
      tabs: [
        "BusinessInformation",
        "ContactInformation",
        "LocationInformation",
        "CompanyUsers",
        "AccountSecurity",
        "UserNotification",
      ],
      isDisabled: true,
      validForm: "",
      changeForm: false,
      dropdownOptions: false,
      lang: this.$validator.dictionary.locale
    };
  },
  methods: {
    titleTab(val) {
      return val.split(/(?=[A-Z])/).join(" ");
    },
    editProfile() {
      this.isDisabled = false;
    },
    validateForm() {
      this.$validator.validateAll().then((response) => {
        if (!response) {
          this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
          return;
        }
        this.saveProfile();
      });
    },
    saveProfile() {
      this.isLoading = true;
      const id = this.companyProfile.id;
      this.$store.dispatch("company/updateProfile", { id: id, company: this.companyProfile })
        .then(() => {
          this.unsavedChanges = false;
          this.isDisabled = true;
          this.isLoading = false;
          this.showAlertSuccess(this.$t("Updated"));
          this.changeForm = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    changeTab(tab) {
      this.$validator.validateAll().then((response) => {
        if (!response) {
          this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
          return;
        }
        if (!this.changeForm) {
          this.isDisabled = true;
          this.currentTab = tab;
        } else {
          this.showAlertConfirm(this.$t("Warning"), this.$t("ChangesYouMadeMayNotBeSaved"))
            .then((response) => {
              if (response) {
                this.changeForm = false;
                this.currentTab = tab;
                this.unsavedChanges = false;
              }
            })
            .catch((error) => {
              this.showAlertError(error);
            });

          this.unsavedChanges = false;
          this.changeForm = false;
        }
      });
    },
    resetForm() {
      this.isDisabled = true;
      this.changeForm = false;
      this.unsavedChanges = false;
    },
    getProfile() {
      this.isLoading = true;
      this.$store.dispatch("company/getProfile")
        .then((data) => (this.isLoading = false))
        .catch((error) => {
          this.showAlertError(error.data);
          this.isLoading = false;
        });
    },
  },
  created() {
    this.getProfile();
  },
  computed: {
    currentTabComponent() {
      return this.currentTab;
    },
    companyProfile() {
      return this.$store.state.company.companyProfile;
    },
  },
  mixins: [switchLocaleMixin, confirmationAlert],
};
</script>
