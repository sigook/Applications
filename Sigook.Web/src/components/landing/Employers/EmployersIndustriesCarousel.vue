<template>
  <IndustriesCarousel
    eyebrow="Sectors We Staff"
    heading="Eleven industries."
    heading-accent="Where your team is hiding."
    subtitle="From skilled trades to specialized professionals — we recruit across the sectors that move the U.S. economy. Pick yours."
    learn-more-title="Learn more"
    learn-more-subtitle="See all sectors"
  />
</template>

<script setup lang="ts">
/**
 * Employers — Industries carousel.
 *
 * Thin wrapper around the shared IndustriesCarousel — supplies employer-side
 * copy (hiring outcomes). Behavior, layout, and the 11-industries list live
 * in the shared component.
 */
import IndustriesCarousel from '@/components/landing/shared/IndustriesCarousel.vue'
</script>
