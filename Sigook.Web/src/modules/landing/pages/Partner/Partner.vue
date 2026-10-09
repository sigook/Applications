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
import PartnerHeroSection from '@/modules/landing/components/Partner/PartnerHeroSection.vue'
import PartnerTypesSection from '@/modules/landing/components/Partner/PartnerTypesSection.vue'
import PartnerBenefitsSection from '@/modules/landing/components/Partner/PartnerBenefitsSection.vue'
import PartnerProcessSection from '@/modules/landing/components/Partner/PartnerProcessSection.vue'
import ContactSection from '@/modules/landing/components/shared/sections/ContactSection.vue'
</script>
