<template>
  <section class="why-work" data-aos="fade-up">
    <div class="why-work__inner">
      <h2 class="why-work__title">Why work with us?</h2>

      <div class="why-work__item why-work__item--right">
        <div class="why-work__content">
          <h3 class="why-work__heading">Working where you are — for you</h3>
          <p class="why-work__text">
            With clients located across multiple regions, you’ll have the chance
            to find roles that match your skills and interests — and to build
            meaningful relationships that foster long-term stability and career
            development.
          </p>
        </div>

        <div class="why-work__media why-work__media--1">
          <img src="@/assets/images/why-work-1.png" alt="Toronto skyline" />
          <div class="decor-ring-green"></div> <div class="decor-lines-left"></div> </div>
      </div>

      <div class="why-work__item why-work__item--left">
        <div class="why-work__media why-work__media--2">
          <img
            src="@/assets/images/why-work-2.png"
            alt="People talking in an interview"
          />
          <div class="decor-ring-dark"></div> <div class="decor-blob-green-front"></div> </div>

        <div class="why-work__content">
          <h3 class="why-work__heading">We care about you</h3>
          <p class="why-work__text">
            We care deeply about every person we work with. You’re not just a
            candidate — your aspirations and vision define our mission as a
            company.
          </p>
        </div>
      </div>

      <div class="why-work__item why-work__item--right">
        <div class="why-work__content">
          <h3 class="why-work__heading">Focused on your experience</h3>
          <p class="why-work__text">
            Our team of recruiters is focused and experienced in the sectors
            where your skills and abilities are most valued and in demand. We
            understand the unique requirements of each industry and take the
            time to match your expertise with the right opportunities.
          </p>
        </div>

        <div class="why-work__media why-work__media--3">
          <img src="@/assets/images/why-work-3.png" alt="People reviewing reports" />
          <div class="decor-ring-green-bottom"></div> <div class="decor-blob-blue-back"></div> <div class="decor-lines-center"></div> </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

.why-work {
  width: 100%;
  background-color: #ffffff;
  padding: 90px 0 200px;
  overflow: hidden;
}

.why-work__inner {
  max-width: 1360px;
  margin: 0 auto;
  padding: 0 40px;
}

.why-work__title {
  text-align: center;
  font-size: 2.7rem;
  font-weight: 700;
  margin-bottom: 120px;
  color: #171717;
}

/* === LAYOUT ITEMS === */
.why-work__item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 60px;
  /* Margen negativo para acercar los items verticalmente (efecto zigzag) */
  margin-bottom: -100px;
}

/* Contenido Texto Base */
.why-work__content {
  max-width: 480px;
  display: flex;
  flex-direction: column;
  z-index: 2;
}

/* --- LÓGICA DE ALINEACIÓN DE TEXTO HACIA EL CENTRO --- */

/* Caso 1: Imagen a la derecha -> Texto a la izquierda -> Alinear texto a la DERECHA */
.why-work__item--right .why-work__content {
  text-align: right;       /* Alinea el texto */
  align-items: flex-end;   /* Alinea el contenedor flex (para que el h3 pegue a la derecha) */
}

/* Caso 2: Imagen a la izquierda -> Texto a la derecha -> Alinear texto a la IZQUIERDA */
.why-work__item--left .why-work__content {
  text-align: left;        /* Alinea el texto */
  align-items: flex-start; /* Alinea el contenedor flex */
}

.why-work__heading {
  font-size: 1.8rem;
  font-weight: 800;
  margin-bottom: 20px;
  color: #171717;
}

.why-work__text {
  font-size: 1.05rem;
  line-height: 1.8;
  color: #4b4b4b;
}

/* === MEDIA CONTAINER BASE === */
.why-work__media {
  position: relative;
  width: 720px;
  height: 720px;
  flex-shrink: 0;
}

/* Imagen principal circular */
.why-work__media img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 50%;
  position: relative;
  z-index: 1; /* Imagen base en nivel 1 */
}

/* AJUSTES DE POSICIÓN Y "SALIDA" POR LOS BORDES */
.why-work__item--right .why-work__media {
  margin-right: -280px;
}

.why-work__item--left .why-work__media {
  margin-left: -280px;
}

/* ========================================= */
/* === DECORACIONES === */
/* ========================================= */

/* --- ITEM 1 (TORONTO) --- */

/* 1. Mancha verde sólida detrás (Arriba Izquierda) */
.why-work__media--1::before {
  content: '';
  position: absolute;
  width: 200px;
  height: 200px;
  background-color: #5ce07d; /* Verde brillante */
  border-radius: 50%;
  top: 0;
  left: 10%;
  z-index: 0; /* Detrás de la imagen */
}

