<template>
  <main>
    <SpecialProjectsHeroSection />
    <SpecialProjectsFocusSection />

    <div id="sp-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
import SpecialProjectsHeroSection from '@/components/landing/SpecialProjects/SpecialProjectsHeroSection.vue'
import SpecialProjectsFocusSection from '@/components/landing/SpecialProjects/SpecialProjectsFocusSection.vue'
import ContactSection from '@/components/landing/shared/sections/ContactSection.vue'
</script>
