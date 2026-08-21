<template>
  <section class="hero" data-aos="fade-in">
    <!-- Fondo Slider -->
    <div class="hero__bg-container">
      <transition-group name="hero-fade">
        <div
          v-for="(slide, index) in slides"
          :key="index"
          class="hero__bg"
          v-show="currentSlide === index"
        >
          <picture>
            <source media="(max-width: 1023px)" :srcset="slide.mobile" />
            <img :src="slide.desktop" alt="Hero background" />
          </picture>
          <div class="hero__overlay"></div>
        </div>
      </transition-group>
    </div>

    <!-- Contenido centrado -->
    <div class="hero__content">
      <!-- Logo -->
      <div class="hero__logo-wrapper">
        <img
          class="hero__logo"
          src="@/assets/images/logo-covenant-white.png"
          alt="Covenant Group Ltd."
        />
      </div>

      <!-- Botón principal -->
      <div class="hero__cta-wrapper">
        <button class="hero__cta" @click="scrollToNext">
          Get Started
        </button>
      </div>

      <!-- Texto inferior con transición -->
      <transition name="text-fade" mode="out-in">
        <p class="hero__subtitle" :key="currentSlide" v-html="slides[currentSlide].text"></p>
      </transition>

      <!-- Dots del slider -->
      <div class="hero__dots">
        <button
          v-for="(slide, index) in slides"
          :key="index"
          class="hero__dot"
          :class="{ 'hero__dot--active': index === currentSlide }"
          @click="setSlide(index)"
          :aria-label="'Go to slide ' + (index + 1)"
        ></button>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue'

// Importar imágenes
import imgDesktop1 from '@/assets/images/hero-team.jpg'
// Asumimos que la 1 no tiene versión móvil específica, usamos la misma o una por defecto
import imgDesktop2 from '@/assets/images/hero-home-desktop-2.jpg'
import imgMobile2 from '@/assets/images/hero-home-movile-2.jpg'
import imgDesktop3 from '@/assets/images/hero-home-desktop-3.jpg'
import imgMobile3 from '@/assets/images/hero-home-movile-3.jpg'

const slides = [
  {
    desktop: imgDesktop1,
    mobile: imgDesktop1, // Usamos la misma si no hay especifica para la 1
    text: 'Behind Every Great Canadian<br />Company is a Great Talent'
  },
  {
    desktop: imgDesktop2,
    mobile: imgMobile2,
    text: 'The new experience in<br /> staffing begins here'
  },
  {
    desktop: imgDesktop3,
    mobile: imgMobile3,
    text: 'Behind Every Great Canadian<br /> Company is a Great Talent'
  }
]

const currentSlide = ref(0)
let slideInterval: ReturnType<typeof setInterval> | null = null

const nextSlide = () => {
  currentSlide.value = (currentSlide.value + 1) % slides.length
}

const setSlide = (index: number) => {
  currentSlide.value = index
  resetTimer()
}

const startTimer = () => {
  slideInterval = setInterval(nextSlide, 5000) // Cambia cada 5s
}

const resetTimer = () => {
  if (slideInterval) clearInterval(slideInterval)
  startTimer()
}

const scrollToNext = () => {
  const el = document.querySelector<HTMLElement>('#for-employers')
  if (el) {
    el.scrollIntoView({ behavior: 'smooth' })
  }
}

onMounted(() => {
  startTimer()
})

onBeforeUnmount(() => {
  if (slideInterval) clearInterval(slideInterval)
})
</script>

