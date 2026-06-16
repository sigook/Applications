<template>
  <div id="app">
    <!-- Non-invasive bottom-right toast that nudges the user to refresh
         when a newer build is detected. Replaces the legacy top banner
         which (a) shifted layout down and (b) collided with the landing
         fixed navbar. Component lives in src/components/landing/shared/. -->
    <AppVersionToast v-model="isANewVersion" @update="updateAppVersion" />
    <div v-if="isCallback">
      <router-view />
    </div>
    <div v-else-if="isLogged" class="logged-layout">
      <SidebarLogged />
      <div class="logged-content">
        <router-view />
      </div>
    </div>
    <div v-else-if="isLandingRoute" class="landing-page">
      <LandingBackground />
      <LandingHeader />
      <router-view />
      <LandingFooter />
    </div>
    <div v-else class="web-layout">
      <header class="web-layout__bar">
        <router-link to="/" class="web-layout__brand" aria-label="Sigook home">
          <img src="@/assets/images/sm-logo.png" alt="Sigook" />
        </router-link>
      </header>
      <main class="web-layout__content">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref, computed, watch } from 'vue';
import { useRoute, type RouteLocationNormalizedLoaded } from 'vue-router';
import { useAppStore } from '@/stores/app';
import { useSecurityStore } from '@/stores/security';
import axios from 'axios';
import SidebarLogged from '@/components/SidebarLogged.vue';
import LandingHeader from '@/components/landing/shared/Header.vue';
import LandingFooter from '@/components/landing/shared/Footer.vue';
import LandingBackground from '@/components/landing/shared/GlobalBackground.vue';
import AppVersionToast from '@/components/landing/shared/AppVersionToast.vue';

const route = useRoute();
const appStore = useAppStore();
const securityStore = useSecurityStore();

const isANewVersion = ref(false);
const isLogged = ref(false);

const isCallback = computed(() => route.name === 'callback');
const isLandingRoute = computed(() => route.meta?.layout === 'landing');

const MOBILE_REGEX = /Android|iPhone|iPod|BlackBerry/i;
const VERSION_CHECK_INTERVAL_MS = 60 * 60 * 1000;

let versionIntervalId: ReturnType<typeof setInterval> | null = null;

async function getAppVersion() {
  const item = localStorage.getItem('versionApp');
  const { data } = await axios.get('/version.json');
  if (item === 'null' || item === null) {
    localStorage.setItem('versionApp', data.version);
  } else if (data.version !== item) {
    isANewVersion.value = true;
  }
}

async function updateAppVersion() {
  localStorage.removeItem('versionApp');
  if ('caches' in window) {
    const names = await caches.keys();
    await Promise.all(names.map((name) => caches.delete(name)));
  }
  window.location.reload();
}

async function setIsLogged(currentRoute: RouteLocationNormalizedLoaded) {
  const user = await securityStore.getUser().catch(() => null);
  if (!currentRoute.meta.requiresAuth) {
    isLogged.value = false;
    return;
  }
  isLogged.value = !!user;
}

if (MOBILE_REGEX.test(navigator.userAgent)) {
  appStore.showMobile();
}

watch(() => route.fullPath, () => setIsLogged(route), { immediate: true });

onMounted(() => {
  getAppVersion();
  versionIntervalId = setInterval(getAppVersion, VERSION_CHECK_INTERVAL_MS);
});

onUnmounted(() => {
  if (versionIntervalId !== null) {
    clearInterval(versionIntervalId);
  }
});
</script>

<style lang="scss">
@import "assets/scss/variables";
@import "assets/fonts.css";
@import "assets/scss/master";

.no-menu {
  display: none;
}

/* Neutral chrome for standalone non-landing pages (worker registration,
   worker apply, email preferences, 404, unauthorized). Replaces the old
   legacy marketing Header/Footer so those can be removed. Light background,
   slim brand bar that links home. */
.web-layout__bar {
  display: flex;
  align-items: center;
  height: 64px;
  padding: 0 24px;
  background: #fff;
  border-bottom: 1px solid #eee;
}

.web-layout__brand {
  display: inline-flex;
  line-height: 0;
}

.web-layout__brand img {
  height: 32px;
  width: auto;
}

.web-layout__content {
  position: relative;
  min-height: calc(100vh - 64px);
}

/* Landing layout — clip any horizontal overflow from decorative absolute
   elements (glows, blurs, photos, circles) so the page never shows a
   horizontal scrollbar. `clip` is preferred over `hidden` because it does
   NOT create a scroll container (preserves position: sticky behavior). */
.landing-page {
  overflow-x: clip;
  position: relative;
}

.logged-layout {
  display: flex;
  align-items: flex-start;
  position: fixed;
  inset: 0;
  overflow: hidden;
}

.logged-content {
  flex: 1 1 auto;
  min-width: 0;
  height: 100vh;
  overflow-y: auto;
  // extra top padding leaves room for the fixed top-right user menu
  padding: 64px 32px 24px;
}

.main-container {
  background-color: $gray-bg;
  min-height: calc(100vh - 63px);
  padding: 30px;
}

/* Legacy .message-version banner removed — replaced by AppVersionToast
   (src/components/landing/shared/AppVersionToast.vue) which renders
   non-invasively in the bottom-right corner. */

@media (max-width: 767px) {
  .main-container {
    min-height: auto;
    padding: 0;
    background-color: #fff;
  }

  .logged-layout,
  .logged-content {
    position: static;
    height: auto;
    overflow: visible;
  }
}
</style>
