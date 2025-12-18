<template>
  <section class="why-work" data-aos="fade-up">
    <div class="why-work__inner">
      <h2 class="why-work__title">Why work with us?</h2>

      <!-- ITEM 1 -->
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

        <div class="why-work__media">
          <img
            src="@/assets/images/why-work-1.png"
            alt="Toronto skyline"
          />
        </div>
      </div>

      <!-- ITEM 2 -->
      <div class="why-work__item why-work__item--left">
        <div class="why-work__media">
          <img
            src="@/assets/images/why-work-2.png"
            alt="People talking in an interview"
          />
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

      <!-- ITEM 3 -->
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

        <div class="why-work__media why-work__media--with-lines">
          <img
            src="@/assets/images/why-work-3.png"
            alt="People reviewing reports"
          />
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
// sin lógica por ahora
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
  padding: 90px 0 300px;
}

.why-work__inner {
  /* un poco más ancho para dar aire a las imágenes grandes */
  max-width: 1360px;
  margin: 0 auto;
  padding: 0 40px;
}

/* Título */
.why-work__title {
  text-align: center;
  font-size: 2.7rem;
  font-weight: 700;
  margin-bottom: 80px;
  color: #171717;
}

/* Items */
.why-work__item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 80px;
  margin-bottom: -180px;
}

/* Texto */
.why-work__content {
  max-width: 460px;
  display: flex;
  flex-direction: column;
}

.why-work__heading {
  font-size: 1.6rem;
  font-weight: 700;
  margin-bottom: 18px;
  color: #171717;
}

.why-work__text {
  font-size: 1.05rem;
  line-height: 1.9;
  color: #4b4b4b;
}

/* Burbuja de imagen */
.why-work__media {
  position: relative;
  /* tamaño base grande (desktop) */
  width: 720px;
  height: 720px; /* ~750px de alto, puedes subir a 750 si quieres exacto */
  overflow: visible; /* dejamos ver los elementos verdes fuera del círculo */
}

/* círculo de la imagen */
.why-work__media img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 50%;
}

/* Blob verde decorativo */
.why-work__media::before {
  content: '';
  position: absolute;
  border-radius: 50%;
  background: #32d26a;
  z-index: -1; /* queda detrás del círculo principal */
}

/* líneas blancas decorativas (última burbuja) */
.why-work__media--with-lines::after {
  content: '';
  position: absolute;
  bottom: 36px;
  left: 50%;
  transform: translateX(-50%);
  width: 100px;
  height: 3px;
  background-image: linear-gradient(
      to right,
      #ffffff 0,
      #ffffff 32%,
      transparent 32%,
      transparent 34%,
      #ffffff 34%,
      #ffffff 66%,
      transparent 66%,
      transparent 68%,
      #ffffff 68%,
      #ffffff 100%
    );
  background-size: 100% 100%;
  opacity: 0.9;
  border-radius: 999px;
}

/* Posiciones específicas de los blobs verdes y “salida” por los bordes */
.why-work__item--right .why-work__media {
  margin-right: -260px; /* saca la imagen por el borde derecho */
}

.why-work__item--left .why-work__media {
  margin-left: -260px; /* saca la imagen por el borde izquierdo */
}

/* blob del item derecho (arriba) */
.why-work__item--right .why-work__media::before {
  width: 220px;
  height: 220px;
  top: -40px;
  left: 68%;
}

/* blob del item izquierdo (abajo) */
.why-work__item--left .why-work__media::before {
  width: 240px;
  height: 240px;
  bottom: -50px;
  left: -40px;
}

/* Ajuste extra para centrar el texto visualmente */
.why-work__item--right .why-work__content {
  margin-left: 40px;
}
.why-work__item--left .why-work__content {
  margin-right: 40px;
}

/* ======= RESPONSIVE ======= */

@media (max-width: 1280px) {
  .why-work__media {
    width: 560px;
    height: 560px;
  }

  .why-work__item--right .why-work__media {
    margin-right: -160px;
  }

  .why-work__item--left .why-work__media {
    margin-left: -160px;
  }

  .why-work__item--right .why-work__media::before {
    width: 190px;
    height: 190px;
  }

  .why-work__item--left .why-work__media::before {
    width: 210px;
    height: 210px;
  }
}

@media (max-width: 1024px) {
  .why-work__inner {
    padding: 0 28px;
  }

  .why-work__media {
    width: 460px;
    height: 460px;
  }

  .why-work__item--right .why-work__media {
    margin-right: -90px;
  }

  .why-work__item--left .why-work__media {
    margin-left: -90px;
  }

  .why-work__title {
    font-size: 2.3rem;
  }

  .why-work__heading {
    font-size: 1.4rem;
  }

  .why-work__text {
    font-size: 1rem;
  }
}

@media (max-width: 800px) {
  .why-work {
    padding: 70px 0 80px;
  }

  .why-work__item {
    flex-direction: column;
    align-items: center;
    text-align: center;
    margin-bottom: 80px;
    gap: 32px;
  }

  .why-work__content {
    max-width: 520px;
    order: 2;
    margin-left: 0;
    margin-right: 0;
  }

  .why-work__item--left .why-work__media,
  .why-work__item--right .why-work__media {
    order: 1;
    margin-right: 0;
    margin-left: 0;
  }

  .why-work__media {
    width: 360px;
    height: 360px;
  }

  .why-work__media::before {
    opacity: 0.9;
  }
}

@media (max-width: 520px) {
  .why-work__inner {
    padding: 0 20px;
  }

  .why-work__media {
    width: 260px;
    height: 260px;
  }

  .why-work__title {
    font-size: 2rem;
  }

  .why-work__heading {
    font-size: 1.25rem;
  }

  .why-work__text {
    font-size: 0.95rem;
  }
}
</style>
