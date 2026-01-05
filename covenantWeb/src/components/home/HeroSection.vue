<template>
  <section class="hero" data-aos="fade-in">
    <!-- Fondo -->
    <div class="hero__bg">
      <!-- cambia la ruta de la imagen por la tuya -->
      <img src="@/assets/images/hero-team.jpg" alt="Team high five" />
      <div class="hero__overlay"></div>
    </div>

    <!-- Contenido centrado -->
    <div class="hero__content">
      <!-- Logo (puede ser un solo svg con icono+texto) -->
      <img
        class="hero__logo"
        src="@/assets/images/logo-covenant-white.svg"
        alt="Covenant Group Ltd."
      />

      <!-- Botón principal -->
      <button class="hero__cta" @click="scrollToNext">
        Get Started
      </button>

      <!-- Texto inferior -->
      <p class="hero__subtitle">
        Behind Every Great Canadian<br />
        Company is Great Talent
      </p>

      <!-- Dots del slider -->
      <div class="hero__dots">
        <span class="hero__dot hero__dot--active"></span>
        <span class="hero__dot"></span>
        <span class="hero__dot"></span>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
  const scrollToNext = () => {
    const el = document.querySelector<HTMLElement>('#for-employers')
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' })
    }
  }
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
}

/* contenedor principal */
.hero__inner {
  position: relative;
  z-index: 2;
  width: 100%;
  max-width: 1280px;
  margin: 0 auto;
  padding: 140px 7vw 80px;   /* desktop */
  display: flex;
  align-items: center;
  justify-content: center;
  box-sizing: border-box;
}

/* Imagen de fondo */
.hero__bg {
  position: absolute;
  inset: 0;
  width: 100%;            /* 🔧 cambiado de 100vw a 100% */
  height: 100%;
  overflow: hidden;
}

/* 🔧 ANTES: width: 100vw; */
.hero__bg img {
  width: 100%;            /* 🔧 cambiado de 100vw a 100% */
  height: 100%;
  object-fit: cover;
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

/* Contenido centrado */
.hero__content {
  position: relative;
  z-index: 1;
  max-width: 540px;
  margin: 0 auto;
  padding-top: 40vh;
  text-align: center;
  animation: hero-enter 1s ease-out;
}

/* Logo */
.hero__logo {
  width: 360px; /* ajusta según tu svg */
  max-width: 80vw;
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
  }

  .hero__content > :first-child {
    display: flex;
    flex-direction: column;  /* logo arriba, botón abajo */
    align-items: center;     /* centrados horizontalmente */
    gap: 16px;
  }

  /* Aseguramos que el logo no tenga desplazamientos raros */
  .hero__content img,
  .hero__logo {
    margin: 0 auto;
    display: block;
  }

  /* Y que el botón quede centrado también */
  .hero__content button {
    margin: 0 auto;
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
  padding: 90px 6vw 50px;  /* aún menos padding para centrar verticalmente */
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
