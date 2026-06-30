<template>
  <b-upload
    class="landing-fileupload"
    :class="{ 'landing-fileupload--disabled': disabled }"
    :model-value="fileModel"
    :accept="accept"
    :disabled="disabled"
    @update:model-value="onChange"
  >
    <span class="landing-fileupload__icon" aria-hidden="true">
      <LandingIcon name="upload" />
    </span>
    <span class="landing-fileupload__label">
      <slot>{{ label }}</slot>
    </span>
  </b-upload>
</template>

<script setup lang="ts">
import { ref, nextTick } from 'vue'
import LandingIcon from '@/components/landing/shared/icons/LandingIcon.vue'

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

const fileModel = ref<File | File[] | null>(null)

function onChange(value: File | File[] | null): void {
  const file = Array.isArray(value) ? (value[0] ?? null) : (value ?? null)
  fileModel.value = value
  emit('file', file)
  void nextTick(() => {
    fileModel.value = null
  })
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

.landing-fileupload.upload :deep(input[type="file"]) {
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
