<template>
  <section class="main-section">

    <div class="hero-top">
      <div class="hero-bg">
        <img src="@/assets/images/hero-bg.png" alt="Employers Background" />
        <div class="hero-overlay"></div>
      </div>

      <div class="hero-content container">
        <h1 class="hero-title">
          <span class="hero-subtitle">• Find Talents •</span><br>
          EMPLOYERS
        </h1>

        <div class="switch-box">
          <button
            class="switch-btn"
            :class="{ 'active': activeTab === 'professional' }"
            @click="activeTab = 'professional'"
          >
            Professional
          </button>
          <button
            class="switch-btn"
            :class="{ 'active': activeTab === 'industrial' }"
            @click="activeTab = 'industrial'"
          >
            Industrial
          </button>
        </div>
      </div>
    </div>

    <div class="dynamic-content-wrapper">

      <div class="intro-icon-wrapper">
        <div class="intro-icon-circle">
            <svg v-if="activeTab === 'industrial'" width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="#32d26a" stroke-width="2">
               <path d="M12 2a10 10 0 1 0 10 10A10 10 0 0 0 12 2zm0 18a8 8 0 1 1 8-8 8 8 0 0 1-8 8z"/><path d="M12 6a6 6 0 1 0 6 6 6 6 0 0 0-6-6zm0 10a4 4 0 1 1 4-4 4 4 0 0 1-4 4z"/>
            </svg>
            <svg v-else width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="#32d26a" stroke-width="2">
               <circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line>
            </svg>
        </div>
      </div>

      <transition name="fade" mode="out-in">
        <div :key="activeTab" class="intro-block container">

          <p class="intro-text">
            {{ currentContent.introText }}
          </p>

          <button class="btn-contact-outline" @click="scrollToContact">
            Contact Us
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="5" y1="12" x2="19" y2="12"></line><polyline points="12 5 19 12 12 19"></polyline></svg>
          </button>
        </div>
      </transition>

      <div class="hiring-options container">
        <h2 class="options-title">Professional<br>Hiring Options</h2>

        <transition name="fade" mode="out-in">
          <div :key="activeTab" class="cards-layout">

            <div class="card-row row-left">
              <div class="side-icon left-icon">
                 <component :is="currentContent.iconLeftComponent" />
              </div>
              <div class="puzzle-card card-green">
                <h3>{{ currentContent.card1Title }}</h3>
                <p>{{ currentContent.card1Desc }}</p>
                <button class="btn-pill white">&lt;&lt; Benefits</button>
              </div>
            </div>

            <div class="card-row row-right">
              <div class="puzzle-card card-blue">
                <div class="text-right-content">
                  <h3>{{ currentContent.card2Title }}</h3>
                  <p>{{ currentContent.card2Desc }}</p>
                  <button class="btn-pill white">Benefits &gt;&gt;</button>
                </div>
              </div>
              <div class="side-icon right-icon">
                 <component :is="currentContent.iconRightComponent" />
              </div>
            </div>

          </div>
        </transition>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

// --- LÓGICA DE ESTADO ---
const activeTab = ref<'professional' | 'industrial'>('professional');

const contentData = {
  professional: {
    introText: "We help companies recruit highly qualified professionals for specialized positions, ensuring long-term impact and measurable business success.",
    card1Title: "DIRECT HIRING",
    card1Desc: "Find long-term professionals who align with your company's culture and goals.",
    card2Title: "CONTRACT",
    card2Desc: "Access skilled talent quickly with flexible contracts tailored to project needs.",
    // Asegúrate de importar o definir tus iconos aquí
    iconLeftComponent: 'div',
    iconRightComponent: 'div'
  },
  industrial: {
    introText: "Our industrial solutions deliver reliable, pre-screened workers for temporary or ongoing staffing needs, keeping your workforce agile and efficient.",
    card1Title: "TEMP TO PERM",
    card1Desc: "We help you transition top temporary talent into long-term, high-performing employees.",
    card2Title: "TEMPORAL / SEASONAL",
    card2Desc: "Our team delivers fast, flexible staffing solutions to keep your operations running smoothly.",
    iconLeftComponent: 'div',
    iconRightComponent: 'div'
  }
}

const currentContent = computed(() => contentData[activeTab.value]);

const scrollToContact = () => {
  const el = document.querySelector('#contact-section')
  if (el) el.scrollIntoView({ behavior: 'smooth' })
}
</script>

<style scoped>
.main-section {
  width: 100%;
  position: relative;
  /* Fondo base para evitar huecos blancos durante la carga */
  background-color: #05162d;
}

.container {
  max-width: 1100px;
  margin: 0 auto;
  padding: 0 20px;
}

/* === HERO (AJUSTADO) === */
.hero-top {
  position: relative;
  width: 100%;

  /* 1. ALTURA FIJA Y AUMENTADA */
  /* Usar height (no min-height) evita que el triángulo cambie de forma */
  height: 85vh;
  /* Fallback para pantallas muy pequeñas */
  min-height: 650px;

  display: flex;
  justify-content: center;
  align-items: center;
  text-align: center;
  color: white;
  z-index: 2;

  /* 2. CLIP-PATH INVERTIDO (Punta mirando hacia arriba) */
  /* Los puntos son:
     0 0 (Arriba Izq),
     100% 0 (Arriba Der),
     100% 100% (Abajo Der),
     50% 88% (El punto medio sube al 88% de la altura),
     0 100% (Abajo Izq) */
  clip-path: polygon(0 0, 100% 0, 100% 100%, 50% 78%, 0 100%);

  /* Asegura que el contenido del hero no se corte por el pico */
  padding-bottom: 100px;
}

