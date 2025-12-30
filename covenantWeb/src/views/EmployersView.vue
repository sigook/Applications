<template>
  <div class="employers-page">
    <MainNavbar />

    <EmployersMainSection />

    <IndustriesCTA />

    <RolesWeRecruit />

    <div id="contact-section">
      <PartnerContactSection />
    </div>

    <MainFooter />
  </div>
</template>

<script setup lang="ts">
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'
import PartnerContactSection from '@/components/partners/PartnerContactSection.vue'
import RolesWeRecruit from '@/components/employers/RolesWeRecruit.vue'
import IndustriesCTA from '@/components/employers/IndustriesCTA.vue'
import EmployersMainSection from '@/components/employers/EmployersMainSection.vue'
</script>

<style scoped>
.employers-page {
  background-color: #05162d;
  min-height: 100vh;
}
</style>
