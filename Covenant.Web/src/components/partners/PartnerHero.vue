<template>
  <section class="partner-hero" data-aos="fade-up">
    <div class="partner-hero__wrapper">
      <div class="partner-hero__content">
        <h1 class="partner-hero__title">Become a Partner</h1>
        <p class="partner-hero__text">
          If you're an independent recruiter or want to grow your staffing
          sales with the support of a company with national coverage and full
          corporate backing, contact us and become our business partner.
        </p>
      </div>

      <div class="partner-hero__visual">
        <div class="partner-hero__circle"></div>

        <img
          src="@/assets/images/partner-hero-man.png"
          alt="Covenant Partner"
          class="partner-hero__img"
        />

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
  background: #FFFFFF;
  background: linear-gradient(180deg,rgba(255, 255, 255, 1) 50%, rgba(50, 210, 106, 1) 50%);
  padding-top: 100px;
  overflow: hidden; /* Evita scroll horizontal si la imagen sale un poco */
}

/* ====== MOBILE / BASE (Ajustado a Fila) ====== */
.partner-hero__wrapper {
  margin: 0 auto;
  padding: 20px 24px 40px; /* Padding reducido arriba porque ya lo dimos en la sección */
  display: flex;
  flex-direction: row; /* <--- Fila desde móvil */
  align-items: center;
  justify-content: space-between;
  gap: 10px; /* Gap pequeño para aprovechar espacio */
  border-radius: 0 0 0 65px;
  background: #ffffff;
}

/* Texto Mobile */
.partner-hero__content {
  width: 50%; /* <--- Texto ocupa la mitad para dejar espacio a la imagen */
  flex-shrink: 0;
  margin-left: 0;
  z-index: 10; /* Asegura que el texto quede sobre decoraciones si se cruzan */
}

.partner-hero__title {
  /* Clamp para ajustar fuente suavemente */
  font-size: clamp(1.8rem, 5vw, 2.4rem);
  line-height: 1.1;
  color: #59dc76;
  font-weight: 700;
  margin-bottom: 20px;
  position: relative;
}

.partner-hero__title::after {
  content: "";
  position: absolute;
  left: 0;
  bottom: -8px;
  width: 80%;
  height: 3px;
  background: #59dc76;
  border-radius: 999px;
}

.partner-hero__text {
  font-size: 0.85rem; /* Un poco más pequeño en móvil para que quepa */
  line-height: 1.5;
  color: #555b69;
}

/* Visual Mobile (Ajustado para parecerse a la referencia) */
.partner-hero__visual {
  position: relative;
  width: 50%; /* <--- Imagen más ancha */
  height: 250px; /* Altura forzada en mobile para contener la imagen */
  display: flex;
  align-items: flex-end;
  justify-content: flex-end;
}

/* Círculo verde mobile */
.partner-hero__circle {
  position: absolute;
  top: 50%;
  right: -100px; /* Desplazado a la derecha como referencia */
  width: 280px;
  height: 280px;
  border-radius: 50%;
  background: #32d26a;
  transform: translateY(-50%);
  z-index: 1;
}

/* Hombre mobile */
.partner-hero__img {
  position: absolute;
  bottom: -15px;
  right: -70px; /* Pegado a la derecha */
  height: 320px; /* Altura controlada en móvil */
  width: auto;
  object-fit: contain;
  z-index: 2;
  border-radius: 0 0 0 500px;
}

/* Decoraciones Mobile (Escaladas para que no tapen todo) */
.partner-hero__dot {
  position: absolute;
  border-radius: 50%;
  z-index: 3;
}

.partner-hero__dot--navy {
  width: 60px;
  height: 60px;
  background: #102f44;
  bottom: 0;
  left: 0; /* Ubicación simple en móvil */
}

.partner-hero__dot--green {
  width: 30px;
  height: 30px;
  background: #32d26a;
  top: 100px;
  right: 10px;
}

.partner-hero__ring {
  position: absolute;
  width: 200px;
  height: 200px;
  border-radius: 50%;
  border: 2px solid #ffffff;
  bottom: -50px;
  right: -20px;
  z-index: 2;
  opacity: 0.7;
}

.partner-hero__grid {
  position: absolute;
  width: 30px;
  height: 20px;
  bottom: 20px;
  right: 50px;
  z-index: 3;
  background-image: radial-gradient(#ffffff 2px, transparent 2px);
  background-size: 6px 6px;
}

/* ====== TABLET / DESKTOP (≥ 900px) ====== */
/* IMPORTANTE: Bloque intacto según tu código original */
@media (min-width: 900px) {
  .partner-hero {
    padding-top: 100px; /* Reset del padding extra de mobile */
  }

  .partner-hero__wrapper {
    padding: 80px 40px 60px;
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
    gap: 40px;
  }

  .partner-hero__content {
    width: auto; /* Reset del width mobile */
    max-width: 520px;
    margin-left: 80px;
    flex-shrink: 1;
  }

  .partner-hero__title {
    font-size: 3.1rem;
    margin-bottom: 28px;
  }

  .partner-hero__title::after {
    width: 320px;
  }

  .partner-hero__text {
      font-size: 0.95rem; /* Restaurar tamaño original */
  }

  .partner-hero__visual {
    width: 100%; /* Reset width mobile */
    max-width: 460px;
    margin: 0;
    min-height: 420px;
    height: auto; /* Reset height mobile */
    display: block; /* Reset flex mobile */
  }

  .partner-hero__circle {
    /* Medidas originales desktop */
    width: 360px;
    height: 360px;
    top: 40px;
    left: 55%;
    right: auto; /* Reset right mobile */
    transform: translateX(-50%);
  }

  .partner-hero__img {
    /* Medidas originales desktop */
    height: 450px;
    bottom: 20px;
    left: 30%;
    right: auto; /* Reset right mobile */
    transform: translateX(-50%);
    border-radius: 0 0 0 500px; /* Estilo original desktop */
  }

  .partner-hero__dot--navy {
    width: 80px;
    height: 80px;
    bottom: 10px;
    left: calc(55% - 140px);
  }

  .partner-hero__dot--green {
    width: 32px;
    height: 32px;
    top: 160px;
    right: calc(50% - 90px);
    left: auto;
  }

  .partner-hero__ring {
    width: 320px;
    height: 320px;
    bottom: -60px;
    left: 25%;
    right: auto;
    transform: translateX(-10%);
    opacity: 1;
  }

  .partner-hero__grid {
    width: 48px;
    height: 32px;
    bottom: 32px;
    right: calc(50% - 60px);
    background-size: 8px 8px;
  }
}

@media (min-width: 1200px) {
  .partner-hero__wrapper {
    padding-inline: 80px;
  }

  .partner-hero__content {
    margin-left: 100px;
  }

  .partner-hero__title {
    font-size: 3.1rem;
  }
}
</style>
