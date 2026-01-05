<template>
  <div class="profile white-container-mobile" v-if="agency">
    <b-loading v-model="isLoading"></b-loading>
    <nav class="profile-menu">
      <div class="scroll">
        <a v-for="tab in tabs" v-bind:key="tab" v-bind:class="['tab-button', { active: currentTab === tab }]"
          v-on:click="changeTab(tab)">
          {{ $t(titleTab(tab)) }}
        </a>
      </div>
      <div class="options-profile">
        <p @click="dropdownRolVisible = !dropdownRolVisible" :class="{ 'dropdown-open': dropdownRolVisible }">
          <span>▶</span> {{ $t("Options") }}
        </p>
        <transition name="fade">
          <div class="dropdown-rol" v-if="dropdownRolVisible">
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
        <upload-image @imageSelected="(profileImg) => (agency.logo.fileName = profileImg)"
          :edited-image="agency.logo" :class="{ disabled: isDisabled }" :required="false">
        </upload-image>

        <div v-if="agency.locations">
          <h1 class="capitalize">{{ agency.fullName | lowercase }}</h1>
          <p class="icon-before-location icon-before uppercase" v-if="agency.locations[0]">
            {{ agency.locations[0].address }}
            {{ agency.locations[0].city.value }}
            {{ agency.locations[0].city.province.code }}
            {{ agency.locations[0].postalCode }}
          </p>
        </div>
      </div>
      <component v-bind:is="currentTabComponent" v-if="agency" class="tab" :agency-data="agency">
      </component>
    </div>
  </div>
</template>

<script>
import confirmationAlert from "../../mixins/confirmationAlert";
import switchLocaleMixin from "../../mixins/switchLocaleMixin";

export default {
  data() {
    return {
      isLoading: false,
      currentTab: "BusinessInformation",
      isDisabled: true,
      dropdownRolVisible: false,
      lang: this.$validator.dictionary.locale,
      tabs: [
        "BusinessInformation",
        "BillingInformation",
        "ContactInformation",
        "AccountSecurity",
        "Users",
        "UserNotification",
      ],
    };
  },
  components: {
    BusinessInformation: () => import("@/components/agency/ProfileBusiness"),
    BillingInformation: () => import("@/components/agency/ProfileBilling"),
    ContactInformation: () => import("@/components/agency/ProfileContact"),
    AccountSecurity: () => import("@/components/agency/ProfileAccountInformation"),
    UploadImage: () => import("@/components/PreviewImage"),
    UserNotification: () => import("../../components/UserNotification"),
    Users: () => import("../../components/agency/AgencyPersonnel"),
  },
  async created() {
    this.isLoading = true;
    await this.$store.dispatch("agency/getAgencyProfile");
    this.isLoading = false;
  },
  methods: {
    titleTab(val) {
      return val.split(/(?=[A-Z])/).join(" ");
    },
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
      this.$store.dispatch("agency/updateAgency", this.agency)
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

      this.change = false;
    },
    changeTab(tab) {
      this.$validator.validateAll().then((response) => {
        if (!response) {
          this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
          return;
        }
        if (!this.change) {
          this.currentTab = tab;
        } else {
          this.change = false;
          this.currentTab = tab;
        }
      });
    },
  },
  computed: {
    currentTabComponent() {
      return this.currentTab;
    },
    agency() {
      return this.$store.state.agency.agency;
    }
  },
  mixins: [confirmationAlert, switchLocaleMixin],
};
</script>

<style lang="scss" scoped>
.profile-top {
  &>.disabled {
    pointer-events: none;
  }
}
</style>
