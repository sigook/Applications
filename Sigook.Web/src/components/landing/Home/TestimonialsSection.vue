<template>
  <section class="testimonials">
    <!-- Depth back-layer — transparent, peeks ~20px above & below the slide bg -->
    <div class="testimonials__back" aria-hidden="true"></div>

    <!-- Slide backgrounds — fade between them, start at the visible zone -->
    <div
      v-for="(slide, i) in testimonials"
      :key="i"
      class="testimonials__bg"
      :class="{ 'testimonials__bg--active': currentSlide === i }"
      aria-hidden="true"
    >
      <img v-if="slide.bg" :src="slide.bg" alt="" class="testimonials__bg-img" />
      <div v-else class="testimonials__bg-color" :style="{ background: slide.gradient }"></div>
      <div class="testimonials__overlay"></div>
    </div>

    <!-- Decorative cyan glow + brand magnifier -->
    <div class="testimonials__glow" aria-hidden="true"></div>
    <img
      src="@/assets/images/v2/branding/sigook-magnifier.png"
      alt=""
      aria-hidden="true"
      class="testimonials__magnifier"
    />

    <div class="testimonials__inner">
      <!-- Header — eyebrow + heading + cyan divider (matches Numbers / WhyChooseUs) -->
      <header class="testimonials__header">
        <span class="testimonials__eyebrow">Testimonials</span>
        <h2 class="testimonials__title">What our clients say</h2>
        <div class="testimonials__divider" aria-hidden="true"></div>
      </header>

      <!-- Animated glass quote card -->
      <transition name="test-fade" mode="out-in">
        <article :key="currentSlide" class="testimonials__card">
          <img
            src="@/assets/images/v2/testimonials/testimonials-quote-mark.png"
            alt=""
            aria-hidden="true"
            class="testimonials__quote-mark"
          />
          <p class="testimonials__quote">{{ testimonials[currentSlide].quote }}</p>
          <div class="testimonials__attribution">
            <p class="testimonials__author">{{ testimonials[currentSlide].author }}</p>
            <p class="testimonials__location">{{ testimonials[currentSlide].location }}</p>
          </div>
        </article>
      </transition>

      <!-- Glass pill dots (matches Hero) -->
      <div class="testimonials__dots" role="tablist" aria-label="Testimonials navigation">
        <button
          v-for="(_, i) in testimonials"
          :key="i"
          class="testimonials__dot"
          :class="{ 'testimonials__dot--active': currentSlide === i }"
          @click="goToSlide(i)"
          :aria-label="`Testimonial ${i + 1}`"
          :aria-selected="currentSlide === i"
          role="tab"
        ></button>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import slide1Bg from '@/assets/images/v2/testimonials/testimonials-slide1.jpg'
import slide2Bg from '@/assets/images/v2/testimonials/testimonials-slide2.jpg'
import slide3Bg from '@/assets/images/v2/testimonials/testimonials-slide3.jpg'

const testimonials = [
  {
    bg: slide1Bg,
    gradient: '',
    quote: '"I recommend Sigook Work Factory as an exceptional and reliable employment agency. Our company has been partnering with them since July 2020, and their service has been consistently outstanding."',
    author: 'HR Manager, Manufacturer',
    location: 'Doral, Florida',
  },
  {
    bg: slide2Bg,
    gradient: '',
    quote: '"Sigook transformed how we manage seasonal staffing. Their team is responsive, professional, and always delivers the right talent at the right time. Highly recommended."',
    author: 'Business Owner, Retail',
    location: 'Seattle, WA',
  },
  {
    bg: slide3Bg,
    gradient: '',
    quote: '"From onboarding to invoicing, the entire process is seamless. Sigook is not just a staffing agency — they are a true workforce partner."',
    author: 'Operations Manager, Logistics',
    location: 'Atlanta, GA',
  },
]

const currentSlide = ref(0)
let timer: ReturnType<typeof setInterval> | null = null

function goToSlide(index: number) {
  currentSlide.value = index
  resetTimer()
}

function nextSlide() {
  currentSlide.value = (currentSlide.value + 1) % testimonials.length
}

function startTimer() {
  timer = setInterval(nextSlide, 6000)
}

function resetTimer() {
  if (timer) clearInterval(timer)
  startTimer()
}

onMounted(startTimer)
onUnmounted(() => { if (timer) clearInterval(timer) })
</script>

