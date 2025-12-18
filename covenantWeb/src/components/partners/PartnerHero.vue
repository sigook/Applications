<template>
  <section class="partner-hero" data-aos="fade-up">
    <div class="partner-hero__wrapper">
      <!-- Texto izquierda -->
      <div class="partner-hero__content">
        <h1 class="partner-hero__title">Become a Partner</h1>
        <p class="partner-hero__text">
          If you're an independent recruiter or want to grow your staffing
          sales with the support of a company with national coverage and full
          corporate backing, contact us and become our business partner.
        </p>
      </div>

      <!-- Imagen derecha -->
      <div class="partner-hero__visual">
        <!-- Círculo verde de fondo -->
        <div class="partner-hero__circle"></div>

        <!-- Imagen del hombre, por fuera del círculo -->
        <img
          src="@/assets/images/partner-hero-man.png"
          alt="Covenant Partner"
          class="partner-hero__img"
        />

        <!-- elementos decorativos -->
        <span class="partner-hero__dot partner-hero__dot--navy"></span>
        <span class="partner-hero__dot partner-hero__dot--green"></span>
        <span class="partner-hero__ring"></span>
        <span class="partner-hero__grid"></span>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts"></script>

<style scoped>
.partner-hero {
  background: #32d26a;
}

/* ====== MOBILE / BASE ====== */
.partner-hero__wrapper {
  margin: 0 auto;
  padding: 60px 24px 40px;
  display: flex;
  flex-direction: column;        /* en mobile: una columna */
  align-items: flex-start;
  gap: 32px;
  border-radius: 0 0 0 65px;
  background: #ffffff;
}

/* Texto */
.partner-hero__content {
  max-width: 100%;
  margin-left: 0;
}

.partner-hero__title {
  font-size: 2.4rem;
  line-height: 1.1;
  color: #59dc76;
  font-weight: 700;
  margin-bottom: 28px;
  position: relative;
}

/* línea bajo el título (ancho relativo para que sea responsivo) */
.partner-hero__title::after {
  content: "";
  position: absolute;
  left: 0;
  bottom: -8px;
  width: 80%;              /* antes era px fijos */
  height: 3px;
  background: #59dc76;
  border-radius: 999px;
}

.partner-hero__text {
  font-size: 0.95rem;
  line-height: 1.6;
  color: #555b69;
}

/* Imagen + círculo */
.partner-hero__visual {
  position: relative;
  width: 100%;
  max-width: 420px;
  min-height: 330px;
  margin: 0 auto;           /* centrado en mobile */
}

/* Círculo verde centrado */
.partner-hero__circle {
  position: absolute;
  top: 70px;
  left: 50%;
  width: 260px;
  height: 260px;
  border-radius: 50%;
  background: #32d26a;
  transform: translateX(-50%);
  z-index: 1;
}

/* Hombre por delante del círculo */
.partner-hero__img {
  position: absolute;
  bottom: 0;
  left: 25%;
  height: 320px;
  transform: translateX(-50%);
  object-fit: contain;
  z-index: 2;
  border-radius: 0 0 0 500px;
}

/* puntos decorativos */
.partner-hero__dot {
  position: absolute;
  border-radius: 50%;
  z-index: 3;
}

/* círculo azul grande abajo izquierda del conjunto */
.partner-hero__dot--navy {
  width: 60px;
  height: 60px;
  background: #05162d;
  bottom: -3px;
  left: calc(55% - 110px);
}

/* círculo verde pequeño */
.partner-hero__dot--green {
  width: 22px;
  height: 22px;
  background: #32d26a;
  top: 150px;
  right: calc(50% - 120px);
}

/* aro blanco */
.partner-hero__ring {
  position: absolute;
  width: 220px;
  height: 220px;
  border-radius: 50%;
  border: 2px solid #ffffff;
  bottom: -30px;
  left: 15%;
  transform: translateX(-10%);
  z-index: 2;
}

/* cuadrito de puntitos blancos */
.partner-hero__grid {
  position: absolute;
  width: 48px;
  height: 32px;
  bottom: 32px;
  right: calc(50% - 60px);
  z-index: 3;
  background-image: radial-gradient(#ffffff 2px, transparent 2px);
  background-size: 8px 8px;
}

/* ====== TABLET / DESKTOP (≥ 900px) ====== */
@media (min-width: 900px) {
  .partner-hero__wrapper {
    padding: 80px 40px 60px;
    flex-direction: row;          /* dos columnas */
    align-items: center;
    justify-content: space-between;
    gap: 40px;
  }

  .partner-hero__content {
    max-width: 520px;
    margin-left: 80px;
  }

  .partner-hero__title {
    font-size: 3.2rem;
  }

  .partner-hero__title::after {
    width: 320px;                /* un poco más larga en desktop */
  }

  .partner-hero__visual {
    max-width: 460px;
    margin: 0;
    min-height: 420px;
  }

  .partner-hero__circle {
    width: 360px;
    height: 360px;
    top: 40px;
    left: 55%;                   /* un poco desplazado hacia la derecha */
    transform: translateX(-50%);
  }

  .partner-hero__img {
    height: 450px;
    bottom: 20px;
    left: 30%;
    transform: translateX(-50%);
  }

  .partner-hero__dot--navy {
    width: 70px;
    height: 70px;
    bottom: 24px;
    left: calc(65% - 190px);
  }

  .partner-hero__dot--green {
    width: 26px;
    height: 26px;
    top: 185px;
    right: calc(45% - 40px);
  }

  .partner-hero__ring {
    width: 260px;
    height: 260px;
    bottom: -30px;
    left: 48%;
    transform: translateX(-50%);
  }

  .partner-hero__grid {
    width: 60px;
    height: 40px;
    bottom: 40px;
    right: calc(45% - 30px);
  }
}

/* Opcional: ajustar un poco más en pantallas muy grandes */
@media (min-width: 1200px) {
  .partner-hero__wrapper {
    padding-inline: 80px;
  }

  .partner-hero__content {
    margin-left: 100px;
  }

  .partner-hero__title {
    font-size: 3.6rem;
  }
}

</style>
