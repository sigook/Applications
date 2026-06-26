<template>
  <label class="landing-fileupload" :class="{ 'landing-fileupload--disabled': disabled }">
    <input
      type="file"
      class="landing-fileupload__input"
      :accept="accept"
      :disabled="disabled"
      @change="onChange"
      ref="inputRef"
    />
    <span class="landing-fileupload__icon" aria-hidden="true">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
        <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
        <polyline points="17 8 12 3 7 8" />
        <line x1="12" y1="3" x2="12" y2="15" />
      </svg>
    </span>
    <span class="landing-fileupload__label">
      <slot>{{ label }}</slot>
    </span>
  </label>
</template>

<script setup lang="ts">
import { ref } from 'vue'

withDefaults(defineProps<{
  label?: string
  accept?: string
  disabled?: boolean
}>(), {
  label: 'Upload file',
  accept: '.pdf,.jpg,.jpeg,.png,.gif,.doc,.docx,.xls,.xlsx',
  disabled: false,
})

const emit = defineEmits<{
  (e: 'file', value: File | null): void
}>()

const inputRef = ref<HTMLInputElement | null>(null)

function onChange(event: Event): void {
  const file = (event.target as HTMLInputElement).files?.[0] ?? null
  emit('file', file)
  if (inputRef.value) inputRef.value.value = ''
}
</script>

<style scoped>
.landing-fileupload {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 10px 22px;
  background: rgba(255, 255, 255, 0.08);
  border: 1.5px dashed rgba(255, 255, 255, 0.45);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.04em;
  cursor: pointer;
  transition: background 0.25s ease, border-color 0.25s ease, transform 0.25s ease;
}

.landing-fileupload:hover {
  background: rgba(0, 173, 239, 0.16);
  border-color: var(--c-brand-cyan);
  transform: translateY(-1px);
}

.landing-fileupload__input {
  position: absolute;
  width: 1px;
  height: 1px;
  opacity: 0;
  pointer-events: none;
}

.landing-fileupload__icon {
  display: inline-flex;
  width: 16px;
  height: 16px;
  color: var(--c-brand-cyan);
}

.landing-fileupload__icon svg {
  width: 100%;
  height: 100%;
}

.landing-fileupload--disabled {
  opacity: 0.45;
  cursor: not-allowed;
}

.landing-fileupload--disabled:hover {
  background: rgba(255, 255, 255, 0.08);
  border-color: rgba(255, 255, 255, 0.45);
  transform: none;
}
</style>