<style scoped>
/*
  Geometry (desktop):
  ─────────────────────────────────────────────────────────────
  AppDownload height          = 1380px
  Stores pill bottom          = 940 + 52 = 992px  (from AppDL top)
  Target bg start             = 992 + 60 = 1052px (from AppDL top)

  testimonials margin-top     = -700px
    → section top             = 1380 - 700 = 680px (from AppDL top)
  bg_offset                   = 1052 - 680 = 372px (inside section)
    → bg starts at AppDL y    = 680 + 372 = 1052  ✓

  Transparent zone (shows AppDownload through): 0 – 372px within section
  Blue zone (covers AppDownload + extends below): 372px – 1460px within section
  Visible testimonials below AppDownload end: 1460 - 700 = 760px
  ─────────────────────────────────────────────────────────────
*/

/* ── Section shell — preserves AppDownload overlap geometry ──────────────── */
.testimonials {
  position: relative;
  height: 1460px;
  /* No overflow:hidden — lets the bg shadow extend down into Contact for the smooth transition.
     The bg has its own overflow:hidden so its photo still clips to the rounded corners. */
  background: transparent;
  margin-top: -700px;
  z-index: 3;
  isolation: isolate;
}

/* ── Depth back-layer — transparent glass, peeks ~20px above & below the
   slide bg (same brand mirror shape: top-right + bottom-left rounded) ────── */
.testimonials__back {
  position: absolute;
  top: 520px;
  bottom: -20px;
  left: 0;
  right: 0;
  z-index: 0;
  border-radius: 0 var(--r-brand) 0 var(--r-brand);
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: blur(10px) saturate(120%);
  -webkit-backdrop-filter: blur(10px) saturate(120%);
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow: 0 18px 40px -20px rgba(0, 0, 0, 0.4);
  pointer-events: none;
}

/* ── Slide backgrounds — start at y=540 so top is transparent (AppDL bleeds through),
       bottom carries the photo + navy veil. Same geometry as before. */
.testimonials__bg {
  position: absolute;
  top: 540px;
  left: 0;
  right: 0;
  bottom: 0;
  opacity: 0;
  transition: opacity 1s ease;
  pointer-events: none;
  /* Asymmetric brand shape — top-right + bottom-left rounded */
  border-radius: 0 var(--r-brand) 0 var(--r-brand);
  overflow: hidden;
  /* Soft shadows top + bottom — reinforces overlap with AppDownload (above) and Contact (below) */
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
}

.testimonials__bg--active {
  opacity: 1;
}

.testimonials__bg-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center top;
}

.testimonials__bg-color {
  position: absolute;
  inset: 0;
}

/* Heavy navy gradient overlay — matches DualCta veil language */
.testimonials__overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(90deg, rgba(15, 47, 68, 0.55) 0%, rgba(15, 47, 68, 0.30) 50%, rgba(15, 47, 68, 0.55) 100%),
    linear-gradient(180deg, rgba(15, 47, 68, 0.45) 0%, rgba(15, 47, 68, 0.70) 100%);
}

/* ── Decorative cyan glow (Hero language) — top-right of visible blue zone ── */
.testimonials__glow {
  position: absolute;
  z-index: 1;
  pointer-events: none;
  top: 580px;
  right: -120px;
  width: 560px;
  height: 560px;
  background: var(--c-brand-cyan);
  border-radius: 50%;
  filter: blur(160px);
  opacity: 0.22;
}

/* ── Brand magnifier decoration — bottom-left of visible blue zone ─────── */
.testimonials__magnifier {
  position: absolute;
  z-index: 2;
  bottom: 170px;
  left: 7%;
  width: 88px;
  height: 88px;
  pointer-events: none;
  filter: drop-shadow(0 8px 16px rgba(0, 0, 0, 0.30));
  animation: test-magnifier-float 6.5s ease-in-out infinite;
  will-change: transform;
}

@keyframes test-magnifier-float {
  0%, 100% { transform: translate(0, 0) rotate(-6deg); }
  25%      { transform: translate(6px, -8px) rotate(4deg); }
  50%      { transform: translate(0, -14px) rotate(8deg); }
  75%      { transform: translate(-6px, -8px) rotate(-4deg); }
}

@media (prefers-reduced-motion: reduce) {
  .testimonials__magnifier { animation: none; }
}

/* ── Inner container ─────────────────────────────────────────────────────── */
.testimonials__inner {
  position: relative;
  z-index: 3;
  height: 1460px;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 var(--gutter-desktop);
}

