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
          <div class="decor-outline-green-large"></div>
          <div class="decor-lines-left"></div>
        </div>
      </div>

      <div class="why-work__item why-work__item--left">
        <div class="why-work__media why-work__media--2">
          <img
            src="@/assets/images/why-work-2.png"
            alt="People talking in an interview"
          />
          <div class="decor-outline-dark-big"></div>
          <div class="decor-green-dots-right"></div>
        </div>

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
          <div class="decor-lines-center"></div>
          <div class="decor-outline-small-bottom"></div>
        </div>
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
  padding: 90px 0 200px; /* Ajustado padding inferior */
  overflow: hidden; /* Para que los circulos grandes no rompan el layout */
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
  /* Margen negativo para acercar los items verticalmente como en el diseño */
  margin-bottom: -100px;
}

/* Contenido Texto */
.why-work__content {
  max-width: 480px;
  display: flex;
  flex-direction: column;
  z-index: 2; /* Texto por encima */
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
  z-index: 1; /* Imagen base */
}

/* AJUSTES DE POSICIÓN Y "SALIDA" POR LOS BORDES */
.why-work__item--right .why-work__media {
  margin-right: -280px; /* Saca la imagen hacia la derecha */
}

.why-work__item--left .why-work__media {
  margin-left: -280px; /* Saca la imagen hacia la izquierda */
}

/* ========================================= */
/* === DECORACIONES ESPECÍFICAS POR ITEM === */
/* ========================================= */

/* --- ITEM 1 (TORONTO) --- */
/* Blob sólido verde arriba a la izquierda */
.why-work__media--1::before {
  content: '';
  position: absolute;
  width: 180px;
  height: 180px;
  background-color: #5ce07d; /* Verde brillante */
  border-radius: 50%;
  top: 0;
  left: 10%;
  z-index: 0; /* Detrás de la imagen */
}

/* Círculo contorno verde grande desplazado a la izquierda */
.decor-outline-green-large {
  position: absolute;
  top: 5%;
  left: -10%;
  width: 110%;
  height: 110%;
  border: 1px solid #5ce07d;
  border-radius: 50%;
  z-index: 0;
}

/* Líneas blancas a la izquierda */
.decor-lines-left {
  position: absolute;
  bottom: 25%;
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
/* Círculo contorno oscuro grande centrado */
.decor-outline-dark-big {
  position: absolute;
  top: -10%;
  left: -10%;
  width: 120%;
  height: 120%;
  border: 1px solid #05162d; /* Azul oscuro */
  border-radius: 50%;
  z-index: 0;
}

/* Decoración pequeña verde abajo a la derecha */
.decor-green-dots-right {
  position: absolute;
  bottom: 15%;
  right: 0;
  width: 60px;
  height: 60px;
  border-radius: 50%;
  /* Usamos box-shadow para crear el punto sólido y el contorno */
  background: #5ce07d; /* El punto sólido */
  z-index: 2;
  box-shadow: 40px 20px 0 -15px transparent,
              50px 50px 0 0px #ffffff, /* espacio blanco */
              50px 50px 0 2px #5ce07d; /* anillo verde */
}
/* Alternativa más simple para los puntos si el box-shadow es confuso: */
.decor-green-dots-right::after {
  content: '';
  position: absolute;
  width: 40px; height: 40px;
  border: 2px solid #5ce07d;
  border-radius: 50%;
  top: 50px; left: 40px;
}

/* --- ITEM 3 (TABLET) --- */
/* Blob sólido verde abajo a la izquierda */
.why-work__media--3::before {
  content: '';
  position: absolute;
  width: 250px;
  height: 250px;
  background-color: #5ce07d;
  border-radius: 50%;
  bottom: -20px;
  left: -20px;
  z-index: 0; /* Detrás */
}

/* Pequeño círculo contorno verde abajo a la izquierda */
.decor-outline-small-bottom {
  position: absolute;
  bottom: 0;
  left: -40px;
  width: 60px;
  height: 60px;
  border: 2px solid #5ce07d;
  border-radius: 50%;
  z-index: 2;
}

/* Líneas blancas centradas abajo */
.decor-lines-center {
  position: absolute;
  bottom: 30px;
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
  .why-work__content { order: 2; margin: 0 !important; }
  .why-work__media {
    order: 1; margin: 0 !important;
    width: 360px; height: 360px;
  }
}

@media (max-width: 520px) {
  .why-work__media { width: 280px; height: 280px; }
  .why-work__title { font-size: 2rem; }
}
</style>
