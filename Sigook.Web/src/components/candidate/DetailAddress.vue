<template>
    <a :href="'https://www.google.ca/maps/place/' + url" target="_blank" class="fz-2 d-block color-gray-dark">
        <i>{{ user.address }} <span class="uppercase">{{user.postalCode}}</span></i>
    </a>
</template>
<script>
export default {
    props: ['user'],
    computed: {
        url() {
            if (this.user.postalCode){
                return this.user.postalCode
            } else {
                return this.user.address
            }
        }
    }
}
</script>