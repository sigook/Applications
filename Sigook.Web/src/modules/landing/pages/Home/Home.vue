<template>
  <main>
    <HeroSection />
    <DualCtaSection />
    <NumbersSection />
    <WhyChooseUsSection />
    <CertifiedSection />
    <AppDownloadSection />

    <TestimonialsSection />
    <div id="home-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
import HeroSection from '@/modules/landing/components/Home/HeroSection.vue'
import DualCtaSection from '@/modules/landing/components/Home/DualCtaSection.vue'
import NumbersSection from '@/modules/landing/components/Home/NumbersSection.vue'
import WhyChooseUsSection from '@/modules/landing/components/Home/WhyChooseUsSection.vue'
import CertifiedSection from '@/modules/landing/components/Home/CertifiedSection.vue'
import AppDownloadSection from '@/modules/landing/components/Home/AppDownloadSection.vue'
import TestimonialsSection from '@/modules/landing/components/Home/TestimonialsSection.vue'
import ContactSection from '@/modules/landing/components/shared/sections/ContactSection.vue'
</script>
