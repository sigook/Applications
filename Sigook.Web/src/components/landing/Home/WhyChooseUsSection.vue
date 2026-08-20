<template>
  <section
    ref="sectionRef"
    class="why"
    :class="{ 'is-visible': visible }"
  >
    <div class="why__hero-back" aria-hidden="true"></div>

    <div class="why__hero">
      <img src="@/assets/images/landing/why-choose-us/why-bg.webp" alt="" class="why__hero-bg" aria-hidden="true" loading="lazy" decoding="async" />
      <div class="why__hero-overlay" aria-hidden="true"></div>

      <div class="why__hero-glow" aria-hidden="true"></div>
      <DecoMagnifier class="why__hero-magnifier" />

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

    <div class="why__panel">
      <div class="why__panel-inner">

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
            <div class="why__map">
              <img
                src="@/assets/images/landing/why-choose-us/usa-minimal-map.png"
                alt="Sigook® coverage across the United States"
                class="why__map-img"
                width="600"
                height="400"
                loading="lazy"
                decoding="async"
              />
            </div>
          </div>
        </div>

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
import { useRevealOnScroll } from '@/composables/useRevealOnScroll'
import SecondaryCard from '@/components/landing/shared/cards/SecondaryCard.vue'
import DecoMagnifier from '@/components/landing/shared/hero/DecoMagnifier.vue'

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

const { el: sectionRef, visible } = useRevealOnScroll()
</script>

<style scoped>
.why {
  position: relative;
  z-index: 1;
  margin-top: -260px;
}

.why__hero-back {
  position: absolute;
  top: -20px;
  left: 0;
  width: 100%;
  height: 628px;
  border-radius: 150px 0 150px 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: var(--glass-blur-soft);
  -webkit-backdrop-filter: var(--glass-blur-soft);
  border: 1px solid var(--c-glass-border-soft);
  box-shadow: var(--sh-back);
  z-index: 1;
  pointer-events: none;
}

.why__hero {
  position: relative;
  z-index: 2;
  width: 100%;
  height: 588px;
  border-radius: 150px 0 150px 0;
  overflow: hidden;
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

.why__hero-overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(90deg, rgba(15, 47, 68, 0.78) 0%, rgba(15, 47, 68, 0.45) 100%),
    linear-gradient(180deg, rgba(9, 48, 85, 0.20) 0%, rgba(15, 47, 68, 0.55) 100%);
}

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

.why__hero-magnifier {
  bottom: 48px;
  left: 56px;
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

.why__panel {
  position: relative;
  border-radius: 150px 0 150px 0;
  margin-top: -196px;
  z-index: 1;
  padding-bottom: 160px;
  overflow: hidden;
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

.why__top {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  gap: 80px;
  width: 100%;
  margin-bottom: 96px;
}

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

.why__panel-divider {
  width: 88px;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
}

.why__map-wrap {
  flex: 1;
  position: relative;
  z-index: 2;
  display: flex;
  justify-content: center;
  align-items: center;
  margin: 0;
  opacity: 0;
  transform: translateY(30px) scale(0.96);
  transition:
    opacity 0.8s ease-out,
    transform 0.8s var(--ease-brand);
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

.why__map {
  position: relative;
  display: block;
  width: 100%;
  max-width: 600px;
  margin: 0;
}

.why__map-img {
  display: block;
  width: 100%;
  height: auto;
}

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

@media (max-width: 1023px), (max-height: 899px) {
  .why {
    margin-top: -100px;
  }

  .why__hero-back {
    display: none;
  }

  .why__hero {
    height: auto;
    min-height: 280px;
    border-radius: 80px 0 80px 0;
    padding: 60px 24px;
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
    bottom: 20px;
    left: 20px;
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
    margin: 16px 0 0 0;
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
    min-height: 0;
    padding-bottom: 220px;
  }

  .why__panel-inner {
    padding: 140px 24px 0;
  }

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

  .why__map-wrap {
    width: 100%;
  }

  .why__map-halo {
    width: 110%;
    height: 80%;
    filter: blur(40px);
  }

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
