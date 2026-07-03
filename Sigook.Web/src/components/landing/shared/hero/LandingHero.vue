<template>
  <section class="landing-hero" :style="magnifierVars">
    <HeroBackground :image="image" :image-sm="imageSm" :focal="focal" />
    <DecoMagnifier class="landing-hero__magnifier" />

    <div class="landing-hero__content">
      <EyebrowPill :variant="eyebrowVariant" class="landing-hero__eyebrow">
        {{ eyebrowText }}
      </EyebrowPill>

      <h1 class="landing-hero__heading">
        <slot name="heading" />
      </h1>

      <p class="landing-hero__subtitle">{{ subtitle }}</p>

      <slot />
    </div>

    <ScrollIndicator :href="scrollHref" class="landing-hero__scroll" />
  </section>
</template>

<script setup lang="ts">
import HeroBackground from '@/components/landing/shared/hero/HeroBackground.vue'
import DecoMagnifier from '@/components/landing/shared/hero/DecoMagnifier.vue'
import EyebrowPill from '@/components/landing/shared/ui/EyebrowPill.vue'
import ScrollIndicator from '@/components/landing/shared/hero/ScrollIndicator.vue'

withDefaults(
  defineProps<{
    image: string
    imageSm: string
    focal?: string
    eyebrowVariant?: 'cyan' | 'red' | 'white'
    eyebrowText: string
    subtitle: string
    scrollHref: string
    magnifierVars?: Record<string, string>
  }>(),
  {
    focal: 'center',
    eyebrowVariant: 'cyan',
    magnifierVars: () => ({}),
  },
)
</script>

<style scoped>
.landing-hero {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

.landing-hero__magnifier {
  top: var(--lh-mag-top, auto);
  right: var(--lh-mag-right, auto);
  bottom: var(--lh-mag-bottom, auto);
  left: var(--lh-mag-left, auto);
}

.landing-hero__content {
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

.landing-hero__eyebrow {
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

.landing-hero__heading {
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

.landing-hero__subtitle {
  font-family: var(--font-family);
  font-size: var(--text-hero-sub-size);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(44px, 6vw, 64px);
  max-width: 680px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

.landing-hero__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

@media (max-width: 1023px) {
  .landing-hero {
    min-height: 100svh;
  }

  .landing-hero__content {
    min-height: 100svh;
  }

  .landing-hero__scroll {
    display: none;
  }

  .landing-hero__magnifier {
    top: var(--lh-mag-top-m, var(--lh-mag-top, auto));
    right: var(--lh-mag-right-m, var(--lh-mag-right, auto));
    bottom: var(--lh-mag-bottom-m, var(--lh-mag-bottom, auto));
    left: var(--lh-mag-left-m, var(--lh-mag-left, auto));
  }
}
</style>
