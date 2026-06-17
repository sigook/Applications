<template>
  <router-link v-if="to" :to="to" class="glass-pill-cta" :class="`glass-pill-cta--${size}`">
    <span><slot /></span>
    <ArrowIcon
      v-if="showArrow"
      :width="arrowWidth"
      :height="arrowHeight"
      :stroke-width="1.5"
      color="currentColor"
    />
  </router-link>

  <a v-else-if="href" :href="href" class="glass-pill-cta" :class="`glass-pill-cta--${size}`">
    <span><slot /></span>
    <ArrowIcon
      v-if="showArrow"
      :width="arrowWidth"
      :height="arrowHeight"
      :stroke-width="1.5"
      color="currentColor"
    />
  </a>

  <button v-else type="button" class="glass-pill-cta" :class="`glass-pill-cta--${size}`">
    <span><slot /></span>
    <ArrowIcon
      v-if="showArrow"
      :width="arrowWidth"
      :height="arrowHeight"
      :stroke-width="1.5"
      color="currentColor"
    />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import ArrowIcon from '@/components/landing/shared/ArrowIcon.vue'

/**
 * Glass-pill CTA — the canonical "Learn More / Browse Jobs / Post a Job"
 * button used across Hero, DualCta, WhyChooseUs, Certified, and Numbers.
 *
 * Renders as <router-link> when `to` is provided, <a> when `href` is provided,
 * or <button type="button"> otherwise. Click events bubble naturally from
 * the rendered element.
 */
const props = withDefaults(defineProps<{
  to?: string
  href?: string
  size?: 'sm' | 'md' | 'lg'
  showArrow?: boolean
}>(), {
  size: 'md',
  showArrow: false,
})

const arrowWidth = computed(() =>
  props.size === 'sm' ? 28 : props.size === 'lg' ? 36 : 32
)
const arrowHeight = computed(() =>
  props.size === 'sm' ? 10 : props.size === 'lg' ? 14 : 11
)
</script>

<style scoped>
.glass-pill-cta {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  border: 1.5px solid rgba(255, 255, 255, 0.85);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(10px) saturate(150%);
  -webkit-backdrop-filter: blur(10px) saturate(150%);
  color: #fff;
  font-family: var(--font-family);
  font-weight: 600;
  letter-spacing: 0.04em;
  text-decoration: none;
  cursor: pointer;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.25);
  transition: background 0.3s ease, color 0.3s ease, transform 0.3s ease;
}

.glass-pill-cta:hover,
.glass-pill-cta:focus-visible {
  background: #fff;
  color: var(--c-brand-navy);
  transform: translateY(-2px);
}

.glass-pill-cta--sm { padding: 12px 24px; font-size: 14px; }
.glass-pill-cta--md { padding: 14px 32px; font-size: 15px; }
.glass-pill-cta--lg { padding: 16px 36px; font-size: 16px; }

@media (max-width: 1023px) {
  .glass-pill-cta {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    background: rgba(255, 255, 255, 0.24);
  }
}
</style>
