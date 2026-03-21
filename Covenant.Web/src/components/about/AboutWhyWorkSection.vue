<template>
  <section class="why-work" data-aos="fade-up">
    <div class="why-work__inner">
      <h2 class="why-work__title">Why work with us?</h2>

      <div class="why-work__item why-work__item--right">
        <div class="why-work__content">
          <h3 class="why-work__heading">Working where you are for you</h3>
          <p class="why-work__text">
            With clients located across multiple regions, you’ll have the chance
            to find roles that match your skills and interests — and to build
            meaningful relationships that foster long-term stability and career
            development.
          </p>
        </div>

        <div class="why-work__media why-work__media--1">
          <img src="@/assets/images/why-work-1.png" alt="Toronto skyline" />
          <div class="decor-ring-green"></div>
          <div class="decor-lines-left"></div>
        </div>
      </div>

      <div class="why-work__item why-work__item--left">
        <div class="why-work__media why-work__media--2">
          <img
            src="@/assets/images/why-work-2.png"
            alt="People talking in an interview"
          />
          <div class="decor-ring-dark"></div>
          <div class="decor-blob-green-front"></div>
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
          <div class="decor-ring-green-bottom"></div>
          <div class="decor-blob-blue-back"></div>
          <div class="decor-lines-center"></div>
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
  font-size: 3.2rem;
  font-weight: 700;
  margin-bottom: 120px;
  color: #171717;
}

/* === LAYOUT ITEMS (Desktop) === */
.why-work__item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 60px;
  margin-bottom: -100px;
}

/* Contenido Texto Base */
.why-work__content {
  max-width: 480px;
  display: flex;
  flex-direction: column;
  z-index: 2;
}

/* Alineación Desktop */
.why-work__item--right .why-work__content {
  text-align: right;
  align-items: flex-end;
  margin-left: 150px; /* Desplazar texto hacia el centro */
}

.why-work__item--left .why-work__content {
  text-align: left;
  align-items: flex-start;
  margin-right: 150px; /* Desplazar texto hacia el centro */
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

/* === MEDIA CONTAINER BASE (Desktop) === */
.why-work__media {
  position: relative;
  width: 720px;
  height: 720px;
  flex-shrink: 0;
}

.why-work__media img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 50%;
  position: relative;
  z-index: 1;
}

/* Márgenes negativos Desktop */
.why-work__item--right .why-work__media {
  margin-right: -280px;
}

.why-work__item--left .why-work__media {
  margin-left: -280px;
}

/* ========================================= */
/* === DECORACIONES (Intactas) === */
/* ========================================= */

.why-work__media--1::before {
  content: '';
  position: absolute;
  width: 200px;
  height: 200px;
  background-color: #5ce07d;
  border-radius: 50%;
  top: 0;
  left: 10%;
  z-index: 0;
}

.decor-ring-green {
  position: absolute;
  top: 60%;
  left: 25%;
  transform: translate(-50%, -50%);
  width: 60%;
  height: 60%;
  border: 1px solid #5ce07d;
  border-radius: 50%;
  z-index: 0;
}

.decor-lines-left {
  position: absolute;
  bottom: 30%;
  left: 15%;
  width: 80px;
  height: 40px;
  z-index: 2;
  background-image: repeating-linear-gradient(
    to bottom,
    rgba(255, 255, 255, 0.9) 0px,
    rgba(255, 255, 255, 0.9) 2px,
    transparent 2px,
    transparent 10px
  );
}

.decor-ring-dark {
  position: absolute;
  top: 42%;
  left: 60%;
  transform: translate(-50%, -50%);
  width: 90%;
  height: 90%;
  border: 1px solid #05162d;
  border-radius: 50%;
  z-index: 2;
}

.decor-blob-green-front {
  position: absolute;
  bottom: 10px;
  left: 350px;
  width: 380px;
  height: 380px;
  background-color: #5ce07d;
  border-radius: 50%;
  z-index: 0;
}