/* ── Header — eyebrow + heading + cyan divider centered above quote card ─── */
.testimonials__header {
  position: absolute;
  top: 660px;
  left: 50%;
  transform: translateX(-50%);
  text-align: center;
}

.testimonials__eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 16px;
}

.testimonials__title {
  font-family: var(--font-family);
  font-size: 44px;
  font-weight: 700;
  line-height: 1.1;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0;
}

.testimonials__divider {
  width: 64px;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
  margin: 22px auto 0;
}

/* ── Quote glass card — centered, asymmetric brand radius ─────────────────── */
.testimonials__card {
  position: absolute;
  top: 880px;
  left: 50%;
  transform: translateX(-50%);
  width: 100%;
  max-width: 680px;
  padding: 48px 56px 40px;
  background: linear-gradient(135deg,
    rgba(255, 255, 255, 0.12) 0%,
    rgba(255, 255, 255, 0.04) 100%
  );
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.20);
  border-radius: 24px 64px 24px 64px;
  box-shadow: 0 18px 44px rgba(0, 0, 0, 0.30);
  text-align: center;
}

.testimonials__quote-mark {
  display: block;
  margin: 0 auto 18px;
  width: 36px;
  height: 36px;
  opacity: 0.85;
  filter: brightness(0) invert(1);  /* force white tint regardless of source */
}

.testimonials__quote {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.65;
  color: #fff;
  margin: 0 0 32px;
}

.testimonials__attribution {
  display: inline-block;
  padding-top: 18px;
  border-top: 1px solid rgba(255, 255, 255, 0.20);
}

.testimonials__author {
  font-family: var(--font-family);
  font-size: 14px;
  font-weight: 700;
  letter-spacing: 0.04em;
  line-height: 1.4;
  color: #fff;
  margin: 0 0 4px;
  text-transform: uppercase;
}

.testimonials__location {
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 400;
  line-height: 1.5;
  color: rgba(255, 255, 255, 0.75);
  margin: 0;
}

/* Card fade transition */
.test-fade-enter-active,
.test-fade-leave-active {
  transition: opacity 0.45s ease, transform 0.45s cubic-bezier(0.22, 1, 0.36, 1);
}
.test-fade-enter-from {
  opacity: 0;
  transform: translateX(-50%) translateY(12px);
}
.test-fade-leave-to {
  opacity: 0;
  transform: translateX(-50%) translateY(-12px);
}

/* ── Glass pill dots — matches Hero exactly ──────────────────────────────── */
.testimonials__dots {
  position: absolute;
  bottom: 80px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  gap: 10px;
  background: rgba(255, 255, 255, 0.10);
  backdrop-filter: blur(12px) saturate(150%);
  -webkit-backdrop-filter: blur(12px) saturate(150%);
  border: 1px solid rgba(255, 255, 255, 0.25);
  padding: 8px 14px;
  border-radius: 999px;
}

.testimonials__dot {
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

.testimonials__dot--active {
  width: 10px;
  height: 10px;
  background-color: #fff;
}

/* ── Mobile ────────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .testimonials {
    height: auto;
    min-height: 500px;
    padding-bottom: 56px;
    margin-top: 0;
    background: transparent;
  }

  .testimonials__back { display: none; }

  .testimonials__bg {
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
  }

  .testimonials__glow {
    top: 60px;
    right: -80px;
    width: 320px;
    height: 320px;
    filter: blur(110px);
    opacity: 0.20;
  }

  .testimonials__magnifier {
    width: 56px;
    height: 56px;
    bottom: 20px;
    left: 20px;
  }

  .testimonials__inner {
    height: auto;
    padding: 0 var(--gutter-mobile);
  }

  .testimonials__header {
    position: static;
    transform: none;
    padding-top: 80px;
    margin-bottom: 32px;
  }

  .testimonials__title {
    font-size: 28px;
  }

  .testimonials__card {
    position: static;
    transform: none;
    max-width: 100%;
    padding: 36px 28px 32px;
    backdrop-filter: blur(14px) saturate(150%);
    -webkit-backdrop-filter: blur(14px) saturate(150%);
  }

  .testimonials__quote {
    font-size: 15px;
  }

  .test-fade-enter-from,
  .test-fade-leave-to {
    transform: translateY(10px);   /* reset translateX(-50%) for static mobile layout */
  }

  .test-fade-leave-to {
    transform: translateY(-10px);
  }

  .testimonials__dots {
    position: static;
    transform: none;
    margin: 32px auto 0;
    width: fit-content;
  }
}
</style>
