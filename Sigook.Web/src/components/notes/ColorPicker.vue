<template>
    <div class="color-picker">
        <button @mouseover="showColors = true"
                @mouseleave="showColors = false"
                id="button-select-color" class="relative">
            <img src="../../assets/images/palette.png" alt="select color button">
            <transition name="fade">
                <ul class="colors-container" v-if="showColors">
                    <li class="color-item"
                        v-for="(color, index) in colors"
                        :key="'color' + index"
                        :class="{'active': color === selected}"
                        @click="onSelectColor(color)">
                        <span :style="{background: color}" class="dot-color"></span>
                    </li>
                </ul>
            </transition>
        </button>
    </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';

const emit = defineEmits<{ (e: 'onSelectColor', color: string): void }>();

const showColors = ref(false);
const selected = ref('#fefefe');
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
  selected.value = color;
  emit('onSelectColor', color);
}
</script>
