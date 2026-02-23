<template>
  <div class="industries-page">
    <MainNavbar />

    <IndustriesHero />
    <IndustriesGrid />
    <IndustriesContact />

    <MainFooter />
  </div>
</template>

<script setup lang="ts">
import { useSeoMeta } from '@unhead/vue'
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'

import IndustriesHero from '@/components/industries/IndustriesHero.vue'
import IndustriesGrid from '@/components/industries/IndustriesGrid.vue'
import IndustriesContact from '@/components/industries/IndustriesContact.vue'

useSeoMeta({
  title: 'Industries We Serve - Covenant Group',
  description: 'Covenant Group provides staffing solutions across multiple industries including skilled trades, industrial, manufacturing, construction, and professional services.',
  ogTitle: 'Industries We Serve - Covenant Group',
  ogDescription: 'Staffing solutions across skilled trades, industrial, manufacturing, construction, and professional services.',
})
</script>

<style scoped>
.industries-page {
  min-height: 100vh;
  background-color: #0F2F44; /* mismo mood oscuro que el home */
  margin: 0;
  padding: 0;
}
</style>
