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
      <div class="hero-v2__overlay"></div>
    </div>

    <!-- Static content (shared across all slides) -->
    <div class="hero-v2__content">
      <img
        src="@/assets/images/v2/hero-logo.png"
        alt="Sigook Work Factory"
        class="hero-v2__logo"
      />

      <!-- Animated tagline -->
      <transition name="hero-tag" mode="out-in">
        <p :key="currentSlide" class="hero-v2__tagline">
          {{ slides[currentSlide].tagline }}
        </p>
      </transition>

      <a href="#" class="btn btn--secondary btn--lg hero-v2__cta" @click.prevent>
        Learn More
      </a>

      <!-- Clickable SliderDots -->
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
import heroImg from '@/assets/images/v2/hero-bg.jpg'

const slides = [
  {
    bg: heroImg,
    gradient: '',
    tagline: 'Behind Every Great American Company is Great Talent',
  },
  {
    bg: '',
    gradient: 'linear-gradient(135deg, #0f2f44 0%, #1575bb 60%, #13629e 100%)',
    tagline: 'Connecting Top Talent with Leading Employers Across North America',
  },
  {
    bg: '',
    gradient: 'linear-gradient(135deg, #13629e 0%, #00adef 60%, #1575bb 100%)',
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
/* ── Section shell ─────────────────────────────────────────────────────────── */
.hero-v2 {
  position: relative;
  width: 100%;
  height: 865px;
  overflow: hidden;
}

/* ── Slide backgrounds ─────────────────────────────────────────────────────── */
.hero-v2__bg {
  position: absolute;
  inset: 0;
  opacity: 0;
  transition: opacity 1s ease;
  pointer-events: none;
}

.hero-v2__bg--active {
  opacity: 1;
}

.hero-v2__bg-img {
  position: absolute;
  width: 100%;
  height: 296%;
  top: -20%;
  left: 0;
  object-fit: cover;
  object-position: center top;
}

.hero-v2__bg-color {
  position: absolute;
  inset: 0;
}

.hero-v2__overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(90deg, rgba(15, 47, 68, 0.35) 0%, rgba(15, 47, 68, 0.35) 100%),
    linear-gradient(180deg, rgba(15, 47, 68, 0) 71.55%, rgb(15, 47, 68) 100%);
}

/* ── Content ───────────────────────────────────────────────────────────────── */
.hero-v2__content {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-top: 240px;
}

.hero-v2__logo {
  width: 260px;
  height: 260px;
  object-fit: contain;
  object-position: center;
  display: block;
}

/* ── Tagline ───────────────────────────────────────────────────────────────── */
.hero-v2__tagline {
  margin-top: 24px;
  font-family: var(--font-family);
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  color: #fff;
  text-align: center;
  max-width: 260px;
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

/* ── CTA ───────────────────────────────────────────────────────────────────── */
.hero-v2__cta {
  margin-top: 28px;
  border-color: var(--c-brand-blue) !important;
  box-shadow: 0 4px 2px rgba(0, 0, 0, 0.25);
}

/* ── SliderDots ────────────────────────────────────────────────────────────── */
.hero-v2__dots {
  display: flex;
  align-items: center;
  gap: 8px;
  background-color: #fff;
  border: 1px solid rgba(170, 206, 220, 0.3);
  padding: 4px 10px;
  border-radius: 30px;
  margin-top: 20px;
}

.hero-v2__dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: var(--c-neutral-muted);
  flex-shrink: 0;
  border: none;
  padding: 0;
  cursor: pointer;
  transition: width 0.2s ease, height 0.2s ease, background-color 0.2s ease;
}

.hero-v2__dot--active {
  width: 10px;
  height: 10px;
  background-color: var(--c-brand-blue);
}

/* ── Mobile ────────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .hero-v2 {
    height: 100svh;
    min-height: 600px;
  }

  .hero-v2__content {
    padding-top: 120px;
    padding-left: 24px;
    padding-right: 24px;
  }

  .hero-v2__logo {
    width: 180px;
    height: 180px;
  }

  .hero-v2__cta {
    margin-top: 20px;
  }

  .hero-v2__tagline {
    margin-top: 16px;
    font-size: 14px;
    max-width: 280px;
  }
}
</style>
