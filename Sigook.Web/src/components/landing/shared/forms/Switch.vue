<template>
  <b-switch
    class="landing-switch"
    :class="{ 'landing-switch--on': modelValue }"
    :model-value="modelValue"
    :disabled="disabled"
    @update:model-value="$emit('update:modelValue', $event)"
  >
    <slot />
  </b-switch>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  modelValue: boolean
  disabled?: boolean
}>(), {
  disabled: false,
})

defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()
</script>

<style scoped>
.landing-switch {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  color: rgba(255, 255, 255, 0.85);
  cursor: pointer;
  user-select: none;
}

.landing-switch :deep(.control-label) {
  padding-left: 0;
  color: rgba(255, 255, 255, 0.85);
}

.landing-switch.switch :deep(input[type="checkbox"] + .check) {
  position: relative;
  width: 42px;
  height: 24px;
  padding: 0;
  background: rgba(255, 255, 255, 0.18);
  border-radius: 999px;
  transition: background 0.25s ease;
  flex-shrink: 0;
}

.landing-switch.switch :deep(input[type="checkbox"] + .check::before) {
  position: absolute;
  top: 3px;
  left: 3px;
  width: 18px;
  height: 18px;
  background: #fff;
  border-radius: 50%;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.30);
  transition: transform 0.25s var(--ease-brand);
}

.landing-switch.switch :deep(input[type="checkbox"]:checked + .check) {
  background: var(--c-brand-cyan);
}

.landing-switch.switch :deep(input[type="checkbox"]:checked + .check::before) {
  transform: translateX(18px);
}

.landing-switch.is-disabled {
  opacity: 0.55;
  cursor: not-allowed;
}
</style>
