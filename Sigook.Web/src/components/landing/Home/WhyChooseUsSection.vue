<template>
  <section
    ref="sectionRef"
    class="why"
    :class="{ 'is-visible': visible }"
  >
    <!-- Back layer — same shape as the photo, sits 20px above so it peeks out at the top -->
    <div class="why__hero-back" aria-hidden="true"></div>

    <!-- Block A — Corporate buildings photo -->
    <div class="why__hero">
      <img src="@/assets/images/v2/why-choose-us/why-bg.webp" alt="" class="why__hero-bg" aria-hidden="true" loading="lazy" decoding="async" />
      <div class="why__hero-overlay" aria-hidden="true"></div>

      <!-- Decorative cyan glow + brand magnifier -->
      <div class="why__hero-glow" aria-hidden="true"></div>
      <img
        src="@/assets/images/v2/branding/sigook-magnifier.webp"
        alt=""
        aria-hidden="true"
        class="why__hero-magnifier"
        loading="lazy"
        decoding="async"
      />

      <div class="why__hero-content">
        <div class="why__hero-left">
          <span class="why__hero-eyebrow">Our Approach</span>
          <h2 class="why__hero-title">Why Choose Us?</h2>
          <div class="why__hero-divider" aria-hidden="true"></div>
        </div>
        <div class="why__hero-right">
          <p class="why__hero-body">
            We are a Talent Management Agency dedicated to connecting businesses
            with skilled professionals through customized workforce solutions, 
            dependable support, and a partnership-driven approach
            that helps every operation run smoothly and efficiently.
          </p>
        </div>
      </div>
    </div>

    <!-- Block B — Blue content panel -->
    <div class="why__panel">
      <div class="why__panel-inner">

        <!-- Top zone: asymmetric editorial — heading left, map right -->
        <div class="why__top">
          <header class="why__panel-header">
            <span class="why__panel-eyebrow">What Sets Us Apart</span>
            <h3 class="why__panel-heading">Nationwide reach,<br />local focus</h3>
            <p class="why__panel-sub">
              A network that scales coast-to-coast while staying personally
              invested in every local market we serve.
            </p>
            <div class="why__panel-divider" aria-hidden="true"></div>
          </header>

          <div class="why__map-wrap">
            <span class="why__map-halo" aria-hidden="true"></span>
            <img
              src="@/assets/images/v2/why-choose-us/usa-map.webp"
              alt="United States coverage map"
              class="why__map-img"
              width="1224"
              height="1102"
              loading="lazy"
              decoding="async"
            />
          </div>
        </div>

        <!-- Feature cards — canonical SecondaryCard instances -->
        <div class="why__features">
          <SecondaryCard
            variant="blue"
            eyebrow="Network"
            title="Coast-to-coast reach"
            :list="NETWORK_POINTS"
            class="why__feature"
            :delay="300"
          />

          <SecondaryCard
            variant="cyan"
            eyebrow="Local Focus"
            title="Tailored solutions"
            :list="LOCAL_POINTS"
            class="why__feature"
            :delay="440"
          />

          <SecondaryCard
            variant="red"
            eyebrow="Leadership"
            title="Trusted innovators"
            :list="LEADERSHIP_POINTS"
            class="why__feature"
            :delay="580"
          />
        </div>

      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import SecondaryCard from '@/components/landing/shared/SecondaryCard.vue'

const NETWORK_POINTS = [
  'Recruiters and coverage across all major US markets',
  'From the East Coast to the West Coast — we\'ve got you covered',
  'Fast placements wherever your business operates in the US',
] as const

const LOCAL_POINTS = [
  'Dedicated recruiters who know your local industry and market',
  'Custom hiring strategies built around your specific needs',
  'Ongoing support from first contact to successful placement',
] as const

const LEADERSHIP_POINTS = [
  'Deep expertise in US staffing across skilled trades and professional sectors',
  'Modern tools and processes that speed up hiring decisions',
  'A partner invested in long-term success for workers and employers alike',
] as const

const sectionRef = ref<HTMLElement | null>(null)
const visible = ref(false)
let observer: IntersectionObserver | null = null

onMounted(() => {
  if (!sectionRef.value) return
  // Continuous observer — fade in entering viewport, fade out leaving (matches Numbers)
  observer = new IntersectionObserver(
    (entries) => {
      visible.value = entries[0].isIntersecting
    },
    { threshold: 0.15, rootMargin: '0px 0px -10% 0px' }
  )
  observer.observe(sectionRef.value)
})

onUnmounted(() => observer?.disconnect())
</script>

