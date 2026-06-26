<template>
  <span class="eyebrow-pill" :class="`eyebrow-pill--${variant}`">
    <slot />
  </span>
</template>

<script setup lang="ts">
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
}

.eyebrow-pill--cyan {
  color: #fff;
  background: var(--c-brand-cyan);
  border: 1px solid var(--c-brand-cyan);
  box-shadow: 0 8px 18px -6px rgba(0, 173, 239, 0.55);
}

.eyebrow-pill--red {
  color: #fff;
  background: var(--c-brand-red);
  border: 1px solid var(--c-brand-red);
  box-shadow: 0 8px 18px -6px rgba(229, 45, 39, 0.55);
}

.eyebrow-pill--white {
  color: var(--c-brand-navy);
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
