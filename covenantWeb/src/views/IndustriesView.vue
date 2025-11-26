<template>
  <div class="industries-page">
    <MainNavbar />

    <IndustriesHero />
    <IndustriesGrid />
    <IndustriesContact />

    <MainFooter />
  </div>
</template>

<script setup>
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'

import IndustriesHero from '@/components/industries/IndustriesHero.vue'
import IndustriesGrid from '@/components/industries/IndustriesGrid.vue'
import IndustriesContact from '@/components/industries/IndustriesContact.vue'
</script>

<style scoped>
.industries-page {
  min-height: 100vh;
  background-color: #020d1e; /* mismo mood oscuro que el home */
  margin: 0;
  padding: 0;
}
</style>
