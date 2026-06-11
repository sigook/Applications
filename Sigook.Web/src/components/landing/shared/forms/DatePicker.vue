<template>
  <label class="landing-datepicker" :class="{ 'landing-datepicker--error': !!error }">
    <span v-if="label" class="landing-datepicker__label">
      {{ label }}<span v-if="required" class="landing-datepicker__required">*</span>
    </span>
    <input
      type="date"
      :value="isoValue"
      :max="maxIso"
      :min="minIso"
      :name="name"
      :disabled="disabled"
      class="landing-datepicker__control"
      @change="onChange"
    />
    <span v-if="error" class="landing-datepicker__error">{{ error }}</span>
  </label>
</template>

<script setup lang="ts">
import { computed } from 'vue'

/**
 * DatePicker — wraps the native HTML5 date input with V2 glass styling.
 *
 * Accepts/emits Date objects. Native input gives us calendar UI for free,
 * mobile keyboards, and a11y; we just need to translate Date <-> ISO string.
 */
const props = withDefaults(defineProps<{
  modelValue?: Date | null
  label?: string
  required?: boolean
  disabled?: boolean
  name?: string
  error?: string
  max?: Date | null
  min?: Date | null
}>(), {
  required: false,
  disabled: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: Date | null): void
}>()

function toIso(d: Date | null | undefined): string {
  if (!d) return ''
  const yyyy = d.getFullYear()
  const mm = String(d.getMonth() + 1).padStart(2, '0')
  const dd = String(d.getDate()).padStart(2, '0')
  return `${yyyy}-${mm}-${dd}`
}

const isoValue = computed(() => toIso(props.modelValue))
const maxIso = computed(() => toIso(props.max))
const minIso = computed(() => toIso(props.min))

function onChange(event: Event): void {
  const raw = (event.target as HTMLInputElement).value
  if (!raw) {
    emit('update:modelValue', null)
    return
  }
  const [yyyy, mm, dd] = raw.split('-').map(Number)
  emit('update:modelValue', new Date(yyyy, mm - 1, dd))
}
</script>

<style scoped>
.landing-datepicker {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-family: var(--font-family);
}

.landing-datepicker__label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
}

.landing-datepicker__required {
  color: var(--c-brand-red);
  margin-left: 4px;
}

.landing-datepicker__control {
  width: 100%;
  height: clamp(44px, 4.4vw, 48px);
  padding: 0 14px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.22);
  border-radius: 12px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 500;
  outline: none;
  color-scheme: dark;
  cursor: pointer;
  transition: background 0.25s ease, border-color 0.25s ease, box-shadow 0.25s ease;
}

.landing-datepicker__control::-webkit-calendar-picker-indicator {
  filter: invert(0.8);
  cursor: pointer;
}

.landing-datepicker__control:hover {
  background: rgba(255, 255, 255, 0.10);
  border-color: rgba(255, 255, 255, 0.36);
}

.landing-datepicker__control:focus {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 3px rgba(0, 173, 239, 0.20);
}

.landing-datepicker__control:disabled {
  opacity: 0.55;
  cursor: not-allowed;
}

.landing-datepicker--error .landing-datepicker__control {
  border-color: var(--c-brand-red);
}

.landing-datepicker__error {
  font-size: 11px;
  font-weight: 600;
  color: var(--c-brand-red);
  letter-spacing: 0.02em;
}
</style>
