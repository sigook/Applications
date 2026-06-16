<template>
  <main>
    <IndustriesHeroSection />
    <IndustriesGridSection />

    <!-- ── Contact ───────────────────────────────────────────────────────── -->
    <div id="industries-contact">
      <ContactSection />
    </div>

    <!-- Worker registration modal — opened from the grid's "Talk to us" card -->
    <WorkerRegisterModal />
  </main>
</template>

<script setup lang="ts">
import IndustriesHeroSection from '@/components/landing/Industries/IndustriesHeroSection.vue'
import IndustriesGridSection from '@/components/landing/Industries/IndustriesGridSection.vue'
import ContactSection from '@/components/landing/shared/ContactSection.vue'
import WorkerRegisterModal from '@/components/landing/shared/forms/WorkerRegisterModal.vue'
</script>
