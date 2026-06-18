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

<style scoped lang="scss">
.color-picker {
  display: inline-block;
}

#button-select-color {
  border: 0;
  margin: 0 10px 0 0;
  vertical-align: middle;

  img {
    width: 20px;
    height: auto;
  }
}

.colors-container {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  width: 180px;
  padding: 6px;
  position: absolute;
  background: rgba(255, 255, 255, 0.8);
  box-shadow: 0 0 4px #f1f1f1;
  bottom: -75px;
  left: -10px;
  border-radius: 6px;
}

.color-item {
  flex-basis: 25%;

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
