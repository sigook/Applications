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

        <article class="partner-types__card">
          <div class="partner-types__img-wrapper">

            <div class="partner-types__img-main">
              <img src="@/assets/images/partner-toronto.jpg" alt="City skyline" />
            </div>

            <div class="decor-circle-green-right"></div> <div class="decor-outline-left"></div>       <div class="decor-dot-blue-top"></div>       <div class="decor-dot-green-bottom"></div>   </div>

          <p class="partner-types__text">
            As a Business Partner, you remain independent, managing your own
            relationships and pace, while
            <strong>Covenant handles contracts, compliance, payroll, and client support.</strong>
            Once a client begins working with us, you participate in the
            ongoing revenue from each successful placement or project.
          </p>
        </article>

        <article class="partner-types__card">
          <div class="partner-types__img-wrapper">

            <div class="partner-types__img-main">
              <img src="@/assets/images/partner-business-woman.jpg" alt="Business partner" />
            </div>

            <div class="decor-circle-green-left"></div>  <div class="decor-outline-right"></div>      <div class="decor-semicircle-blue"></div>    <div class="decor-dot-green-far-right"></div></div>

          <p class="partner-types__text">
            This model is ideal for
            <strong>business developers, industry professionals, or consultants</strong>
            who want to expand their reach and income by partnering with a
            licensed, established staffing and talent management company.
            Together, we build success through trust, fairness, and shared results.
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

      <div class="partner-types__grid">
        <article class="partner-types__card">
          <div class="partner-types__img-wrapper">
             <div class="partner-types__img-main">
              <img src="@/assets/images/partner-recruiter-flag.png" alt="Canada flag" />
            </div>
             <div class="decor-circle-green-right"></div>
            <div class="decor-outline-left"></div>
            <div class="decor-dot-blue-top"></div>
            <div class="decor-dot-green-bottom"></div>
          </div>

          <p class="partner-types__text">
            As a Recruiter Partner, you'll operate independently, sourcing and
            screening candidates for the industries you know best — whether it's
            <strong>
              engineering, logistics, construction, IT, or professional services.
            </strong>
          </p>
        </article>

        <article class="partner-types__card">
           <div class="partner-types__img-wrapper">
             <div class="partner-types__img-main">
               <img src="@/assets/images/partner-recruiter-meeting.png" alt="Recruiter meeting" />
            </div>
             <div class="decor-circle-green-left"></div>
            <div class="decor-outline-right"></div>
            <div class="decor-semicircle-blue"></div>
            <div class="decor-dot-green-far-right"></div>
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

  const activeTab = ref<'business' | 'recruiter'>('business')
  </script>

<style scoped>
.partner-types {
  background: #32D26A;
  background: linear-gradient(180deg,rgba(50, 210, 106, 1) 43%, #0F2F44 61%);
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
  padding: 30px 0 0 0;
  border-radius: 0 65px 65px 0;
  background: #ffffff;
  height: auto;
}

.partner-types__title {
  text-align: center;
  font-size: 2rem;
  color: #59DC76;
  margin: 0;
  padding: 40px 0 40px 0;
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

/* =========================================
   NUEVOS ESTILOS DE IMAGEN Y DECORACIÓN
========================================= */

/* Wrapper principal que posiciona todo el conjunto */
.partner-types__img-wrapper {
  position: relative;
  width: 260px;
  height: 260px;
  margin: 0 auto 40px; /* Aumenté el margen inferior para dar aire a las decoraciones */
  z-index: 1;
}

/* El círculo central con la foto */
.partner-types__img-main {
  position: relative;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  overflow: hidden;
  z-index: 10; /* La foto va por encima de los fondos grandes */
  /* Sombra suave opcional para separar del fondo */
  box-shadow: 0 10px 20px rgba(0,0,0,0.1);
}

.partner-types__img-main img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

/* === ELEMENTOS DECORATIVOS COMUNES === */
[class^="decor-"] {
  position: absolute;
  border-radius: 50%;
}

/* Colores base */
.bg-green { background-color: #5ce07d; } /* Verde brillante de la captura */
.bg-blue { background-color: #05162d; }  /* Azul oscuro */
.border-outline { border: 1px solid #e0e0e0; }

/* === DECORACIONES TARJETA IZQUIERDA (Toronto) === */

/* 1. Círculo Verde Grande (Detrás, derecha) */
.decor-circle-green-right {
  width: 100%;
  height: 100%;
  background-color: #5ce07d;
  top: 0;
  left: 60px; /* Desplazado a la derecha */
  z-index: 0; /* Detrás de la foto */
}

/* 2. Círculo Outline (Detrás, izquierda) */
.decor-outline-left {
  width: 110%;
  height: 110%;
  border: 1px solid #cccccc;
  top: -5%;
  left: -75%; /* Desplazado bastante a la izquierda */
  z-index: 0;
}

/* 3. Punto Azul Oscuro (Arriba Izquierda, superpuesto) */
.decor-dot-blue-top {
  width: 100px;
  height: 100px;
  background-color: #05162d;
  top: 10px;
  left: -15px;
  z-index: 1; /* Encima de la foto */
}

/* 4. Punto Verde Pequeño (Abajo Izquierda, superpuesto) */
.decor-dot-green-bottom {
  width: 60px;
  height: 60px;
  background-color: #5ce07d;
  bottom: -40px;
  left: -100px;
  z-index: 11; /* Encima de la foto */
}


/* === DECORACIONES TARJETA DERECHA (Mujer) === */

/* 1. Círculo Verde Grande (Detrás, izquierda) */
.decor-circle-green-left {
  width: 100%;
  height: 100%;
  background-color: #5ce07d;
  top: 0;
  left: -70px; /* Desplazado a la izquierda */
  z-index: 0;
}

/* 2. Círculo Outline (Detrás, derecha) */
.decor-outline-right {
  width: 115%;
  height: 115%;
  border: 1px solid #cccccc;
  top: -10%;
  left: 20%; /* Desplazado a la derecha */
  z-index: 0;
}

/* 3. Semicírculo/Círculo Azul (Arriba Izquierda, detrás) */
.decor-semicircle-blue {
  width: 140px;
  height: 140px;
  background-color: #05162d;
  top: -20px;
  left: -10px;
  z-index: 1; /* Detrás del verde incluso */
}

/* 4. Punto Verde Lejano (Arriba Derecha) */
.decor-dot-green-far-right {
  width: 80px;
  height: 80px;
  background-color: #5ce07d;
  top: 10px;
  right: -100px; /* Muy a la derecha, flotando */
  z-index: 11;
}

/* === RESPONSIVE === */
@media (max-width: 768px) {
  /* En móvil reducimos un poco el wrapper para que las decoraciones no se salgan de pantalla */
  .partner-types__img-wrapper {
    width: 200px;
    height: 200px;
    margin-bottom: 30px;
  }

  /* Ajustamos tamaños de decoraciones proporcionalmente si es necesario */
  .decor-outline-left { left: -45%; }
  .decor-circle-green-right { left: 40px; }

  /* 3. Punto Azul Oscuro (Arriba Izquierda, superpuesto) */
.decor-dot-blue-top {
  width: 100px;
  height: 100px;
  background-color: #05162d;
  top: 10px;
  left: -25px;
  z-index: 1; /* Encima de la foto */
}

/* 4. Punto Verde Pequeño (Abajo Izquierda, superpuesto) */
.decor-dot-green-bottom {
  width: 40px;
  height: 40px;
  background-color: #5ce07d;
  bottom: -25px;
  left: -30px;
  z-index: 11; /* Encima de la foto */
}
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
  padding: 18px 130px;
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
