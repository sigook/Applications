<template>
  <div class="home-page">
    <MainNavbar />
    <HeroSection />
    <EmployersTalentsSection />
    <StatsOverview />
    <WhyChoose />
    <NationwideSection />
    <CertifiedAgencySection />
    <DownloadAppSection />
    <HomeContact />
    <MainFooter />
  </div>
</template>

<style scoped>
.home-page {
  min-height: 100vh;
  background-color: #59DC76;
  margin: 0;
  padding: 0;
}
</style>

<script setup lang="ts">
import { useSeoMeta } from '@unhead/vue'
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'

useSeoMeta({
  title: 'Covenant Group - Certified Temporary Help & Recruitment Agency',
  description: 'Covenant Group is a certified temporary help and recruitment agency fully licensed in Ontario. We deliver top-tier staffing solutions for employers across Canada.',
  ogTitle: 'Covenant Group - Certified Temporary Help & Recruitment Agency',
  ogDescription: 'Certified temporary help and recruitment agency. Fully licensed in Ontario, delivering top-tier staffing solutions across Canada.',
})
import HeroSection from '@/components/home/HeroSection.vue'
import EmployersTalentsSection from '@/components/home/EmployersTalentsSection.vue'
import StatsOverview from '@/components/home/StatsOverview.vue'
import WhyChoose from '@/components/home/WhyChoose.vue'
import NationwideSection from '@/components/home/NationwideSection.vue'
import CertifiedAgencySection from '@/components/home/CertifiedAgencySection.vue'
import DownloadAppSection from '@/components/home/DownloadAppSection.vue'
import HomeContact from '@/components/home/HomeContact.vue'
</script>
