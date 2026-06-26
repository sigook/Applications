<template>
  <div class="labeled-chip-list">
    <span class="labeled-chip-list__label">{{ label }}</span>
    <div class="labeled-chip-list__chips">
      <span
        v-for="item in items"
        :key="item"
        class="labeled-chip-list__chip"
      >{{ item }}</span>

      <router-link
        v-if="moreLabel && moreTo"
        :to="moreTo"
        class="labeled-chip-list__chip labeled-chip-list__chip--more labeled-chip-list__chip--link"
      >{{ moreLabel }}</router-link>
      <span
        v-else-if="moreLabel"
        class="labeled-chip-list__chip labeled-chip-list__chip--more"
      >{{ moreLabel }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  label: string
  items: readonly string[]
  moreLabel?: string
  moreTo?: string
}>(), {
  moreLabel: undefined,
  moreTo: undefined,
})
</script>

<style scoped>
.labeled-chip-list {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(10px, 1vw, 14px);
}

.labeled-chip-list__label {
  font-family: var(--font-family);
  font-size: clamp(10px, 0.8vw, 11px);
  font-weight: 600;
  line-height: 1.2;
  color: rgba(255, 255, 255, 0.55);
  letter-spacing: 0.22em;
  text-transform: uppercase;
}

.labeled-chip-list__chips {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: clamp(8px, 0.8vw, 10px);
  max-width: 720px;
}

.labeled-chip-list__chip {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: clamp(6px, 0.6vw, 8px) clamp(14px, 1.4vw, 18px);
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.18);
  border-radius: 999px;
  backdrop-filter: blur(10px) saturate(140%);
  -webkit-backdrop-filter: blur(10px) saturate(140%);
  font-family: var(--font-family);
  font-size: clamp(12px, 1vw, 13px);
  font-weight: 500;
  line-height: 1;
  color: rgba(255, 255, 255, 0.88);
  letter-spacing: 0.02em;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    transform 0.25s ease;
}

.labeled-chip-list__chip::before {
  content: '';
  width: clamp(5px, 0.5vw, 6px);
  height: clamp(5px, 0.5vw, 6px);
  border-radius: 50%;
  background: var(--c-brand-cyan);
  flex-shrink: 0;
}

.labeled-chip-list__chip:hover {
  background: rgba(255, 255, 255, 0.10);
  border-color: rgba(255, 255, 255, 0.30);
  transform: translateY(-1px);
}

.labeled-chip-list__chip--more {
  background: transparent;
  border-color: rgba(255, 255, 255, 0.14);
  color: rgba(255, 255, 255, 0.60);
}

.labeled-chip-list__chip--more::before {
  background: rgba(255, 255, 255, 0.40);
}

.labeled-chip-list__chip--link {
  text-decoration: none;
  cursor: pointer;
}

.labeled-chip-list__chip--link:hover {
  background: rgba(0, 173, 239, 0.12);
  border-color: rgba(0, 173, 239, 0.55);
  color: #fff;
  transform: translateY(-1px);
}

.labeled-chip-list__chip--link:hover::before {
  background: var(--c-brand-cyan);
}

@media (max-width: 1023px) {
  .labeled-chip-list__chip {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    background: rgba(255, 255, 255, 0.16);
  }

  .labeled-chip-list__chip--more {
    background: transparent;
  }
}
</style>