/* 2. Anillo fino verde grande (Rodeando la imagen) */
.decor-ring-green {
  position: absolute;
  top: 60%;
  left: 25%;
  transform: translate(-50%, -50%); /* Centrado */
  width: 60%; /* Ligeramente más grande que la imagen */
  height: 60%;
  border: 1px solid #5ce07d;
  border-radius: 50%;
  z-index: 0;
}

/* 3. Líneas blancas a la izquierda */
.decor-lines-left {
  position: absolute;
  bottom: 30%;
  left: 15%;
  width: 80px;
  height: 40px;
  z-index: 2; /* Encima de la imagen */
  background-image: repeating-linear-gradient(
    to bottom,
    rgba(255, 255, 255, 0.9) 0px,
    rgba(255, 255, 255, 0.9) 2px,
    transparent 2px,
    transparent 10px
  );
}

/* --- ITEM 2 (ENTREVISTA) --- */

/* 1. Anillo fino oscuro grande (Azul/Gris) */
.decor-ring-dark {
  position: absolute;
  top: 42%;
  left: 60%;
  transform: translate(-50%, -50%);
  width: 90%;
  height: 90%;
  border: 1px solid #05162d; /* Azul oscuro */
  border-radius: 50%;
  z-index: 2;
}

/* 2. Mancha verde sólida EN FRENTE (Abajo Izquierda) */
/* Según captura, esta mancha muerde la imagen */
.decor-blob-green-front {
  position: absolute;
  bottom: 10px;
  left: 350px;
  width: 380px;
  height: 380px;
  background-color: #5ce07d;
  border-radius: 50%;
  z-index: 0; /* Encima de la imagen */
}

/* --- ITEM 3 (TABLET/ANALYTICS) --- */

/* 1. Anillo/Sombra verde abajo */
.decor-ring-green-bottom {
  position: absolute;
  bottom: -20px;
  right: 20px;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  /* Borde grueso solo abajo y derecha simulando el efecto de la captura */
  border-bottom: 40px solid #5ce07d;
  border-right: 40px solid #5ce07d;
  border-top: 40px solid #5ce07d;
  border-left: 40px solid #5ce07d;
  transform: rotate(45deg); /* Rotamos para acomodar el borde */
  opacity: 0.8;
  z-index: 0;
}

/* 2. Mancha azul oscura sólida DETRÁS (Abajo Izquierda) */
.decor-blob-blue-back {
  position: absolute;
  bottom: -40px;
  left: 20px;
  width: 160px;
  height: 160px;
  background-color: #05162d; /* Azul oscuro corporativo */
  border-radius: 50%;
  z-index: 0; /* Detrás */
}

/* 3. Líneas blancas centradas abajo */
.decor-lines-center {
  position: absolute;
  bottom: 60px;
  left: 50%;
  transform: translateX(-50%);
  width: 100px;
  height: 40px;
  z-index: 2;
  background-image: repeating-linear-gradient(
    to bottom,
    rgba(255, 255, 255, 0.9) 0px,
    rgba(255, 255, 255, 0.9) 2px,
    transparent 2px,
    transparent 12px
  );
}

/* ======= RESPONSIVE ======= */

@media (max-width: 1280px) {
  .why-work__media { width: 560px; height: 560px; }
  .why-work__item--right .why-work__media { margin-right: -160px; }
  .why-work__item--left .why-work__media { margin-left: -160px; }
}

@media (max-width: 1024px) {
  .why-work__inner { padding: 0 28px; }
  .why-work__media { width: 460px; height: 460px; }
  .why-work__item--right .why-work__media { margin-right: -90px; }
  .why-work__item--left .why-work__media { margin-left: -90px; }
  .why-work__title { font-size: 2.3rem; }
}

@media (max-width: 800px) {
  .why-work { padding: 70px 0 80px; }
  .why-work__item {
    flex-direction: column;
    align-items: center;
    text-align: center;
    margin-bottom: 80px;
    gap: 40px;
  }

  /* En móvil reseteamos la alineación al centro */
  .why-work__item--right .why-work__content,
  .why-work__item--left .why-work__content {
    text-align: center;
    align-items: center;
    order: 2;
    margin: 0 !important;
  }

  .why-work__media {
    order: 1; margin: 0 !important;
    width: 360px; height: 360px;
  }

  /* Ajuste decoraciones en movil para que no estorben */
  .decor-blob-green-front { width: 100px; height: 100px; bottom: 20px; left: -10px; }
}

@media (max-width: 520px) {
  .why-work__media { width: 280px; height: 280px; }
  .why-work__title { font-size: 2rem; }
}
</style>
