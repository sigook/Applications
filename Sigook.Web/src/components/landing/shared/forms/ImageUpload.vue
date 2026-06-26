<template>
  <div class="landing-imageupload">
    <label class="landing-imageupload__circle" :class="{ 'landing-imageupload__circle--has-image': !!previewUrl }">
      <input
        type="file"
        accept="image/*"
        class="landing-imageupload__input"
        :disabled="disabled"
        @change="onChange"
        ref="inputRef"
      />
      <img v-if="previewUrl" :src="previewUrl" alt="Profile preview" class="landing-imageupload__preview" />
      <span v-else class="landing-imageupload__placeholder" aria-hidden="true">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
          <path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z" />
          <circle cx="12" cy="13" r="4" />
        </svg>
      </span>
      <span class="landing-imageupload__overlay">
        {{ previewUrl ? 'Change photo' : 'Add photo' }}
      </span>
    </label>
    <p v-if="hint" class="landing-imageupload__hint">{{ hint }}</p>
    <p v-if="error" class="landing-imageupload__error">{{ error }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'

const ALLOWED = ['jpeg', 'jpg', 'png', 'gif']
const MAX_SIZE_KB = 10_000

const props = withDefaults(defineProps<{
  initialUrl?: string
  hint?: string
  disabled?: boolean
}>(), {
  disabled: false,
})

const emit = defineEmits<{
  (e: 'file', value: File | null): void
}>()

const inputRef = ref<HTMLInputElement | null>(null)
const previewUrl = ref<string | null>(props.initialUrl ?? null)
const error = ref('')
const ownedUrls: string[] = []

watch(() => props.initialUrl, (url) => {
  previewUrl.value = url ?? null
})

function onChange(event: Event): void {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (!file) return

  const ext = file.name.split('.').pop()?.toLowerCase() ?? ''
  if (!ALLOWED.includes(ext)) {
    error.value = `Allowed: ${ALLOWED.join(', ')}`
    return
  }

  if (file.size / 1024 > MAX_SIZE_KB) {
    error.value = `Max size ${MAX_SIZE_KB / 1000}MB`
    return
  }

  error.value = ''
  const url = URL.createObjectURL(file)
  ownedUrls.push(url)
  previewUrl.value = url
  emit('file', file)
}

onMounted(() => {
})

onUnmounted(() => {
  for (const u of ownedUrls) URL.revokeObjectURL(u)
})
</script>

<style scoped>
.landing-imageupload {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  font-family: var(--font-family);
}

.landing-imageupload__circle {
  position: relative;
  width: clamp(120px, 14vw, 160px);
  height: clamp(120px, 14vw, 160px);
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.06);
  border: 2px dashed rgba(255, 255, 255, 0.40);
  overflow: hidden;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgba(255, 255, 255, 0.55);
  transition: border-color 0.25s ease, background 0.25s ease;
}

.landing-imageupload__circle:hover {
  border-color: var(--c-brand-cyan);
  background: rgba(0, 173, 239, 0.08);
}

.landing-imageupload__circle--has-image {
  border-style: solid;
  border-color: rgba(255, 255, 255, 0.30);
  background: transparent;
}

.landing-imageupload__input {
  position: absolute;
  width: 1px;
  height: 1px;
  opacity: 0;
  pointer-events: none;
}

.landing-imageupload__preview {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.landing-imageupload__placeholder {
  display: inline-flex;
  width: 36%;
  height: 36%;
}

.landing-imageupload__placeholder svg {
  width: 100%;
  height: 100%;
}

.landing-imageupload__overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 6px;
  background: rgba(15, 47, 68, 0.78);
  color: #fff;
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  text-align: center;
  opacity: 0;
  transform: translateY(100%);
  transition: opacity 0.25s ease, transform 0.25s ease;
}

.landing-imageupload__circle:hover .landing-imageupload__overlay {
  opacity: 1;
  transform: translateY(0);
}

.landing-imageupload__hint {
  font-size: 11px;
  color: rgba(255, 255, 255, 0.55);
  text-align: center;
  margin: 0;
  max-width: 220px;
  line-height: 1.4;
}

.landing-imageupload__error {
  font-size: 11px;
  font-weight: 600;
  color: var(--c-brand-red);
  text-align: center;
  margin: 0;
}
</style>
