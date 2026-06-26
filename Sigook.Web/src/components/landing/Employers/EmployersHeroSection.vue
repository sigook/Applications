<template>
  <LandingHero
    :image="heroImage"
    :image-sm="heroImageSm"
    focal="center 40%"
    eyebrow-variant="red"
    eyebrow-text="For Employers"
    subtitle="We help companies recruit highly qualified professionals for specialized positions — turning hiring from a months-long search into a confident, measurable decision."
    scroll-href="#employers-solutions"
    :magnifier-vars="{
      '--lh-mag-top': 'clamp(28%, 30vw, 36%)',
      '--lh-mag-left': 'clamp(6%, 7vw, 10%)',
      '--lh-mag-top-m': '28%',
      '--lh-mag-left-m': '6%',
    }"
  >
    <template #heading>
      Your next great <span class="landing-hero-accent">hire.</span><br>
      <span class="landing-hero-accent">Found.</span>
    </template>

    <LabeledChipList label="We staff for" :items="STAFFING_TYPES" />
  </LandingHero>
</template>

<script setup lang="ts">
/**
 * Employers hero — thin wrapper over the shared LandingHero shell.
 * Magnifier anchor: center-left. Red eyebrow. Staffing-types chip list.
 */
import LandingHero from '@/components/landing/shared/LandingHero.vue'
import LabeledChipList from '@/components/landing/shared/LabeledChipList.vue'
import heroImage from '@/assets/images/v2/hero/employers.webp'
import heroImageSm from '@/assets/images/v2/hero/employers-960.webp'

const STAFFING_TYPES = [
  'Professional hires',
  'Industrial hires',
] as const
</script>
