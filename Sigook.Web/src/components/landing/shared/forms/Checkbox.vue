<template>
  <b-checkbox
    class="landing-checkbox"
    :model-value="modelValue"
    :native-value="nativeValue"
    :disabled="disabled"
    @update:model-value="$emit('update:modelValue', $event)"
  >
    <slot />
  </b-checkbox>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  modelValue?: boolean | unknown[]
  nativeValue?: unknown
  disabled?: boolean
}>(), {
  disabled: false,
})

defineEmits<{
  (e: 'update:modelValue', value: boolean | unknown[]): void
}>()
</script>

<style scoped>
.landing-checkbox {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  color: rgba(255, 255, 255, 0.85);
  cursor: pointer;
  user-select: none;
}

.landing-checkbox.checkbox :deep(input[type="checkbox"] + .check) {
  width: 18px;
  height: 18px;
  border-radius: 5px;
  background: var(--c-glass-fill);
  border: 1px solid rgba(255, 255, 255, 0.30);
  transition: background 0.2s ease, border-color 0.2s ease;
  flex-shrink: 0;
}

.landing-checkbox.checkbox:hover :deep(input[type="checkbox"] + .check) {
  background: rgba(255, 255, 255, 0.12);
  border-color: rgba(255, 255, 255, 0.50);
}

.landing-checkbox.checkbox :deep(input[type="checkbox"]:checked + .check) {
  background: var(--c-brand-cyan)
    url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16'%3E%3Cpath fill='none' stroke='%230f2f44' stroke-width='2.6' stroke-linecap='round' stroke-linejoin='round' d='M3 8.4l3.3 3.3L13 4.4'/%3E%3C/svg%3E")
    no-repeat center / 12px 12px;
  border-color: var(--c-brand-cyan);
}

.landing-checkbox :deep(.control-label) {
  padding-left: 0;
  line-height: 1.3;
  color: rgba(255, 255, 255, 0.85);
}

.landing-checkbox.is-disabled {
  opacity: 0.55;
  cursor: not-allowed;
}
</style>