<style scoped>
.why {
  position: relative;
  z-index: 1;           /* above Numbers (z-index:0) */
  margin-top: -260px;   /* overlaps the bottom 260px of Numbers: Numbers ends at y=2724, WhyChooseUs starts at y=2464 */
}

/* ── Back layer for the hero — peeks 20px above & below, same shape ─────── */
.why__hero-back {
  position: absolute;
  top: -20px;
  left: 0;
  width: 100%;
  height: 628px;
  border-radius: 150px 0 150px 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: blur(10px) saturate(120%);
  -webkit-backdrop-filter: blur(10px) saturate(120%);
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow: 0 18px 40px -20px rgba(0, 0, 0, 0.4);
  z-index: 1;
  pointer-events: none;
}

/* ── Block A: Photo ── */
.why__hero {
  position: relative;
  z-index: 2;       /* above the blue panel (z-index:1) — matches Figma layer order:
                       corporate buildings 01 1 (node 425:5791) is listed after
                       Rectangle 16 (the panel) in Figma, meaning it renders on top */
  width: 100%;
  height: 588px;
  border-radius: 150px 0 150px 0;
  overflow: hidden;
  /* Soft drop shadows top + bottom — smooth transitions with Numbers (above) and panel (below) */
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
}

.why__hero-bg {
  position: absolute;
  width: 100%;
  height: 200%;
  top: -30%;
  left: 0;
  object-fit: cover;
  object-position: center 30%;
}

/* Navy gradient overlay — replaces the green-tinted one, matches DualCta veil language */
.why__hero-overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(90deg, rgba(15, 47, 68, 0.78) 0%, rgba(15, 47, 68, 0.45) 100%),
    linear-gradient(180deg, rgba(9, 48, 85, 0.20) 0%, rgba(15, 47, 68, 0.55) 100%);
}

/* Cyan glow accent — tertiary color, top-right corner */
.why__hero-glow {
  position: absolute;
  top: -80px;
  right: -120px;
  width: 540px;
  height: 540px;
  background: var(--c-brand-cyan);
  border-radius: 50%;
  filter: blur(160px);
  opacity: 0.24;
  z-index: 1;
  pointer-events: none;
}

/* Brand magnifier decoration — floats with subtle sway */
.why__hero-magnifier {
  position: absolute;
  bottom: 48px;
  left: 56px;
  width: 88px;
  height: 88px;
  z-index: 1;
  pointer-events: none;
  filter: drop-shadow(0 8px 16px rgba(0, 0, 0, 0.30));
  animation: why-magnifier-float 6.5s ease-in-out infinite;
  will-change: transform;
}

@keyframes why-magnifier-float {
  0%, 100% { transform: translate(0, 0) rotate(-6deg); }
  25%      { transform: translate(6px, -8px) rotate(4deg); }
  50%      { transform: translate(0, -14px) rotate(8deg); }
  75%      { transform: translate(-6px, -8px) rotate(-4deg); }
}

@media (prefers-reduced-motion: reduce) {
  .why__hero-magnifier { animation: none; }
}

.why__hero-content {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  height: 100%;
  padding: 0 80px;
  gap: 94px;
}

.why__hero-left {
  flex: 0 0 480px;
  text-align: right;
}

.why__hero-eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 14px;
}

.why__hero-title {
  font-family: var(--font-family);
  font-size: 60px;
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0;
}

/* Cyan divider line under title — right-aligned to match text */
.why__hero-divider {
  width: 88px;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
  margin: 24px 0 0 auto;
}

.why__hero-right {
  flex: 1;
  display: flex;
  justify-content: flex-end;
}

.why__hero-body {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.92);
  margin: 0;
  max-width: 560px;
}

/* ── Block B: Blue panel — transparent (background lives in GlobalBackground) ── */
.why__panel {
  position: relative;
  border-radius: 150px 0 150px 0;
  margin-top: -196px;
  z-index: 1;
  padding-bottom: 160px;
  overflow: hidden;
  /* Sized so feature cards clear Certified's -556px overlap with a comfortable buffer */
  min-height: 1880px;
}

.why__panel-inner {
  position: relative;
  max-width: 1280px;
  margin: 0 auto;
  padding: 280px 80px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  z-index: 1;
}

/* ── Top zone — asymmetric editorial split (heading left, map right) ─── */
.why__top {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  gap: 80px;
  width: 100%;
  margin-bottom: 96px;
}

/* Panel header — text block on the left */
.why__panel-header {
  flex: 1;
  position: relative;
  z-index: 2;
  text-align: left;
  max-width: 520px;
  margin: 0;
}

.why__panel-eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 16px;
}

.why__panel-heading {
  font-family: var(--font-family);
  font-size: 52px;
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0 0 20px;
}

