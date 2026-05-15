<template>
  <section class="testimonials-v2">
    <!-- Slide backgrounds — fade between them -->
    <div
      v-for="(slide, i) in testimonials"
      :key="i"
      class="testimonials-v2__bg"
      :class="{ 'testimonials-v2__bg--active': currentSlide === i }"
      aria-hidden="true"
    >
      <img :src="slide.bg" alt="" class="testimonials-v2__bg-img" />
      <div class="testimonials-v2__overlay"></div>
    </div>

    <!-- Decorative quotation marks (top-right) -->
    <img
      src="@/assets/images/v2/testimonials-quote-mark.png"
      alt=""
      aria-hidden="true"
      class="testimonials-v2__quote-mark"
    />

    <div class="testimonials-v2__inner">
      <!-- Top-left: section title + divider (static) -->
      <div class="testimonials-v2__header">
        <h2 class="testimonials-v2__title">
          What Our<br />Clients Say
        </h2>
        <div class="testimonials-v2__divider"></div>
      </div>

      <!-- Center: animated quote, author, location -->
      <transition name="test-fade" mode="out-in">
        <div :key="currentSlide" class="testimonials-v2__body">
          <p class="testimonials-v2__quote">{{ testimonials[currentSlide].quote }}</p>
          <p class="testimonials-v2__author">{{ testimonials[currentSlide].author }}</p>
          <p class="testimonials-v2__location">{{ testimonials[currentSlide].location }}</p>
        </div>
      </transition>

      <!-- Clickable SliderDots -->
      <div class="testimonials-v2__dots" role="tablist" aria-label="Testimonials navigation">
        <button
          v-for="(_, i) in testimonials"
          :key="i"
          class="testimonials-v2__dot"
          :class="{ 'testimonials-v2__dot--active': currentSlide === i }"
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
import officeBg from '@/assets/images/v2/testimonials-office.jpg'

const testimonials = [
  {
    bg: officeBg,
    quote: '"I recommend Sigook Work Factory as an exceptional and reliable employment agency. Our company has been partnering with them since July 2020, and their service has been consistently outstanding."',
    author: 'HR Manager, Manufacturer',
    location: 'Doral, Florida',
  },
  {
    bg: officeBg, // placeholder — replace with a different photo when available
    quote: '"Sigook transformed how we manage seasonal staffing. Their team is responsive, professional, and always delivers the right talent at the right time. Highly recommended."',
    author: 'Business Owner, Retail',
    location: 'Vancouver, BC',
  },
  {
    bg: officeBg, // placeholder — replace with a different photo when available
    quote: '"From onboarding to invoicing, the entire process is seamless. Sigook is not just a staffing agency — they are a true workforce partner."',
    author: 'Operations Manager, Logistics',
    location: 'Montréal, QC',
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
/* ── Section shell ─────────────────────────────────────────────────────────── */
.testimonials-v2 {
  position: relative;
  height: 760px;
  overflow: hidden;
  background: var(--c-brand-navy);
  border-radius: 0 var(--r-brand) 0 0;
}

/* ── Slide backgrounds ─────────────────────────────────────────────────────── */
.testimonials-v2__bg {
  position: absolute;
  inset: 0;
  opacity: 0;
  transition: opacity 1s ease;
  pointer-events: none;
}

.testimonials-v2__bg--active {
  opacity: 1;
}

.testimonials-v2__bg-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center top;
}

.testimonials-v2__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    to bottom,
    var(--c-overlay-blue),
    var(--c-brand-blue)
  );
}

/* ── Decorative quotation mark ─────────────────────────────────────────────── */
.testimonials-v2__quote-mark {
  position: absolute;
  top: -10px;
  right: 0;
  width: 45px;
  height: 45px;
  z-index: 1;
  object-fit: contain;
}

/* ── Inner container ───────────────────────────────────────────────────────── */
.testimonials-v2__inner {
  position: relative;
  z-index: 1;
  height: 760px;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 var(--gutter-desktop);
}

/* ── Header (top-left, static) ─────────────────────────────────────────────── */
.testimonials-v2__header {
  position: absolute;
  top: 100px;
  left: var(--gutter-desktop);
}

.testimonials-v2__title {
  color: var(--c-bg);
  font-size: 30px;
  font-weight: 700;
  line-height: 1.15;
  text-transform: none;
  margin: 0;
}

.testimonials-v2__divider {
  width: 36px;
  height: 2px;
  background: var(--c-bg);
  margin-top: 16px;
}

/* ── Quote body (centered, animated) ──────────────────────────────────────── */
.testimonials-v2__body {
  position: absolute;
  top: 270px;
  left: 50%;
  transform: translateX(-50%);
  width: 380px;
  text-align: center;
}

.testimonials-v2__quote {
  color: var(--c-bg);
  font-size: 15px;
  line-height: 1.6;
  margin: 0 0 24px;
}

.testimonials-v2__author {
  color: var(--c-bg);
  font-size: 15px;
  font-weight: 700;
  line-height: 1.4;
  margin: 0 0 4px;
  text-transform: none;
}

.testimonials-v2__location {
  color: var(--c-bg);
  font-size: 12px;
  line-height: 1.5;
  margin: 0;
}

/* Quote fade transition */
.test-fade-enter-active,
.test-fade-leave-active {
  transition: opacity 0.4s ease, transform 0.4s ease;
}
.test-fade-enter-from {
  opacity: 0;
  transform: translateY(10px);
}
.test-fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

/* ── SliderDots (bottom-center, clickable) ─────────────────────────────────── */
.testimonials-v2__dots {
  position: absolute;
  bottom: 72px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  gap: 8px;
  background: var(--c-bg);
  border: 1px solid rgba(170, 206, 220, 0.3);
  border-radius: var(--r-pill);
  padding: 4px 10px;
}

.testimonials-v2__dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: var(--c-neutral-muted);
  flex-shrink: 0;
  border: none;
  padding: 0;
  cursor: pointer;
  transition: width 0.2s ease, height 0.2s ease, background-color 0.2s ease;
}

.testimonials-v2__dot--active {
  width: 10px;
  height: 10px;
  background: var(--c-brand-blue);
}

/* ── Mobile ────────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .testimonials-v2 {
    height: auto;
    min-height: 500px;
    padding-bottom: 48px;
  }

  .testimonials-v2__inner {
    height: auto;
    padding: 0 var(--gutter-mobile);
  }

  .testimonials-v2__header {
    position: static;
    padding-top: 80px;
  }

  .testimonials-v2__body {
    position: static;
    transform: none;
    width: 100%;
    margin-top: 40px;
  }

  .testimonials-v2__dots {
    position: static;
    transform: none;
    justify-content: center;
    margin: 32px auto 0;
    width: fit-content;
  }
}
</style>
