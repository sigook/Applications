<template>
  <b-autocomplete
    :model-value="search"
    :data="filtered"
    field="label"
    :size="size"
    :placeholder="placeholder"
    :loading="loading"
    :clearable="clearable"
    open-on-focus
    expanded
    append-to-body
    @focus="onFocus"
    @blur="onBlur"
    @update:model-value="onModelUpdate"
    @typing="onTyping"
    @select="onSelect"
  >
    <template v-if="belowThreshold" #empty>Type at least {{ minSearchLength }} characters</template>
  </b-autocomplete>
</template>

<script setup lang="ts" generic="V extends string | number">
import { computed, onBeforeUnmount, ref, watch } from 'vue';

interface Option {
  value: V;
  label: string;
}

const props = withDefaults(
  defineProps<{
    modelValue: V | null;
    options: readonly Option[];
    placeholder?: string;
    size?: string;
    loading?: boolean;
    clearable?: boolean;
    remote?: boolean;
    minSearchLength?: number;
  }>(),
  { placeholder: 'Search…', loading: false, clearable: false, remote: false, minSearchLength: 0 }
);

const emit = defineEmits<{
  (e: 'update:modelValue', value: V | null): void;
  (e: 'search', term: string): void;
}>();

const SEARCH_DEBOUNCE_MS = 300;

const search = ref('');
const selectedLabel = ref('');
const isFocused = ref(false);
let debounceTimer: ReturnType<typeof setTimeout> | undefined;

function syncFromModel(): void {
  if (props.modelValue === null) {
    selectedLabel.value = '';
    search.value = '';
    return;
  }
  const match = props.options.find((o) => o.value === props.modelValue);
  if (match) selectedLabel.value = match.label;
  search.value = selectedLabel.value;
}

watch(() => props.modelValue, syncFromModel, { immediate: true });
watch(
  () => props.options,
  () => {
    if (!isFocused.value && props.modelValue !== null) syncFromModel();
  }
);

const belowThreshold = computed(() => {
  const length = search.value.trim().length;
  return props.remote && length < props.minSearchLength;
});

// A term shorter than the threshold never reached the server, so there is nothing to show:
// an empty list keeps the #empty hint visible instead of leaving stale results on screen.
const filtered = computed(() => {
  if (props.remote) return belowThreshold.value ? [] : [...props.options];
  const term = search.value.trim().toLowerCase();
  return props.options.filter((o) => o.label.toLowerCase().includes(term));
});

function emitSearch(term: string): void {
  if (!props.remote) return;
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => emit('search', term), SEARCH_DEBOUNCE_MS);
}

// Fires for programmatic changes too, so it only mirrors text. The one change that must still
// reach the parent is Buefy's clear button: it empties the value without emitting `typing`.
// It is told apart from our own focus reset because that one empties `search` first.
function onModelUpdate(text: string): void {
  const isClearButton = text === '' && search.value !== '';
  search.value = text;
  if (!isClearButton || !props.clearable) return;
  selectedLabel.value = '';
  emit('update:modelValue', null);
}

// Buefy emits `typing` only on a real keystroke, so clearing and searching stay user-driven.
function onTyping(text: string): void {
  search.value = text;
  if (text === '' && props.clearable) emit('update:modelValue', null);
  emitSearch(text);
}

function onSelect(option: Option | null): void {
  if (option) {
    selectedLabel.value = option.label;
    emit('update:modelValue', option.value);
  }
}

function onFocus(): void {
  isFocused.value = true;
  search.value = '';
  // With a minimum search length there is no initial list: the parent only fetches once the
  // user types enough characters, so focus must not trigger an unfiltered request.
  if (props.remote && props.minSearchLength === 0) {
    clearTimeout(debounceTimer);
    emit('search', '');
  }
}

function onBlur(): void {
  isFocused.value = false;
  clearTimeout(debounceTimer);
  syncFromModel();
}

onBeforeUnmount(() => clearTimeout(debounceTimer));
</script>