.why__panel-sub {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 28px;
  max-width: 460px;
}

/* Cyan divider under sub — closes the heading block, matches Hero divider language */
.why__panel-divider {
  width: 88px;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
}

/* ── USA Map — right side of the split ───────────────────────────────── */
.why__map-wrap {
  flex: 1;
  position: relative;
  z-index: 2;
  display: flex;
  justify-content: center;
  align-items: center;
  margin: 0;
  /* Fade in/out with viewport (matches Numbers behavior) */
  opacity: 0;
  transform: translateY(30px) scale(0.96);
  transition:
    opacity 0.8s ease-out,
    transform 0.8s cubic-bezier(0.22, 1, 0.36, 1);
}

.why.is-visible .why__map-wrap {
  opacity: 1;
  transform: translateY(0) scale(1);
  transition-delay: 0.10s;
}

.why__map-halo {
  position: absolute;
  width: 100%;
  height: 100%;
  background: radial-gradient(ellipse at center,
    rgba(0, 173, 239, 0.45) 0%,
    rgba(0, 173, 239, 0.12) 40%,
    transparent 70%);
  filter: blur(60px);
  pointer-events: none;
  z-index: -1;
}

.why__map-img {
  position: relative;
  width: 100%;
  max-width: 600px;
  height: auto;
  display: block;
  filter: drop-shadow(0 24px 48px rgba(0, 0, 0, 0.30));
}

/* ── Feature cards row — SecondaryCard instances in a flex row ────────── */
.why__features {
  position: relative;
  z-index: 2;
  display: flex;
  gap: 28px;
  justify-content: center;
  align-items: stretch;
  flex-wrap: wrap;
  width: 100%;
}

.why__feature {
  flex: 1 1 280px;
  max-width: 360px;
}

/* ── Mobile ── */
@media (max-width: 1023px) {
  .why {
    margin-top: -100px;  /* overlaps bottom 100px of Numbers background (z-index:1 already set) */
  }

  .why__hero-back {
    display: none;       /* skip on mobile — overlap math + smaller card make it noisy */
  }

  .why__hero {
    height: auto;
    min-height: 280px;
    border-radius: 80px 0 80px 0;
    padding: 60px 24px;
    /* Cap large drop-shadow blur for mobile GPU */
    box-shadow:
      0 -22px 24px -12px rgba(0, 0, 0, 0.50),
      0  22px 24px -12px rgba(0, 0, 0, 0.50);
  }

  .why__hero-bg {
    height: 100%;
    top: 0;
  }

  .why__hero-content {
    flex-direction: column;
    align-items: flex-start;
    padding: 0;
    height: auto;
    gap: 20px;
  }

  .why__hero-glow {
    width: 320px;
    height: 320px;
    top: -60px;
    right: -100px;
    filter: blur(40px);
    opacity: 0.20;
  }

  .why__hero-magnifier {
    width: 56px;
    height: 56px;
    bottom: 20px;
    left: 20px;
    animation: none;
  }

  .why__hero-left {
    flex: none;
    width: 100%;
    text-align: left;
  }

  .why__hero-eyebrow {
    margin-bottom: 10px;
  }

  .why__hero-title {
    font-size: 36px;
  }

  .why__hero-divider {
    margin: 16px 0 0 0;       /* left-aligned on mobile to match left-aligned text */
    width: 64px;
  }

  .why__hero-right {
    width: 100%;
    justify-content: flex-start;
  }

  .why__hero-body {
    font-size: 15px;
  }

  .why__panel {
    border-radius: 80px 0 80px 0;
    margin-top: -80px;
    min-height: 0; /* reset desktop min-height — mobile stacks naturally */
    padding-bottom: 220px; /* extends blue downward — certified section overlaps this extra space */
  }

  .why__panel-inner {
    padding: 140px 24px 0;
  }

  /* Top zone — stack vertically on mobile */
  .why__top {
    flex-direction: column;
    gap: 40px;
    margin-bottom: 56px;
  }

  .why__panel-header {
    text-align: center;
    max-width: 100%;
  }

  .why__panel-heading {
    font-size: 32px;
  }

  .why__panel-sub {
    font-size: 15px;
    margin-left: auto;
    margin-right: auto;
  }

  .why__panel-divider {
    margin: 0 auto;
  }

  /* Map */
  .why__map-wrap {
    width: 100%;
  }

  .why__map-halo {
    width: 110%;
    height: 80%;
    filter: blur(40px);
  }

  /* Feature cards stack on mobile */
  .why__features {
    flex-direction: column;
    gap: 18px;
    align-items: center;
  }

  .why__feature {
    width: 100%;
    max-width: 360px;
  }
}
</style>
