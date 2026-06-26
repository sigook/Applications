<template>
  <nav class="step-nav" :aria-label="`Step ${current + 1} of ${steps.length}`">
    <ol class="step-nav__list">
      <li
        v-for="(step, idx) in steps"
        :key="step.key"
        class="step-nav__item"
        :class="[
          {
            'step-nav__item--current': idx === current,
            'step-nav__item--done': idx < current,
            'step-nav__item--pending': idx > current,
          },
        ]"
      >
        <span class="step-nav__circle" aria-hidden="true">
          <svg
            v-if="idx < current"
            viewBox="0 0 14 14"
            class="step-nav__tick"
            fill="none"
            stroke="currentColor"
            stroke-width="2.5"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <polyline points="11 4 6 9 3 6" />
          </svg>
          <span v-else>{{ idx + 1 }}</span>
        </span>
        <span class="step-nav__label">{{ step.label }}</span>
        <span
          v-if="idx < steps.length - 1"
          class="step-nav__connector"
          aria-hidden="true"
        ></span>
      </li>
    </ol>
  </nav>
</template>

<script setup lang="ts">
export interface StepDescriptor {
  readonly key: string
  readonly label: string
}

defineProps<{
  steps: readonly StepDescriptor[]
  current: number
}>()
</script>

<style scoped>
.step-nav {
  width: 100%;
  font-family: var(--font-family);
}

.step-nav__list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  gap: clamp(8px, 1.6vw, 18px);
}

.step-nav__item {
  flex: 1;
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  text-align: center;
}

.step-nav__circle {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: clamp(32px, 3.4vw, 40px);
  height: clamp(32px, 3.4vw, 40px);
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.06);
  border: 1.5px solid rgba(255, 255, 255, 0.30);
  color: rgba(255, 255, 255, 0.60);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 700;
  transition: background 0.3s ease, border-color 0.3s ease, color 0.3s ease;
  z-index: 2;
}

.step-nav__label {
  font-size: clamp(10px, 0.95vw, 12px);
  font-weight: 700;
  letter-spacing: 0.14em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.55);
  transition: color 0.3s ease;
}

.step-nav__connector {
  position: absolute;
  top: clamp(15px, 1.65vw, 19px);
  left: calc(50% + clamp(18px, 1.9vw, 22px));
  right: calc(-50% + clamp(18px, 1.9vw, 22px));
  height: 2px;
  background: rgba(255, 255, 255, 0.18);
  z-index: 1;
}

.step-nav__item--current .step-nav__circle {
  background: rgba(0, 173, 239, 0.18);
  border-color: var(--c-brand-cyan);
  color: #fff;
  box-shadow: 0 0 0 4px rgba(0, 173, 239, 0.16);
}

.step-nav__item--current .step-nav__label {
  color: var(--c-brand-cyan);
}

.step-nav__item--done .step-nav__circle {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
}

.step-nav__item--done .step-nav__connector {
  background: var(--c-brand-cyan);
}

.step-nav__item--done .step-nav__label {
  color: rgba(255, 255, 255, 0.85);
}

.step-nav__tick {
  width: 14px;
  height: 14px;
}

@media (max-width: 479px) {
  .step-nav__label { display: none; }
}
</style>
