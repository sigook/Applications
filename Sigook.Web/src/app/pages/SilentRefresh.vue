<template>
  <div></div>
</template>

<script setup lang="ts">
import mgr from '@/app/security/securityService';

mgr.signinSilentCallback();
</script>
