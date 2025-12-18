<template>
  <section class="why-section" data-aos="fade-up">
    <div class="why-card">
      <!-- Contenido sobre la imagen -->
      <div class="why-content">
        <h2 class="why-title">Why Choose Us?</h2>

        <p class="why-text">
          We are a licensed Talent Management Agency committed
          to delivering skilled professionals, tailored workforce
          solutions, and peace of mind for every partnership.
        </p>
      </div>

      <!-- Franja verde inferior -->
      <div class="why-strip"></div>
    </div>

  </section>

</template>

<script setup>
</script>

<style scoped>
/* CONTEXTO GENERAL (fondo azul oscuro debajo de la tarjeta) */
.why-section {
  width: 100%;
  min-height: 40rem;
  height: auto;
  background: linear-gradient(
    180deg,
    rgba(255, 255, 255, 1) 43%,
    rgba(6, 35, 56, 1) 61%
  );
  display: flex;
  justify-content: center;
}

/* TARJETA PRINCIPAL CON IMAGEN */
.why-card {
  position: relative;
  width: 100%;
  border-radius: 150px 0 150px 0; /* similar al diseño */
  overflow: hidden;
  background-image: url("@/assets/images/why-choose.png"); /* 👈 cambia la ruta */
  background-size: cover;
  background-position: center;
}

/* filtro / overlay sobre la imagen */
.why-card::before {
  content: "";
  position: absolute;
  inset: 0;
  /* ligero filtro verdoso/azulado para oscurecer la imagen */
  background: linear-gradient(
      to bottom right,
      rgba(4, 20, 31, 0.45),
      rgba(4, 20, 31, 0.25)
    );
  z-index: 0;
}

/* contenido dentro de la tarjeta */
.why-content {
  position: relative;
  z-index: 1;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  color: #ffffff;
}

/* título grande a la izquierda */
.why-title {
  flex: 0 0 40%;
  margin: 0;
  font-size: 3rem;
  font-weight: 700;
  letter-spacing: 0.04em;
}

/* texto a la derecha */
.why-text {
  flex: 1;
  margin: 0;
  font-size: 1rem;
  line-height: 1.7;
  max-width: 520px;
}

/* franja verde inferior */
.why-strip {
  position: absolute;
  left: 0;
  right: 0;
  bottom: -26px; /* sobresale hacia abajo como en el diseño */
  height: 46px;
  width: 100%;
  background: #35d66d;
  border-radius: 0 0 120px 0;
  z-index: 20;
}

/* ================== RESPONSIVE ================== */

@media (max-width: 1024px) {
  .why-content {
    padding: 70px 50px 80px;
    gap: 40px;
  }

  .why-title {
    font-size: 2.2rem;
  }
}

/* ================== RESPONSIVE ================== */

@media (max-width: 768px) {
  .why-section {
    min-height: 20rem;
    padding: 40px 0 50px;
  }

  .why-card {
    border-radius: 80px 0 80px 0;
    background-position: center top;
  }

  .why-content {
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    padding: 40px 24px 60px;
    gap: 14px;
  }

  .why-title {
    flex: none;
    font-size: 2.5rem;
    line-height: 1.2;
    margin: 0 0 6px;
  }

  .why-text {
    max-width: 260px;
    font-size: 1rem;                    
    line-height: 1.5;
  }

  .why-strip {
    height: 32px;
    bottom: -18px;
    border-radius: 0 0 70px 0;
  }
}

</style>
