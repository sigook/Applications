<template>
  <section class="licensed" data-aos="fade-up">
    <div class="licensed__hero">

      <div class="licensed__hero-bg">
        <img
          src="@/assets/images/licensed-skyline.jpg"
          alt="Toronto skyline"
        />
        <div class="licensed__hero-overlay"></div>
      </div>

      <div class="licensed__hero-content">

        <img
          class="licensed__hero-icon"
          src="@/assets/images/medal-icon-placeholder.png"
          alt=""
        />

        <div class="licensed__text-wrapper">
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
    </div>

    <div class="licensed__certs-wrapper">
      <div class="licensed__certs-grid">
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

<script setup lang="ts">
  import { useRouter, useRoute } from 'vue-router'
  import { nextTick } from 'vue'

  const router = useRouter()
  const route = useRoute()

  const goToContact = async () => {
    // Si estamos en el home, scrolleamos directo
    if (route.path === '/') {
       const el = document.getElementById('contact-form')
       if (el) {
         el.scrollIntoView({ behavior: 'smooth' })
       }
    } else {
      // Si estamos en otra página, vamos al home y el router o el onMounted del home deberían manejarlo
      // pero mandamos el hash explícitamente y intentamos scrollear tras la navegación por si acaso.
      await router.push({ path: '/', hash: '#contact-form' })
      
      // Intentar asegurar el scroll si la navegación no recarga la app completa (SPA navigation)
      nextTick(() => {
        const el = document.getElementById('contact-form')
        if (el) el.scrollIntoView({ behavior: 'smooth' })
      })
    }
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
  background-color: #0F2F44;
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
  z-index: 1;
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
    #0F2F44
  );
}

.licensed__hero-content {
  position: relative;
  z-index: 10;
  max-width: 620px;
  padding: 200px 8vw 80px;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

/* --- ICONO: Estilos Base (Desktop) --- */
.licensed__hero-icon {
  position: absolute;
  z-index: 0;

  /* Centrado en desktop */
  top: 35%;
  left: 250%;
  transform: translate(-50%, -50%);

  width: 80%;
  max-width: 450px;

  height: auto;
  opacity: 0.08;
  pointer-events: none;
}

.licensed__text-wrapper {
  position: relative;
  z-index: 1;
}

.licensed__title {
  font-size: 2.8rem;
  line-height: 1.1;
  font-weight: 700;
  position: relative;
  padding-bottom: 20px;
  margin-bottom: 15px;
}

.licensed__title::after {
  content: "";
  position: absolute;
  left: 0;
  bottom: 0;
  width: 110px;
  height: 2px;
  background-color: #ffffff;
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
  padding: 16px 36px;
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

.licensed__card {
  position: relative;
  background-color: #ffffff;
  color: black;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  aspect-ratio: 1 / 1;
  cursor: default;
  transition: background-color 0.2s ease, color 0.2s ease,
    transform 0.2s ease, box-shadow 0.2s ease;
}

.licensed__card:nth-child(1) { border-radius: 150px 150px 0 150px; }
.licensed__card:nth-child(2) { border-radius: 150px 150px 150px 0; }
.licensed__card:nth-child(3) { border-radius: 150px 0 150px 150px; }
.licensed__card:nth-child(4) { border-radius: 0 150px 150px 150px; }

.licensed__card:hover {
  background-color: #32d26a;
  color: #ffffff;
  transform: translateY(-4px);
  box-shadow: 0 16px 40px rgba(0, 0, 0, 0.45);
}

.licensed__card-inner {
  padding: 18px 14px;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  max-width: 250px;
}

.licensed__card-title {
  font-size: 1rem;
  line-height: 1.4;
}

.licensed__card-logo {
  display: block;
  margin: 0 auto 3px;
  max-width: 80%;
}

.licensed__card:nth-child(1) .licensed__card-logo { width: 100px; }
.licensed__card:nth-child(2) .licensed__card-logo { width: 130px; }
.licensed__card:nth-child(3) .licensed__card-logo { width: 300px; }
.licensed__card:nth-child(4) .licensed__card-logo { width: 130px; }

/* ========= RESPONSIVE ========= */

/* Ajuste para Laptops/Tablets (1024px) */
@media (max-width: 1200px) {
  .licensed__hero-icon {
    left: 120%; /* Traerlo dentro de la pantalla */
    max-width: 280px; /* Reducir tamaño */
    top: 25%;
  }
}

@media (max-width: 900px) {
  .licensed__hero-content {
    max-width: 420px;
    margin: 0 auto;
    padding: 100px 24px 50px;
  }

  .licensed__hero-icon {
    width: 60%;
    opacity: 0.06;
  }

  .licensed__title { font-size: 2.2rem; }
  .licensed__text { max-width: none; }

  .licensed__certs-wrapper { margin-top: 60px; padding: 0 20px 40px; }
  .licensed__certs-grid { max-width: 380px; padding: 24px; }
}

@media (max-width: 600px) {
  .licensed__hero { min-height: 75vh; }

  /* Ajuste de fondo para enfocar edificios (correr imagen a la izquierda) */
  .licensed__hero-bg img {
    object-position: 70% center;
  }

  .licensed__hero-content {
    max-width: 360px;
    text-align: left;

    /* CAMBIO 1: Aumentamos padding superior para despegar del Navbar */
    padding: 300px 20px 40px;
  }

  /* CAMBIO 2: Icono en esquina superior derecha en móvil */
  .licensed__hero-icon {
    /* Resetear centrado de desktop */
    left: auto;
    transform: none;

    /* Posicionar arriba a la derecha */
    top: -20px;
    right: -40px; /* O right: -10px si quieres que se corte un poco */

    /* Tamaño reducido */
    width: 250px;

    opacity: 0.08; /* Un poco más visible o igual según gusto */
  }

  .licensed__title { font-size: 1.9rem; }
  .licensed__text { font-size: 0.9rem; }

  .licensed__certs-wrapper { margin-top: 40px; padding: 0 16px 32px; }
  .licensed__certs-grid { max-width: 400px; padding: 20px; gap: 12px; }

  .licensed__card-inner {
    padding: 14px 10px;
    max-width: 150px;
    gap: 2px;
  }
  .licensed__card-title { font-size: 0.6rem; }

  .licensed__card:nth-child(1) .licensed__card-logo { width: 70px; }
  .licensed__card:nth-child(2) .licensed__card-logo { width: 70px; }
  .licensed__card:nth-child(3) .licensed__card-logo { width: 150px; }
  .licensed__card:nth-child(4) .licensed__card-logo { width: 70px; }

}
</style>

