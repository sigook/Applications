<template>
  <div class="sidebar-shell">
    <b-loading v-model="isLoading"></b-loading>

    <header class="mobile-topbar">
      <button class="mobile-topbar-burger" @click="isOpen = true" aria-label="Open menu">
        <b-icon icon="menu" size="is-medium"></b-icon>
      </button>
      <router-link to="/" class="mobile-topbar-brand">
        <img src="../assets/images/sm-logo.png" alt="logo" />
      </router-link>
      <div class="sidebar-user">
        <notification-bell v-if="isAgency" />
        <b-dropdown :key="isDrawer ? 'drawer' : 'desktop'" position="is-bottom-left" :mobile-modal="isDrawer"
          :append-to-body="!isDrawer" aria-role="menu">
          <template #trigger>
            <div class="sidebar-user-trigger">
              <span class="sidebar-user-name">{{ currentUser.fullName }}</span>
              <img v-if="currentUser.logo" :src="currentUser.logo.pathFile" class="img-30 image-profile"
                alt="profile" />
              <svg v-else width="40" height="40" viewBox="0 0 40 40">
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
    </header>

    <div v-if="isDrawer && isOpen" class="sidebar-overlay" @click="isOpen = false"></div>

    <aside class="sidebar-logged" :class="{ 'is-collapsed': collapsed, 'is-open': isOpen }">
      <div class="sidebar-brand">
        <router-link to="/" class="sidebar-brand-link">
          <img src="../assets/images/sm-logo.png" class="sidebar-logo" alt="logo" />
        </router-link>
        <button v-if="isDrawer" class="sidebar-toggle" @click="isOpen = false" aria-label="Close menu">
          <b-icon icon="close"></b-icon>
        </button>
        <button v-else class="sidebar-toggle" @click="isCollapsed = !isCollapsed"
          :title="isCollapsed ? 'Expand' : 'Collapse'">
          <b-icon :icon="isCollapsed ? 'menu' : 'menu-open'"></b-icon>
        </button>
      </div>

      <nav class="sidebar-nav">
        <div v-if="collapsed" class="sidebar-rail">
          <div v-for="(group, i) in menuGroups" :key="i" class="sidebar-rail-group">
            <span v-if="group.label" class="sidebar-rail-group-label" :title="group.label">{{ group.label }}</span>
            <ul>
              <li v-for="item in group.items" :key="item.to + item.label">
                <router-link :to="item.to" class="sidebar-rail-item" :class="{ 'is-active': isActive(item.to) }"
                  :title="item.label">
                  <b-icon :icon="item.icon" size="is-medium"></b-icon>
                  <span class="sidebar-rail-label">{{ item.label }}</span>
                </router-link>
              </li>
            </ul>
          </div>
        </div>

        <b-menu v-else>
          <b-menu-list>
            <template v-for="(group, i) in menuGroups" :key="i">
              <b-menu-item v-if="group.label" :title="group.label" class="sidebar-group"
                :ref="(el) => setGroupRef(el, i)" :model-value="isGroupActive(group)"
                @update:expanded="expandedGroups[i] = $event">
                <template #label="{ expanded }">
                  <span class="sidebar-group-label">
                    <b-icon :icon="group.icon" size="is-medium" class="mr-2"></b-icon>
                    <span>{{ group.label }}</span>
                  </span>
                  <b-icon :icon="expanded ? 'chevron-up' : 'chevron-down'" size="is-small"
                    class="sidebar-group-arrow"></b-icon>
                </template>
                <b-menu-item v-for="item in group.items" :key="item.to + item.label" tag="router-link" :to="item.to"
                  :model-value="isActive(item.to)" :title="item.label">
                  <template #label>
                    <span>{{ item.label }}</span>
                  </template>
                </b-menu-item>
              </b-menu-item>
              <b-menu-item v-else v-for="item in group.items" :key="item.to + item.label" tag="router-link"
                :to="item.to" :model-value="isActive(item.to)" :title="item.label">
                <template #label>
                  <b-icon :icon="item.icon" size="is-medium" class="mr-2"></b-icon>
                  <span>{{ item.label }}</span>
                </template>
              </b-menu-item>
            </template>
          </b-menu-list>
        </b-menu>
      </nav>

    </aside>
  </div>
</template>


<script setup lang="ts">
import { computed, ref, watch, nextTick, onUnmounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAgencyStore } from '@/stores/agency';
import { useSecurityStore } from '@/stores/security';
import { useBreakpoint } from '@/composables/useBreakpoint';
import { avatarLetters } from '@/utils/filters';
import menu from '@/security/menu';
import roles, { agencyStaff } from '@/security/roles';
import { getMyProfile } from '@/api/workerApi';
import { getAgencyProfile, getPersonnelAgencies, switchPersonnelAgency } from '@/api/agencyApi';
import { getCompanyProfile } from '@/api/companyApi';
import NotificationBell from '@/components/notifications/NotificationBell.vue';

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
const { isTouch: isDrawer, isCompactDesktop } = useBreakpoint();
const isCollapsed = ref(isCompactDesktop.value);
const isOpen = ref(false);

watch(isCompactDesktop, (compact) => {
  isCollapsed.value = compact;
});

