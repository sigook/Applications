<template>
  <b-loading v-model="isLoading"></b-loading>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useSecurityStore } from '@/stores/security';
import mgr from '@/security/securityService';
import menu from '@/security/menu';

const router = useRouter();
const securityStore = useSecurityStore();
const isLoading = ref(true);

(async () => {
  try {
    if (!securityStore.user) {
      const params = new URLSearchParams(window.location.search);
      if (!params.has('code')) {
        router.push('/');
        return;
      }
      await mgr.signinRedirectCallback();
      await securityStore.getUser();
    }
    const homePageUrl = menu.getDefaultHomePageUrlBaseOnRoles(securityStore.userRoles);
    router.push(homePageUrl);
  } catch {
    router.push('/');
  } finally {
    isLoading.value = false;
  }
})();
</script>
