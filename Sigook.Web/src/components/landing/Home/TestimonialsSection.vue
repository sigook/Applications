<template>
  <section
    class="testimonials"
    @mouseenter="stopTimer"
    @mouseleave="startTimer"
    @focusin="stopTimer"
    @focusout="startTimer"
  >
    <div class="testimonials__back" aria-hidden="true"></div>

    <div
      v-for="(slide, i) in testimonials"
      :key="i"
      class="testimonials__bg"
      :class="{ 'testimonials__bg--active': currentSlide === i }"
      aria-hidden="true"
    >
      <img v-if="slide.bg" :src="slide.bg" alt="" class="testimonials__bg-img" loading="lazy" decoding="async" />
      <div v-else class="testimonials__bg-color" :style="{ background: slide.gradient }"></div>
      <div class="testimonials__overlay"></div>
    </div>

    <div class="testimonials__glow" aria-hidden="true"></div>
    <DecoMagnifier class="testimonials__magnifier" />

    <div class="testimonials__inner">
      <header class="testimonials__header">
        <span class="testimonials__eyebrow">Testimonials</span>
        <h2 class="testimonials__title">What our clients say</h2>
        <div class="testimonials__divider" aria-hidden="true"></div>
      </header>

      <transition name="test-fade" mode="out-in">
        <article :key="currentSlide" class="testimonials__card">
          <img
            src="@/assets/images/landing/testimonials/testimonials-quote-mark.png"
            alt=""
            aria-hidden="true"
            class="testimonials__quote-mark"
            loading="lazy"
            decoding="async"
          />
          <p class="testimonials__quote">{{ testimonials[currentSlide].quote }}</p>
          <div class="testimonials__attribution">
            <p class="testimonials__author">{{ testimonials[currentSlide].author }}</p>
            <p class="testimonials__location">{{ testimonials[currentSlide].location }}</p>
          </div>
        </article>
      </transition>

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
import DecoMagnifier from '@/components/landing/shared/hero/DecoMagnifier.vue'

interface Testimonial {
  bg: string
  gradient: string
  quote: string
  author: string
  location: string
}

const testimonials: readonly Testimonial[] = [
  {
    bg: '/images/landing/testimonials/testimonials-slide1.webp',
    gradient: '',
    quote:
      '"I recommend Sigook Work Factory as an exceptional and reliable employment agency. Our company has been partnering with them since July 2020, and their service has been consistently outstanding."',
    author: 'HR Manager, Manufacturer',
    location: 'Doral, Florida',
  },
  {
    bg: '/images/landing/testimonials/testimonials-slide2.webp',
    gradient: '',
    quote:
      '"Sigook transformed how we manage seasonal staffing. Their team is responsive, professional, and always delivers the right talent at the right time. Highly recommended."',
    author: 'Business Owner, Retail',
    location: 'Seattle, WA',
  },
  {
    bg: '/images/landing/testimonials/testimonials-slide3.webp',
    gradient: '',
    quote:
      '"From onboarding to invoicing, the entire process is seamless. Sigook is not just a staffing agency — they are a true workforce partner."',
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

function prefersReducedMotion(): boolean {
  return (
    typeof window !== 'undefined' &&
    typeof window.matchMedia === 'function' &&
    window.matchMedia('(prefers-reduced-motion: reduce)').matches
  )
}

function stopTimer() {
  if (timer) {
    clearInterval(timer)
    timer = null
  }
}

function startTimer() {
  stopTimer()
  if (prefersReducedMotion()) return
  timer = setInterval(nextSlide, 6000)
}

function resetTimer() {
  startTimer()
}

onMounted(startTimer)
onUnmounted(stopTimer)
</script>

<style scoped>
.testimonials {
  position: relative;
  height: 1460px;
  background: transparent;
  margin-top: -700px;
  z-index: 3;
  isolation: isolate;
}

.testimonials__back {
  position: absolute;
  top: 520px;
  bottom: -20px;
  left: 0;
  right: 0;
  z-index: 0;
  border-radius: 0 var(--r-brand) 0 var(--r-brand);
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: var(--glass-blur-soft);
  -webkit-backdrop-filter: var(--glass-blur-soft);
  border: 1px solid var(--c-glass-border-soft);
  box-shadow: var(--sh-back);
  pointer-events: none;
}

.testimonials__bg {
  position: absolute;
  top: 540px;
  left: 0;
  right: 0;
  bottom: 0;
  opacity: 0;
  transition: opacity 1s ease;
  pointer-events: none;
  border-radius: 0 var(--r-brand) 0 var(--r-brand);
  overflow: hidden;
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

.testimonials__overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(90deg, rgba(15, 47, 68, 0.55) 0%, rgba(15, 47, 68, 0.30) 50%, rgba(15, 47, 68, 0.55) 100%),
    linear-gradient(180deg, rgba(15, 47, 68, 0.45) 0%, rgba(15, 47, 68, 0.70) 100%);
}

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

.testimonials__magnifier {
  z-index: 2;
  bottom: 170px;
  left: 7%;
}

.testimonials__inner {
  position: relative;
  z-index: 3;
  height: 1460px;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 var(--gutter-desktop);
}

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
    var(--c-glass-fill-soft) 100%
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
  filter: brightness(0) invert(1);  
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

.test-fade-enter-active,
.test-fade-leave-active {
  transition: opacity 0.45s ease, transform 0.45s var(--ease-brand);
}
.test-fade-enter-from {
  opacity: 0;
  transform: translateX(-50%) translateY(12px);
}
.test-fade-leave-to {
  opacity: 0;
  transform: translateX(-50%) translateY(-12px);
}

.testimonials__dots {
  position: absolute;
  bottom: 80px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  gap: 10px;
  background: var(--c-glass-fill-strong);
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
    box-shadow:
      0 -22px 24px -12px rgba(0, 0, 0, 0.45),
      0  22px 24px -12px rgba(0, 0, 0, 0.45);
  }

  .testimonials__glow {
    top: 60px;
    right: -80px;
    width: 320px;
    height: 320px;
    filter: blur(40px);
    opacity: 0.20;
  }

  .testimonials__magnifier {
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
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    background: linear-gradient(135deg,
      rgba(255, 255, 255, 0.26) 0%,
      rgba(255, 255, 255, 0.18) 100%
    );
    box-shadow: 0 18px 24px rgba(0, 0, 0, 0.30);
  }

  .testimonials__quote {
    font-size: 15px;
  }

  .test-fade-enter-from,
  .test-fade-leave-to {
    transform: translateY(10px);
  }

  .test-fade-leave-to {
    transform: translateY(-10px);
  }

  .testimonials__dots {
    position: static;
    transform: none;
    margin: 32px auto 0;
    width: fit-content;
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    background: rgba(255, 255, 255, 0.24);
  }
}
</style>
