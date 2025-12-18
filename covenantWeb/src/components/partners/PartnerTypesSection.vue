<template>
  <section class="partner-types" data-aos="fade-up">
    <!-- CONTENEDOR SUPERIOR VERDE -->
    <div class="partner-types__switch-wrapper">
      <!-- Switch -->
      <div class="partner-types__switch">
        <button
          class="partner-types__btn"
          :class="{ 'partner-types__btn--active': activeTab === 'recruiter' }"
          @click="activeTab = 'recruiter'"
        >
          Recruiter<br />
          Partner
        </button>
        <button
          class="partner-types__btn"
          :class="{ 'partner-types__btn--active': activeTab === 'business' }"
          @click="activeTab = 'business'"
        >
          Business<br />
          Partner
        </button>
      </div>
    </div>

    <!-- Contenido dinámico -->
    <div v-if="activeTab === 'business'" class="partner-types__content">
      <h2 class="partner-types__title">Business Partner</h2>

      <div class="partner-types__grid">
        <!-- Columna izquierda -->
        <article class="partner-types__card">
          <div class="partner-types__image-circle">
            <!-- Cambia la imagen -->
            <img
              src="@/assets/images/partner-toronto.jpg"
              alt="City skyline"
            />
          </div>

          <p class="partner-types__text">
            As a Business Partner, you remain independent, managing your own
            relationships and pace, while
            <strong>Covenant handles contracts, compliance, payroll, and
              client support.</strong>
            Once a client begins working with us, you participate in the
            ongoing revenue from each successful placement or project.
          </p>
        </article>

        <!-- Columna derecha -->
        <article class="partner-types__card">
          <div class="partner-types__image-circle">
            <!-- Cambia la imagen -->
            <img
              src="@/assets/images/partner-business-woman.jpg"
              alt="Business partner"
            />
          </div>

          <p class="partner-types__text">
            This model is ideal for
            <strong>business developers, industry professionals, or
              consultants</strong>
            who want to expand their reach and income by partnering with a
            licensed, established staffing and talent management company.
            Together, we build success through trust, fairness, and shared
            results.
          </p>
        </article>
      </div>

      <div class="partner-types__cta-wrapper">
        <button class="partner-types__cta">Start Here</button>
      </div>
    </div>

    <!-- Contenido Recruiter -->
    <div
      v-else
      class="partner-types__content partner-types__content--recruiter"
    >
      <h2 class="partner-types__title">Recruiter Partner</h2>

      <!-- ahora usamos el grid normal, igual que Business -->
      <div class="partner-types__grid">
        <!-- Columna izquierda -->
        <article class="partner-types__card">
          <div
            class="partner-types__image-circle partner-types__image-circle--decor"
          >
            <img
              src="@/assets/images/partner-recruiter-flag.png"
              alt="Canada flag"
            />
          </div>

          <p class="partner-types__text">
            As a Recruiter Partner, you'll operate independently, sourcing and
            screening candidates for the industries you know best — whether it's
            <strong>
              engineering, logistics, construction, IT, or professional services.
            </strong>
          </p>
        </article>

        <!-- Columna derecha -->
        <article class="partner-types__card">
          <div
            class="partner-types__image-circle partner-types__image-circle--decor"
          >
            <img
              src="@/assets/images/partner-recruiter-meeting.png"
              alt="Recruiter meeting"
            />
          </div>

          <p class="partner-types__text">
            You'll have full access to
            <strong>Covenant's support system</strong>, including our compliance
            framework, payroll department, and client onboarding structure.
          </p>
        </article>
      </div>

      <div class="partner-types__cta-wrapper">
        <button class="partner-types__cta">Start Here</button>
      </div>
    </div>


  </section>
</template>

<script setup lang="ts">
  import { ref } from 'vue'

  const activeTab = ref<'business' | 'recruiter'>('business') // ajusta las opciones si tienes más tabs
  </script>

<style scoped>
.partner-types {
  background: #32D26A;
  background: linear-gradient(180deg,rgba(50, 210, 106, 1) 43%, rgba(5, 22, 45, 1) 61%);
  padding: 0;
}

