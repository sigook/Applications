<template>
  <IndustriesCarouselPanel>
    <IndustriesCarousel
      eyebrow="Sectors We Staff"
      heading="Many industries."
      heading-accent="Where your team is hiding."
      subtitle="From skilled trades to specialized professionals — we recruit across the sectors that move the U.S. economy. Pick yours."
      learn-more-title="Learn more"
      learn-more-subtitle="See all sectors"
    />
  </IndustriesCarouselPanel>
</template>

<script setup lang="ts">
/**
 * Employers — Industries carousel.
 *
 * Thin wrapper around the shared IndustriesCarousel — supplies employer-side
 * copy (hiring outcomes). Wrapped in IndustriesCarouselPanel so it shows the
 * brand panel on this page while the carousel component stays reusable.
 */
import IndustriesCarousel from '@/components/landing/shared/IndustriesCarousel.vue'
import IndustriesCarouselPanel from '@/components/landing/shared/IndustriesCarouselPanel.vue'
</script>
