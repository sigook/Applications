<template>
  <IndustriesCarousel
    eyebrow="By Industry"
    heading="Or browse by sector."
    heading-accent="Find the field you know best."
    subtitle="Eleven industries we recruit for — pick yours to see the roles, the rate ranges, and the recruiters who specialize in it."
    learn-more-title="Full directory"
    learn-more-subtitle="See all sectors"
  />
</template>

<script setup lang="ts">
/**
 * OpenPositions — Industries carousel.
 *
 * Thin wrapper around the shared IndustriesCarousel with job-board copy —
 * gives users an alternative discovery path when title search doesn't fit.
 *
 * The "Learn more" card already links to /v2/industries (handled inside
 * the shared component), which keeps cross-page navigation consistent.
 */
import IndustriesCarousel from '@/components/landing/shared/IndustriesCarousel.vue'
</script>
