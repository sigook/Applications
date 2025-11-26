<template>
  <div class="about-page">
    <MainNavbar />
    <AboutHeroStatsSection />
    <AboutStorySection />
    <AboutWhyWorkSection />
    <ContactSection />
    <MainFooter />
  </div>
</template>

<style scoped>
.about-page {
  min-height: 100vh;
  background-color: #020d1e;
  margin: 0;
  padding: 0;
}
</style>

<script setup>
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'
import AboutHeroStatsSection from '@/components/about/AboutHeroStatsSection.vue'
import AboutStorySection from '@/components/about/AboutStorySection.vue'
import AboutWhyWorkSection from '@/components/about/AboutWhyWorkSection.vue'
import ContactSection from '@/components/about/ContactSection.vue'
</script>
