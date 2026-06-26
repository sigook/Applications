<template>
  <LandingHero
    :image="heroImage"
    :image-sm="heroImageSm"
    focal="center 35%"
    eyebrow-variant="cyan"
    eyebrow-text="Talents"
    subtitle="At Sigook we connect skilled people with top employers — matching your ambition with opportunities to grow, earn, and build a career that actually fits the life you want."
    scroll-href="#talents-solutions"
    :magnifier-vars="{
      '--lh-mag-top': 'clamp(28%, 30vw, 36%)',
      '--lh-mag-right': 'clamp(6%, 7vw, 10%)',
      '--lh-mag-top-m': '28%',
      '--lh-mag-right-m': '6%',
    }"
  >
    <template #heading>
      Your <span class="landing-hero-accent">career.</span><br>
      Your <span class="landing-hero-accent">way.</span>
    </template>

    <LabeledChipList label="We recruit for" :items="ROLE_TYPES" />
  </LandingHero>
</template>

<script setup lang="ts">
import LandingHero from '@/components/landing/shared/LandingHero.vue'
import LabeledChipList from '@/components/landing/shared/LabeledChipList.vue'
import heroImage from '@/assets/images/v2/hero/talents.webp'
import heroImageSm from '@/assets/images/v2/hero/talents-960.webp'

const ROLE_TYPES = [
  'Professional roles',
  'Industrial roles',
] as const
</script>
