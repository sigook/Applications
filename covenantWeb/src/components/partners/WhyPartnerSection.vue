<template>
  <section class="why-partner" data-aos="fade-up">
    <div class="why-partner__inner">
      <h2 class="why-partner__title">Why Partner With Us?</h2>

      <div class="why-partner__grid">
        <article class="why-card why-card--highlight">
          <div class="why-card__icon">
            <!-- aquí podrías usar un svg / icono -->
          </div>
          <h3 class="why-card__title">Full Autonomy</h3>
          <p class="why-card__text">
            Set your own schedule and work from anywhere in Canada.
          </p>
        </article>

        <article class="why-card">
          <div class="why-card__icon"></div>
          <h3 class="why-card__title">Nationwide Reach</h3>
          <p class="why-card__text">
            Conduct business with employers across the country for a
            competitive advantage.
          </p>
        </article>

        <article class="why-card">
          <div class="why-card__icon"></div>
          <h3 class="why-card__title">Entrepreneurial</h3>
          <p class="why-card__text">
            Enjoy high earning potential in an entrepreneurial role focused on
            performance.
          </p>
        </article>

        <article class="why-card">
          <div class="why-card__icon"></div>
          <h3 class="why-card__title">Licensed Agency</h3>
          <p class="why-card__text">
            Partner with a licensed agency with strong compliance, operations,
            and sales tools.
          </p>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup></script>

<style scoped>
.why-partner {
  background: #05162d;
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
  font-size: 1.7rem;
  margin-bottom: 40px;
}

.why-partner__grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 24px;
}

/* Card */
.why-card {
  background: #ffffff;
  color: #0b1a33;
  border-radius: 18px;
  padding: 22px 20px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.why-card--highlight {
  background: #32d26a;
  color: #05162d;
}

.why-card__icon {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  border: 2px solid currentColor;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.why-card__title {
  font-size: 1rem;
  font-weight: 700;
}

.why-card__text {
  font-size: 0.85rem;
  line-height: 1.6;
  color: inherit;
}

@media (max-width: 900px) {
  .why-partner__grid {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
