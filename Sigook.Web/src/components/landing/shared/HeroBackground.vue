<template>
  <div class="hero-bg" aria-hidden="true">
    <img :src="image" alt="" class="hero-bg__img" :style="{ objectPosition: focal }" />
    <div class="hero-bg__scrim"></div>
    <div class="hero-bg__fade"></div>
  </div>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  image: string
  focal?: string
}>(), {
  focal: 'center',
})
</script>

<style scoped>
.hero-bg {
  position: absolute;
  inset: 0;
  z-index: 0;
  overflow: hidden;
  pointer-events: none;
}

.hero-bg__img {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  filter: saturate(0.92) brightness(0.86);
  transform: scale(1.06);
  animation: hero-bg-zoom 26s ease-in-out infinite alternate;
}

@keyframes hero-bg-zoom {
  from { transform: scale(1.06) translate3d(0, 0, 0); }
  to   { transform: scale(1.16) translate3d(0, -1.5%, 0); }
}

.hero-bg__scrim {
  position: absolute;
  inset: 0;
  background:
    radial-gradient(70% 55% at 80% 10%,
      rgba(0, 173, 239, 0.18) 0%,
      transparent 60%),
    linear-gradient(180deg,
      rgba(9, 32, 52, 0.74) 0%,
      rgba(9, 32, 52, 0.55) 42%,
      rgba(9, 32, 52, 0.66) 72%,
      rgba(15, 47, 68, 0.95) 100%);
}

.hero-bg__fade {
  position: absolute;
  left: 0;
  right: 0;
  bottom: 0;
  height: 28%;
  background: linear-gradient(180deg, transparent 0%, #0f2f44 100%);
}

@media (prefers-reduced-motion: reduce) {
  .hero-bg__img {
    animation: none;
    transform: scale(1.04);
  }
}
</style>