// The icon-only rail is a desktop affordance; on mobile and tablet the sidebar
// is an off-canvas drawer that keeps the collapsible groups.
const collapsed = computed(() => isCollapsed.value && !isDrawer.value);

watch(isDrawer, (drawer) => {
  if (!drawer) isOpen.value = false;
});
onUnmounted(() => {
  document.body.classList.remove('has-drawer-open');
});

watch(isOpen, (open) => {
  document.body.classList.toggle('has-drawer-open', open && isDrawer.value);
});
watch(() => route.fullPath, () => { isOpen.value = false; });
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

const isAgency = computed(() =>
  securityStore.userRoles.some((ur) => agencyStaff.includes(ur)));

function isActive(to: string): boolean {
  return route.path === to || route.path.startsWith(`${to}/`);
}

function isGroupActive(group: MenuGroup): boolean {
  return group.items.some((item) => isActive(item.to));
}

function logout() {
  isLoading.value = true;
  securityStore.signOut().then(() => router.push('/'));
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
  currentUser.value.fullName = response.fullName;
  currentUser.value.profileImage = response.logo?.pathFile ?? null;
  profileUrl.value = '/company-profile';
}

async function getCompanyUserInfo() {
  const user = await securityStore.getUser();
  currentUser.value.fullName = user.profile.name;
  currentUser.value.profileImage = null;
  profileUrl.value = '/company-user-profile';
}

async function getWorkerInfo() {
  const data = await getMyProfile();
  currentUser.value.fullName = `${data.firstName} ${data.lastName}`;
  currentUser.value.profileImage = data.workerProfileImage;
  profileUrl.value = '/worker-profile';
}

function switchAgency(agency: { id: string; isPrimary: boolean }) {
  if (agency.isPrimary) return;
  switchPersonnelAgency(agency.id).then(async () => {
    router.push(menu.getDefaultHomePageUrlBaseOnRoles(securityStore.userRoles));
    await getAgencyInfo();
    window.location.reload();
  });
}

async function init() {
  const userRoles = securityStore.userRoles;
  for (const role of userRoles) {
    switch (role) {
      case roles.superAdmin:
      case roles.admin:
      case roles.recruiting:
      case roles.sales:
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
@import "../assets/scss/variables";

$sidebar-width: 250px;
$sidebar-width-collapsed: 100px;
$sidebar-width-drawer: 280px;
$topbar-height: 56px;

.sidebar-shell {
  display: contents;
}

.sidebar-user {
  position: fixed;
  top: 12px;
  right: 24px;
  z-index: 40;
  display: flex;
  align-items: center;
  gap: 12px;
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

// On desktop the bar itself is not a box: it collapses so its only visible
// child, the user pill, keeps behaving as the fixed top-right element.
.mobile-topbar {
  display: contents;

  .mobile-topbar-burger,
  .mobile-topbar-brand {
    display: none;
  }
}

.sidebar-overlay {
  position: fixed;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.45);
  z-index: 50;
}

.sidebar-logged {
  position: sticky;
  top: 0;
  align-self: flex-start;
  flex: 0 0 auto;
  z-index: 40;
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

  .sidebar-rail {
    ul {
      list-style: none;
      margin: 0;
      padding: 0;
    }

    .sidebar-rail-group + .sidebar-rail-group {
      margin-top: 14px;
    }

    .sidebar-rail-group-label {
      display: block;
      text-align: center;
      font-size: 10px;
      font-weight: 800;
      letter-spacing: 0.06em;
      text-transform: uppercase;
      color: #00adef;
      background-color: rgba(0, 173, 239, 0.1);
      border-radius: 6px;
      padding: 4px 6px;
      margin-bottom: 6px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .sidebar-rail-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 4px;
      padding: 10px 4px;
      border-radius: 8px;
      color: #555;
      text-align: center;

      &:hover {
        background-color: #f3f3f3;
      }

      &.is-active {
        background-color: rgba(0, 173, 239, 0.12);
        color: $blue-dark;
      }
    }

    .sidebar-rail-label {
      font-size: 11px;
      line-height: 1.2;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
      word-break: break-word;
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

@media (max-width: 1023px) {
  .mobile-topbar {
    display: flex;
    align-items: center;
    gap: 12px;
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    height: $topbar-height;
    padding: 0 12px;
    background-color: #fff;
    border-bottom: 1px solid #eee;
    z-index: 45;

    .mobile-topbar-burger {
      display: inline-flex;
      border: 0;
      background: transparent;
      cursor: pointer;
      color: #555;
      padding: 4px;
    }

    .mobile-topbar-brand {
      display: inline-flex;
      line-height: 0;

      img {
        max-height: 32px;
      }
    }
  }

  body.has-drawer-open {
    overflow: hidden;
  }

  .sidebar-logged {
    position: fixed;
    top: 0;
    left: 0;
    width: $sidebar-width-drawer;
    max-width: 85vw;
    height: 100dvh;
    z-index: 60;
    transform: translateX(-100%);
    transition: transform 0.25s ease;

    &.is-open {
      transform: none;
    }
  }

  .sidebar-user {
    position: static;
    margin-left: auto;
    border: 0;
    box-shadow: none;
    padding: 0;
    gap: 8px;

    .sidebar-user-name {
      display: none;
    }

    svg,
    .image-profile {
      width: 32px;
      height: 32px;
    }

  }
}
</style>