.hero-bg, .hero-overlay {
  position: absolute;
  inset: 0;
}
.hero-bg img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.hero-overlay {
  background: linear-gradient(135deg, rgba(168, 189, 67, 0.4) 0%, rgba(12, 34, 59, 0.95) 70%);
}

.hero-content {
  position: relative;
  z-index: 3;
}

.hero-title {
  font-size: 3.5rem;
  font-weight: 800;
  margin-bottom: 30px;
  text-transform: uppercase;
  text-shadow: 0 4px 10px rgba(0,0,0,0.3);
}
.hero-subtitle {
  font-size: 1.5rem;
  font-weight: 400;
  text-transform: none;
}

/* Switch Styles */
.switch-box {
  display: inline-flex;
  background: rgba(255,255,255,0.15);
  backdrop-filter: blur(8px);
  border-radius: 999px;
  padding: 5px;
  border: 1px solid rgba(255,255,255,0.2);
}
.switch-btn {
  background: transparent;
  border: none;
  color: white;
  padding: 12px 35px;
  border-radius: 999px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}
.switch-btn.active {
  background: white;
  color: #12223b;
  box-shadow: 0 4px 15px rgba(0,0,0,0.2);
}

/* === CONTENIDO DINÁMICO === */
.dynamic-content-wrapper {
  background-color: #05162d;
  color: white;

  /* 3. CONEXIÓN VISUAL */
  /* Margen negativo para subir detrás del recorte del hero */
  margin-top: -12vh;
  padding-top: 50px; /* Espacio interno */
  padding-bottom: 80px;
  position: relative;
  z-index: 1;
}

/* POSICIONAMIENTO DEL ICONO EN LA PUNTA */
.intro-icon-wrapper {
  display: flex;
  justify-content: center;
  /* Movemos el icono hacia arriba para que quede justo en el vértice del triángulo azul */
  transform: translateY(-50%);
  position: relative;
  z-index: 5;
  margin-bottom: 20px;
}

.intro-icon-circle {
  background: #12223b; /* Mismo fondo que la sección para tapar la imagen si es necesario */
  width: 80px;
  height: 80px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  /* Borde o sombra para resaltarlo sobre la imagen de fondo */
  box-shadow: 0 -5px 20px rgba(0,0,0,0.2);
  border: 4px solid #32d26a; /* Opcional: Borde verde para destacar */
}

/* Texto Intro */
.intro-block {
  text-align: center;
  max-width: 700px;
  margin: 0 auto 80px;
  display: flex;
  flex-direction: column;
  align-items: center;
}
.intro-text {
  font-size: 1.15rem;
  line-height: 1.7;
  margin-bottom: 30px;
  opacity: 0.95;
}
.btn-contact-outline {
  background: transparent;
  border: 1px solid rgba(255,255,255,0.4);
  color: white;
  padding: 12px 35px;
  border-radius: 50px;
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  transition: all 0.3s;
  font-weight: 600;
}
.btn-contact-outline:hover {
  border-color: #32d26a;
  background: rgba(50, 210, 106, 0.1);
  color: #32d26a;
}

/* === TARJETAS === */
.hiring-options { margin-top: 20px; }
.options-title {
  font-size: 2rem;
  margin-bottom: 50px;
  font-weight: 700;
  text-align: left;
  margin-left: 10%;
}
.card-row { display: flex; align-items: center; margin-bottom: -5px; }
.side-icon { width: 15%; display: flex; justify-content: center; opacity: 0.8; }
.puzzle-card { flex: 1; padding: 50px 60px; position: relative; }
.card-green {
  background-color: #5ce07d;
  color: #fff;
  border-radius: 50px 50px 0 50px;
  margin-right: 15%;
}
.card-blue {
  background-color: #24465f;
  color: #fff;
  border-radius: 50px 0 50px 50px;
  margin-left: 15%;
  text-align: right;
}
.puzzle-card h3 { font-size: 1.8rem; font-weight: 800; margin-bottom: 15px; text-transform: uppercase; }
.text-right-content { display: flex; flex-direction: column; align-items: flex-end; }
.btn-pill.white {
  background: white; color: #12223b; border: none; padding: 10px 25px; border-radius: 999px; font-weight: 700; cursor: pointer; transition: transform 0.2s;
}
.btn-pill.white:hover { transform: scale(1.05); }

/* TRANSICIONES */
.fade-enter-active, .fade-leave-active { transition: opacity 0.3s ease, transform 0.3s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; transform: translateY(10px); }

/* RESPONSIVE */
@media (max-width: 768px) {
  .hero-top { height: 75vh; min-height: 500px; clip-path: polygon(0 0, 100% 0, 100% 100%, 50% 92%, 0 100%); }
  .card-row { flex-direction: column; }
  .card-green, .card-blue { margin: 0; width: 100%; border-radius: 30px; text-align: center; margin-bottom: 20px; }
  .text-right-content { align-items: center; }
  .side-icon { display: none; }
  .options-title { text-align: center; margin-left: 0; }
}
</style>
