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
  background-color: transparent;
  margin: 0;
  padding: 0;
}
</style>

<script setup lang="ts">
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'
import HeroSection from '@/components/home/HeroSection.vue'
import EmployersTalentsSection from '@/components/home/EmployersTalentsSection.vue'
import StatsOverview from '@/components/home/StatsOverview.vue'
import WhyChoose from '@/components/home/WhyChoose.vue'
import NationwideSection from '@/components/home/NationwideSection.vue'
import CertifiedAgencySection from '@/components/home/CertifiedAgencySection.vue'
import DownloadAppSection from '@/components/home/DownloadAppSection.vue'
import HomeContact from '@/components/home/HomeContact.vue'
</script>
