<template>
  <section class="main-section">

    <div class="hero-top">
      <div class="hero-bg">

        <img
          :src="imgProfessional"
          alt="Professional Background"
          class="hero-img-layer"
          :class="{ 'img-active': activeTab === 'professional' }"
        />

        <img
          :src="imgIndustrial"
          alt="Industrial Background"
          class="hero-img-layer"
          :class="{ 'img-active': activeTab === 'industrial' }"
        />

        <div class="hero-overlay"></div>
      </div>

      <div class="hero-content container">
        <h1 class="hero-title">
            <span class="hero-subtitle">• Find Talents •</span><br>
            EMPLOYERS
        </h1>
        <div class="switch-box">
             <button class="switch-btn" :class="{ 'active': activeTab === 'professional' }" @click="activeTab = 'professional'">Professional</button>
             <button class="switch-btn" :class="{ 'active': activeTab === 'industrial' }" @click="activeTab = 'industrial'">Industrial</button>
        </div>
      </div>
    </div>

    <div class="dynamic-content-wrapper">

      <div class="intro-icon-wrapper">
        <div class="intro-icon-circle">
            <img
              :src="getIcon(activeTab === 'industrial' ? 'industrial.svg' : 'professional.svg')"
              alt="Section Icon"
            />
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

// 1. IMPORTAR LAS IMÁGENES
// Ajusta los nombres de archivo según los tengas guardados
// Si usas Vite, esta es la forma más segura de importar assets estáticos
import imgProfessional from '@/assets/images/hero-professional.png'
import imgIndustrial from '@/assets/images/hero-industrial.png'

// (Aquí van tus definiciones de iconos anteriores...)
const activeTab = ref<'professional' | 'industrial'>('industrial');

// --- FUNCIÓN NUEVA PARA CARGAR ICONOS ---
// Busca en la misma carpeta donde pusiste los iconos de los roles
const getIcon = (iconName: string) => {
  return new URL(`../../assets/images/roles/icons/${iconName}`, import.meta.url).href
}

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
  background-color: #05162d;
}

.container {
  max-width: 100%;
  margin: 0;
  padding: 0;
}

/* === HERO === */
.hero-top {
  position: relative;
  width: 100%;
  height: 85vh;
  min-height: 650px;
  display: flex;
  justify-content: center;
  align-items: center;
  text-align: center;
  color: white;
  z-index: 2;
  padding-bottom: 80px;
}

.hero-bg {
  position: absolute;
  inset: 0;
  z-index: -1;
  overflow: hidden;
  clip-path: polygon(0 0, 100% 0, 100% 100%, 50% 72%, 0 100%);
  transform: translateZ(0);
  -webkit-mask-image: -webkit-radial-gradient(white, black);
  border-radius: 0;
}

/* === IMÁGENES === */
.hero-img-layer {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  z-index: 0;
  opacity: 0;
  transition: opacity 0.3s ease-in-out;
  will-change: opacity;
}

/* Imagen Activa */
.hero-img-layer.img-active {
  opacity: 1;
}

/* === OVERLAY === */
.hero-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(168, 189, 67, 0.4) 0%, rgba(12, 34, 59, 0.95) 70%);
  z-index: 1;
  pointer-events: none;
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
  margin-top: -12vh;
  padding-top: 10px;
  padding-bottom: 5px;
  position: relative;
  z-index: 1;
}

/* POSICIONAMIENTO DEL ICONO EN LA PUNTA */

.intro-icon-circle {
  background: #32d26a;
  width: 100px;
  height: 100px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 -5px 20px rgba(0,0,0,0.2);
  border: 4px solid #32d26a;
  overflow: hidden; /* Por seguridad */
}

/* === NUEVO: Ajuste para la imagen dentro del círculo === */
.intro-icon-circle img {
  width: 75%;
  height: 75%;
  object-fit: contain;
}

.intro-icon-wrapper {
  display: flex;
  justify-content: center;
  transform: translateY(-50%);
  position: relative;
  z-index: 5;
  margin-bottom: 20px;
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
