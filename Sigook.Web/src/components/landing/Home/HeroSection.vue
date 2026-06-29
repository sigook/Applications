<template>
  <section class="hero">
    <HeroBackground :image="heroImage" :image-sm="heroImageSm" focal="center 28%" />

    <DecoMagnifier class="hero__magnifier" />

    <div class="hero__content">
      <EyebrowPill variant="red" class="hero__eyebrow">
        Workforce Platform
      </EyebrowPill>

      <img
        :src="logoWhite"
        alt="Sigook Work Factory"
        class="hero__logo"
      />

      <h1 class="hero__heading">
        Where great <span class="hero__heading-accent">talent</span> meets<br>
        great <span class="hero__heading-accent">opportunities</span>
      </h1>

      <p class="hero__subtitle">
        Sigook® connects leading U.S. employers with skilled workers
        from onboarding and timesheets to payroll, fully connected in one platform.
      </p>

      <LabeledChipList
        label="Trusted across"
        :items="INDUSTRIES"
        more-label="+ more"
        more-to="/industries"
        class="hero__industries"
      />
    </div>

    <ScrollIndicator href="#dual-cta" class="hero__scroll" />
  </section>
</template>

<script setup lang="ts">
import DecoMagnifier from '@/components/landing/shared/hero/DecoMagnifier.vue'
import HeroBackground from '@/components/landing/shared/hero/HeroBackground.vue'
import EyebrowPill from '@/components/landing/shared/ui/EyebrowPill.vue'
import LabeledChipList from '@/components/landing/shared/ui/LabeledChipList.vue'
import ScrollIndicator from '@/components/landing/shared/hero/ScrollIndicator.vue'
import logoWhite from '@/assets/images/logo-white-v2.png'
import heroImage from '@/assets/images/v2/hero/home.webp'
import heroImageSm from '@/assets/images/v2/hero/home-960.webp'

const INDUSTRIES = [
  'Cybersecurity',
  'Artificial Intelligence',
  'Manufacturing',
  'Logistics',
  'Retail',
  'Construction',
] as const
</script>

<style scoped>
.hero {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

.hero__magnifier {
  top: clamp(14%, 16vw, 18%);
  left: clamp(6%, 7vw, 9%);
}

.hero__content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: max(100vh, 1080px);
  padding:
    clamp(80px, 12vw, 140px)
    clamp(20px, 3vw, 32px)
    clamp(120px, 16vw, 220px);
  text-align: center;
  max-width: 1100px;
  margin: 0 auto;
}

.hero__eyebrow {
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

.hero__logo {
  display: block;
  width: auto;
  height: clamp(64px, 8vw, 96px);
  max-width: clamp(200px, 22vw, 280px);
  object-fit: contain;
  margin: 0 auto clamp(28px, 4vw, 44px);
  filter: drop-shadow(0 10px 30px rgba(0, 0, 0, 0.45));
}

.hero__heading {
  font-family: var(--font-family);
  font-size: var(--text-hero-size);
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(24px, 3.5vw, 36px);
  max-width: 920px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.hero__heading-accent {
  color: var(--c-brand-cyan);
}

.hero__subtitle {
  font-family: var(--font-family);
  font-size: var(--text-hero-sub-size);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(44px, 6vw, 64px);
  max-width: 640px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

.hero__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

@media (max-width: 1023px) {
  .hero {
    min-height: 100svh;
  }

  .hero__content {
    min-height: 100svh;
  }

  .hero__scroll {
    display: none;
  }
}
</style>
