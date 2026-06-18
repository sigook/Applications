<template>
    <div class="page-not-found">
        <h2 class="text-center color-primary">404</h2>
        <p class="text-center">{{ "The page you’re looking for can’t be found." }}</p>
    </div>
</template>

<style lang="scss">
.page-not-found {
    display: block;
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 100%;

    h2 {
        font-size: 8em;
        font-weight: bold;
        letter-spacing: 2px;
    }

    p {
        font-size: 1.4em;
    }
}</style>