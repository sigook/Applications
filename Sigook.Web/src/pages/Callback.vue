<template>
  <b-loading v-model="isLoading"></b-loading>
</template>

<script lang="ts">
import { mapStores } from 'pinia';
import { useSecurityStore } from '@/stores/security';
import mgr from '@/security/securityService';
import menu from "@/security/menu";

export default {
  data() {
    return {
      isLoading: true
    }
  },
  computed: {
    ...mapStores(useSecurityStore),
  },
  async created() {
    if (!this.securityStore.user) {
      await mgr.signinRedirectCallback();
      await this.securityStore.getUser();
      const roles = await this.securityStore.userRoles;
      let homePageUrl = menu.getDefaultHomePageUrlBaseOnRoles(roles);
      this.$router.push(homePageUrl);
    } else {
      const roles = await this.securityStore.userRoles;
      let homePageUrl = menu.getDefaultHomePageUrlBaseOnRoles(roles);
      this.$router.push(homePageUrl);
    }
  }
}
</script>