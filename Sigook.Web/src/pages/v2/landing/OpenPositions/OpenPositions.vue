<template>
  <main>
    <OpenPositionsHeroSection />
    <OpenPositionsListSection />
    <OpenPositionsBrowseSection />
    <OpenPositionsCtaSection />

    <!-- ── Contact ───────────────────────────────────────────────────────── -->
    <div id="openpos-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
/**
 * /v2/open-positions landing page — the job board.
 *
 * Section rhythm:
 *  1. OpenPositionsHeroSection    — Window with glass search form + stats
 *  2. OpenPositionsListSection    — Panel master-detail (desktop) / accordion
 *                                   (mobile) job list
 *  3. OpenPositionsBrowseSection  — Panel reusing IndustriesCarousel with
 *                                   "browse by sector" copy
 *  4. OpenPositionsCtaSection     — Window "send your resume" bridge CTA
 *  5. ContactSection (shared)     — "Get in Touch" form
 *
 * Two consecutive panels (List + Browse) intentionally stack with the same
 * border-radius — the margin-top negative on each creates a layered effect
 * rather than a zig-zag, which still reads as "list of open roles".
 */
import OpenPositionsHeroSection from '@/components/v2/landing/OpenPositions/OpenPositionsHeroSection.vue'
import OpenPositionsListSection from '@/components/v2/landing/OpenPositions/OpenPositionsListSection.vue'
import OpenPositionsBrowseSection from '@/components/v2/landing/OpenPositions/OpenPositionsBrowseSection.vue'
import OpenPositionsCtaSection from '@/components/v2/landing/OpenPositions/OpenPositionsCtaSection.vue'
import ContactSection from '@/components/v2/landing/shared/ContactSection.vue'
</script>
