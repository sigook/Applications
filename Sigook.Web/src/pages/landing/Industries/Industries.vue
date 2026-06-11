<template>
  <main>
    <IndustriesHeroSection />
    <IndustriesGridSection />

    <!-- ── Contact ───────────────────────────────────────────────────────── -->
    <div id="industries-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
import IndustriesHeroSection from '@/components/landing/Industries/IndustriesHeroSection.vue'
import IndustriesGridSection from '@/components/landing/Industries/IndustriesGridSection.vue'
import ContactSection from '@/components/landing/shared/ContactSection.vue'
</script>
