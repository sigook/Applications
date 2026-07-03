<template>
  <LandingHero
    :image="heroImage"
    :image-sm="heroImageSm"
    focal="center 35%"
    eyebrow-variant="cyan"
    eyebrow-text="Our Areas of Expertise"
    subtitle="We recruit highly qualified professionals for specialized positions across the industries that power the country's economy—combining deep sector knowledge with long-term, measurable impact."
    scroll-href="#industries-grid"
    :magnifier-vars="{
      '--lh-mag-bottom': 'clamp(12%, 14vw, 18%)',
      '--lh-mag-left': 'clamp(6%, 7vw, 10%)',
      '--lh-mag-bottom-m': '14%',
      '--lh-mag-left-m': '6%',
    }"
  >
    <template #heading>
      Multiple <span class="landing-hero-accent">sectors.</span><br>
      Endless <span class="landing-hero-accent">possibilities.</span>
    </template>

    <LabeledChipList label="Sectors we serve" :items="PREVIEW_INDUSTRIES" more-label="+ many more" />
  </LandingHero>
</template>

<script setup lang="ts">
import LandingHero from '@/components/landing/shared/hero/LandingHero.vue'
import LabeledChipList from '@/components/landing/shared/ui/LabeledChipList.vue'
import heroImage from '@/assets/images/landing/hero/industries.webp'
import heroImageSm from '@/assets/images/landing/hero/industries-960.webp'

const PREVIEW_INDUSTRIES = [
  'Manufacturing',
  'Construction',
  'Logistics',
  'Engineering',
  'Technology',
] as const
</script>
