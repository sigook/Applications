<template>
  <div class="licensed-page">
    <MainNavbar />
    <LicensedCertifiedSection />
    <MainFooter />
  </div>
</template>

<style scoped>
.licensed-page {
  min-height: 100vh;
  background: RGBA(7, 24, 50, 10.05);
  background: linear-gradient(180deg,rgba(7, 24, 50, 1) 43%, rgba(201, 201, 201, 1) 61%);  /* mismo tono oscuro del diseño */
  margin: 0;
  padding: 0;
}

</style>

<script setup>
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'
import LicensedCertifiedSection from '@/components/licensed/LicensedCertifiedSection.vue'
</script>
