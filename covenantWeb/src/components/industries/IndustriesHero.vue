<template>
  <section class="ind-hero" data-aos="fade-up">
    <!-- Fondo con imagen y gradiente -->
    <div class="ind-hero__bg">
      <!-- cambia la ruta por tu imagen de fondo -->
      <img src="@/assets/images/industries-hero.png" alt="Industries" />
      <div class="ind-hero__overlay"></div>
    </div>

    <!-- Contenido -->
    <div class="ind-hero__content">
      <p class="ind-hero__eyebrow">INDUSTRIES</p>
      <h1 class="ind-hero__title">• Our Areas of Expertise •</h1>
      <p class="ind-hero__subtitle">
        We recruit highly qualified professionals for specialized positions,
        ensuring long-term impact and measurable business success.
      </p>
    </div>
  </section>
</template>

<script setup lang="ts"></script>

<style scoped>
.ind-hero {
  position: relative;
  width: 100%;
  min-height: 680px;
  overflow: hidden;
  color: #ffffff;
}

/* Imagen de fondo */
.ind-hero__bg {
  position: absolute;
  inset: 0;
}
.ind-hero__bg img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: top center;
}
.ind-hero__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
      to bottom,
      rgba(0, 0, 0, 0.2),
      rgba(2, 13, 30, 1)
    ),
    radial-gradient(circle at 20% 0, rgba(82, 201, 131, 0.6), transparent 60%);
}

/* Contenido centrado */
.ind-hero__content {
  position: relative;
  z-index: 1;
  max-width: 720px;
  margin: 0 auto;
  padding: 230px 24px 80px;
  text-align: center;
}

.ind-hero__eyebrow {
  font-size: 2.8rem;
  font-weight: bold;
  text-transform: uppercase;
  margin-bottom: 0;
  opacity: 0.85;
  text-shadow: 4px 4px 8px rgba(0, 0, 0, 0.3);
}

.ind-hero__title {
  font-size: 2.3rem;
  letter-spacing: 0.2rem;
  font-weight: 400;
  margin-bottom: 32px;
}

.ind-hero__subtitle {
  font-size: 0.95rem;
  line-height: 1.7;
  max-width: 540px;
  margin: 0 auto;
  opacity: 0.85;
}

@media (max-width: 768px) {
  .ind-hero {
    min-height: 380px;
  }
  .ind-hero__content {
    padding-top: 100px;
    padding-bottom: 60px;
  }
  .ind-hero__title {
    font-size: 1.9rem;
  }
}
</style>
