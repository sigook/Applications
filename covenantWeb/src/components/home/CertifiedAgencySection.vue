<template>
  <section class="cert-section" data-aos="fade-up">
    <div class="cert-card">
      <!-- Contenido centrado sobre la imagen -->
      <div class="cert-content">
        <h2 class="cert-title">
          CERTIFIED TEMPORARY HELP &amp;<br />
          RECRUITMENT AGENCY
        </h2>

        <p class="cert-text">
          As a fully certified and licensed agency, we meet the highest provincial
          standards, providing trusted recruitment services that prioritize compliance,
          professionalism, and reliable support for both businesses and job seekers.
        </p>

        <!-- Logos / iconos alineados al centro -->
        <div class="cert-logos">
          <div class="cert-logo">
            <img
              src="@/assets/images/logo-proudly-canadian.png"
              alt="Proudly Canadian"
              class="cert-logo__img"
            />
            <span class="cert-logo__text">PROUDLY<br />CANADIAN</span>
          </div>

          <div class="cert-logo">
            <img
              src="@/assets/images/logo-ontario-ministry.png"
              alt="Certified by the Ontario Ministry"
              class="cert-logo__img"
            />
            <span class="cert-logo__text">
              CERTIFIED BY THE<br />
              ONTARIO MINISTRY
            </span>
          </div>
        </div>
      </div>

      <!-- Franja verde inferior con curvatura -->
      <div class="cert-strip"></div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
.cert-section {
  width: 100%;
  height: 30rem;
  background: #062338;
  background: linear-gradient(180deg,rgba(6, 35, 56, 1) 43%, rgba(255, 255, 255, 1) 61%);      /* azul de fondo general */
  display: flex;
  justify-content: center;
}

/* Tarjeta con imagen de fondo */
.cert-card {
  position: relative;
  width: 100%;
  min-height: 340px;
  border-radius: 150px 0 150px 0;   /* curva fuerte arriba izq / abajo der */
  overflow: hidden;
  background-image: url("@/assets/images/certified-agency-bg.jpg"); /* 👉 cambia la ruta */
  background-size: cover;
  background-position: center;
}

/* Overlay para oscurecer un poco la imagen y que el texto se lea mejor */
.cert-card::before {
  content: "";
  position: absolute;
  inset: 0;
  background: linear-gradient(
    to bottom,
    rgba(4, 28, 45, 0.65),
    rgba(4, 28, 45, 0.25)
  );
  z-index: 0;
}

/* Contenedor central del texto */
.cert-content {
  position: relative;
  z-index: 1;
  max-width: 750px;
  margin: 0 auto;
  padding: 90px 40px 80px;
  box-sizing: border-box;

  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  color: #ffffff;
}

.cert-title {
  margin: 0 0 24px;
  font-size: 2.1rem;
  font-weight: 600;
  letter-spacing: 0.18em;
}

.cert-text {
  margin: 0 0 40px;
  font-size: 0.95rem;
  line-height: 1.7;
  max-width: 640px;
}

/* Contenedor de logos */
.cert-logos {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 40px;
}

.cert-logo {
  display: flex;
  align-items: center;
  gap: 14px;
}

.cert-logo__img {
  width: 44px;
  height: 44px;
  object-fit: contain;
}

.cert-logo__text {
  font-size: 0.75rem;
  line-height: 1.4;
  letter-spacing: 0.12em;
  text-transform: uppercase;
}

/* Franja verde inferior */
.cert-strip {
  position: absolute;
  left: 0;
  right: 0;
  bottom: -26px;        /* que sobresalga como en el diseño */
  height: 48px;
  background: #37d56e;
  border-radius: 0 0 90px 0;
  z-index: 0;
}

/* ================== RESPONSIVE ================== */

@media (max-width: 1024px) {
  .cert-content {
    padding: 80px 26px 70px;
  }

  .cert-title {
    font-size: 1.9rem;
    letter-spacing: 0.14em;
  }

  .cert-text {
    font-size: 0.9rem;
  }
}

@media (max-width: 768px) {
  .cert-section {
    height: 34rem;
    padding: 60px 0 70px;
  }

  .cert-card {
    border-radius: 70px 0 70px 70px;
  }

  .cert-title {
    font-size: 1.6rem;
    letter-spacing: 0.12em;
  }

  .cert-logos {
    gap: 24px;
  }

  .cert-strip {
    height: 40px;
    bottom: -22px;
    border-radius: 0 0 70px 70px;
  }
}

@media (min-width: 768px) {
  .cert-section {
    height: 26rem;
    padding: 60px 0 70px;
  }

}
</style>
