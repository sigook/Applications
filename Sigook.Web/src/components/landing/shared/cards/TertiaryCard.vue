<template>
  <article
    ref="cardRef"
    class="tertiary-card"
    :class="[
      `tertiary-card--${variant}`,
      { 'tertiary-card--visible': visible },
    ]"
    :style="{ transitionDelay: `${delay}ms` }"
  >
    <div v-if="$slots.icon" class="tertiary-card__icon">
      <slot name="icon" />
    </div>

    <span v-if="eyebrow" class="tertiary-card__eyebrow">{{ eyebrow }}</span>

    <h4 v-if="title" class="tertiary-card__title">{{ title }}</h4>

    <div v-if="$slots.default" class="tertiary-card__body">
      <slot />
    </div>

    <ul v-if="list && list.length" class="tertiary-card__list">
      <li v-for="item in list" :key="item">{{ item }}</li>
    </ul>

    <div v-if="$slots.button" class="tertiary-card__button-wrap">
      <slot name="button" />
    </div>
  </article>
</template>

<script lang="ts">
export type TertiaryCardVariant = 'blue' | 'cyan' | 'red'
</script>

<script setup lang="ts">
import { useRevealOnScroll } from '@/composables/useRevealOnScroll'

withDefaults(defineProps<{
  variant?: TertiaryCardVariant
  eyebrow?: string
  title?: string
  list?: readonly string[]
  delay?: number
}>(), {
  variant: 'cyan',
  delay: 0,
})

const { el: cardRef, visible } = useRevealOnScroll()
</script>

<style scoped>
.tertiary-card {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: clamp(10px, 1.2vw, 14px);
  padding:
    clamp(20px, 2.4vw, 28px)
    clamp(16px, 2vw, 24px);
  min-height: clamp(120px, 14vw, 160px);
  backdrop-filter: blur(18px) saturate(160%);
  -webkit-backdrop-filter: blur(18px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.20);
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.20);
  color: #fff;
  isolation: isolate;
  font-family: var(--font-family);
  overflow: hidden;
  text-align: center;
  text-decoration: none;

  opacity: 0;
  transform: translateY(28px) scale(0.94);
  transition:
    opacity 0.6s ease-out,
    transform 0.6s var(--ease-brand),
    border-color 0.3s ease,
    box-shadow 0.3s ease;
}

.tertiary-card--visible {
  opacity: 1;
  transform: translateY(0) scale(1);
}

.tertiary-card:hover {
  border-color: rgba(255, 255, 255, 0.32);
  box-shadow: 0 18px 38px rgba(0, 0, 0, 0.30);
}

.tertiary-card::before {
  content: '';
  position: absolute;
  pointer-events: none;
  width: clamp(80px, 10vw, 130px);
  height: clamp(80px, 10vw, 130px);
  z-index: 0;
}

.tertiary-card--blue {
  background: linear-gradient(135deg,
    rgba(21, 117, 187, 0.42) 0%,
    rgba(21, 117, 187, 0.14) 50%,
    rgba(229, 45, 39, 0.32) 100%);
  border-radius:
    clamp(16px, 2vw, 24px) clamp(32px, 4vw, 48px)
    clamp(16px, 2vw, 24px) clamp(32px, 4vw, 48px);
}

.tertiary-card--blue::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(229, 45, 39, 0.55) 0%,
    rgba(229, 45, 39, 0.18) 35%,
    transparent 70%);
  border-radius: 0 clamp(32px, 4vw, 48px) 0 0;
}

.tertiary-card--cyan {
  background: linear-gradient(135deg,
    rgba(0, 173, 239, 0.34) 0%,
    rgba(21, 117, 187, 0.22) 50%,
    rgba(229, 45, 39, 0.30) 100%);
  border-radius:
    clamp(32px, 4vw, 48px) clamp(16px, 2vw, 24px)
    clamp(32px, 4vw, 48px) clamp(16px, 2vw, 24px);
}

.tertiary-card--cyan::before {
  top: 0;
  left: 0;
  background: radial-gradient(circle at top left,
    rgba(0, 173, 239, 0.55) 0%,
    rgba(0, 173, 239, 0.18) 35%,
    transparent 70%);
  border-radius: clamp(32px, 4vw, 48px) 0 0 0;
}

.tertiary-card--red {
  background: linear-gradient(135deg,
    rgba(229, 45, 39, 0.42) 0%,
    rgba(229, 45, 39, 0.14) 50%,
    rgba(21, 117, 187, 0.32) 100%);
  border-radius:
    clamp(16px, 2vw, 24px) clamp(32px, 4vw, 48px)
    clamp(16px, 2vw, 24px) clamp(32px, 4vw, 48px);
}

.tertiary-card--red::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(21, 117, 187, 0.60) 0%,
    rgba(21, 117, 187, 0.20) 35%,
    transparent 70%);
  border-radius: 0 clamp(32px, 4vw, 48px) 0 0;
}

.tertiary-card__icon {
  position: relative;
  z-index: 1;
  font-size: clamp(28px, 3.4vw, 42px);
  color: #fff;
  filter: drop-shadow(0 3px 8px rgba(0, 0, 0, 0.25));
  line-height: 0;
}

.tertiary-card__eyebrow {
  position: relative;
  z-index: 1;
  font-size: clamp(9px, 0.75vw, 10px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
}

.tertiary-card--red .tertiary-card__eyebrow {
  color: #fff;
  opacity: 0.85;
}

.tertiary-card__title {
  position: relative;
  z-index: 1;
  font-size: clamp(14px, 1.3vw, 17px);
  font-weight: 700;
  line-height: 1.2;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.30);
}

.tertiary-card__body {
  position: relative;
  z-index: 1;
  font-size: clamp(11px, 0.95vw, 13px);
  font-weight: 400;
  line-height: 1.5;
  color: rgba(255, 255, 255, 0.82);
}

.tertiary-card__list {
  position: relative;
  z-index: 1;
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: clamp(11px, 0.95vw, 13px);
  color: rgba(255, 255, 255, 0.85);
}

.tertiary-card__button-wrap {
  position: relative;
  z-index: 1;
  margin-top: clamp(4px, 0.5vw, 8px);
}

.tertiary-card__body :deep(> *:first-child) { margin-top: 0; }
.tertiary-card__body :deep(> *:last-child)  { margin-bottom: 0; }

@media (max-width: 1023px) {
  .tertiary-card {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    box-shadow: 0 12px 24px rgba(0, 0, 0, 0.20);
  }

  .tertiary-card:hover {
    box-shadow: 0 18px 24px rgba(0, 0, 0, 0.30);
  }

  .tertiary-card--blue {
    background: linear-gradient(135deg,
      rgba(21, 117, 187, 0.56) 0%,
      rgba(21, 117, 187, 0.28) 50%,
      rgba(229, 45, 39, 0.46) 100%);
  }

  .tertiary-card--cyan {
    background: linear-gradient(135deg,
      rgba(0, 173, 239, 0.48) 0%,
      rgba(21, 117, 187, 0.36) 50%,
      rgba(229, 45, 39, 0.44) 100%);
  }

  .tertiary-card--red {
    background: linear-gradient(135deg,
      rgba(229, 45, 39, 0.56) 0%,
      rgba(229, 45, 39, 0.28) 50%,
      rgba(21, 117, 187, 0.46) 100%);
  }
}
</style>
