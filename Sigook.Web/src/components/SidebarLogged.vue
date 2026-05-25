<template>
  <aside class="sidebar-logged" :class="{ 'is-collapsed': collapsed }">
    <b-loading v-model="isLoading"></b-loading>

    <div class="sidebar-brand">
      <router-link to="/" class="sidebar-brand-link">
        <img src="../assets/images/sm-logo.png" class="sidebar-logo" alt="logo" />
      </router-link>
      <button v-if="!isMobile" class="sidebar-toggle" @click="isCollapsed = !isCollapsed"
        :title="isCollapsed ? 'Expand' : 'Collapse'">
        <b-icon :icon="isCollapsed ? 'menu' : 'menu-open'"></b-icon>
      </button>
    </div>

    <nav class="sidebar-nav">
      <b-menu>
        <b-menu-list>
          <template v-for="(group, i) in menuGroups" :key="i">
            <b-menu-item v-if="group.label" :title="group.label" class="sidebar-group"
              :ref="(el) => setGroupRef(el, i)" :model-value="isGroupActive(group)"
              @update:expanded="expandedGroups[i] = $event">
              <template #label="{ expanded }">
                <span class="sidebar-group-label">
                  <b-tooltip :label="group.label" position="is-right" :active="isCollapsed && !isMobile" append-to-body>
                    <b-icon :icon="group.icon" class="mr-2"></b-icon>
                  </b-tooltip>
                  <span v-if="!collapsed">{{ group.label }}</span>
                </span>
                <b-icon v-if="!collapsed" :icon="expanded ? 'chevron-up' : 'chevron-down'" size="is-small"
                  class="sidebar-group-arrow"></b-icon>
              </template>
              <b-menu-item v-for="item in group.items" :key="item.to + item.label" tag="router-link" :to="item.to"
                :model-value="isActive(item.to)" :title="item.label">
                <template #label>
                  <b-tooltip :label="item.label" position="is-right" :active="isCollapsed && !isMobile" append-to-body>
                    <b-icon :icon="item.icon" class="mr-2"></b-icon>
                  </b-tooltip>
                  <span v-if="!collapsed">{{ item.label }}</span>
                </template>
              </b-menu-item>
            </b-menu-item>
            <b-menu-item v-else v-for="item in group.items" :key="item.to + item.label" tag="router-link"
              :to="item.to" :model-value="isActive(item.to)" :title="item.label">
              <template #label>
                <b-tooltip :label="item.label" position="is-right" :active="isCollapsed && !isMobile" append-to-body>
                  <b-icon :icon="item.icon" class="mr-2"></b-icon>
                </b-tooltip>
                <span v-if="!collapsed">{{ item.label }}</span>
              </template>
            </b-menu-item>
          </template>
        </b-menu-list>
      </b-menu>
    </nav>

    <div class="sidebar-user">
      <b-dropdown position="is-bottom-left" :mobile-modal="false" aria-role="menu">
        <template #trigger>
          <div class="sidebar-user-trigger">
            <span class="sidebar-user-name">{{ currentUser.fullName }}</span>
            <img v-if="currentUser.logo" :src="currentUser.logo.pathFile" class="img-30 image-profile" alt="profile" />
            <svg v-else width="40" height="40">
              <circle cx="20" cy="20" r="20" fill="#aeaeae" />
              <text x="50%" y="50%" text-anchor="middle" fill="white" font-size="20px" font-family="Arial" dy=".3em">
                {{ avatarLetters(currentUser.fullName) }}
              </text>
            </svg>
          </div>
        </template>
        <b-dropdown-item has-link aria-role="menuitem">
          <router-link :to="profileUrl">Edit Profile</router-link>
        </b-dropdown-item>
        <b-dropdown-item v-for="(item, i) in currentUser.agencies" :key="i" @click="switchAgency(item)"
          :class="{ 'primary-agency': item.isPrimary }" aria-role="menuitem">
          {{ item.name }}
        </b-dropdown-item>
        <b-dropdown-item @click="logout" aria-role="menuitem">
          Log Out
        </b-dropdown-item>
      </b-dropdown>
    </div>
  </aside>
</template>


<script setup lang="ts">
import { computed, ref, nextTick, onMounted, onUnmounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAgencyStore } from '@/stores/agency';
import { useSecurityStore } from '@/stores/security';
import { avatarLetters } from '@/utils/filters';
import menu from '@/security/menu';
import roles from '@/security/roles';
import { getMyProfile } from '@/api/workerApi';
import { getAgencyProfile, getPersonnelAgencies, switchPersonnelAgency } from '@/api/agencyApi';
import { getCompanyProfile } from '@/api/companyApi';

interface MenuLink {
  to: string;
  icon?: string;
  label: string;
  external?: boolean;
}

interface MenuGroup {
  label?: string;
  icon?: string;
  items: MenuLink[];
}

