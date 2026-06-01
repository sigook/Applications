<template>
  <main>
    <SpecialProjectsHeroSection />
    <SpecialProjectsFocusSection />

    <!-- ── Contact ───────────────────────────────────────────────────────── -->
    <div id="sp-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
import SpecialProjectsHeroSection from '@/components/v2/landing/SpecialProjects/SpecialProjectsHeroSection.vue'
import SpecialProjectsFocusSection from '@/components/v2/landing/SpecialProjects/SpecialProjectsFocusSection.vue'
import ContactSection from '@/components/v2/landing/shared/ContactSection.vue'
</script>
