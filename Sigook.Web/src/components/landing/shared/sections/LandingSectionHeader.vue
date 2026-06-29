<template>
  <header class="section-header" :style="{ marginBottom }">
    <EyebrowPill
      :variant="eyebrowVariant"
      class="section-header__eyebrow"
      :style="{ marginBottom: eyebrowMarginBottom }"
    >
      {{ eyebrow }}
    </EyebrowPill>

    <h2
      class="section-header__heading"
      :style="{
        maxWidth: headingMaxWidth,
        fontSize: headingSize,
        lineHeight: headingLineHeight,
        marginBottom: headingMarginBottom,
      }"
    >
      {{ heading }}
      <span v-if="headingAccent" class="section-header__accent">{{ headingAccent }}</span>
    </h2>

    <p
      v-if="subtitle"
      class="section-header__subtitle"
      :style="{
        maxWidth: subtitleMaxWidth,
        color: subtitleColor,
        textShadow: subtitleTextShadow,
        marginTop: subtitleMarginTop,
      }"
    >
      {{ subtitle }}
    </p>
  </header>
</template>

<script setup lang="ts">
import EyebrowPill from '@/components/landing/shared/ui/EyebrowPill.vue'

withDefaults(defineProps<{
  eyebrow: string
  heading: string
  headingAccent?: string
  subtitle?: string
  eyebrowVariant?: 'cyan' | 'red' | 'white'
  headingMaxWidth?: string
  subtitleMaxWidth?: string
  marginBottom?: string
  eyebrowMarginBottom?: string
  headingSize?: string
  headingLineHeight?: string
  headingMarginBottom?: string
  subtitleColor?: string
  subtitleTextShadow?: string
  subtitleMarginTop?: string
}>(), {
  eyebrowVariant: 'white',
  headingMaxWidth: '780px',
  subtitleMaxWidth: '560px',
})
</script>

<style scoped>
.section-header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.section-header__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.section-header__heading {
  font-size: var(--text-section-heading-size);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  text-shadow: var(--sh-text-heading);
}

.section-header__accent {
  color: var(--c-brand-cyan);
  display: block;
}

.section-header__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: var(--c-on-dark-78);
  margin: 0;
  text-shadow: var(--sh-text-sub);
}
</style>
