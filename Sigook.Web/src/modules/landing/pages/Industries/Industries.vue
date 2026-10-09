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
import IndustriesHeroSection from '@/modules/landing/components/Industries/IndustriesHeroSection.vue'
import IndustriesGridSection from '@/modules/landing/components/Industries/IndustriesGridSection.vue'
import ContactSection from '@/modules/landing/components/shared/sections/ContactSection.vue'
import CandidateApplyModal from '@/modules/landing/components/shared/forms/CandidateApplyModal.vue'
</script>
