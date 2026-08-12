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
import { computed, ref, watch } from 'vue';

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
  }>(),
  { placeholder: 'Search…', loading: false, clearable: false }
);

const emit = defineEmits<{ (e: 'update:modelValue', value: V | null): void }>();

const search = ref('');

function syncFromModel(): void {
  search.value = props.options.find((o) => o.value === props.modelValue)?.label ?? '';
}

watch(() => props.modelValue, syncFromModel, { immediate: true });
watch(
  () => props.options,
  () => {
    if (props.modelValue !== null) syncFromModel();
  }
);

const filtered = computed(() => {
  const term = search.value.toLowerCase();
  return props.options.filter((o) => o.label.toLowerCase().includes(term));
});

function onInput(text: string): void {
  search.value = text;
  if (text === '' && props.clearable) emit('update:modelValue', null);
}

function onSelect(option: Option | null): void {
  if (option) emit('update:modelValue', option.value);
}

function onFocus(): void {
  search.value = '';
}

function onBlur(): void {
  syncFromModel();
}
</script>
