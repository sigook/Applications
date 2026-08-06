<template>
    <b-dropdown aria-role="list" position="is-top-right" append-to-body>
        <template #trigger>
            <b-button type="is-text" size="is-small" icon-left="palette" class="color-picker-trigger">
                <span class="note-color-icon" :class="{ 'has-border': modelValue === '#fefefe' }"
                    :style="{ backgroundColor: modelValue }"></span>
            </b-button>
        </template>
        <b-dropdown-item custom aria-role="listitem">
            <div class="colors-container">
                <button v-for="color in colors" :key="color" type="button" class="color-item"
                    :class="{ 'active': color === modelValue }" @click="onSelectColor(color)">
                    <span :style="{ background: color }" class="dot-color"></span>
                </button>
            </div>
        </b-dropdown-item>
    </b-dropdown>
</template>
<script setup lang="ts">
defineProps<{ modelValue: string }>();

const emit = defineEmits<{ (e: 'update:modelValue', color: string): void }>();

const colors = [
  '#fefefe',
  '#f28c82',
  '#fcbc05',
  '#fff475',
  '#ccff91',
  '#a7ffeb',
  '#cbf0f9',
  '#aecbfa',
  '#d8aefb',
  '#fdcfe8',
  '#e6c9a8',
  '#e8eaed'
];

function onSelectColor(color: string) {
  emit('update:modelValue', color);
}
</script>

<style scoped lang="scss">
.color-picker-trigger {
  text-decoration: none;
}

.colors-container {
  display: flex;
  flex-wrap: wrap;
  width: 180px;
}

.color-item {
  flex-basis: 25%;
  background: transparent;
  border: 0;
  padding: 3px;
  cursor: pointer;

  .dot-color {
    width: 30px;
    height: 30px;
    display: inline-block;
    border-radius: 50%;
    border: 1px solid #f1f1f1;
  }

  &.active .dot-color {
    border: 2px solid #ff9932;
  }
}
</style>
