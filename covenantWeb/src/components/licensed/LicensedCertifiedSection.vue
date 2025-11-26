<template>
  <section class="licensed" data-aos="fade-up">
    <!-- HERO SUPERIOR -->
    <div class="licensed__hero">
      <div class="licensed__hero-bg">
        <img
          src="@/assets/images/licensed-skyline.jpg"
          alt="Toronto skyline"
        />
        <div class="licensed__hero-overlay"></div>
      </div>

      <div class="licensed__hero-content">
        <h1 class="licensed__title">
          Licensed &amp; <span>Certified</span>
        </h1>

        <p class="licensed__text">
          Our company is fully licensed and certified to
          meet the highest standards of compliance and
          safety. From government recognition to
          industry-leading certifications, we ensure
          transparency, reliability, and trust in every
          placement we make.
        </p>

        <button class="licensed__cta" @click="goToContact">
          Get in Touch >
        </button>
      </div>
    </div>

    <!-- GRID CERTIFICACIONES -->
    <div class="licensed__certs-wrapper">
      <div class="licensed__certs-grid">
        <!-- Card 1 -->
        <article class="licensed__card">
          <div class="licensed__card-inner">
            <img
              class="licensed__card-logo"
              src="@/assets/images/cert-ontario-labour.svg"
              alt="Ontario Ministry of Labour"
            />
            <p class="licensed__card-title">
              Ontario Ministry of Labour – Licensed Temporary Help Agency &amp;
              Recruiter
            </p>
          </div>
        </article>

        <!-- Card 2 -->
        <article class="licensed__card">
          <div class="licensed__card-inner">
            <img
              class="licensed__card-logo"
              src="@/assets/images/cert-wsib.svg"
              alt="WSIB Ontario"
            />
            <p class="licensed__card-title">
              WSIB &amp; Employment Standards Act Compliance
            </p>
          </div>
        </article>

        <!-- Card 3 -->
        <article class="licensed__card">
          <div class="licensed__card-inner">
            <img
              class="licensed__card-logo"
              src="@/assets/images/cert-access.svg"
              alt="Access Certified"
            />
            <p class="licensed__card-title">
              ACCESS Certified
            </p>
          </div>
        </article>

        <!-- Card 4 -->
        <article class="licensed__card">
          <div class="licensed__card-inner">
            <img
              class="licensed__card-logo"
              src="@/assets/images/cert-avetta.svg"
              alt="Avetta Certified"
            />
            <p class="licensed__card-title">
              Avetta Certified
            </p>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

const goToContact = () => {
  router.push({ name: 'contact' }) // ajusta si tu ruta es distinta
}
</script>

<style scoped>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

.licensed {
  position: relative;
  background-color: rgba(7, 24, 50, 0.95);
  color: #ffffff;
  padding-bottom: 60px;
  border-radius: 0 0 200px 0;
}

/* ========= HERO ========= */

.licensed__hero {
  position: relative;
  min-height: 70vh;
  width: 100%;
  overflow: hidden;
}

.licensed__hero-bg {
  position: absolute;
  inset: 0;
}

.licensed__hero-bg img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.licensed__hero-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    to bottom,
    rgba(0, 0, 0, 0.15),
    rgba(7, 24, 50, 0.95)
  );
}

.licensed__hero-content {
  position: relative;
  z-index: 1;
  max-width: 520px;
  padding: 120px 8vw 60px;
}

.licensed__title {
  font-size: 2.8rem;
  line-height: 1.1;
  margin-bottom: 20px;
  font-weight: 700;
}

.licensed__title span {
  color: #32d26a;
}

.licensed__text {
  font-size: 0.95rem;
  line-height: 1.7;
  max-width: 460px;
  margin-bottom: 26px;
  color: #e4edf6;
}

