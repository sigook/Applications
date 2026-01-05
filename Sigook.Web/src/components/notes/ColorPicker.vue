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
<script>
export default {
    data(){
        return {
            showColors: false,
            selected: '#fefefe',
            colors: [
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
            ]
        }
    },
    methods: {
        onSelectColor(color){
            this.selected = color;
            this.$emit("onSelectColor", color)
        }
    }
}
</script>