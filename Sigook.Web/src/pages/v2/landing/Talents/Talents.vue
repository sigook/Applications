<template>
  <main>
    <TalentsHeroSection />
    <TalentsSolutionsSection />
    <TalentsWhyWorkSection />
    <TalentsIndustriesCarousel />

    <!-- ── Contact ───────────────────────────────────────────────────────── -->
    <div id="talents-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
import TalentsHeroSection from '@/components/v2/landing/Talents/TalentsHeroSection.vue'
import TalentsSolutionsSection from '@/components/v2/landing/Talents/TalentsSolutionsSection.vue'
import TalentsWhyWorkSection from '@/components/v2/landing/Talents/TalentsWhyWorkSection.vue'
import TalentsIndustriesCarousel from '@/components/v2/landing/Talents/TalentsIndustriesCarousel.vue'
import ContactSection from '@/components/v2/landing/shared/ContactSection.vue'
</script>
