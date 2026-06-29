<template>
  <a :href="href" class="scroll-indicator" :aria-label="ariaLabel">
    <span class="scroll-indicator__lbl"><slot>Scroll</slot></span>
    <span class="scroll-indicator__line" aria-hidden="true"></span>
  </a>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  href?: string
  ariaLabel?: string
}>(), {
  href: '#',
  ariaLabel: 'Scroll to next section',
})
</script>

<style scoped>
.scroll-indicator {
  display: inline-flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  color: rgba(255, 255, 255, 0.55);
  text-decoration: none;
  font-family: var(--font-family);
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  transition: color 0.25s ease;
}

.scroll-indicator:hover {
  color: var(--c-brand-cyan);
}

.scroll-indicator__line {
  width: 1px;
  height: 36px;
  background: linear-gradient(180deg, currentColor, transparent);
  position: relative;
  overflow: hidden;
}

.scroll-indicator__line::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 30%;
  background: var(--c-brand-cyan);
  animation: scroll-tick 2s ease-in-out infinite;
}

@keyframes scroll-tick {
  0%   { transform: translateY(-100%); opacity: 0; }
  20%  { opacity: 1; }
  100% { transform: translateY(280%); opacity: 0; }
}

@media (prefers-reduced-motion: reduce) {
  .scroll-indicator__line::after { animation: none; }
}
</style>
