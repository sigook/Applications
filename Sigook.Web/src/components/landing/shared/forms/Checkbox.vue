<template>
  <label class="landing-checkbox" :class="{ 'landing-checkbox--checked': checked }">
    <input
      type="checkbox"
      class="landing-checkbox__input"
      :checked="checked"
      :disabled="disabled"
      @change="onChange"
    />
    <span class="landing-checkbox__box" aria-hidden="true">
      <LandingIcon v-if="checked" name="check-sm" class="landing-checkbox__tick" />
    </span>
    <span class="landing-checkbox__label">
      <slot />
    </span>
  </label>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import LandingIcon from '@/components/landing/shared/LandingIcon.vue'

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

.landing-checkbox__input {
  position: absolute;
  width: 1px;
  height: 1px;
  opacity: 0;
  pointer-events: none;
}

.landing-checkbox__box {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 18px;
  height: 18px;
  border-radius: 5px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.30);
  color: var(--c-brand-navy);
  transition: background 0.2s ease, border-color 0.2s ease;
  flex-shrink: 0;
}

.landing-checkbox:hover .landing-checkbox__box {
  background: rgba(255, 255, 255, 0.12);
  border-color: rgba(255, 255, 255, 0.50);
}

.landing-checkbox--checked .landing-checkbox__box {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
}

.landing-checkbox__tick {
  width: 14px;
  height: 14px;
}

.landing-checkbox__label {
  line-height: 1.3;
}

.landing-checkbox:has(.landing-checkbox__input:disabled) {
  opacity: 0.55;
  cursor: not-allowed;
}
</style>
