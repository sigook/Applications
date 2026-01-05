<template>
    <div class="page-not-found">
        <h2 class="text-center color-primary">404</h2>
        <p class="text-center">{{ $t("PageNotFound") }}</p>
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
}

.main-container.path-not-found {
    min-height: auto;

    &>div {
        min-height: calc(100vh - 210px);
    }
}

@media(max-width: 767px) {
    .main-container.path-not-found .white-container {
        background-color: white;
    }
}</style>