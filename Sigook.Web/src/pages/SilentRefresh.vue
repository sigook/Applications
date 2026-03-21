<template>
  <div></div>
</template>
<script lang="ts">
import mgr from '../security/securityService';

export default {
  created() {
    mgr.signinSilentCallback();
  }
}
</script>