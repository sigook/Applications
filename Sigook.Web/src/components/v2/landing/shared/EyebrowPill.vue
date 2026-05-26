<template>
  <span class="eyebrow-pill" :class="`eyebrow-pill--${variant}`">
    <slot />
  </span>
</template>

<script setup lang="ts">
/**
 * Uppercase tracked label inside a glass pill — the canonical "category tag"
 * used as eyebrow in Hero, AppDownload, etc.
 *
 * Variants pick the brand color of the border + tinted background; text color
 * follows the same hue.
 *
 * Usage:
 *   <EyebrowPill variant="red">Workforce Platform</EyebrowPill>
 */
withDefaults(defineProps<{
  variant?: 'cyan' | 'red' | 'white'
}>(), { variant: 'cyan' })
</script>

<style scoped>
.eyebrow-pill {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  padding: 8px 18px;
  border-radius: 999px;
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
  line-height: 1.2;
}

.eyebrow-pill--cyan {
  color: var(--c-brand-cyan);
  border: 1px solid rgba(0, 173, 239, 0.40);
  background: rgba(0, 173, 239, 0.10);
}

.eyebrow-pill--red {
  color: var(--c-brand-red);
  border: 1px solid rgba(229, 45, 39, 0.40);
  background: rgba(229, 45, 39, 0.10);
}

.eyebrow-pill--white {
  color: rgba(255, 255, 255, 0.85);
  border: 1px solid rgba(255, 255, 255, 0.30);
  background: rgba(255, 255, 255, 0.06);
}

@media (max-width: 1023px) {
  .eyebrow-pill {
    font-size: 10px;
    padding: 6px 14px;
  }
}
</style>
