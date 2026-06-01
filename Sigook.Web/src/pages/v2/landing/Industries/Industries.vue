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
import IndustriesHeroSection from '@/components/v2/landing/Industries/IndustriesHeroSection.vue'
import IndustriesGridSection from '@/components/v2/landing/Industries/IndustriesGridSection.vue'
import ContactSection from '@/components/v2/landing/shared/ContactSection.vue'
</script>
