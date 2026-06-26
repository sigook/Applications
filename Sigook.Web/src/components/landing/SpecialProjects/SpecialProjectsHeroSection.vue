<template>
  <LandingHero
    :image="heroImage"
    :image-sm="heroImageSm"
    focal="center 58%"
    eyebrow-variant="cyan"
    eyebrow-text="Special Projects"
    subtitle="Where workforce development, specialized training, and innovative programs converge — strengthening local economies and creating opportunity across industries, communities, and public institutions."
    scroll-href="#sp-focus"
    :magnifier-vars="{
      '--lh-mag-bottom': 'clamp(12%, 14vw, 18%)',
      '--lh-mag-right': 'clamp(6%, 7vw, 10%)',
      '--lh-mag-bottom-m': '14%',
      '--lh-mag-right-m': '6%',
    }"
  >
    <template #heading>
      Beyond <span class="landing-hero-accent">staffing.</span><br>
      Built for what's <span class="landing-hero-accent">next.</span>
    </template>

    <LabeledChipList label="Focus areas" :items="FOCUS_AREAS" />
  </LandingHero>
</template>

<script setup lang="ts">
import LandingHero from '@/components/landing/shared/LandingHero.vue'
import LabeledChipList from '@/components/landing/shared/LabeledChipList.vue'
import heroImage from '@/assets/images/v2/hero/special-projects.webp'
import heroImageSm from '@/assets/images/v2/hero/special-projects-960.webp'

const FOCUS_AREAS = [
  'Workforce',
  'Training',
  'Community',
  'Innovation',
] as const
</script>
