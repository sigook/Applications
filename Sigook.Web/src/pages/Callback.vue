<template>
  <b-loading active></b-loading>
</template>

<script>
import Oidc from 'oidc-client';
import menu from "@/security/menu";

export default {
  data() {
    return {
      userManager: new Oidc.UserManager()
    }
  },
  async created() {
    if (!this.$store.state.security.user) {
      await this.userManager.signinRedirectCallback();
      await this.$store.dispatch("getUser");
      const roles = await this.$store.state.security.userRoles;
      let homePageUrl = menu.getDefaultHomePageUrlBaseOnRoles(roles);
      this.$router.push(homePageUrl);
    } else {
      const roles = await this.$store.state.security.userRoles;
      let homePageUrl = menu.getDefaultHomePageUrlBaseOnRoles(roles);
      this.$router.push(homePageUrl);
    }
  }
}
</script>