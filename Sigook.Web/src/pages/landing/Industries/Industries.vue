<template>
  <main>
    <IndustriesHeroSection />
    <IndustriesGridSection />

    <div id="industries-contact">
      <ContactSection />
    </div>

    <CandidateApplyModal />
  </main>
</template>

<script setup lang="ts">
import IndustriesHeroSection from '@/components/landing/Industries/IndustriesHeroSection.vue'
import IndustriesGridSection from '@/components/landing/Industries/IndustriesGridSection.vue'
import ContactSection from '@/components/landing/shared/ContactSection.vue'
import CandidateApplyModal from '@/components/landing/shared/forms/CandidateApplyModal.vue'
</script>
