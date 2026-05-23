<template>
  <section class="hero-v2">
    <!-- Slide backgrounds — fade between them -->
    <div
      v-for="(slide, i) in slides"
      :key="i"
      class="hero-v2__bg"
      :class="{ 'hero-v2__bg--active': currentIndex === i }"
      aria-hidden="true"
    >
      <img v-if="slide.bg" :src="slide.bg" alt="" class="hero-v2__bg-img" />
      <div v-else class="hero-v2__bg-color" :style="{ background: slide.gradient }"></div>
    </div>

    <!-- Downward gradient overlay — strengthens as it approaches DualCta -->
    <div class="hero-v2__overlay" aria-hidden="true"></div>

    <!-- Decorative cyan glow + brand magnifier -->
    <div class="hero-v2__glow" aria-hidden="true"></div>
    <DecoMagnifierV2 class="hero-v2__magnifier" />

    <!-- Static content (shared across all slides) -->
    <div class="hero-v2__content">
      <img
        src="@/assets/images/v2/hero/hero-logo.png"
        alt="Sigook Work Factory"
        class="hero-v2__logo"
      />

      <transition name="hero-tag" mode="out-in">
        <p :key="currentIndex" class="hero-v2__tagline">
          {{ currentItem.tagline }}
        </p>
      </transition>

      <GlassPillCtaV2 to="/v2/about" size="md" class="hero-v2__cta">
        Learn More
      </GlassPillCtaV2>

      <SliderDotsV2
        v-model="currentIndex"
        :count="slides.length"
        aria-label="Carousel navigation"
        class="hero-v2__dots"
      />
    </div>
  </section>
</template>

<script setup lang="ts">
import { useCarousel } from '@/composables/useCarousel'
import DecoMagnifierV2 from '@/components/v2/shared/DecoMagnifierV2.vue'
import SliderDotsV2 from '@/components/v2/shared/SliderDotsV2.vue'
import GlassPillCtaV2 from '@/components/v2/shared/GlassPillCtaV2.vue'

import heroSlide1 from '@/assets/images/v2/hero/hero-slide1.jpg'
import heroSlide2 from '@/assets/images/v2/hero/hero-slide2.jpg'
import heroSlide3 from '@/assets/images/v2/hero/hero-slide3.jpg'

interface HeroSlide {
  bg?: string
  gradient?: string
  tagline: string
}

const slides: HeroSlide[] = [
  { bg: heroSlide1, tagline: 'Behind Every Great American Company is Great Talent' },
  { bg: heroSlide2, tagline: 'Connecting Top Talent with Leading Employers Across North America' },
  { bg: heroSlide3, tagline: 'Your Workforce Solution — From Onboarding to Payroll, Fully Connected' },
]

const { currentIndex, currentItem } = useCarousel(slides, { intervalMs: 5000 })
</script>

<style scoped>
/* ── Section shell — full screen ───────────────────────────────────────────── */
.hero-v2 {
  position: relative;
  width: 100%;
  height: 100vh;
  min-height: 720px;
  overflow: hidden;
  isolation: isolate;
}

/* ── Slide backgrounds ─────────────────────────────────────────────────────── */
.hero-v2__bg {
  position: absolute;
  inset: 0;
  opacity: 0;
  transition: opacity 1s ease;
  pointer-events: none;
  z-index: 0;
}

.hero-v2__bg--active { opacity: 1; }

.hero-v2__bg-img {
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  object-fit: cover;
  object-position: center center;
}

.hero-v2__bg-color {
  position: absolute;
  inset: 0;
}

/* ── Downward gradient — heavy navy at bottom flows into DualCta ──────────── */
.hero-v2__overlay {
  position: absolute;
  inset: 0;
  z-index: 1;
  pointer-events: none;
  background:
    linear-gradient(90deg, rgba(15, 47, 68, 0.30) 0%, rgba(15, 47, 68, 0.30) 100%),
    linear-gradient(180deg,
      rgba(15, 47, 68, 0.10) 0%,
      rgba(15, 47, 68, 0.30) 35%,
      rgba(15, 47, 68, 0.75) 75%,
      rgba(15, 47, 68, 0.98) 100%
    );
}

/* ── Decorative cyan glow — tertiary accent, bottom-right ─────────────────── */
.hero-v2__glow {
  position: absolute;
  right: -200px;
  bottom: -200px;
  width: 720px;
  height: 720px;
  background: var(--c-brand-cyan);
  border-radius: 50%;
  filter: blur(180px);
  opacity: 0.22;
  z-index: 1;
  pointer-events: none;
}

/* ── Magnifier position (component owns the size + animation) ─────────────── */
.hero-v2__magnifier {
  top: 24%;
  left: 6%;
}

/* ── Content — vertically centered ────────────────────────────────────────── */
.hero-v2__content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  /* Bottom padding shifts the centered block upward, clearing the DualCta overlap (140px).
     Caps at 32vh so short viewports (e.g. 1024×768) don't squeeze content off-screen. */
  padding: 80px 24px min(300px, 32vh);
}

.hero-v2__logo {
  /* Caps at 32vh so the logo stays in-frame on short viewports */
  width: min(320px, 32vh);
  height: min(320px, 32vh);
  object-fit: contain;
  object-position: center;
  display: block;
}

/* ── Tagline — larger, modern ─────────────────────────────────────────────── */
.hero-v2__tagline {
  margin-top: 28px;
  font-family: var(--font-family);
  font-size: 32px;
  font-weight: 500;
  line-height: 1.3;
  color: #fff;
  text-align: center;
  max-width: 720px;
  letter-spacing: -0.01em;
  text-shadow: 0 2px 16px rgba(0, 0, 0, 0.35);
}

/* Tagline fade transition */
.hero-tag-enter-active,
.hero-tag-leave-active {
  transition: opacity 0.4s ease, transform 0.4s ease;
}
.hero-tag-enter-from { opacity: 0; transform: translateY(8px); }
.hero-tag-leave-to   { opacity: 0; transform: translateY(-8px); }

/* CTA + dots spacing (visual styling lives in their components) */
.hero-v2__cta  { margin-top: 36px; }
.hero-v2__dots { margin-top: 40px; }

/* ── Mobile ────────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .hero-v2 {
    height: 100svh;
    min-height: 600px;
  }

  .hero-v2__content {
    padding: 80px 24px 200px;
  }

  .hero-v2__logo {
    width: 220px;
    height: 220px;
  }

  .hero-v2__tagline {
    margin-top: 20px;
    font-size: 20px;
    max-width: 340px;
  }

  .hero-v2__cta  { margin-top: 24px; }
  .hero-v2__dots { margin-top: 28px; }

  .hero-v2__glow {
    width: 420px;
    height: 420px;
    filter: blur(140px);
    opacity: 0.18;
    right: -120px;
    bottom: -120px;
  }

  .hero-v2__magnifier {
    top: 20%;
    left: 5%;
  }
}
</style>
