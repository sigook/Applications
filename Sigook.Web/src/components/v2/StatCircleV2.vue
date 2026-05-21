<template>
  <div class="stat-circle-v2">
    <div class="stat-circle-v2__circle">
      <span class="v2-h2 stat-circle-v2__number" :class="{ 'stat-circle-v2__number--accent': accent }">
        {{ number }}
      </span>
      <span class="v2-body stat-circle-v2__label">{{ label }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  number: string
  label: string
  accent?: boolean
}>(), {
  accent: false,
})
</script>

<style scoped>
.stat-circle-v2__circle {
  width: 212px;
  height: 212px;
  border-radius: 50%;
  background: #fff;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.10);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.stat-circle-v2__number {
  display: block;
  color: var(--c-ink);
  margin: 0;
  font-size: var(--text-h2-size);
  font-weight: var(--text-h2-weight);
  line-height: var(--text-h2-lh);
}

.stat-circle-v2__number--accent {
  color: var(--c-brand-red);
}

.stat-circle-v2__label {
  display: block;
  color: var(--c-ink);
  margin: 4px 0 0;
  font-size: var(--text-body-size);
  font-weight: var(--text-body-weight);
  line-height: var(--text-body-lh);
}
</style>
