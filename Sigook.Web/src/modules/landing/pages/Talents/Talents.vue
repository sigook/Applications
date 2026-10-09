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
import TalentsHeroSection from '@/modules/landing/components/Talents/TalentsHeroSection.vue'
import TalentsSolutionsSection from '@/modules/landing/components/Talents/TalentsSolutionsSection.vue'
import TalentsWhyWorkSection from '@/modules/landing/components/Talents/TalentsWhyWorkSection.vue'
import TalentsIndustriesCarousel from '@/modules/landing/components/Talents/TalentsIndustriesCarousel.vue'
import ContactSection from '@/modules/landing/components/shared/sections/ContactSection.vue'
</script>