/* Contenedor superior verde (barra) */
.partner-types__switch-wrapper {
  background: #32d26a;           /* verde de la página */
  padding: 40px 0 40px;         /* espacio arriba y abajo */
  display: flex;
  justify-content: center;
}

/* Pill blanco del switch */
.partner-types__switch {
  max-width: 360px;
  width: 100%;
  background: #ffffff;           /* blanco como en el diseño */
  border-radius: 999px;
  display: flex;
  padding: 4px;
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.15);
}

/* Botones internos */
.partner-types__btn {
  flex: 1;
  border: none;
  background: transparent; /* transparente para que se vea el blanco del pill */
  font-size: 1rem;
  color: #32d26a;                /* texto verde cuando está inactivo */
  padding: 10px 0;
  border-radius: 999px;
  cursor: pointer;
  line-height: 1.3;
  transition:
    background 0.2s ease,
    color 0.2s ease;
}

/* Botón activo: fondo verde, texto blanco */
.partner-types__btn--active {
  background: #32d26a;
  color: #ffffff;
}

/* contenido */
.partner-types__content {
  max-width: 100%;
  margin: 0 auto;
  padding: 0;
  border-radius: 0 65px 65px 0;
  background: #ffffff;
  height: auto;
}

.partner-types__title {
  text-align: center;
  font-size: 2rem;
  color: #59DC76;
  margin: 0;
  padding: 40px 0 0 0;
}

.partner-types__grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 40px;
  align-items: center;
}

.partner-types__grid--single {
  grid-template-columns: minmax(0, 1fr);
}

.partner-types__card {
  text-align: center;
}

.partner-types__image-circle {
  width: 260px;
  height: 260px;
  border-radius: 50%;
  margin: 0 auto 22px;
  position: relative;
  overflow: hidden;
  background: #32d26a;
}

.partner-types__image-circle img {
  width: 115%;
  height: 115%;
  object-fit: cover;
}

.partner-types__text {
  font-size: 0.9rem;
  line-height: 1.7;
  color: #4c5664;
  max-width: 380px;
  margin: 0 auto;
}

.partner-types__cta-wrapper {
  display: flex;
  justify-content: center;
  margin-top: 40px;
  padding: 0 0 40px 0;
}

.partner-types__cta {
  border: none;
  padding: 18px 100px;
  border-radius: 999px;
  background: #32d26a;
  color: #ffffff;
  font-weight: 600;
  font-size: 0.95rem;
  cursor: pointer;
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.2);
  transition: transform 0.18s ease, box-shadow 0.18s ease, background 0.18s ease;
}

.partner-types__cta:hover {
  transform: translateY(-2px);
  background: #24a654;
  box-shadow: 0 18px 40px rgba(0, 0, 0, 0.3);
}


/* Decoración de circulitos verdes alrededor de las fotos */
.partner-types__image-circle--decor {
  position: relative;
}

.partner-types__image-circle--decor::before,
.partner-types__image-circle--decor::after {
  content: "";
  position: absolute;
  border-radius: 50%;
  background: #32d26a;
  z-index: -1;
}

/* punto pequeño abajo-izquierda */
.partner-types__image-circle--decor::before {
  width: 18px;
  height: 18px;
  bottom: 18px;
  left: -10px;
}

/* punto un poco más grande arriba-derecha */
.partner-types__image-circle--decor::after {
  width: 26px;
  height: 26px;
  top: 24px;
  right: -14px;
}

/* Solo escritorio: asegura una altura mínima
   para que no salte al cambiar de pestaña */
   @media (min-width: 1024px) {
  .partner-types__content {
    min-height: 650px;
  }
}

/* 💡 RESPONSIVE:
   - Desktop: 2 columnas (ya lo tienes en partner-types__grid)
   - Mobile: 1 columna (queda como la captura vertical) */
@media (max-width: 900px) {
  .partner-types__grid {
    grid-template-columns: minmax(0, 1fr);
    gap: 32px;
  }
}



@media (max-width: 900px) {
  .partner-types__grid {
    grid-template-columns: minmax(0, 1fr);
  }

  .partner-types__text {
    max-width: 100%;
  }
}
</style>
