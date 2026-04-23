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
  if (!securityStore.user) {
    await mgr.signinRedirectCallback();
    await securityStore.getUser();
  }
  const roles = await securityStore.userRoles;
  const homePageUrl = menu.getDefaultHomePageUrlBaseOnRoles(roles);
  router.push(homePageUrl);
})();
</script>
