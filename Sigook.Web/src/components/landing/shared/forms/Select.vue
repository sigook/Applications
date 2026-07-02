<template>
  <label class="landing-select" :class="{ 'landing-select--error': !!error }">
    <span v-if="label" class="landing-select__label">
      {{ label }}<span v-if="required" class="landing-select__required">*</span>
    </span>
    <b-select
      class="landing-select__control"
      :model-value="selectedKey"
      :name="name"
      :disabled="disabled"
      expanded
      @update:model-value="onKeyChange"
    >
      <option v-if="placeholder" value="" disabled>{{ placeholder }}</option>
      <option
        v-for="(opt, idx) in options"
        :key="getKey(opt, idx)"
        :value="getKey(opt, idx)"
      >{{ getLabel(opt) }}</option>
    </b-select>
    <span v-if="error" class="landing-select__error">{{ error }}</span>
  </label>
</template>

<script setup lang="ts" generic="T">
import { computed } from 'vue'

const props = withDefaults(defineProps<{
  modelValue?: T | null
  options: readonly T[]
  label?: string
  placeholder?: string
  required?: boolean
  disabled?: boolean
  name?: string
  error?: string
  optionKey?: keyof T & string
  optionLabel?: keyof T & string
}>(), {
  required: false,
  disabled: false,
  optionKey: 'id' as never,
  optionLabel: 'value' as never,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: T | null): void
}>()

function getKey(option: T, idx: number): string {
  const k = (option as unknown as Record<string, unknown>)[props.optionKey]
  return k != null ? String(k) : String(idx)
}

function getLabel(option: T): string {
  const v = (option as unknown as Record<string, unknown>)[props.optionLabel]
  return v != null ? String(v) : ''
}

const selectedKey = computed(() => {
  if (!props.modelValue) return ''
  return String((props.modelValue as Record<string, unknown>)[props.optionKey] ?? '')
})

function onKeyChange(key: string): void {
  const found = props.options.find((opt, idx) => getKey(opt, idx) === key) ?? null
  emit('update:modelValue', found)
}
</script>

<style scoped>
.landing-select {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-family: var(--font-family);
}

.landing-select__label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
}

.landing-select__required {
  color: var(--c-brand-red);
  margin-left: 4px;
}

.landing-select :deep(.control),
.landing-select :deep(.select) {
  width: 100%;
  height: auto;
  max-width: 100%;
}

.landing-select :deep(.select:not(.is-multiple):not(.is-loading)::after) {
  display: none;
}

.landing-select :deep(.select select) {
  appearance: none;
  width: 100%;
  height: clamp(44px, 4.4vw, 48px);
  padding: 0 38px 0 14px;
  background-color: var(--c-glass-fill);
  border: 1px solid var(--c-glass-border);
  border-radius: 12px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 500;
  outline: none;
  cursor: pointer;
  background-image:
    linear-gradient(45deg, transparent 50%, rgba(255, 255, 255, 0.55) 50%),
    linear-gradient(135deg, rgba(255, 255, 255, 0.55) 50%, transparent 50%);
  background-position: calc(100% - 20px) 50%, calc(100% - 14px) 50%;
  background-size: 6px 6px, 6px 6px;
  background-repeat: no-repeat;
  box-shadow: none;
  transition:
    background-color 0.25s ease,
    border-color 0.25s ease,
    box-shadow 0.25s ease;
}

.landing-select :deep(.select select option) {
  background-color: var(--c-brand-navy);
  color: #fff;
}

.landing-select :deep(.select select:hover) {
  background-color: var(--c-glass-fill-strong);
  border-color: var(--c-glass-border-hover);
}

.landing-select :deep(.select select:focus) {
  background-color: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: var(--focus-ring-cyan);
}

.landing-select :deep(.select select:disabled) {
  opacity: 0.55;
  cursor: not-allowed;
}

.landing-select--error :deep(.select select) {
  border-color: var(--c-brand-red);
}

.landing-select__error {
  font-size: 11px;
  font-weight: 600;
  color: var(--c-brand-red);
  letter-spacing: 0.02em;
}
</style>
