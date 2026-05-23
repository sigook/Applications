<template>
  <section class="hero-v2">
    <!-- Slide backgrounds — fade between them -->
    <div
      v-for="(slide, i) in slides"
      :key="i"
      class="hero-v2__bg"
      :class="{ 'hero-v2__bg--active': currentSlide === i }"
      aria-hidden="true"
    >
      <img v-if="slide.bg" :src="slide.bg" alt="" class="hero-v2__bg-img" />
      <div class="hero-v2__bg-color" :style="{ background: slide.gradient }" v-if="!slide.bg"></div>
    </div>

    <!-- Downward gradient overlay — strengthens as it approaches DualCta -->
    <div class="hero-v2__overlay" aria-hidden="true"></div>

    <!-- Decorative cyan glow (tertiary accent, bottom-right) -->
    <div class="hero-v2__glow" aria-hidden="true"></div>

    <!-- Decorative brand magnifier — replaces the previous thin lines -->
    <img
      src="@/assets/images/v2/branding/sigook-magnifier.png"
      alt=""
      aria-hidden="true"
      class="hero-v2__magnifier"
    />

    <!-- Static content (shared across all slides) -->
    <div class="hero-v2__content">
      <img
        src="@/assets/images/v2/hero/hero-logo.png"
        alt="Sigook Work Factory"
        class="hero-v2__logo"
      />

      <!-- Animated tagline -->
      <transition name="hero-tag" mode="out-in">
        <p :key="currentSlide" class="hero-v2__tagline">
          {{ slides[currentSlide].tagline }}
        </p>
      </transition>

      <router-link to="/v2/about" class="hero-v2__cta">
        <span>Learn More</span>
      </router-link>

      <!-- Glass pill carousel dots -->
      <div class="hero-v2__dots" role="tablist" aria-label="Carousel navigation">
        <button
          v-for="(_, i) in slides"
          :key="i"
          class="hero-v2__dot"
          :class="{ 'hero-v2__dot--active': currentSlide === i }"
          @click="goToSlide(i)"
          :aria-label="`Slide ${i + 1}`"
          :aria-selected="currentSlide === i"
          role="tab"
        ></button>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import heroSlide1 from '@/assets/images/v2/hero/hero-slide1.jpg'
import heroSlide2 from '@/assets/images/v2/hero/hero-slide2.jpg'
import heroSlide3 from '@/assets/images/v2/hero/hero-slide3.jpg'

const slides = [
  {
    bg: heroSlide1,
    gradient: '',
    tagline: 'Behind Every Great American Company is Great Talent',
  },
  {
    bg: heroSlide2,
    gradient: '',
    tagline: 'Connecting Top Talent with Leading Employers Across North America',
  },
  {
    bg: heroSlide3,
    gradient: '',
    tagline: 'Your Workforce Solution — From Onboarding to Payroll, Fully Connected',
  },
]

const currentSlide = ref(0)
let timer: ReturnType<typeof setInterval> | null = null

function goToSlide(index: number) {
  currentSlide.value = index
  resetTimer()
}

function nextSlide() {
  currentSlide.value = (currentSlide.value + 1) % slides.length
}

function startTimer() {
  timer = setInterval(nextSlide, 5000)
}

function resetTimer() {
  if (timer) clearInterval(timer)
  startTimer()
}

onMounted(startTimer)
onUnmounted(() => { if (timer) clearInterval(timer) })
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

.hero-v2__bg--active {
  opacity: 1;
}

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

/* ── Decorative thin lines — subtle geometric accent ──────────────────────── */
.hero-v2__magnifier {
  position: absolute;
  top: 24%;
  left: 6%;
  width: 88px;
  height: 88px;
  z-index: 1;
  pointer-events: none;
  filter: drop-shadow(0 8px 16px rgba(0, 0, 0, 0.30));
  animation: magnifier-float 6.5s ease-in-out infinite;
  will-change: transform;
}

@keyframes magnifier-float {
  0%, 100% { transform: translate(0, 0) rotate(-6deg); }
  25%      { transform: translate(6px, -8px) rotate(4deg); }
  50%      { transform: translate(0, -14px) rotate(8deg); }
  75%      { transform: translate(-6px, -8px) rotate(-4deg); }
}

@media (prefers-reduced-motion: reduce) {
  .hero-v2__magnifier { animation: none; }
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

/* tagline fade transition */
.hero-tag-enter-active,
.hero-tag-leave-active {
  transition: opacity 0.4s ease, transform 0.4s ease;
}
.hero-tag-enter-from {
  opacity: 0;
  transform: translateY(8px);
}
.hero-tag-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}

/* ── CTA — glass pill matching DualCta language ───────────────────────────── */
.hero-v2__cta {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  margin-top: 36px;
  padding: 14px 34px;
  border: 1.5px solid rgba(255, 255, 255, 0.85);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(10px) saturate(150%);
  -webkit-backdrop-filter: blur(10px) saturate(150%);
  color: #fff;
  font-family: var(--font-family);
  font-size: 15px;
  font-weight: 600;
  letter-spacing: 0.04em;
  text-decoration: none;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.25);
  transition: background 0.3s ease, color 0.3s ease, transform 0.3s ease;
}

.hero-v2__cta:hover,
.hero-v2__cta:focus-visible {
  background: #fff;
  color: var(--c-brand-navy);
  transform: translateY(-2px);
}

/* ── Glass pill carousel dots ─────────────────────────────────────────────── */
.hero-v2__dots {
  display: flex;
  align-items: center;
  gap: 10px;
  background: rgba(255, 255, 255, 0.10);
  backdrop-filter: blur(12px) saturate(150%);
  -webkit-backdrop-filter: blur(12px) saturate(150%);
  border: 1px solid rgba(255, 255, 255, 0.25);
  padding: 8px 14px;
  border-radius: 999px;
  margin-top: 40px;
}

.hero-v2__dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: rgba(255, 255, 255, 0.50);
  flex-shrink: 0;
  border: none;
  padding: 0;
  cursor: pointer;
  transition: width 0.25s ease, height 0.25s ease, background-color 0.25s ease;
}

.hero-v2__dot--active {
  width: 10px;
  height: 10px;
  background-color: #fff;
}

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

  .hero-v2__cta {
    margin-top: 24px;
    padding: 12px 28px;
    font-size: 14px;
  }

  .hero-v2__dots {
    margin-top: 28px;
  }

  .hero-v2__glow {
    width: 420px;
    height: 420px;
    filter: blur(140px);
    opacity: 0.18;
    right: -120px;
    bottom: -120px;
  }

  .hero-v2__magnifier {
    width: 56px;
    height: 56px;
    top: 20%;
    left: 5%;
  }
}
</style>
