<template>
  <IndustriesCarouselPanel>
    <IndustriesCarousel
      eyebrow="Roles We Recruit"
      heading="Many industries."
      heading-accent="Where you'll find your role."
      subtitle="Browse the sectors we recruit for. Each one comes with deep market knowledge and a recruiter who speaks the language."
      learn-more-title="Learn more"
      learn-more-subtitle="Explore all sectors"
    />
  </IndustriesCarouselPanel>
</template>

<script setup lang="ts">
/**
 * Talents — Industries carousel.
 *
 * Thin wrapper around the shared IndustriesCarousel — supplies Talents-side
 * copy (candidate-facing). Wrapped in IndustriesCarouselPanel so it shows the
 * brand panel on this page while the carousel component stays reusable.
 */
import IndustriesCarousel from '@/components/landing/shared/IndustriesCarousel.vue'
import IndustriesCarouselPanel from '@/components/landing/shared/IndustriesCarouselPanel.vue'
</script>
