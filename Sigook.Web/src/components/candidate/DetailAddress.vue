<template>
    <a :href="'https://www.google.ca/maps/place/' + url" target="_blank" class="fz-2 d-block color-gray-dark">
        <i>{{ user.address }} <span class="text-uppercase">{{user.postalCode}}</span></i>
    </a>
</template>
<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{ user: any }>();

const url = computed(() => {
    if (props.user.postalCode) {
        return props.user.postalCode;
    } else {
        return props.user.address;
    }
});
</script>
