<template>
  <IndustriesCarousel
    eyebrow="Roles We Recruit"
    heading="Many industries."
    heading-accent="Where you'll find your role."
    subtitle="Browse the sectors we recruit for. Each one comes with deep market knowledge and a recruiter who speaks the language."
    learn-more-title="Learn more"
    learn-more-subtitle="Explore all sectors"
  />
</template>

<script setup lang="ts">
/**
 * Talents — Industries carousel.
 *
 * Thin wrapper around the shared IndustriesCarousel — supplies Talents-side
 * copy (candidate-facing). Behavior, layout, and the 11-industries list live
 * in the shared component.
 */
import IndustriesCarousel from '@/components/landing/shared/IndustriesCarousel.vue'
</script>
