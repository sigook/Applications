<template>
  <main>
    <EmployersHeroSection />
    <EmployersSolutionsSection />
    <EmployersWhyChooseSection />
    <EmployersIndustriesCarousel />

    <!-- ── Contact ───────────────────────────────────────────────────────── -->
    <div id="employers-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
import EmployersHeroSection from '@/components/v2/landing/Employers/EmployersHeroSection.vue'
import EmployersSolutionsSection from '@/components/v2/landing/Employers/EmployersSolutionsSection.vue'
import EmployersWhyChooseSection from '@/components/v2/landing/Employers/EmployersWhyChooseSection.vue'
import EmployersIndustriesCarousel from '@/components/v2/landing/Employers/EmployersIndustriesCarousel.vue'
import ContactSection from '@/components/v2/landing/shared/ContactSection.vue'
</script>
