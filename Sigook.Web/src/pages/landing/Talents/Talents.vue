<template>
  <main>
    <TalentsHeroSection />
    <TalentsSolutionsSection />
    <TalentsWhyWorkSection />
    <TalentsIndustriesCarousel />

    <div id="talents-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
import TalentsHeroSection from '@/components/landing/Talents/TalentsHeroSection.vue'
import TalentsSolutionsSection from '@/components/landing/Talents/TalentsSolutionsSection.vue'
import TalentsWhyWorkSection from '@/components/landing/Talents/TalentsWhyWorkSection.vue'
import TalentsIndustriesCarousel from '@/components/landing/Talents/TalentsIndustriesCarousel.vue'
import ContactSection from '@/components/landing/shared/ContactSection.vue'
</script>