.decor-ring-green-bottom {
  position: absolute;
  bottom: -20px;
  right: 20px;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  border-bottom: 40px solid #5ce07d;
  border-right: 40px solid #5ce07d;
  border-top: 40px solid #5ce07d;
  border-left: 40px solid #5ce07d;
  transform: rotate(45deg);
  opacity: 0.8;
  z-index: 0;
}

.decor-blob-blue-back {
  position: absolute;
  bottom: -40px;
  left: 20px;
  width: 160px;
  height: 160px;
  background-color: #05162d;
  border-radius: 50%;
  z-index: 0;
}

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

/* Tablet Grande */
@media (max-width: 1280px) {
  .why-work__media { width: 560px; height: 560px; }
  .why-work__item--right .why-work__media { margin-right: -160px; }
  .why-work__item--left .why-work__media { margin-left: -160px; }
}

/* Tablet / Laptop pequeño */
@media (max-width: 1024px) {
  .why-work__inner { padding: 0 28px; }
  .why-work__media { width: 460px; height: 460px; }
  .why-work__item--right .why-work__media { margin-right: -90px; }
  .why-work__item--left .why-work__media { margin-left: -90px; }
  .why-work__title { font-size: 2.3rem; }
}

/* === MÓVIL (Cambios solicitados) === */
@media (max-width: 800px) {
  .why-work { padding: 60px 0 0; }
  .why-work__inner { padding: 0 16px; }
  .why-work__title { margin-bottom: 60px; font-size: 1.8rem; }

  /* 1. Mantenemos el Layout en Fila (Row) */
  .why-work__item {
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 40px;
    gap: 0; /* Quitamos gap para controlar con anchos */
  }

  /* 2. Configuración de Texto (Mitad de ancho) */
  .why-work__content {
    /* CAMBIO CLAVE: Ancho restringido al 45-50% */
    width: 48%;
    flex: 0 0 48%; /* No crece más de la mitad */
    max-width: none;
  }

  .why-work__heading { font-size: 1rem; margin-bottom: 8px; }
  .why-work__text { font-size: 0.8rem; line-height: 1.4; }

  /* 3. Configuración de Imagen (Más grande, ocupando la otra mitad + extra) */
  .why-work__media {
    /* CAMBIO CLAVE: Imagen más grande basada en VW */
    width: 52vw;
    height: 52vw;
    max-width: 320px;
    max-height: 320px;
    flex-shrink: 0;
    order: unset !important;
  }

  /* 4. Ajuste de márgenes negativos en móvil para que la imagen sobresalga sutilmente */
  .why-work__item--right .why-work__media {
    margin-right: -50px;
  }
  .why-work__item--left .why-work__media {
    margin-left: -50px;
  }

  /* Respetamos la alineación de texto original */
  .why-work__item--right .why-work__content {
    text-align: left;
    align-items: flex-end;
    margin-left: 0; /* Reset desktop margin */
  }
  .why-work__item--left .why-work__content {
    text-align: left;
    align-items: flex-start;
    margin-right: 0; /* Reset desktop margin */
  }

  /* Ajustes de decoraciones específicas para que no se vean mal al reducir tamaño */
  .decor-blob-green-front {
    width: 100px; height: 100px; bottom: 10px; left: -10px;
  }
  .decor-ring-green-bottom { border-width: 20px; }
}

/* Pantallas muy pequeñas */
@media (max-width: 450px) {
  .why-work__media {
    /* En pantallas muy chicas, la imagen sigue grande */
    width: 55vw; height: 55vw;
  }
  .why-work__content {
    /* El texto se hace un poquito más pequeño para caber en su mitad */
    width: 45%; flex: 0 0 45%;
  }
  .why-work__heading { font-size: 0.95rem; }
  .why-work__text { font-size: 0.75rem; }
}
</style>
