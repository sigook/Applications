<template>
    <span :style="{ background: 'rgb(' + backgroundColor + ')' }" class="container-default-image">
        <span>{{ name ? name.substring(0, 2) : '' }}</span>
    </span>
</template>
<script>
export default {
    props: ['name'],
    data() {
        return {
            purple: '156,39,176',
            indigo: '63,81,181',
            lightBlue: '3,169,244',
            teal: '0,150,136',
            lime: '205,220,57',
            amber: '255,193,7'
        }
    },
    computed: {
        backgroundColor() {
            let letter = this.name ? this.name.charAt(this.name.length - 1) : 'a';
            if (letter < 'd') {
                return this.purple;
            } else if (letter < 'h') {
                return this.indigo;
            } else if (letter < 'l') {
                return this.lightBlue;
            } else if (letter < 'p') {
                return this.lightBlue;
            } else if (letter < 't') {
                return this.teal;
            } else if (letter < 'w') {
                return this.lime;
            } else {
                return this.amber;
            }
        }
    }
}

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