<template>
  <b-autocomplete
    :model-value="search"
    :data="filtered"
    field="label"
    :placeholder="placeholder"
    :loading="loading"
    :clearable="clearable"
    open-on-focus
    expanded
    append-to-body
    @focus="onFocus"
    @blur="onBlur"
    @update:model-value="onInput"
    @select="onSelect"
  />
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
    loading?: boolean;
    clearable?: boolean;
    remote?: boolean;
  }>(),
  { placeholder: 'Search…', loading: false, clearable: false, remote: false }
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

const filtered = computed(() => {
  if (props.remote) return [...props.options];
  const term = search.value.toLowerCase();
  return props.options.filter((o) => o.label.toLowerCase().includes(term));
});

function emitSearch(term: string): void {
  if (!props.remote) return;
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => emit('search', term), SEARCH_DEBOUNCE_MS);
}

function onInput(text: string): void {
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
