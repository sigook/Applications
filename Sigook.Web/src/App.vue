<template>
  <div id="app">
    <div class="message-version" v-if="isANewVersion">
      New version available!
      <button @click="updateAppVersion">
        Please click here to get the latest version.
      </button>
    </div>
    <div v-if="isCallback">
      <router-view />
    </div>
    <div v-else-if="isLogged" class="logged-layout">
      <SidebarLogged />
      <div class="logged-content">
        <router-view />
      </div>
    </div>
    <div v-else-if="isV2Route" class="v2-page">
      <GlobalBackgroundV2 />
      <HeaderV2 />
      <router-view />
      <FooterV2 />
    </div>
    <div v-else>
      <Header />
      <router-view />
      <Footer />
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
import Header from '@/components/landing/Header.vue';
import Footer from '@/components/landing/Footer.vue';
import HeaderV2 from '@/components/v2/HeaderV2.vue';
import FooterV2 from '@/components/v2/FooterV2.vue';
import GlobalBackgroundV2 from '@/components/v2/shared/GlobalBackgroundV2.vue';

const route = useRoute();
const appStore = useAppStore();
const securityStore = useSecurityStore();

const isANewVersion = ref(false);
const isLogged = ref(false);

const isCallback = computed(() => route.name === 'callback');
const isV2Route = computed(() => route.meta?.layout === 'v2');

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

.message-version {
  background-color: #13dde8;
  color: white;
  text-align: center;
  padding: 5px 10px;

  button {
    display: block;
    font-size: 14px;
    color: #fff;
    margin: 5px auto;
    border: 0;
    border-bottom: 1px solid white;
  }
}

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
