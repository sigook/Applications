<template>
  <section class="why-partner" data-aos="fade-up">
  <div class="why-partner__inner">
    <h2 class="why-partner__title">Why Partner With Us?</h2>

    <div class="why-partner__grid">
      <article class="why-card why-card--highlight">
        <div class="why-card__icon">
          <img src="@/assets/images/Independent icon.png" alt="Full Autonomy">
        </div>
        <div class="why-card__body">
          <h3 class="why-card__title">Full Autonomy</h3>
          <p class="why-card__text">
            Set your own schedule, work from anywhere in Canada.
          </p>
        </div>
      </article>

      <article class="why-card">
        <div class="why-card__icon">
          <img src="@/assets/images/map-canada.png" alt="Full Autonomy">
        </div>
        <div class="why-card__body">
          <h3 class="why-card__title">Nationwide Reach</h3>
          <p class="why-card__text">
            Conduct business with employers across the country (competitive advantage).
          </p>
        </div>
      </article>

      <article class="why-card">
        <div class="why-card__icon">
          <img src="@/assets/images/entrepreneur icon.png" alt="Full Autonomy">
        </div>
        <div class="why-card__body">
          <h3 class="why-card__title">Entrepreneurial</h3>
          <p class="why-card__text">
            By being an entrepreneurial role there’s high reward for high performance.
          </p>
        </div>
      </article>

      <article class="why-card">
        <div class="why-card__icon">
          <img src="@/assets/images/certified icon.png" alt="Full Autonomy">
        </div>
        <div class="why-card__body">
          <h3 class="why-card__title">Licensed Agency</h3>
          <p class="why-card__text">
            With strong compliance, recruitment operation and access to sales tools.
          </p>
        </div>
      </article>
    </div>
  </div>
</section>

</template>

<script setup lang="ts"></script>

<style scoped>
.why-partner {
  background: #0F2F44;
  padding: 70px 0;
  color: #ffffff;
}

.why-partner__inner {
  max-width: 1180px;
  margin: 0 auto;
  padding: 0 24px;
}

.why-partner__title {
  text-align: center;
  font-size: 2rem;
  margin-bottom: 60px;
}

/* grid 2x2 en desktop */
.why-partner__grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 24px;
}

/* CARD */
.why-card {
  background: #ffffff;
  color: #0b1a33;
  border-radius: 24px;
  padding: 18px 24px;
  display: flex;              /* icono + contenido horizontal */
  align-items: center;
  gap: 16px;
}

/* card verde destacada */
.why-card--highlight {
  background: #32d26a;
  color: #05162d;
}

/* Icono a la izquierda */
.why-card__icon {
  flex: 0 0 90px;
  width: 90px;
  height: 90px;
  border-radius: 50%;
  background: #05162d;      /* azul oscuro de la web */
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Imagen dentro del círculo */
.why-card__icon img {
  max-width: 60%;           /* más pequeño para que no se salga */
  max-height: 60%;
  object-fit: contain;
  display: block;
}

/* Contenido a la derecha */
.why-card__body {
  flex: 1;
}

.why-card__title {
  font-size: 1.3rem;
  font-weight: 700;
  margin: 0 0 4px 0;
}

.why-card__text {
  font-size: 0.95rem;
  line-height: 1.6;
  margin: 0;
  color: inherit;
}

/* Responsive: una tarjeta debajo de la otra,
   pero conservando icono izquierda + texto derecha */
@media (max-width: 900px) {
  .why-partner__grid {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
