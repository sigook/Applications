<template>
  <span class="eyebrow-pill" :class="`eyebrow-pill--${variant}`">
    <slot />
  </span>
</template>

<script setup lang="ts">
/**
 * Uppercase tracked label inside a SOLID pill — the canonical "category tag"
 * used as eyebrow across the V2 landing pages.
 *
 * All three variants share the same SIGOOK APP vocabulary from the
 * AppDownloadSection: opaque brand-color fill, white text, soft colored
 * glow shadow. Glass / translucent backgrounds were removed by design so
 * the pill always reads as a strong anchor regardless of what's behind it.
 *
 * Usage:
 *   <EyebrowPill variant="red">Workforce Platform</EyebrowPill>
 *
 * Variants:
 *   • cyan  — solid cyan fill, white text, cyan glow
 *   • red   — solid red fill,  white text, red glow
 *   • white — solid white fill, navy text, white glow (use on dark panels)
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
  line-height: 1.2;
  /* No backdrop-filter — pills are opaque, glass blur would be wasted. */
}

/* Solid cyan fill — for atmospheric / discovery-toned sections. */
.eyebrow-pill--cyan {
  color: #fff;
  background: var(--c-brand-cyan);
  border: 1px solid var(--c-brand-cyan);
  box-shadow: 0 8px 18px -6px rgba(0, 173, 239, 0.55);
}

/* Solid red fill — for entrepreneurial / urgent / brand-anchor sections.
   Matches the SIGOOK APP pill in AppDownloadSection. */
.eyebrow-pill--red {
  color: #fff;
  background: var(--c-brand-red);
  border: 1px solid var(--c-brand-red);
  box-shadow: 0 8px 18px -6px rgba(229, 45, 39, 0.55);
}

/* Solid white fill — for use on dark panels (where colored pills would
   compete with the panel's own tone). Text shifts to navy for contrast. */
.eyebrow-pill--white {
  color: var(--c-brand-navy, #0f2f44);
  background: #fff;
  border: 1px solid #fff;
  box-shadow: 0 8px 18px -6px rgba(255, 255, 255, 0.35);
}

@media (max-width: 1023px) {
  .eyebrow-pill {
    font-size: 10px;
    padding: 6px 14px;
  }
}
</style>
