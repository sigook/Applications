<template>
  <section class="partner-hero">
    <HeroBackground :image="heroImage" :image-sm="heroImageSm" focal="center 30%" />
    <DecoMagnifier class="partner-hero__magnifier" />

    <div class="partner-hero__content">
      <EyebrowPill variant="red" class="partner-hero__eyebrow">
        Partner Program
      </EyebrowPill>

      <h1 class="partner-hero__heading">
        Grow your <span class="partner-hero__heading-accent">book.</span><br>
        Keep what's <span class="partner-hero__heading-accent">yours.</span>
      </h1>

      <p class="partner-hero__subtitle">
        Independent recruiters and business developers — plug into Sigook's
        compliance, payroll, and client base while keeping your own brand
        and 100% of your relationships.
      </p>

      <ul class="partner-hero__stats" aria-label="Partner program stats">
        <li class="partner-hero__stat">
          <span class="partner-hero__stat-value">12+</span>
          <span class="partner-hero__stat-label">Active partners</span>
        </li>
        <li class="partner-hero__stat partner-hero__stat--accent">
          <span class="partner-hero__stat-value">+35%</span>
          <span class="partner-hero__stat-label">Avg YoY revenue</span>
        </li>
        <li class="partner-hero__stat">
          <span class="partner-hero__stat-value">~4 wks</span>
          <span class="partner-hero__stat-label">Apply to first deal</span>
        </li>
      </ul>
    </div>

    <ScrollIndicator href="#partner-types" class="partner-hero__scroll" />
  </section>
</template>

<script setup lang="ts">
import DecoMagnifier from '@/components/landing/shared/hero/DecoMagnifier.vue'
import HeroBackground from '@/components/landing/shared/hero/HeroBackground.vue'
import heroImage from '@/assets/images/landing/hero/partner.webp'
import heroImageSm from '@/assets/images/landing/hero/partner-960.webp'
import EyebrowPill from '@/components/landing/shared/ui/EyebrowPill.vue'
import ScrollIndicator from '@/components/landing/shared/hero/ScrollIndicator.vue'
</script>

<style scoped>
.partner-hero {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

.partner-hero__magnifier {
  bottom: clamp(14%, 16vw, 20%);
  left: 50%;
  transform: translateX(-50%);
}

.partner-hero__content {
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

.partner-hero__eyebrow {
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

.partner-hero__heading {
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

.partner-hero__heading-accent {
  color: var(--c-brand-cyan);
}

.partner-hero__subtitle {
  font-family: var(--font-family);
  font-size: var(--text-hero-sub-size);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(36px, 5vw, 56px);
  max-width: 680px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

.partner-hero__stats {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  align-items: stretch;
  flex-wrap: wrap;
  justify-content: center;
  gap: clamp(14px, 1.8vw, 22px);
}

.partner-hero__stat {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6px;
  padding: clamp(14px, 1.8vw, 20px) clamp(20px, 2.6vw, 32px);
  background: var(--c-glass-fill);
  border: 1px solid rgba(255, 255, 255, 0.18);
  border-radius: clamp(16px, 2vw, 22px);
  backdrop-filter: blur(12px) saturate(150%);
  -webkit-backdrop-filter: blur(12px) saturate(150%);
  min-width: clamp(140px, 14vw, 180px);
}

.partner-hero__stat--accent {
  background: rgba(229, 45, 39, 0.12);
  border-color: rgba(229, 45, 39, 0.45);
}

.partner-hero__stat-value {
  font-family: var(--font-family);
  font-size: clamp(26px, 3.4vw, 38px);
  font-weight: 700;
  line-height: 1;
  letter-spacing: -0.02em;
  color: #fff;
  text-shadow: 0 4px 16px rgba(0, 0, 0, 0.40);
}

.partner-hero__stat--accent .partner-hero__stat-value {
  color: var(--c-brand-red);
}

.partner-hero__stat-label {
  font-family: var(--font-family);
  font-size: clamp(10px, 0.9vw, 12px);
  font-weight: 600;
  letter-spacing: 0.16em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
  text-align: center;
}

.partner-hero__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

@media (max-width: 1023px) {
  .partner-hero {
    min-height: 100svh;
  }

  .partner-hero__content {
    min-height: 100svh;
  }

  .partner-hero__scroll {
    display: none;
  }

  .partner-hero__magnifier {
    bottom: 16%;
  }
}

@media (max-width: 599px) {
  .partner-hero__stats {
    flex-direction: column;
    width: 100%;
    max-width: 280px;
  }

  .partner-hero__stat {
    flex-direction: row;
    justify-content: space-between;
    width: 100%;
    min-width: 0;
    text-align: left;
  }

  .partner-hero__stat-label {
    text-align: right;
  }
}
</style>