<style scoped>
*{
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.hero {
  position: relative;
  min-height: 100vh;
  width: 100%;
  overflow: hidden;
  color: #ffffff;
  background-color: #0F2F44; /* Fondo azul para evitar que se vea el verde del home */
}

/* Contenedor de fondos absolute */
.hero__bg-container {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  z-index: 0;
}

/* Imagen de fondo individual */
.hero__bg {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
}

.hero__bg img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

/* Capa oscura */
.hero__overlay {
  position: absolute;
  inset: 0;
  background: radial-gradient(
      circle at 50% 20%,
      rgba(255, 255, 255, 0.05),
      transparent 55%
    ),
    linear-gradient(to bottom, rgba(0, 0, 0, 0.2), #0F2F44);
}

/* TRANSICIONES DE FONDO (Fade) */
.hero-fade-enter-active,
.hero-fade-leave-active {
  transition: opacity 1s ease-in-out;
}
.hero-fade-enter-from,
.hero-fade-leave-to {
  opacity: 0;
}

/* TRANSICION DE TEXTO */
.text-fade-enter-active,
.text-fade-leave-active {
  transition: opacity 0.5s ease, transform 0.5s ease;
}
.text-fade-enter-from,
.text-fade-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

/* Contenido centrado */
.hero__content {
  position: relative;
  z-index: 1;
  max-width: 540px;
  margin: 0 auto;
  padding-top: 40vh;
  text-align: center;
  /* animation: hero-enter 1s ease-out; <- Eliminado para manejarlo con transitions si se desea, o dejarlo solo al inicio */
}
/* ... resto de estilos iguales ... */

/* Logo */
.hero__logo {
  width: 360px; /* ajusta según tu svg */
  max-width: 80vw;
  display: block;
  margin: 0 auto;
}

.hero__logo-wrapper {
  margin-bottom: 40px;
}

/* Botón */
.hero__cta {
  border: none;
  outline: none;
  padding: 14px 60px;
  border-radius: 999px;
  background: #32d26a;
  color: #ffffff;
  font-weight: 600;
  font-size: 1rem;
  cursor: pointer;
  box-shadow: 0 18px 40px rgba(0, 0, 0, 0.35);
  transition: transform 0.18s ease, box-shadow 0.18s ease,
    background-color 0.18s ease;
}

.hero__cta:hover {
  transform: translateY(-2px);
  background-color: #24a654;
  box-shadow: 0 22px 45px rgba(0, 0, 0, 0.4);
}

/* Texto pequeño */
.hero__subtitle {
  margin-top: 26px;
  margin-bottom: 26px;
  font-size: 0.9rem;
  line-height: 1.6;
  min-height: 3rem; /* Para evitar saltos si cambia el largo del texto */
}

/* Dots */
.hero__dots {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 6px 12px;
  border-radius: 999px;
  background: #D9D9D9;
}

.hero__dot {
  width: 10px;
  height: 10px;
  border-radius: 999px;
  background: #ffffff;
  opacity: 0.6;
  border: none; /* Asegurar que sea botón sin borde */
  cursor: pointer;
  padding: 0;
  transition: all 0.3s ease;
}

.hero__dot--active {
  width: 20px;
  opacity: 1;
  background: #1f2f46;
}

/* Animación de entrada del contenido */
@keyframes hero-enter {
  from {
    opacity: 0;
    transform: translateY(25px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* ===== CENTRAR LOGO + BOTÓN EN TABLETS Y MÓVILES ===== */
@media (max-width: 1024px) {
  /* Centrar todo el contenido del hero horizontalmente */
  .hero__content {
    align-items: center;
    text-align: center;
    padding-top: min(40vh, 280px);
  }
}

/* =============== TABLET ================= */
@media (max-width: 1024px) {

/* 🔧 AJUSTE CENTRADO EN TABLETS */
.hero__inner {
  padding: 110px 6vw 60px; /* menos padding arriba y abajo */
}

.hero__title {
  font-size: 2.4rem;
}

.hero__subtitle {
  max-width: 520px;
  font-size: 0.95rem;
}
}

/* =============== MOBILE (≤ 768px) ================= */
@media (max-width: 768px) {

.hero {
  min-height: 90vh;
}

/* 🔧 AJUSTE CENTRADO EN MÓVIL */
.hero__inner {
  padding: 90px 6vw 50px;
}

.hero__content {
  display: flex !important;
  flex-direction: column;
  align-items: center;
}

.hero__logo-wrapper {
  margin-bottom: 90px; /* Separación clara en móvil */
  width: 100%;
  display: flex;
  justify-content: center;
}

.hero__logo {
  max-width: 340px;
}

.hero__title {
  font-size: 2rem;
}

.hero__subtitle {
  max-width: 420px;
  font-size: 0.9rem;
}

.hero__cta {
  padding-inline: 40px;
  margin-top: 0;
}

.hero__scroll-indicator {
  bottom: 18px;
}
}

/* =============== MOBILE PEQUEÑO (≤ 480px) ================= */
@media (max-width: 480px) {

/* 🔧 ajuste fino para teléfonos más chicos */
.hero__inner {
  padding: 80px 6vw 40px;
}

.hero__title {
  font-size: 1.8rem;
}

.hero__subtitle {
  font-size: 0.85rem;
}
}

</style>
