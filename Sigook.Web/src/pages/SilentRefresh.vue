<template>
  <div></div>
</template>
<script>
import mgr from '../security/securityService';

export default {
  created() {
    mgr.signinSilentCallback();
  }
}
</script>