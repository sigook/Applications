<template>
  <div class="landing-autocomplete" :class="{ 'landing-autocomplete--focused': focused, 'landing-autocomplete--error': !!error }">
    <label v-if="label" class="landing-autocomplete__label">
      {{ label }}<span v-if="required" class="landing-autocomplete__required">*</span>
    </label>

    <input
      ref="inputRef"
      :value="modelValue"
      type="text"
      class="landing-autocomplete__control"
      :placeholder="placeholder"
      :disabled="disabled"
      @input="onInput"
      @focus="onFocus"
      @blur="onBlur"
    />

    <ul v-if="open" class="landing-autocomplete__dropdown">
      <li
        v-for="(opt, idx) in filtered"
        :key="getKey(opt, idx)"
        class="landing-autocomplete__option"
        @mousedown.prevent="select(opt)"
      >{{ getLabel(opt) }}</li>
      <li
        v-if="filtered.length === 0 && !allowAddFooter"
        class="landing-autocomplete__option landing-autocomplete__option--empty"
      >No results</li>
      <li
        v-if="allowAddFooter"
        class="landing-autocomplete__option landing-autocomplete__option--footer"
        @mousedown.prevent="onAddNew"
      >
        <span class="landing-autocomplete__plus">+</span>
        <span>Add "{{ modelValue }}"</span>
      </li>
    </ul>

    <span v-if="error" class="landing-autocomplete__error">{{ error }}</span>
  </div>
</template>

<script setup lang="ts" generic="T">
import { ref, computed } from 'vue'

const props = withDefaults(defineProps<{
  modelValue?: string
  data: readonly T[]
  label?: string
  placeholder?: string
  required?: boolean
  disabled?: boolean
  error?: string
  loading?: boolean
  optionKey?: keyof T & string
  optionLabel?: keyof T & string
  allowAdd?: boolean
}>(), {
  required: false,
  disabled: false,
  loading: false,
  optionKey: 'id' as never,
  optionLabel: 'value' as never,
  allowAdd: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'select', option: T): void
  (e: 'addNew', raw: string): void
}>()

const inputRef = ref<HTMLInputElement | null>(null)
const focused = ref(false)

const open = computed(() => focused.value && !props.disabled)

const filtered = computed<readonly T[]>(() => {
  const q = (props.modelValue || '').trim().toLowerCase()
  if (!q) return props.data
  return props.data.filter((opt) =>
    getLabel(opt).toLowerCase().includes(q),
  )
})

const allowAddFooter = computed(() =>
  props.allowAdd && (props.modelValue || '').trim().length > 0,
)

function getKey(opt: T, idx: number): string {
  const k = (opt as unknown as Record<string, unknown>)[props.optionKey]
  return k != null ? String(k) : String(idx)
}

function getLabel(opt: T): string {
  const v = (opt as unknown as Record<string, unknown>)[props.optionLabel]
  return v != null ? String(v) : ''
}

function onInput(event: Event): void {
  emit('update:modelValue', (event.target as HTMLInputElement).value)
}

function onFocus(): void {
  focused.value = true
}

function onBlur(): void {
  setTimeout(() => { focused.value = false }, 120)
}

function select(opt: T): void {
  emit('update:modelValue', getLabel(opt))
  emit('select', opt)
  inputRef.value?.blur()
}

function onAddNew(): void {
  if (!props.modelValue) return
  emit('addNew', props.modelValue.trim())
  inputRef.value?.blur()
}
</script>

<style scoped>
.landing-autocomplete {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-family: var(--font-family);
}

.landing-autocomplete__label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
}

.landing-autocomplete__required {
  color: var(--c-brand-red);
  margin-left: 4px;
}

.landing-autocomplete__control {
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
  transition: background 0.25s ease, border-color 0.25s ease, box-shadow 0.25s ease;
}

.landing-autocomplete__control::placeholder {
  color: rgba(255, 255, 255, 0.40);
}

.landing-autocomplete__control:hover {
  background: rgba(255, 255, 255, 0.10);
  border-color: rgba(255, 255, 255, 0.36);
}

.landing-autocomplete__control:focus {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 3px rgba(0, 173, 239, 0.20);
}

.landing-autocomplete--error .landing-autocomplete__control {
  border-color: var(--c-brand-red);
}

.landing-autocomplete__dropdown {
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  right: 0;
  z-index: 20;
  list-style: none;
  margin: 0;
  padding: 6px;
  background: var(--c-brand-navy);
  border: 1px solid rgba(255, 255, 255, 0.22);
  border-radius: 12px;
  max-height: 220px;
  overflow-y: auto;
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.40);
}

.landing-autocomplete__option {
  padding: 8px 12px;
  border-radius: 8px;
  color: rgba(255, 255, 255, 0.85);
  font-size: 13px;
  cursor: pointer;
  transition: background 0.15s ease;
  display: flex;
  align-items: center;
  gap: 8px;
}

.landing-autocomplete__option:hover {
  background: rgba(0, 173, 239, 0.20);
  color: #fff;
}

.landing-autocomplete__option--empty {
  color: rgba(255, 255, 255, 0.45);
  cursor: default;
  font-style: italic;
}

.landing-autocomplete__option--empty:hover {
  background: transparent;
}

.landing-autocomplete__option--footer {
  border-top: 1px solid rgba(255, 255, 255, 0.10);
  margin-top: 4px;
  padding-top: 10px;
  color: var(--c-brand-cyan);
  font-weight: 600;
}

.landing-autocomplete__plus {
  font-size: 18px;
  line-height: 1;
}

.landing-autocomplete__error {
  font-size: 11px;
  font-weight: 600;
  color: var(--c-brand-red);
  letter-spacing: 0.02em;
}
</style>