const router = useRouter();
const route = useRoute();
const agencyStore = useAgencyStore();
const securityStore = useSecurityStore();

const isLoading = ref(false);
const isCollapsed = ref(false);
const isMobile = ref(false);

// On mobile the sidebar is always icon-only, regardless of the manual toggle.
const collapsed = computed(() => isCollapsed.value || isMobile.value);

const mobileQuery = window.matchMedia('(max-width: 767px)');
function updateIsMobile(e: MediaQueryListEvent | MediaQueryList) {
  isMobile.value = e.matches;
}
onMounted(() => {
  updateIsMobile(mobileQuery);
  mobileQuery.addEventListener('change', updateIsMobile);
});
onUnmounted(() => mobileQuery.removeEventListener('change', updateIsMobile));
const profileUrl = ref('');
const menuGroups = ref<MenuGroup[]>([]);
const expandedGroups = ref<boolean[]>([]);
const groupRefs = ref<Array<{ newExpanded: boolean } | null>>([]);

function setGroupRef(el: unknown, index: number) {
  groupRefs.value[index] = el as { newExpanded: boolean } | null;
}

// Open the group whose child route is currently active (buefy keeps the
// expanded state internally, so we set it directly on the menu-item instance).
function expandActiveGroup() {
  menuGroups.value.forEach((group, i) => {
    if (group.label && isGroupActive(group) && groupRefs.value[i]) {
      groupRefs.value[i]!.newExpanded = true;
    }
  });
}

const currentUser = computed(() => agencyStore.agency);

function isActive(to: string): boolean {
  return route.path === to || route.path.startsWith(`${to}/`);
}

function isGroupActive(group: MenuGroup): boolean {
  return group.items.some((item) => isActive(item.to));
}

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
    router.push('/recruiting/orders');
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
  menuGroups.value = menu.getMenu(userRoles, agencyStore.agency);
  expandedGroups.value = menuGroups.value.map(() => false);
  await nextTick();
  expandActiveGroup();
}

init();
</script>

<style lang="scss">
$sidebar-width: 250px;
$sidebar-width-collapsed: 76px;

.sidebar-logged {
  position: sticky;
  top: 0;
  align-self: flex-start;
  flex: 0 0 auto;
  z-index: 30;
  display: flex;
  flex-direction: column;
  width: $sidebar-width;
  height: 100vh;
  background-color: #fff;
  box-shadow: 2px 0 8px #d6d6d6;
  transition: width 0.2s ease;

  &.is-collapsed {
    width: $sidebar-width-collapsed;
  }

  .sidebar-brand {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 12px 16px;
    min-height: 63px;
  }

  &.is-collapsed .sidebar-brand {
    justify-content: center;

    .sidebar-brand-link {
      display: none;
    }
  }

  .sidebar-logo {
    max-height: 36px;
  }

  .sidebar-toggle {
    border: 0;
    background: transparent;
    cursor: pointer;
    color: #555;
  }

  .sidebar-nav {
    flex: 1;
    overflow-y: auto;
    padding: 8px;
  }

  // The user/profile block is detached from the sidebar and pinned to the
  // top-right corner of the viewport, acting as a permanent navbar menu.
  .sidebar-user {
    position: fixed;
    top: 12px;
    right: 24px;
    z-index: 40;
    background-color: #fff;
    border: 1px solid #eee;
    border-radius: 999px;
    padding: 6px 14px;
    box-shadow: 0 2px 8px #d6d6d6;

    .sidebar-user-trigger {
      display: flex;
      align-items: center;
      gap: 10px;
      cursor: pointer;
    }

    .sidebar-user-name {
      font-size: 14px;
      max-width: 180px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
  }

  &.is-collapsed {
    .menu-list .icon {
      margin-right: 0 !important;
    }

    // submenu visibility follows buefy's v-show (only the expanded group);
    // strip indent/border so children align under the group
    .menu-list li ul {
      margin: 0;
      padding: 0;
      border-left: 0;
    }

    // top-level entries (group parents + flat items) hug the left edge
    .menu-list > li > a {
      justify-content: flex-start;
      padding-left: 16px;
      padding-right: 0;
    }

    // submenu children sit to the right, reading as belonging to their group
    .menu-list li ul a {
      display: flex;
      justify-content: flex-end;
      align-items: center;
      padding-left: 0;
      padding-right: 16px;
    }
  }

  .sidebar-group > a {
    display: flex;
    align-items: center;
  }

  .sidebar-group-label {
    display: inline-flex;
    align-items: center;
  }

  .sidebar-group-arrow {
    margin-left: auto;
  }
}

.dropdown-item.primary-agency,
.primary-agency {
  font-weight: 700;
}

@media (max-width: 767px) {
  .sidebar-logged {
    width: $sidebar-width-collapsed;

    .sidebar-brand-link,
    .sidebar-user-name {
      display: none;
    }
  }
}
</style>
