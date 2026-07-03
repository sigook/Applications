<template>
  <main>
    <PartnerHeroSection />
    <PartnerTypesSection />
    <PartnerBenefitsSection />
    <PartnerProcessSection />

    <div id="partner-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
import PartnerHeroSection from '@/components/landing/Partner/PartnerHeroSection.vue'
import PartnerTypesSection from '@/components/landing/Partner/PartnerTypesSection.vue'
import PartnerBenefitsSection from '@/components/landing/Partner/PartnerBenefitsSection.vue'
import PartnerProcessSection from '@/components/landing/Partner/PartnerProcessSection.vue'
import ContactSection from '@/components/landing/shared/sections/ContactSection.vue'
</script>
