<template>
  <section class="about-intro">
    <HeroBackground :image="heroImage" :image-sm="heroImageSm" focal="center 30%" />
    <DecoMagnifier class="about-intro__magnifier" />

    <div class="about-intro__content">
      <EyebrowPill variant="red" class="about-intro__eyebrow">
        Our Story
      </EyebrowPill>

      <h1 class="about-intro__heading">
        Empowering <span class="about-intro__heading-accent">people.</span><br>
        Strengthening <span class="about-intro__heading-accent">businesses.</span>
      </h1>

      <p class="about-intro__subtitle">
        Since Florida in 2008, Sigook Work Factory has grown with one clear purpose —
        to bring people and opportunities together. Built on fairness, respect, and
        genuine care, we turn trust and integrity into lasting partnerships.
      </p>

      <ul class="about-intro__credentials">
        <li
          v-for="(credential, idx) in CREDENTIALS"
          :key="credential"
          class="about-intro__credential"
        >
          {{ credential }}
          <span
            v-if="idx < CREDENTIALS.length - 1"
            class="about-intro__credential-dot"
            aria-hidden="true"
          ></span>
        </li>
      </ul>

      <LabeledChipList
        label="Our values"
        :items="VALUES"
        class="about-intro__values"
      />
    </div>

    <ScrollIndicator href="#about-numbers" class="about-intro__scroll" />
  </section>
</template>

<script setup lang="ts">
import DecoMagnifier from '@/components/landing/shared/DecoMagnifier.vue'
import HeroBackground from '@/components/landing/shared/HeroBackground.vue'
import heroImage from '@/assets/images/v2/hero/about.webp'
import heroImageSm from '@/assets/images/v2/hero/about-960.webp'
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
import LabeledChipList from '@/components/landing/shared/LabeledChipList.vue'
import ScrollIndicator from '@/components/landing/shared/ScrollIndicator.vue'

const CREDENTIALS = [
  'Since 2008',
  'Florida → coast to coast',
  'Talent Management since 2016',
] as const

const VALUES = [
  'Fairness',
  'Respect',
  'Integrity',
  'Trust',
  'Care',
] as const
</script>

<style scoped>
.about-intro {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

.about-intro__magnifier {
  top: clamp(14%, 16vw, 18%);
  right: clamp(6%, 7vw, 9%);
}

.about-intro__content {
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

.about-intro__eyebrow {
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

.about-intro__heading {
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

.about-intro__heading-accent {
  color: var(--c-brand-cyan);
}

.about-intro__subtitle {
  font-family: var(--font-family);
  font-size: var(--text-hero-sub-size);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(28px, 3.6vw, 44px);
  max-width: 640px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

.about-intro__credentials {
  list-style: none;
  margin: 0 0 clamp(36px, 5vw, 64px);
  padding: 0;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: center;
  gap: clamp(8px, 1vw, 14px);
}

.about-intro__credential {
  display: inline-flex;
  align-items: center;
  gap: clamp(8px, 1vw, 14px);
  font-family: var(--font-family);
  font-size: clamp(10px, 0.85vw, 12px);
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
  white-space: nowrap;
}

.about-intro__credential-dot {
  width: 4px;
  height: 4px;
  border-radius: 50%;
  background: var(--c-brand-cyan);
  flex-shrink: 0;
}

.about-intro__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

@media (max-width: 1023px) {
  .about-intro {
    min-height: 100svh;
  }

  .about-intro__content {
    min-height: 100svh;
  }

  .about-intro__credentials {
    flex-direction: column;
  }

  .about-intro__credential-dot {
    display: none;
  }

  .about-intro__scroll {
    display: none;
  }

  .about-intro__magnifier {
    top: 12%;
    right: 6%;
  }
}
</style>
