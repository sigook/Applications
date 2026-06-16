<template>
  <label class="landing-switch" :class="{ 'landing-switch--on': modelValue }">
    <input
      type="checkbox"
      class="landing-switch__input"
      :checked="modelValue"
      :disabled="disabled"
      @change="$emit('update:modelValue', ($event.target as HTMLInputElement).checked)"
    />
    <span class="landing-switch__track" aria-hidden="true">
      <span class="landing-switch__thumb"></span>
    </span>
    <span v-if="$slots.default" class="landing-switch__label">
      <slot />
    </span>
  </label>
</template>

<script setup lang="ts">
/**
 * Switch — pill toggle for boolean values.
 *
 * Off  → muted track + thumb on the left.
 * On   → cyan track + thumb on the right.
 */
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

.landing-switch__input {
  position: absolute;
  width: 1px;
  height: 1px;
  opacity: 0;
  pointer-events: none;
}

.landing-switch__track {
  position: relative;
  width: 42px;
  height: 24px;
  background: rgba(255, 255, 255, 0.18);
  border-radius: 999px;
  transition: background 0.25s ease;
  flex-shrink: 0;
}

.landing-switch__thumb {
  position: absolute;
  top: 3px;
  left: 3px;
  width: 18px;
  height: 18px;
  background: #fff;
  border-radius: 50%;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.30);
  transition: transform 0.25s cubic-bezier(0.22, 1, 0.36, 1);
}

.landing-switch--on .landing-switch__track {
  background: var(--c-brand-cyan);
}

.landing-switch--on .landing-switch__thumb {
  transform: translateX(18px);
}

.landing-switch:has(.landing-switch__input:disabled) {
  opacity: 0.55;
  cursor: not-allowed;
}
</style>
