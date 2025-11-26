<template>
  <section class="about" data-aos="fade-up">
    <!-- HERO -->
    <div class="about__hero">
      <div class="about__hero-bg">
        <img
          src="@/assets/images/about-hero.jpg"
          alt="Team helping each other on a mountain"
        />
        <div class="about__hero-overlay"></div>
      </div>

      <div class="about__hero-content">
        <h1 class="about__title">
          Empowering<br />
          People.<br />
          Strengthening<br />
          <span>Businesses.</span>
        </h1>

        <p class="about__text">
          Our success is built on values we live by: fairness, respect, and
          genuine care for those we serve. At Covenant, we believe that trust
          and integrity are what truly connect people — and that is where
          lasting partnerships begin.
        </p>

        <button class="about__cta">
          Get in Touch
        </button>
      </div>
    </div>

    <!-- STATS CLOVER -->
    <div class="about__stats-wrapper">
      <div class="about__stats-grid">
        <!-- Card 1 -->
        <article class="about__card">
          <div class="about__card-inner">
            <p class="about__card-number">+10</p>
            <p class="about__card-label">Years of<br />Experience</p>
          </div>
        </article>

        <!-- Card 2 -->
        <article class="about__card">
          <div class="about__card-inner">
            <p class="about__card-number">+330</p>
            <p class="about__card-label">Clients<br />Served</p>
          </div>
        </article>

        <!-- Card 3 -->
        <article class="about__card">
          <div class="about__card-inner">
            <p class="about__card-number">+1,700</p>
            <p class="about__card-label">Jobs<br />Posted</p>
          </div>
        </article>

        <!-- Card 4 -->
        <article class="about__card">
          <div class="about__card-inner">
            <p class="about__card-number">+5,000</p>
            <p class="about__card-label">Applications<br />Filled</p>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup>
</script>

<style scoped>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

.about {
  position: relative;
  background-color: #020d1e;
  color: #ffffff;
  padding-bottom: 70px;
}

/* ========== HERO ========== */

.about__hero {
  position: relative;
  min-height: 70vh;
  width: 100%;
  overflow: hidden;
}

.about__hero-bg {
  position: absolute;
  inset: 0;
}

.about__hero-bg img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.about__hero-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    to bottom,
    rgba(0, 0, 0, 0.2),
    rgba(2, 13, 30, 0.95)
  );
}

.about__hero-content {
  position: relative;
  z-index: 1;
  max-width: 520px;
  padding: 120px 8vw 80px;
}

.about__title {
  font-size: 3rem;
  line-height: 1.1;
  margin-bottom: 24px;
  font-weight: 700;
}

.about__title span {
  color: #32d26a;
}

.about__text {
  font-size: 0.95rem;
  line-height: 1.7;
  max-width: 460px;
  margin-bottom: 28px;
  color: #e4edf6;
}

.about__cta {
  border: none;
  outline: none;
  padding: 12px 32px;
  border-radius: 999px;
  background-color: #ffffff;
  color: #020d1e;
  font-weight: 600;
  font-size: 0.95rem;
  cursor: pointer;
  box-shadow: 0 10px 26px rgba(0, 0, 0, 0.35);
  transition: transform 0.18s ease, box-shadow 0.18s ease,
    background-color 0.18s ease, color 0.18s ease;
}

.about__cta:hover {
  transform: translateY(-2px);
  background-color: #32d26a;
  color: #ffffff;
  box-shadow: 0 16px 36px rgba(0, 0, 0, 0.45);
}

/* ========== STATS ========== */

.about__stats-wrapper {
  margin-top: 80px;
  padding: 0 8vw 40px;
  display: flex;
  justify-content: center;
}

.about__stats-grid {
  max-width: 900px;
  width: 100%;
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 24px;
}

/* Card base */
.about__card {
  position: relative;
  background-color: #323d4f; /* gris oscuro por defecto */
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  aspect-ratio: 1 / 1;
}

/* Clover shapes (igual lógica que en Licensed) */
.about__card:nth-child(1) {
  background-color: #32d26a;
  border-radius: 220px 220px 0 220px; /* TL TR BR BL */
}

.about__card:nth-child(2) {
  border-radius: 220px 220px 220px 0;
}

.about__card:nth-child(3) {
  border-radius: 220px 0 220px 220px;
}

.about__card:nth-child(4) {
  border-radius: 0 220px 220px 220px;
}

/* Sutil contorno para la card 4 como en el diseño */
.about__card:nth-child(4)::after {
  content: '';
  position: absolute;
  inset: 10px;
  border-radius: inherit;
  border: 2px solid rgba(255, 255, 255, 0.1);
}

/* contenido interno */
.about__card-inner {
  text-align: center;
  color: #ffffff;
}

.about__card-number {
  font-size: 4rem;
  font-weight: 700;
  margin-bottom: 8px;
}

/* Números verdes en la tarjeta 2 y 3 */
.about__card:nth-child(2) .about__card-number,
.about__card:nth-child(3) .about__card-number {
  color: #32d26a;
}

.about__card-label {
  font-size: 1rem;
  line-height: 1.4;
  opacity: 0.9;
}

/* ========== RESPONSIVE ========== */

@media (max-width: 900px) {
  .about__hero-content {
    max-width: 420px;
    padding: 100px 24px 60px;
  }

  .about__title {
    font-size: 2.4rem;
  }

  .about__stats-wrapper {
    padding: 0 24px 40px;
  }

  .about__stats-grid {
    max-width: 480px;
  }
}

@media (max-width: 600px) {
  .about__hero {
    min-height: 75vh;
  }

  .about__hero-content {
    max-width: 360px;
    padding: 90px 20px 50px;
  }

  .about__title {
    font-size: 2.1rem;
  }

  .about__text {
    font-size: 0.9rem;
  }

  .about__stats-wrapper {
    margin-top: 60px;
    padding: 0 16px 40px;
  }

  .about__stats-grid {
    max-width: 340px;
    gap: 14px;
  }

  .about__card-number {
    font-size: 1.8rem;
  }

  .about__card-label {
    font-size: 0.8rem;
  }
}
</style>
