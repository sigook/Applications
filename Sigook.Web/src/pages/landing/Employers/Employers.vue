<template>
  <main>
    <EmployersHeroSection />
    <EmployersSolutionsSection />
    <EmployersWhyChooseSection />
    <EmployersIndustriesCarousel />

    <div id="employers-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
import EmployersHeroSection from '@/components/landing/Employers/EmployersHeroSection.vue'
import EmployersSolutionsSection from '@/components/landing/Employers/EmployersSolutionsSection.vue'
import EmployersWhyChooseSection from '@/components/landing/Employers/EmployersWhyChooseSection.vue'
import EmployersIndustriesCarousel from '@/components/landing/Employers/EmployersIndustriesCarousel.vue'
import ContactSection from '@/components/landing/shared/sections/ContactSection.vue'
</script>
