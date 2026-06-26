<template>
  <article
    ref="cardRef"
    class="primary-card"
    :class="[
      `primary-card--${variant}`,
      { 'primary-card--visible': visible },
    ]"
    :style="{ transitionDelay: `${delay}ms` }"
  >
    <div v-if="$slots.icon" class="primary-card__icon">
      <slot name="icon" />
    </div>

    <span v-if="eyebrow" class="primary-card__eyebrow">{{ eyebrow }}</span>

    <h2 v-if="title" class="primary-card__title">{{ title }}</h2>

    <div v-if="showDivider" class="primary-card__divider" aria-hidden="true"></div>

    <div v-if="$slots.default" class="primary-card__body">
      <slot />
    </div>

    <ul v-if="list && list.length" class="primary-card__list">
      <li v-for="item in list" :key="item">{{ item }}</li>
    </ul>

    <div v-if="$slots.button" class="primary-card__button-wrap">
      <slot name="button" />
    </div>
  </article>
</template>

<script lang="ts">
export type PrimaryCardVariant = 'navy' | 'red'
</script>

<script setup lang="ts">
import { useRevealOnScroll } from '@/composables/useRevealOnScroll'

withDefaults(defineProps<{
  variant?: PrimaryCardVariant
  eyebrow?: string
  title?: string
  list?: readonly string[]
  showDivider?: boolean
  delay?: number
}>(), {
  variant: 'navy',
  delay: 0,
  showDivider: false,
})

const { el: cardRef, visible } = useRevealOnScroll()
</script>

<style scoped>
.primary-card {
  display: flex;
  flex-direction: column;
  padding:
    clamp(36px, 4.5vw, 56px)
    clamp(32px, 4vw, 56px);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.20);
  box-shadow: 0 24px 60px rgba(0, 0, 0, 0.30);
  color: #fff;
  isolation: isolate;
  font-family: var(--font-family);
  overflow: hidden;
  opacity: 0;
  transform: translateY(40px) scale(0.95);
  transition:
    opacity 0.7s ease-out,
    transform 0.7s var(--ease-brand),
    box-shadow 0.5s ease;
}

.primary-card--visible {
  opacity: 1;
  transform: translateY(0) scale(1);
}

.primary-card:hover {
  box-shadow: 0 32px 72px rgba(0, 0, 0, 0.40);
  transform: translateY(-8px);
}

.primary-card--navy {
  background: linear-gradient(135deg,
    rgba(15, 47, 68, 0.70) 0%,
    rgba(21, 117, 187, 0.45) 100%);
  border-radius:
    clamp(20px, 2.5vw, 24px) clamp(64px, 8vw, 96px)
    clamp(20px, 2.5vw, 24px) clamp(64px, 8vw, 96px);
}

.primary-card--red {
  background: linear-gradient(135deg,
    rgba(229, 45, 39, 0.62) 0%,
    rgba(229, 45, 39, 0.40) 100%);
  border-radius:
    clamp(64px, 8vw, 96px) clamp(20px, 2.5vw, 24px)
    clamp(64px, 8vw, 96px) clamp(20px, 2.5vw, 24px);
}

.primary-card__icon {
  margin-bottom: clamp(16px, 1.8vw, 22px);
  font-size: clamp(40px, 4.8vw, 60px);
  color: #fff;
  filter: drop-shadow(0 4px 12px rgba(0, 0, 0, 0.30));
  line-height: 0;
}

.primary-card__eyebrow {
  display: inline-block;
  font-size: clamp(11px, 1vw, 13px);
  font-weight: 600;
  letter-spacing: 0.20em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.82);
  margin-bottom: clamp(12px, 1.4vw, 16px);
}

.primary-card__title {
  font-size: clamp(32px, 5vw, 64px);
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0;
}

.primary-card__divider {
  width: clamp(64px, 7vw, 88px);
  height: 2px;
  background: #fff;
  border-radius: 2px;
  margin: clamp(16px, 2vw, 24px) 0;
}

.primary-card--red .primary-card__divider {
  background: var(--c-brand-cyan);
}

.primary-card__body {
  font-size: clamp(15px, 1.4vw, 17px);
  font-weight: 400;
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.92);
  margin: 0;
}

.primary-card__title + .primary-card__body {
  margin-top: clamp(16px, 2vw, 24px);
}

.primary-card__list {
  list-style: none;
  padding: 0;
  margin: clamp(16px, 2vw, 24px) 0 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.primary-card__list li {
  position: relative;
  padding-left: 20px;
  font-size: clamp(13px, 1.2vw, 15px);
  line-height: 1.5;
  color: rgba(255, 255, 255, 0.92);
}

.primary-card__list li::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0.55em;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.85);
}

.primary-card__button-wrap {
  margin-top: clamp(28px, 3.2vw, 36px);
}

.primary-card__body :deep(> *:first-child) { margin-top: 0; }
.primary-card__body :deep(> *:last-child)  { margin-bottom: 0; }

@media (max-width: 1023px) {
  .primary-card {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    box-shadow: 0 24px 24px rgba(0, 0, 0, 0.30);
  }

  .primary-card:hover {
    box-shadow: 0 32px 24px rgba(0, 0, 0, 0.40);
  }

  .primary-card--navy {
    background: linear-gradient(135deg,
      rgba(15, 47, 68, 0.85) 0%,
      rgba(21, 117, 187, 0.60) 100%);
  }

  .primary-card--red {
    background: linear-gradient(135deg,
      rgba(229, 45, 39, 0.77) 0%,
      rgba(229, 45, 39, 0.55) 100%);
  }
}
</style>
