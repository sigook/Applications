<template>
  <label class="v2-checkbox" :class="{ 'v2-checkbox--checked': checked }">
    <input
      type="checkbox"
      class="v2-checkbox__input"
      :checked="checked"
      :disabled="disabled"
      @change="onChange"
    />
    <span class="v2-checkbox__box" aria-hidden="true">
      <svg
        v-if="checked"
        viewBox="0 0 14 14"
        class="v2-checkbox__tick"
        fill="none"
        stroke="currentColor"
        stroke-width="2.5"
        stroke-linecap="round"
        stroke-linejoin="round"
      >
        <polyline points="11 4 6 9 3 6" />
      </svg>
    </span>
    <span class="v2-checkbox__label">
      <slot />
    </span>
  </label>
</template>

<script setup lang="ts">
import { computed } from 'vue'

/**
 * V2Checkbox — supports two modes:
 *   1. Single boolean: bind `v-model` directly.
 *   2. Group-by-value: bind `v-model` to an array + provide `nativeValue`.
 *      Toggling adds/removes `nativeValue` from the array.
 */
const props = withDefaults(defineProps<{
  modelValue?: boolean | unknown[]
  nativeValue?: unknown
  disabled?: boolean
}>(), {
  disabled: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean | unknown[]): void
}>()

const checked = computed<boolean>(() => {
  if (Array.isArray(props.modelValue)) {
    return props.modelValue.includes(props.nativeValue)
  }
  return !!props.modelValue
})

function onChange(event: Event): void {
  const isChecked = (event.target as HTMLInputElement).checked
  if (Array.isArray(props.modelValue)) {
    const next = props.modelValue.filter((v) => v !== props.nativeValue)
    if (isChecked) next.push(props.nativeValue)
    emit('update:modelValue', next)
  } else {
    emit('update:modelValue', isChecked)
  }
}
</script>

<style scoped>
.v2-checkbox {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  color: rgba(255, 255, 255, 0.85);
  cursor: pointer;
  user-select: none;
}

.v2-checkbox__input {
  position: absolute;
  width: 1px;
  height: 1px;
  opacity: 0;
  pointer-events: none;
}

.v2-checkbox__box {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 18px;
  height: 18px;
  border-radius: 5px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.30);
  color: var(--c-brand-navy, #0f2f44);
  transition: background 0.2s ease, border-color 0.2s ease;
  flex-shrink: 0;
}

.v2-checkbox:hover .v2-checkbox__box {
  background: rgba(255, 255, 255, 0.12);
  border-color: rgba(255, 255, 255, 0.50);
}

.v2-checkbox--checked .v2-checkbox__box {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
}

.v2-checkbox__tick {
  width: 14px;
  height: 14px;
}

.v2-checkbox__label {
  line-height: 1.3;
}

.v2-checkbox:has(.v2-checkbox__input:disabled) {
  opacity: 0.55;
  cursor: not-allowed;
}
</style>
