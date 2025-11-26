<template>
  <div class="partner-page">
    <MainNavbar />

    <PartnerHero />
    <PartnerTypesSection />
    <WhyPartnerSection />
    <PartnerContactSection />

    <MainFooter />
  </div>
</template>

<script setup>
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'

import PartnerHero from '@/components/partners/PartnerHero.vue'
import PartnerTypesSection from '@/components/partners/PartnerTypesSection.vue'
import WhyPartnerSection from '@/components/partners/WhyPartnerSection.vue'
import PartnerContactSection from '@/components/partners/PartnerContactSection.vue'
</script>

<style scoped>
.partner-page {
  min-height: 100vh;
  background-color: #ffffff;
  margin: 0;
  padding: 0;
}
</style>
