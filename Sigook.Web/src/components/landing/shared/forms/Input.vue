<template>
  <label class="landing-input" :class="{ 'landing-input--error': !!error, 'landing-input--filled': !!modelValue }">
    <span v-if="label" class="landing-input__label">
      {{ label }}<span v-if="required" class="landing-input__required">*</span>
    </span>
    <input
      :type="type"
      :value="modelValue"
      :placeholder="placeholder"
      :name="name"
      :autocomplete="autocomplete"
      :maxlength="maxlength"
      :disabled="disabled"
      class="landing-input__control"
      @input="onInput"
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

function onInput(event: Event): void {
  emit('update:modelValue', (event.target as HTMLInputElement).value)
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

.landing-input__control {
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

.landing-input__control::placeholder {
  color: rgba(255, 255, 255, 0.40);
}

.landing-input__control:hover {
  background: rgba(255, 255, 255, 0.10);
  border-color: rgba(255, 255, 255, 0.36);
}

.landing-input__control:focus {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 3px rgba(0, 173, 239, 0.20);
}

.landing-input__control:disabled {
  opacity: 0.55;
  cursor: not-allowed;
}

.landing-input--error .landing-input__control {
  border-color: var(--c-brand-red);
}

.landing-input--error .landing-input__control:focus {
  box-shadow: 0 0 0 3px rgba(229, 45, 39, 0.20);
}

.landing-input__error {
  font-size: 11px;
  font-weight: 600;
  color: var(--c-brand-red);
  letter-spacing: 0.02em;
}
</style>
