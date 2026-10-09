<template>
    <span :style="{ background: 'rgb(' + backgroundColor + ')' }" class="container-default-image">
        <span>{{ name ? name.substring(0, 2) : '' }}</span>
    </span>
</template>
<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{ name?: string }>();

const purple = '156,39,176';
const indigo = '63,81,181';
const lightBlue = '3,169,244';
const teal = '0,150,136';
const lime = '205,220,57';
const amber = '255,193,7';

const backgroundColor = computed(() => {
  const letter = props.name ? props.name.charAt(props.name.length - 1) : 'a';
  if (letter < 'd') return purple;
  if (letter < 'h') return indigo;
  if (letter < 'l') return lightBlue;
  if (letter < 'p') return lightBlue;
  if (letter < 't') return teal;
  if (letter < 'w') return lime;
  return amber;
});
</script>
<style lang="scss" scoped>
.container-default-image {
    //width: 100%;
    //height: 100%;
    min-width: 40px;
    min-height: 40px;
    border-radius: 50%;
    margin: 0 auto;
    display: inline-flex;
    justify-content: center;
    align-items: center;

    span {
        color: white;
        font-size: 16px;
        text-align: center;
        text-transform: uppercase;
        font-weight: normal;
    }

    &.img-30 {
        min-width: 30px;
        min-height: 30px;

        span {
            font-size: 14px;
        }
    }
}
</style>
