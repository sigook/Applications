<template>
  <div class="v2-taginput" :class="{ 'v2-taginput--focused': focused }">
    <label v-if="label" class="v2-taginput__label">
      {{ label }}<span v-if="required" class="v2-taginput__required">*</span>
    </label>

    <div class="v2-taginput__field" @click="onFieldClick">
      <span
        v-for="(tag, idx) in modelValue"
        :key="getTagKey(tag, idx)"
        class="v2-taginput__chip"
      >
        <span class="v2-taginput__chip-text">{{ getTagLabel(tag) }}</span>
        <button
          type="button"
          class="v2-taginput__chip-remove"
          :aria-label="`Remove ${getTagLabel(tag)}`"
          @click.stop="removeTag(idx)"
        >×</button>
      </span>

      <input
        ref="inputRef"
        v-model="query"
        type="text"
        class="v2-taginput__input"
        :placeholder="modelValue.length === 0 ? placeholder : ''"
        :disabled="disabled"
        @input="onInput"
        @focus="onFocus"
        @blur="onBlur"
        @keydown.enter.prevent="onEnter"
        @keydown.backspace="onBackspace"
      />
    </div>

    <!-- Dropdown of suggestions -->
    <ul v-if="focused && filteredOptions.length > 0" class="v2-taginput__dropdown">
      <li
        v-for="(opt, idx) in filteredOptions"
        :key="getTagKey(opt, idx)"
        class="v2-taginput__option"
        @mousedown.prevent="addTag(opt)"
      >{{ getTagLabel(opt) }}</li>
    </ul>

    <span v-if="error" class="v2-taginput__error">{{ error }}</span>
  </div>
</template>

<script setup lang="ts" generic="T">
import { ref, computed } from 'vue'

/**
 * V2TagInput — multi-select chip input with typeahead.
 *
 * Accepts an array of objects (`T[]`). Each option is keyed by `optionKey`
 * (default 'id') and labeled by `optionLabel` (default 'value'). The parent
 * supplies a filtered `options` list and listens to `@typing` to update it.
 *
 * When `allowNew` is true and the user presses Enter, a new tag is created
 * via the `createTag` factory function.
 */
const props = withDefaults(defineProps<{
  modelValue: T[]
  options: readonly T[]
  label?: string
  placeholder?: string
  required?: boolean
  disabled?: boolean
  error?: string
  /** Property name to use as the option key (default 'id'). */
  optionKey?: keyof T & string
  /** Property name to use as the option label (default 'value'). */
  optionLabel?: keyof T & string
  /** Allow creating new tags on Enter. */
  allowNew?: boolean
  /** Factory called when the user creates a new tag (allowNew=true). */
  createTag?: (raw: string) => T
}>(), {
  required: false,
  disabled: false,
  optionKey: 'id' as never,
  optionLabel: 'value' as never,
  allowNew: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: T[]): void
  (e: 'typing', value: string): void
}>()

const inputRef = ref<HTMLInputElement | null>(null)
const query = ref('')
const focused = ref(false)

function getTagKey(opt: T, idx: number): string {
  const k = (opt as unknown as Record<string, unknown>)[props.optionKey]
  return k != null ? String(k) : `__idx-${idx}`
}

function getTagLabel(opt: T): string {
  const v = (opt as unknown as Record<string, unknown>)[props.optionLabel]
  return v != null ? String(v) : ''
}

const filteredOptions = computed<readonly T[]>(() => {
  return props.options.filter((opt) => {
    // Exclude already-selected tags
    return !props.modelValue.some(
      (sel) => getTagKey(sel, 0) === getTagKey(opt, 0),
    )
  })
})

function onInput(): void {
  emit('typing', query.value)
}

function onFocus(): void {
  focused.value = true
  emit('typing', query.value)
}

function onBlur(): void {
  // Defer so chip clicks (which trigger blur) still register
  setTimeout(() => { focused.value = false }, 120)
}

function onFieldClick(): void {
  inputRef.value?.focus()
}

function addTag(tag: T): void {
  if (props.disabled) return
  emit('update:modelValue', [...props.modelValue, tag])
  query.value = ''
  emit('typing', '')
  inputRef.value?.focus()
}

function removeTag(idx: number): void {
  if (props.disabled) return
  const next = [...props.modelValue]
  next.splice(idx, 1)
  emit('update:modelValue', next)
}

function onEnter(): void {
  if (!query.value.trim()) return
  // First check if the typed text matches an existing filtered option.
  const exact = filteredOptions.value.find(
    (o) => getTagLabel(o).toLowerCase() === query.value.toLowerCase(),
  )
  if (exact) {
    addTag(exact)
    return
  }
  // Fallback to creating a new tag.
  if (props.allowNew && props.createTag) {
    const newTag = props.createTag(query.value.trim())
    if (newTag) addTag(newTag)
  }
}

function onBackspace(): void {
  if (query.value === '' && props.modelValue.length > 0) {
    removeTag(props.modelValue.length - 1)
  }
}
</script>

<style scoped>
.v2-taginput {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-family: var(--font-family);
}

.v2-taginput__label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
}

.v2-taginput__required {
  color: var(--c-brand-red);
  margin-left: 4px;
}

.v2-taginput__field {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 6px;
  padding: 8px 12px;
  min-height: clamp(44px, 4.4vw, 48px);
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.22);
  border-radius: 12px;
  cursor: text;
  transition: background 0.25s ease, border-color 0.25s ease, box-shadow 0.25s ease;
}

.v2-taginput--focused .v2-taginput__field {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 3px rgba(0, 173, 239, 0.20);
}

.v2-taginput__chip {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 6px 4px 12px;
  background: rgba(0, 173, 239, 0.22);
  border: 1px solid rgba(0, 173, 239, 0.55);
  border-radius: 999px;
  color: #fff;
  font-size: 12px;
  font-weight: 600;
  line-height: 1;
}

.v2-taginput__chip-remove {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 18px;
  height: 18px;
  background: rgba(255, 255, 255, 0.18);
  border: 0;
  border-radius: 50%;
  color: #fff;
  font-size: 14px;
  line-height: 1;
  cursor: pointer;
  transition: background 0.2s ease;
}

.v2-taginput__chip-remove:hover {
  background: rgba(255, 255, 255, 0.35);
}

.v2-taginput__input {
  flex: 1;
  min-width: 100px;
  background: transparent;
  border: 0;
  outline: none;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  padding: 4px 0;
}

.v2-taginput__input::placeholder {
  color: rgba(255, 255, 255, 0.40);
}

.v2-taginput__dropdown {
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  right: 0;
  z-index: 20;
  list-style: none;
  margin: 0;
  padding: 6px;
  background: #0f2f44;
  border: 1px solid rgba(255, 255, 255, 0.22);
  border-radius: 12px;
  max-height: 220px;
  overflow-y: auto;
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.40);
}

.v2-taginput__option {
  padding: 8px 12px;
  border-radius: 8px;
  color: rgba(255, 255, 255, 0.85);
  font-size: 13px;
  cursor: pointer;
  transition: background 0.15s ease;
}

.v2-taginput__option:hover {
  background: rgba(0, 173, 239, 0.20);
  color: #fff;
}

.v2-taginput__error {
  font-size: 11px;
  font-weight: 600;
  color: var(--c-brand-red);
  letter-spacing: 0.02em;
}
</style>
