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
          {{ item.label }}
        </b-navbar-item>
        <b-navbar-dropdown v-for="item in multiMenus" :key="item.label" :active="$route.path.startsWith(item.to)">
          <template #label>
            <b-icon :icon="item.icon" class="mr-2"></b-icon>
            {{ item.label }}
          </template>
          <b-navbar-item v-for="subItem in item.items" :key="subItem.label" tag="router-link"
            :to="item.to + subItem.to">
            {{ subItem.label }}
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
                {{ avatarLetters(currentUser.fu) }}
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


<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAgencyStore } from '@/stores/agency';
import { useSecurityStore } from '@/stores/security';
import { avatarLetters } from '@/utils/filters';
import menu from '@/security/menu';
import roles from '@/security/roles';
import { getMyProfile } from '@/api/workerApi';
import { getAgencyProfile, getPersonnelAgencies, switchPersonnelAgency } from '@/api/agencyApi';
import { getCompanyProfile } from '@/api/companyApi';

interface MenuItem {
  to: string;
  icon?: string;
  label: string;
  external?: boolean;
  items?: { to: string; label: string }[];
}

const router = useRouter();
const agencyStore = useAgencyStore();
const securityStore = useSecurityStore();

const isLoading = ref(false);
const profileUrl = ref('');
const menuItems = ref<MenuItem[]>([]);

const currentUser = computed(() => agencyStore.agency);
const singleMenus = computed(() => menuItems.value.filter((item) => !item.items));
const multiMenus = computed(() => menuItems.value.filter((item) => item.items));

function logout() {
  isLoading.value = true;
  securityStore.signOut().then(() => router.push('/callback'));
}

async function getAgencyInfo() {
  const agency = await getAgencyProfile();
  agencyStore.setAgency(agency);
  const personnelAgencies = await getPersonnelAgencies();
  agencyStore.setPersonnelAgencies(personnelAgencies);
  profileUrl.value = '/agency-profile';
}

async function getCompanyInfo() {
  const response = await getCompanyProfile();
  currentUser.value.fullName = response.businessName;
  (currentUser.value as { profileImage: string | null }).profileImage = response.logo?.pathFile ?? null;
  profileUrl.value = '/company-profile';
}

async function getCompanyUserInfo() {
  const user = await securityStore.getUser();
  currentUser.value.fullName = user.profile.name;
  (currentUser.value as { profileImage: string | null }).profileImage = null;
  profileUrl.value = '/company-user-profile';
}

async function getWorkerInfo() {
  const data = await getMyProfile();
  currentUser.value.fullName = `${data.firstName} ${data.lastName}`;
  (currentUser.value as { profileImage: string | null }).profileImage = data.workerProfileImage;
  profileUrl.value = '/worker-profile';
}

function switchAgency(agency: { id: string; isPrimary: boolean }) {
  if (agency.isPrimary) return;
  switchPersonnelAgency(agency.id).then(async () => {
    router.push('/agency-requests');
    await getAgencyInfo();
    window.location.reload();
  });
}

async function init() {
  const userRoles = securityStore.userRoles;
  for (const role of userRoles) {
    switch (role) {
      case roles.agencyPersonnel:
      case roles.agency:
        await getAgencyInfo();
        break;
      case roles.companyUser:
        await getCompanyUserInfo();
        break;
      case roles.company:
        await getCompanyInfo();
        break;
      case roles.worker:
        await getWorkerInfo();
        break;
    }
  }
  menuItems.value = menu.getMenu(userRoles, agencyStore.agency);
}

init();
</script>
<style lang="scss">
.primary-agency {
  font-weight: bold;
}
</style>
