<template>
  <main>
    <NewsHeroSection />
    <NewsFeaturedSection />
    <NewsLatestGridSection />
    <NewsTopicsSection />
    <ContactSection />
  </main>
</template>

<script setup lang="ts">
/**
 * /v2/news landing page — newsroom experience.
 *
 * Section rhythm: window → panel → window → panel → window
 *  1. NewsHeroSection         — Live pulse + Newsroom eyebrow + category tabs
 *  2. NewsFeaturedSection     — Editor's-picks carousel (3 stories)
 *  3. NewsLatestGridSection   — 6-card grid of recent articles
 *  4. NewsTopicsSection       — 6 SecondaryCards for category browsing
 *  5. ContactSection (shared) — "Get in Touch" form (closes page)
 *
 * NewsletterSection is kept in the codebase but not mounted: it is a UI-only
 * subscription form with no backend mailing-list endpoint yet. Until one
 * exists, the page closes with the shared ContactSection like every other
 * v2 landing page.
 */
import NewsHeroSection from '@/components/landing/News/NewsHeroSection.vue'
import NewsFeaturedSection from '@/components/landing/News/NewsFeaturedSection.vue'
import NewsLatestGridSection from '@/components/landing/News/NewsLatestGridSection.vue'
import NewsTopicsSection from '@/components/landing/News/NewsTopicsSection.vue'
import ContactSection from '@/components/landing/shared/ContactSection.vue'
</script>
