<template>
  <label class="landing-input" :class="{ 'landing-input--error': !!error, 'landing-input--filled': !!modelValue }">
    <span v-if="label" class="landing-input__label">
      {{ label }}<span v-if="required" class="landing-input__required">*</span>
    </span>
    <b-input
      class="landing-input__field"
      :type="type"
      :model-value="modelValue"
      :placeholder="placeholder"
      :name="name"
      :autocomplete="autocomplete"
      :maxlength="maxlength"
      :has-counter="false"
      :disabled="disabled"
      @update:model-value="onInput"
      @blur="$emit('blur')"
    />
    <span v-if="error" class="landing-input__error">{{ error }}</span>
  </label>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  modelValue?: string | number
  label?: string
  type?: 'text' | 'email' | 'password' | 'tel' | 'number'
  placeholder?: string
  required?: boolean
  disabled?: boolean
  name?: string
  autocomplete?: string
  maxlength?: number
  error?: string
}>(), {
  type: 'text',
  required: false,
  disabled: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'blur'): void
}>()

function onInput(value: string | number | null): void {
  emit('update:modelValue', value == null ? '' : String(value))
}
</script>

<style scoped>
.landing-input {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-family: var(--font-family);
}

.landing-input__label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
}

.landing-input__required {
  color: var(--c-brand-red);
  margin-left: 4px;
}

.landing-input__field {
  width: 100%;
}

.landing-input :deep(.control) {
  width: 100%;
}

.landing-input :deep(.input) {
  width: 100%;
  height: clamp(44px, 4.4vw, 48px);
  padding: 0 14px;
  background: var(--c-glass-fill);
  border: 1px solid var(--c-glass-border);
  border-radius: 12px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 500;
  outline: none;
  box-shadow: none;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    box-shadow 0.25s ease;
}

.landing-input :deep(.input)::placeholder {
  color: rgba(255, 255, 255, 0.40);
}

.landing-input :deep(.input):hover {
  background: var(--c-glass-fill-strong);
  border-color: var(--c-glass-border-hover);
}

.landing-input :deep(.input):focus {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: var(--focus-ring-cyan);
}

.landing-input :deep(.input):disabled {
  opacity: 0.55;
  cursor: not-allowed;
}

.landing-input--error :deep(.input) {
  border-color: var(--c-brand-red);
}

.landing-input--error :deep(.input):focus {
  box-shadow: var(--focus-ring-red);
}

.landing-input__error {
  font-size: 11px;
  font-weight: 600;
  color: var(--c-brand-red);
  letter-spacing: 0.02em;
}
</style>
