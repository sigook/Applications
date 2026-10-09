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
import EmployersHeroSection from '@/modules/landing/components/Employers/EmployersHeroSection.vue'
import EmployersSolutionsSection from '@/modules/landing/components/Employers/EmployersSolutionsSection.vue'
import EmployersWhyChooseSection from '@/modules/landing/components/Employers/EmployersWhyChooseSection.vue'
import EmployersIndustriesCarousel from '@/modules/landing/components/Employers/EmployersIndustriesCarousel.vue'
import ContactSection from '@/modules/landing/components/shared/sections/ContactSection.vue'
</script>
