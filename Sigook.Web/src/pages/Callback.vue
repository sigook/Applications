<template>
  <b-loading active></b-loading>
</template>

<script lang="ts">
import { mapStores } from 'pinia';
import { useSecurityStore } from '@/stores/security';
import Oidc from 'oidc-client';
import menu from "@/security/menu";

export default {
  computed: {
    ...mapStores(useSecurityStore),
  },
  data() {
    return {
      userManager: new Oidc.UserManager({} as any)
    }
  },
  async created() {
    if (!this.securityStore.user) {
      await this.userManager.signinRedirectCallback();
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