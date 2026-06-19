<template>
  <component
    :is="tag"
    v-bind="hostAttrs"
    class="arrow-pill"
    :class="[`arrow-pill--${variant}`, `arrow-pill--hover-${hoverVariant}`, `arrow-pill--${size}`]"
  >
    <span class="arrow-pill__label"><slot /></span>
    <span v-if="showArrow" class="arrow-pill__arrow" aria-hidden="true">→</span>
  </component>
</template>

<script setup lang="ts">
/**
 * ArrowPillCta — the canonical pill CTA used across the landing sections.
 *
 * Two looks:
 *  • variant="glass" — translucent fill + white outline; inverts on hover to a
 *    solid fill whose colour is set by `hoverVariant` (navy | cyan | red).
 *  • variant="solid" — opaque brand-red fill that darkens to crimson on hover
 *    (the primary action CTA). `hoverVariant` is ignored for solid.
 *
 * Renders <router-link> when `to` is set, <a> when `href` is set, otherwise a
 * <button> (`type` selects button|submit). The trailing arrow nudges on hover.
 */
import { computed } from 'vue'
import { RouterLink } from 'vue-router'

const props = withDefaults(defineProps<{
  to?: string
  href?: string
  type?: 'button' | 'submit'
  variant?: 'glass' | 'solid'
  hoverVariant?: 'navy' | 'cyan' | 'red'
  size?: 'sm' | 'md' | 'lg'
  showArrow?: boolean
}>(), {
  type: 'button',
  variant: 'glass',
  hoverVariant: 'navy',
  size: 'md',
  showArrow: true,
})

const tag = computed(() => (props.to ? RouterLink : props.href ? 'a' : 'button'))
const hostAttrs = computed(() =>
  props.to ? { to: props.to } : props.href ? { href: props.href } : { type: props.type },
)
</script>

<style scoped>
.arrow-pill {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  border-radius: var(--r-pill-full);
  color: #fff;
  font-family: var(--font-family);
  font-weight: 600;
  letter-spacing: 0.04em;
  text-decoration: none;
  white-space: nowrap;
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease,
    box-shadow 0.25s ease;
}

/* ── Sizes ───────────────────────────────────────────────────────────────── */
.arrow-pill--sm {
  padding: clamp(10px, 1.1vw, 12px) clamp(18px, 2vw, 24px);
  font-size: clamp(12px, 1vw, 13px);
}
.arrow-pill--md {
  padding: clamp(11px, 1.2vw, 14px) clamp(22px, 2.4vw, 30px);
  font-size: clamp(13px, 1.05vw, 14px);
}
.arrow-pill--lg {
  padding: clamp(13px, 1.4vw, 16px) clamp(26px, 2.8vw, 36px);
  font-size: clamp(14px, 1.15vw, 15px);
}

/* ── Glass variant ───────────────────────────────────────────────────────── */
.arrow-pill--glass {
  background: var(--c-glass-fill-strong);
  border: 1.5px solid rgba(var(--c-white-rgb), 0.55);
}
.arrow-pill--glass.arrow-pill--hover-navy:hover,
.arrow-pill--glass.arrow-pill--hover-navy:focus-visible {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-navy);
  transform: translateY(-2px);
}
.arrow-pill--glass.arrow-pill--hover-cyan:hover,
.arrow-pill--glass.arrow-pill--hover-cyan:focus-visible {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: translateY(-2px);
}
.arrow-pill--glass.arrow-pill--hover-red:hover,
.arrow-pill--glass.arrow-pill--hover-red:focus-visible {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-red);
  transform: translateY(-2px);
}

/* ── Solid variant (primary red) ─────────────────────────────────────────── */
.arrow-pill--solid {
  background: var(--c-brand-red);
  border: 1.5px solid var(--c-brand-red);
  font-weight: 700;
}
.arrow-pill--solid:hover,
.arrow-pill--solid:focus-visible {
  background: var(--c-brand-crimson);
  border-color: var(--c-brand-crimson);
  transform: translateY(-2px);
  box-shadow: 0 12px 28px -6px rgba(var(--c-brand-red-rgb), 0.5);
}

/* ── Arrow ───────────────────────────────────────────────────────────────── */
.arrow-pill__arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}
.arrow-pill:hover .arrow-pill__arrow,
.arrow-pill:focus-visible .arrow-pill__arrow {
  transform: translateX(3px);
}
</style>
