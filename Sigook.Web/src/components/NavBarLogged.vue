<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>
    <b-navbar type="is-white" shadow style="z-index: 1;">
      <template #brand>
        <b-navbar-item tag="router-link" to="/">
          <img src="../assets/images/sm-logo.png" class="navbar-logo" alt="logo" />
        </b-navbar-item>
      </template>
      <template #start>
        <b-navbar-item v-for="(item, i) in singleMenus" :key="i" tag="router-link" :to="item.to" :href="item.to"
          :target="item.external ? '_blank' : ''" :active="$route.path === item.to">
          <b-icon :icon="item.icon" class="mr-2"></b-icon>
          {{ $t(item.label) }}
        </b-navbar-item>
        <b-navbar-dropdown v-for="item in multiMenus" :key="item.label" :active="$route.path.startsWith(item.to)">
          <template #label>
            <b-icon :icon="item.icon" class="mr-2"></b-icon>
            {{ $t(item.label) }}
          </template>
          <b-navbar-item v-for="subItem in item.items" :key="subItem.label" tag="router-link"
            :to="item.to + subItem.to">
            {{ $t(subItem.label) }}
          </b-navbar-item>
        </b-navbar-dropdown>
      </template>
      <template #end>
        <b-navbar-dropdown right>
          <template #label>
            <span class="mr-5">{{ currentUser.fullName }}</span>
            <img v-if="currentUser.logo" :src="currentUser.logo.pathFile" class="img-30 image-profile"
              alt="profile" />
            <svg v-else width="40" height="40">
              <circle cx="20" cy="20" r="20" fill="#aeaeae" />
              <text x="50%" y="50%" text-anchor="middle" fill="white" font-size="20px" font-family="Arial" dy=".3em">
                {{ currentUser.fu  | avatarLetters }}
              </text>
            </svg>
          </template>
          <b-navbar-item tag="router-link" :to="profileUrl">
            Edit Profile
          </b-navbar-item>
          <b-navbar-item v-for="(item, i) in currentUser.agencies" :key="i" @click="switchAgency(item)"
            :class="{ 'primary-agency': item.isPrimary }">
            {{ item.name }}
          </b-navbar-item>
          <b-navbar-item @click="logout">
            Log Out
          </b-navbar-item>
        </b-navbar-dropdown>
      </template>
    </b-navbar>
  </section>
</template>


<script lang="ts">
import menu from "@/security/menu";
import roles from "@/security/roles";
import { getMyProfile } from '@/api/workerApi';
import { getAgencyProfile, getPersonnelAgencies, switchPersonnelAgency } from '@/api/agencyApi';
import { getCompanyProfile } from '@/api/companyApi';

export default {
  data() {
    return {
      userRoles: [],
      menuItems: [],
      isLoading: false,
      profileUrl: ""
    };
  },
  methods: {
    logout() {
      this.isLoading = true
      this.$store.dispatch("signOut")
        .then(() => this.$router.push("/callback"));
    },
    switchLocale(lang) {
      this.$i18n.locale = lang;
      this.$validator.locale = lang;
      this.lang = this.$validator.dictionary.locale;
    },
    async getAgencyInfo() {
      const agency = await getAgencyProfile();
      this.$store.commit("agency/setAgency", agency);
      const personnelAgencies = await getPersonnelAgencies();
      this.$store.commit("agency/setPersonnelAgencies", personnelAgencies);
      this.profileUrl = "/agency-profile";
    },
    async getCompanyInfo() {
      const response = await getCompanyProfile();
      this.currentUser.fullName = response.businessName;
      this.currentUser.profileImage = response.logo?.pathFile ?? null;
      this.profileUrl = "/company-profile";
    },
    async getCompanyUserInfo() {
      await this.$store.dispatch("getUser").then((r) => {
        this.currentUser.fullName = r.profile.name;
        this.currentUser.profileImage = null;
        this.profileUrl = "/company-user-profile";
      });
    },
    async getWorkerInfo() {
      await getMyProfile().then((data) => {
        this.currentUser.fullName = `${data.firstName} ${data.lastName}`;
        this.currentUser.profileImage = data.workerProfileImage;
        this.profileUrl = "/worker-profile";
      });
    },
    switchAgency(agency) {
      if (agency.isPrimary) return;
      switchPersonnelAgency(agency.id)
        .then(async () => {
          this.$router.push('/agency-requests');
          await this.getAgencyInfo();
          window.location.reload();
        });
    },
  },
  computed: {
    currentUser() {
      return this.$store.state.agency.agency;
    },
    singleMenus() {
      return this.menuItems.filter(item => !item.items);
    },
    multiMenus() {
      return this.menuItems.filter(item => item.items);
    },
  },
  async created() {
    this.userRoles = this.$store.state.security.userRoles;
    for (let i = 0; i < this.userRoles.length; i++) {
      const role = this.userRoles[i];
      switch (role) {
        case roles.agencyPersonnel:
        case roles.agency: {
          await this.getAgencyInfo();
          break;
        }
        case roles.companyUser:
          await this.getCompanyUserInfo();
          break;
        case roles.company: {
          await this.getCompanyInfo();
          break;
        }
        case roles.worker: {
          await this.getWorkerInfo();
          break;
        }
      }
    }
    this.menuItems = menu.getMenu(this.userRoles, this.$store.state.agency.agency);
  }
};
</script>
<style lang="scss">
.primary-agency {
  font-weight: bold;
}
</style>
