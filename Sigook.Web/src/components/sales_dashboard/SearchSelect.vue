<template>
  <b-autocomplete
    :model-value="search"
    :data="filtered"
    field="label"
    :size="size"
    :placeholder="placeholder"
    :loading="loading"
    open-on-focus
    expanded
    append-to-body
    @focus="onFocus"
    @blur="onBlur"
    @update:model-value="onModelUpdate"
    @typing="onTyping"
    @select="onSelect"
  ></b-autocomplete>
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
    remote?: boolean;
    minSearchLength?: number;
  }>(),
  { placeholder: 'Search…', loading: false, remote: false, minSearchLength: 0 }
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

// A partial term never reached the server, so there is nothing to show. An empty term keeps
// what is already loaded: clicking an option blurs the input (which resets the term) before
// the click lands, and emptying the list there would unmount the option mid-click.
const filtered = computed(() => {
  if (props.remote) {
    const hasPartialTerm = belowThreshold.value && search.value.trim().length > 0;
    return hasPartialTerm ? [] : [...props.options];
  }
  const term = search.value.trim().toLowerCase();
  return props.options.filter((o) => o.label.toLowerCase().includes(term));
});

function emitSearch(term: string): void {
  if (!props.remote) return;
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => emit('search', term), SEARCH_DEBOUNCE_MS);
}

// Fires for programmatic changes too (selecting an option writes its label), so it only mirrors text.
function onModelUpdate(text: string): void {
  search.value = text;
}

// Buefy emits `typing` only on a real keystroke, so emptying the input stays user-driven:
// wiping the text is what clears the current selection.
function onTyping(text: string): void {
  search.value = text;
  if (text === '') {
    selectedLabel.value = '';
    emit('update:modelValue', null);
  }
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
  // The parent owns the list: for an empty term it either clears stale results (with a
  // minimum search length) or fetches the unfiltered list (without one).
  if (props.remote) {
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
