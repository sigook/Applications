<template>
  <section class="about-story" data-aos="fade-up">
    <div class="about-story__bg">
      <img
        src="@/assets/images/about-story.jpg"
        alt="Team presentation"
      />
      <div class="about-story__overlay"></div>
    </div>

    <div class="about-story__content">
      <!-- Columna izquierda: About Us -->
      <div class="about-story__left">
        <h2 class="about-story__title">
          About<br />
          <span>Us</span>
        </h2>
        <div class="about-story__divider"></div>
      </div>

      <!-- Columna derecha: texto -->
      <div class="about-story__right">
        <p class="about-story__text">
          Since our beginnings in Ontario in 2008, Covenant Group has
          grown with one clear purpose — to bring people and
          opportunities together. Active in Talent Management since
          2016, we’ve built lasting relationships across many industries.
          Behind every connection, there’s a story — of people striving
          for better careers, of companies growing stronger, and of a
          team committed to making it happen.
        </p>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
// No logic por ahora
</script>

<style scoped>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

.about-story {
  position: relative;
  width: 100%;
  min-height: 500px;
  overflow: hidden;
  background-color: #020d1e;
  color: #ffffff;
  border-radius: 150px 0 0 0;
}

/* Fondo + overlay */
.about-story__bg {
  position: absolute;
  inset: 0;
}

.about-story__bg img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.about-story__overlay {
  position: absolute;
  inset: 0;
}

/* Contenido */
.about-story__content {
  position: relative;
  z-index: 1;
  max-width: 1100px;
  margin: 100px auto;
  padding: 70px 6vw;
  display: grid;
  grid-template-columns: minmax(0, 260px) minmax(0, 1fr);
  align-items: center;
  gap: 40px;
}

/* Columna izquierda */
.about-story__left {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

.about-story__title {
  font-size: 2.4rem;
  line-height: 1.1;
  font-weight: 700;
  margin-bottom: 18px;
}

.about-story__title span {
  color: #32d26a;
}

.about-story__divider {
  width: 70px;
  height: 3px;
  background-color: #ffffff;
  border-radius: 999px;
}

/* Columna derecha */
.about-story__right {
  max-width: 600px;
}

.about-story__text {
  font-size: 1rem;
  line-height: 1.8;
  color: #e6edf5;
}

/* ======= RESPONSIVE ======= */

@media (max-width: 900px) {
  .about-story__content {
    padding: 60px 24px;
    grid-template-columns: minmax(0, 220px) minmax(0, 1fr);
    gap: 24px;
  }

  .about-story__title {
    font-size: 2rem;
  }
}

@media (max-width: 700px) {
  .about-story__content {
    grid-template-columns: 1fr;
    text-align: left;
  }

  .about-story__left {
    align-items: flex-start;
  }

  .about-story__right {
    max-width: 100%;
  }

  .about-story__title {
    font-size: 1.9rem;
  }

  .about-story__text {
    font-size: 0.9rem;
  }
}
</style>
