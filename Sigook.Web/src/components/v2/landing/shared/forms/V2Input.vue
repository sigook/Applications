<template>
  <label class="v2-input" :class="{ 'v2-input--error': !!error, 'v2-input--filled': !!modelValue }">
    <span v-if="label" class="v2-input__label">
      {{ label }}<span v-if="required" class="v2-input__required">*</span>
    </span>
    <input
      :type="type"
      :value="modelValue"
      :placeholder="placeholder"
      :name="name"
      :autocomplete="autocomplete"
      :maxlength="maxlength"
      :disabled="disabled"
      class="v2-input__control"
      @input="onInput"
      @blur="$emit('blur')"
    />
    <span v-if="error" class="v2-input__error">{{ error }}</span>
  </label>
</template>

<script setup lang="ts">
/**
 * V2Input — text/email/password input with glass V2 styling.
 *
 * Single-line label-above-field layout. Cyan focus border, white text,
 * subtle hover lift. Error state colors the border and shows the message
 * below the field.
 */
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

function onInput(event: Event): void {
  emit('update:modelValue', (event.target as HTMLInputElement).value)
}
</script>

<style scoped>
.v2-input {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-family: var(--font-family);
}

.v2-input__label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
}

.v2-input__required {
  color: var(--c-brand-red);
  margin-left: 4px;
}

.v2-input__control {
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
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    box-shadow 0.25s ease;
}

.v2-input__control::placeholder {
  color: rgba(255, 255, 255, 0.40);
}

.v2-input__control:hover {
  background: rgba(255, 255, 255, 0.10);
  border-color: rgba(255, 255, 255, 0.36);
}

.v2-input__control:focus {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 3px rgba(0, 173, 239, 0.20);
}

.v2-input__control:disabled {
  opacity: 0.55;
  cursor: not-allowed;
}

.v2-input--error .v2-input__control {
  border-color: var(--c-brand-red);
}

.v2-input--error .v2-input__control:focus {
  box-shadow: 0 0 0 3px rgba(229, 45, 39, 0.20);
}

.v2-input__error {
  font-size: 11px;
  font-weight: 600;
  color: var(--c-brand-red);
  letter-spacing: 0.02em;
}
</style>
