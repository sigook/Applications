<template>
  <div class="profile white-container-mobile">

    <b-loading v-model="isLoading"></b-loading>

    <nav class="profile-menu">
      <div class="contain-profile">

        <div class="profile-selected">
          <svg width="100" height="100">
            <circle cx="50" cy="50" r="35" fill="#aeaeae" />
            <text x="50%" y="50%" text-anchor="middle" fill="white" font-size="25px" font-family="Arial" dy=".3em">
              {{ companyUser.email | avatarLetters }}
            </text>
          </svg>
          <span class="no-arrow"></span>
        </div>
      </div>

      <br class="only-sm">
      <div class="scroll">
        <a v-for="(tab, index) in tabs" :key="'tabCompany' + index"
          v-bind:class="['tab-button', { active: currentTab === tab }]" v-on:click="changeTab(tab)">
          {{ $t(tab) }}
        </a>
      </div>

    </nav>

    <div class="profile-content company-profile">
      <div class="profile-top">
        <div v-if="companyUser">
          <h1 class="capitalize fz2">{{ companyUser.name }} {{ companyUser.lastname }}</h1>
        </div>
      </div>
      <component v-bind:is="currentTabComponent" class="tab" :user="companyUser"></component>
    </div>

  </div>
</template>

<script>

import CompanyUserUpdate from "@/components/company/CompanyUserUpdate";
import ProfileAccountInformation from "@/components/agency/ProfileAccountInformation";


export default {
  name: "CompanyUserProfile",
  components: {
    CompanyUserUpdate,
    ProfileAccountInformation,
  },
  data() {
    return {
      isLoading: false,
      currentTab: "CompanyUserUpdate",
      tabs: ["CompanyUserUpdate", "ProfileAccountInformation"],
      companyUser: {},
      isDisabled: true
    }
  },
  methods: {
    changeTab(newTab) {
      this.currentTab = newTab;
    },
    getCompanyUser() {
      this.isLoading = true;
      this.$store.dispatch("company/getCompanyUserDetail")
        .then(companyUser => {
          this.companyUser = companyUser;
          this.isLoading = false;
        }).catch(() => this.isLoading = false)
    },
  },
  created() {
    this.getCompanyUser();
  },
  computed: {
    currentTabComponent() {
      return this.currentTab;
    },
  }
}
</script>

<style lang="scss" scoped>
.profile-top {
  &>.disabled {
    pointer-events: none;
  }
}
</style>