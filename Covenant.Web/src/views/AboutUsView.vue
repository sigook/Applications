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
  background-color: #0F2F44;
  margin: 0;
  padding: 0;
}
</style>

<script setup lang="ts">
import { useSeoMeta } from '@unhead/vue'
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'
import AboutHeroStatsSection from '@/components/about/AboutHeroStatsSection.vue'
import AboutStorySection from '@/components/about/AboutStorySection.vue'
import AboutWhyWorkSection from '@/components/about/AboutWhyWorkSection.vue'
import ContactSection from '@/components/about/ContactSection.vue'

useSeoMeta({
  title: 'About Us - Covenant Group',
  description: 'Learn about Covenant Group, our story, mission, and why we are a trusted staffing and recruitment partner for employers across Canada.',
  ogTitle: 'About Us - Covenant Group',
  ogDescription: 'Learn about Covenant Group, our story, mission, and why we are a trusted staffing and recruitment partner across Canada.',
})
</script>