.licensed__cta {
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

.licensed__cta:hover {
  transform: translateY(-2px);
  background-color: #32d26a;
  color: #ffffff;
  box-shadow: 0 16px 36px rgba(0, 0, 0, 0.45);
}

/* ========= CERTIFICACIONES ========= */

.licensed__certs-wrapper {
  margin-top: 80px;
  padding: 0 8vw 40px;
  display: flex;
  justify-content: center;
}

.licensed__certs-grid {
  max-width: 750px;
  width: 100%;
  border-radius: 40px;
  padding: 30px;
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

/* Card base */
.licensed__card {
  position: relative;
  background-color: #ffffff;
  color: black;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  aspect-ratio: 1 / 1;
  cursor: default;
  transition: background-color 0.2s ease, color 0.2s ease,
    transform 0.2s ease, box-shadow 0.2s ease;
}

/* Esquinas estilo trébol */
.licensed__card:nth-child(1) {
  border-radius: 150px 150px 0 150px; /* TL TR BR BL */
}

.licensed__card:nth-child(2) {
  border-radius: 150px 150px 150px 0;
}

.licensed__card:nth-child(3) {
  border-radius: 150px 0 150px 150px;
}

.licensed__card:nth-child(4) {
  border-radius: 0 150px 150px 150px;
}

/* Hover verde */
.licensed__card:hover {
  background-color: #32d26a;
  color: #ffffff;
  transform: translateY(-4px);
  box-shadow: 0 16px 40px rgba(0, 0, 0, 0.45);
}

/* Contenido interno */
.licensed__card-inner {
  padding: 18px 14px;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
}

.licensed__card-title {
  font-size: 1rem;
  line-height: 1.4;
}

/* Logos centrados y más grandes (individuales) */
.licensed__card-logo {
  display: block;
  margin: 0 auto 3px;
  max-width: 80%;
}

.licensed__card:nth-child(1) .licensed__card-logo {
  width: 100px;
}

.licensed__card:nth-child(2) .licensed__card-logo {
  width: 130px;
}

.licensed__card:nth-child(3) .licensed__card-logo {
  width: 300px;
}

.licensed__card:nth-child(4) .licensed__card-logo {
  width: 130px;
}

/* ========= RESPONSIVE ========= */

/* tablet y abajo: más tipo “columna” */
@media (max-width: 900px) {
  .licensed__hero-content {
    max-width: 420px;
    margin: 0 auto;
    padding: 100px 24px 50px;
  }

  .licensed__title {
    font-size: 2.2rem;
  }

  .licensed__text {
    max-width: none;
  }

  .licensed__certs-wrapper {
    margin-top: 60px;
    padding: 0 20px 40px;
  }

  .licensed__certs-grid {
    max-width: 380px;
    padding: 24px;
  }
}

/* móvil “estrecho”, como la maqueta */
@media (max-width: 600px) {
  .licensed__hero {
    min-height: 75vh;
  }

  .licensed__hero-content {
    max-width: 360px;
    text-align: left; /* puedes poner center si lo prefieres */
    padding: 90px 20px 40px;
  }

  .licensed__title {
    font-size: 1.9rem;
  }

  .licensed__text {
    font-size: 0.9rem;
  }

  .licensed__certs-wrapper {
    margin-top: 40px;
    padding: 0 16px 32px;
  }

  .licensed__certs-grid {
    max-width: 400px;     /* columna estrecha como en el diseño */
    padding: 20px;
    gap: 12px;
  }

  .licensed__card-inner {
    padding: 14px 10px;
  }

  .licensed__card-title {
    font-size: 0.6rem;
  }

  .licensed__card:nth-child(1) .licensed__card-logo {
   width: 70px;
  }

  .licensed__card:nth-child(2) .licensed__card-logo {
    width: 70px;
  }

  .licensed__card:nth-child(3) .licensed__card-logo {
    width: 150px;
  }

  .licensed__card:nth-child(4) .licensed__card-logo {
    width: 70px;
  }
}
</style>

