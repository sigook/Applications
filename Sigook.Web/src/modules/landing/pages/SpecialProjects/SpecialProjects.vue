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
import SpecialProjectsHeroSection from '@/modules/landing/components/SpecialProjects/SpecialProjectsHeroSection.vue'
import SpecialProjectsFocusSection from '@/modules/landing/components/SpecialProjects/SpecialProjectsFocusSection.vue'
import ContactSection from '@/modules/landing/components/shared/sections/ContactSection.vue'
</script>
