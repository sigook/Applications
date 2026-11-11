<template>
  <b-loading v-model="isLoading"></b-loading>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useSecurityStore } from '@/app/stores/security';
import mgr from '@/app/security/securityService';
import menu from '@/app/security/menu';

const router = useRouter();
const securityStore = useSecurityStore();
const isLoading = ref(true);

void (async () => {
  try {
    if (!securityStore.user) {
      const params = new URLSearchParams(window.location.search);
      if (!params.has('code')) {
        void router.push('/');
        return;
      }
      await mgr.signinRedirectCallback();
      await securityStore.getUser();
    }
    const homePageUrl = menu.getDefaultHomePageUrlBaseOnRoles(securityStore.userRoles);
    void router.push(homePageUrl);
  } catch {
    void router.push('/');
  } finally {
    isLoading.value = false;
  }
})();
</script>
