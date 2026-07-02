<template>
  <div class="landing-taginput">
    <label v-if="label" class="landing-taginput__label">
      {{ label }}<span v-if="required" class="landing-taginput__required">*</span>
    </label>

    <b-taginput
      class="landing-taginput__field"
      :model-value="modelValue"
      :data="availableOptions"
      :field="optionLabel"
      :placeholder="modelValue.length === 0 ? placeholder : ''"
      :disabled="disabled"
      :allow-new="allowNew"
      :create-tag="adaptCreateTag"
      :confirm-keys="['Enter']"
      autocomplete
      open-on-focus
      @update:model-value="$emit('update:modelValue', $event)"
      @typing="$emit('typing', $event)"
    />

    <span v-if="error" class="landing-taginput__error">{{ error }}</span>
  </div>
</template>

<script setup lang="ts" generic="T">
import { computed } from 'vue'

const props = withDefaults(defineProps<{
  modelValue: T[]
  options: readonly T[]
  label?: string
  placeholder?: string
  required?: boolean
  disabled?: boolean
  error?: string
  optionKey?: keyof T & string
  optionLabel?: keyof T & string
  allowNew?: boolean
  createTag?: (raw: string) => T
}>(), {
  required: false,
  disabled: false,
  optionKey: 'id' as never,
  optionLabel: 'value' as never,
  allowNew: false,
})

defineEmits<{
  (e: 'update:modelValue', value: T[]): void
  (e: 'typing', value: string): void
}>()

function getKey(opt: T): string {
  const k = (opt as unknown as Record<string, unknown>)[props.optionKey]
  return k != null ? String(k) : ''
}

const availableOptions = computed<readonly T[]>(() =>
  props.options.filter(
    (opt) => !props.modelValue.some((sel) => getKey(sel) === getKey(opt)),
  ),
)

function adaptCreateTag(raw: string | T): T {
  if (typeof raw === 'string') {
    return props.createTag ? props.createTag(raw) : (raw as unknown as T)
  }
  return raw
}
</script>

<style scoped>
.landing-taginput {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-family: var(--font-family);
}

.landing-taginput__label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
}

.landing-taginput__required {
  color: var(--c-brand-red);
  margin-left: 4px;
}

.landing-taginput__field :deep(.taginput-container) {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 6px;
  padding: 8px 12px;
  min-height: clamp(44px, 4.4vw, 48px);
  background: var(--c-glass-fill);
  border: 1px solid var(--c-glass-border);
  border-radius: 12px;
  cursor: text;
  box-shadow: none;
  transition: background 0.25s ease, border-color 0.25s ease, box-shadow 0.25s ease;
}

.landing-taginput__field :deep(.taginput-container.is-focused) {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: var(--focus-ring-cyan);
}

.landing-taginput__field :deep(.taginput-container .tag:not(body)) {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  height: auto;
  padding: 4px 6px 4px 12px;
  background: rgba(0, 173, 239, 0.22);
  border: 1px solid rgba(0, 173, 239, 0.55);
  border-radius: 999px;
  color: #fff;
  font-size: 12px;
  font-weight: 600;
  line-height: 1;
}

.landing-taginput__field :deep(.taginput-container .tag .delete) {
  width: 18px;
  height: 18px;
  min-width: 18px;
  min-height: 18px;
  margin: 0;
  background: rgba(255, 255, 255, 0.18);
  border-radius: 50%;
  flex-shrink: 0;
  transition: background 0.2s ease;
}

.landing-taginput__field :deep(.taginput-container .tag .delete:hover) {
  background: rgba(255, 255, 255, 0.35);
}

.landing-taginput__field :deep(.autocomplete) {
  flex: 1;
  min-width: 100px;
}

.landing-taginput__field :deep(.autocomplete .control),
.landing-taginput__field :deep(.autocomplete .input) {
  background: transparent;
  border: 0;
  box-shadow: none;
  height: auto;
  min-height: 0;
  padding: 4px 0;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  outline: none;
}

.landing-taginput__field :deep(.autocomplete .input::placeholder) {
  color: rgba(255, 255, 255, 0.40);
}

.landing-taginput__field :deep(.dropdown-menu) {
  min-width: 100%;
  padding-top: 4px;
}

.landing-taginput__field :deep(.dropdown-content) {
  margin: 0;
  padding: 6px;
  background: var(--c-brand-navy);
  border: 1px solid var(--c-glass-border);
  border-radius: 12px;
  max-height: 220px;
  overflow-y: auto;
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.40);
}

.landing-taginput__field :deep(.dropdown-item) {
  padding: 8px 12px;
  border-radius: 8px;
  color: rgba(255, 255, 255, 0.85);
  font-size: 13px;
  cursor: pointer;
  transition: background 0.15s ease;
}

.landing-taginput__field :deep(.dropdown-item.is-hovered),
.landing-taginput__field :deep(.dropdown-item:hover) {
  background: rgba(0, 173, 239, 0.20);
  color: #fff;
}

.landing-taginput__error {
  font-size: 11px;
  font-weight: 600;
  color: var(--c-brand-red);
  letter-spacing: 0.02em;
}
</style>
