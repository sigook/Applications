<template>
  <V2Input
    :label="label"
    :model-value="formatted"
    :placeholder="placeholder"
    :required="required"
    :error="error"
    name="phone"
    type="tel"
    :maxlength="12"
    autocomplete="tel"
    @update:model-value="onUpdate"
  />
</template>

<script setup lang="ts">
import { computed } from 'vue'
import V2Input from '@/components/v2/landing/shared/forms/V2Input.vue'

/**
 * V2PhoneInput — V2Input wrapper that auto-formats 10-digit North-American
 * phone numbers as "AAA BBB-CCCC".
 */
const props = withDefaults(defineProps<{
  modelValue?: string
  label?: string
  placeholder?: string
  required?: boolean
  error?: string
}>(), {
  label: 'Mobile Number',
  placeholder: '555 555-5555',
  required: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

function format(raw: string): string {
  const digits = (raw || '').replace(/\D/g, '').slice(0, 10)
  const a = digits.slice(0, 3)
  const b = digits.slice(3, 6)
  const c = digits.slice(6, 10)
  if (digits.length <= 3) return a
  if (digits.length <= 6) return `${a} ${b}`
  return `${a} ${b}-${c}`
}

const formatted = computed(() => format(props.modelValue || ''))

function onUpdate(val: string): void {
  emit('update:modelValue', format(val))
}
</script>
