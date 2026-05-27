<template>
  <main>
    <NewsHeroSection />
    <NewsFeaturedSection />
    <NewsLatestGridSection />
    <NewsTopicsSection />
    <NewsletterSection />
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
 *  5. NewsletterSection       — Weekly digest subscription form (closes page)
 *
 * The newsletter replaces the usual ContactSection at the bottom — the call
 * to action that matches a newsroom is "subscribe", not "get in touch".
 */
import NewsHeroSection from '@/components/v2/landing/News/NewsHeroSection.vue'
import NewsFeaturedSection from '@/components/v2/landing/News/NewsFeaturedSection.vue'
import NewsLatestGridSection from '@/components/v2/landing/News/NewsLatestGridSection.vue'
import NewsTopicsSection from '@/components/v2/landing/News/NewsTopicsSection.vue'
import NewsletterSection from '@/components/v2/landing/News/NewsletterSection.vue'
</script>
