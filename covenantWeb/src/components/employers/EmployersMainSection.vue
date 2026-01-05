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
        <div class="title-wrapper">
            <h2 class="options-title">Professional<br>Hiring Options</h2>
            <img :src="imgDecoration" alt="" class="decoration-lines" />
        </div>
        <transition name="fade" mode="out-in">
          <div :key="activeTab" class="cards-layout">

            <div class="card-row row-left">
              <div class="side-icon left-icon">
                 <img :src="currentContent.iconLeft" alt="Icon" />
              </div>

              <div class="puzzle-card card-green">
                <transition name="slide-fade" mode="out-in">

                  <div v-if="!showBenefitsLeft" key="front" class="card-content">
                    <h3>{{ currentContent.card1Title }}</h3>
                    <p>{{ currentContent.card1Desc }}</p>
                    <button class="btn-pill white" @click="showBenefitsLeft = true">&lt;&lt; Benefits</button>
                  </div>

                  <div v-else key="back" class="card-content benefits-view">
                    <div class="benefits-header">
                       <button class="btn-back-circle" @click="showBenefitsLeft = false">
                         <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="15 18 9 12 15 6"></polyline></svg>
                       </button>
                       <h3>{{ currentContent.card1Title }}</h3>
                    </div>

                    <ul class="benefits-list">
                      <li v-for="(item, index) in currentContent.card1Benefits" :key="index">
                        <svg class="check-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="4"><polyline points="20 6 9 17 4 12"></polyline></svg>
                        <span>
                          <strong v-if="item.title">{{ item.title }} </strong>{{ item.text }}
                        </span>
                      </li>
                    </ul>
                  </div>

                </transition>
              </div>
            </div>

            <div class="card-row row-right">
              <div class="puzzle-card card-blue">
                <transition name="slide-fade" mode="out-in">

                  <div v-if="!showBenefitsRight" key="front" class="card-content text-right-content">
                    <h3>{{ currentContent.card2Title }}</h3>
                    <p>{{ currentContent.card2Desc }}</p>
                    <button class="btn-pill white" @click="showBenefitsRight = true">Benefits &gt;&gt;</button>
                  </div>

                  <div v-else key="back" class="card-content benefits-view right-aligned">
                    <div class="benefits-header right-header">
                       <h3>{{ currentContent.card2Title }}</h3>
                       <button class="btn-back-circle" @click="showBenefitsRight = false">
                         <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="15 18 9 12 15 6"></polyline></svg>
                       </button>
                    </div>

                    <ul class="benefits-list">
                      <li v-for="(item, index) in currentContent.card2Benefits" :key="index">
                        <svg class="check-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="4"><polyline points="20 6 9 17 4 12"></polyline></svg>
                        <span>
                           <strong v-if="item.title">{{ item.title }} </strong>{{ item.text }}
                        </span>
                      </li>
                    </ul>
                  </div>

                </transition>
              </div>

              <div class="side-icon right-icon">
                  <img :src="currentContent.iconRight" alt="Icon" />
              </div>
            </div>

          </div>
        </transition>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">

import { ref, computed, watch } from 'vue'

import imgProfessional from '@/assets/images/hero-professional.png'
import imgIndustrial from '@/assets/images/hero-industrial.png'
import imgDecoration from '@/assets/images/lines-decoration.png'

import iconIndustrialRight from '@/assets/images/employers-icons/industrial-icon-right.png'
import iconIndustrialLeft from '@/assets/images/employers-icons/industrial-icon-left.png'
import iconProfessionalRight from '@/assets/images/employers-icons/professional-icon-right.png'
import iconProfessionalLeft from '@/assets/images/employers-icons/professional-icon-left.png'

const activeTab = ref<'professional' | 'industrial'>('industrial');

const getIcon = (iconName: string) => {
  return new URL(`../../assets/images/roles/icons/${iconName}`, import.meta.url).href
}

const contentData = {
  professional: {
    introText: "We help companies recruit highly qualified professionals for specialized positions, ensuring long-term impact and measurable business success.",
    card1Title: "DIRECT HIRING",
    card1Desc: "Find long-term professionals who align with your company's culture and goals.",
    card1Benefits: [
      { "title": "Stability:", "text": "Secure long-term professionals who grow with your company." },
      { "title": "Culture Fit:", "text": "Hire individuals aligned with your organization's values and vision." },
      { "title": "Retention:", "text": "Build loyalty and reduce turnover through permanent placements." }
    ],
    card2Title: "CONTRACT",
    card2Desc: "Access skilled talent quickly with flexible contracts tailored to project needs.",
    card2Benefits: [
      { "title": "Flexibility:", "text": "Scale your workforce quickly to meet project or seasonal demands." },
      { "title": "On-Demand Talent:", "text": "Access specialized expertise without the long-term obligation." },
      { "title": "Cost Control:", "text": "Pay only for the duration and skills you need." }
    ],
    iconLeft: iconProfessionalLeft,
    iconRight: iconProfessionalRight
  },

  industrial: {
    introText: "Our industrial solutions deliver reliable, pre-screened workers for temporary or ongoing staffing needs, keeping your workforce agile and efficient.",
    card1Title: "TEMP TO PERM",
    card1Desc: "We help you transition top temporary talent into long-term, high-performing employees.",
    card1Benefits: [
      { "title": "", "text": "Evaluate performance and fit before making a long-term hire." },
      { "title": "", "text": "Reduce hiring risks and training costs with pre-screened talent." },
      { "title": "", "text": "Seamless transition from temporary to permanent employment." }
    ],
    card2Title: "TEMPORAL / SEASONAL",
    card2Desc: "Our team delivers fast, flexible staffing solutions to keep your operations running smoothly.",
    card2Benefits: [
      { "title": "", "text": "Quickly fill roles during peak demand or staff shortages." },
      { "title": "", "text": "Maintain productivity with qualified, safety-trained workers." },
      { "title": "", "text": "Flexible contracts that adapt to your business cycles." }
    ],
    iconLeft: iconIndustrialLeft,
    iconRight: iconIndustrialRight
  }
}

// ESTADO PARA SABER SI MOSTRAMOS BENEFICIOS
const showBenefitsLeft = ref(false);
const showBenefitsRight = ref(false);

// IMPORTANTE: Cuando cambiamos de pestaña (Professional <-> Industrial), reseteamos las tarjetas
watch(activeTab, () => {
  showBenefitsLeft.value = false;
  showBenefitsRight.value = false;
});

const currentContent = computed(() => contentData[activeTab.value]);

const scrollToContact = () => {
  const el = document.querySelector('#contact-section')
  if (el) el.scrollIntoView({ behavior: 'smooth' })
}
</script>



<style scoped>
  /* ... ESTILOS DE HERO, SWITCH BOX E INTRO (MANTENIDOS IGUAL) ... */
  .main-section { width: 100%; position: relative; background-color: #0F2F44; }
  .container { max-width: 100%; margin: 0; padding: 0; }
  .hero-top { position: relative; width: 100%; height: 85vh; min-height: 650px; display: flex; justify-content: center; align-items: center; text-align: center; color: white; z-index: 2; padding-bottom: 80px; }
  .hero-bg { position: absolute; inset: 0; z-index: -1; overflow: hidden; clip-path: polygon(0 0, 100% 0, 100% 100%, 50% 72%, 0 100%); transform: translateZ(0); -webkit-mask-image: -webkit-radial-gradient(white, black); border-radius: 0; }
  .hero-img-layer { position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover; z-index: 0; opacity: 0; transition: opacity 0.3s ease-in-out; will-change: opacity; }
  .hero-img-layer.img-active { opacity: 1; }
  .hero-overlay { position: absolute; inset: 0; background: linear-gradient(to bottom, rgba(168, 189, 67, 0.4) 0%, #0F2F44 95%); z-index: 1; pointer-events: none; }
  .hero-content { position: relative; z-index: 3; }
  .hero-title { font-size: 3.5rem; font-weight: 800; margin-bottom: 30px; text-transform: uppercase; text-shadow: 0 4px 10px rgba(0,0,0,0.3); }
  .hero-subtitle { font-size: 2.9rem; font-weight: 400; text-transform: none; }
  .switch-box { display: inline-flex; background-color: #5ce07d; border-radius: 999px; border: none; backdrop-filter: none; }
  .switch-btn { background: transparent; border: none; color: #ffffff; padding: 18px 38px; border-radius: 999px; font-weight: 600; font-size: 1rem; cursor: pointer; transition: all 0.3s ease; }
  .switch-btn:hover { color: rgba(255, 255, 255, 0.9); }
  .switch-btn.active { background: #ffffff; color: #5ce07d; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15); }
  .dynamic-content-wrapper { background-color: #0F2F44; color: white; margin-top: -14vh; padding-top: 10px; position: relative; z-index: 1; }
  .intro-icon-circle { background: #32d26a; width: 100px; height: 100px; border-radius: 50%; display: flex; align-items: center; justify-content: center; box-shadow: 0 -5px 20px rgba(0,0,0,0.2); border: 4px solid #32d26a; overflow: hidden; }
  .intro-icon-circle img { width: 75%; height: 75%; object-fit: contain; }
  .intro-icon-wrapper { display: flex; justify-content: center; transform: translateY(-50%); position: relative; z-index: 5; margin-bottom: -40px; }
  .intro-block { text-align: center; max-width: 500px; margin: 0 auto 80px; display: flex; flex-direction: column; align-items: center; }
  .intro-text { font-size: 1.15rem; line-height: 1.7; margin-bottom: 30px; opacity: 0.95; }
  .btn-contact-outline { background: transparent; border: 2px solid rgba(255,255,255,0.4); color: white; padding: 16px 40px; border-radius: 50px; display: flex; align-items: center; gap: 10px; cursor: pointer; transition: all 0.3s; font-weight: 500; font-size: 1.2rem; }
  .btn-contact-outline:hover { border-color: #32d26a; background: rgba(50, 210, 106, 0.1); color: #32d26a; }

  .hiring-options {
    margin-top: 40px;
    width: 100%;
    padding: 0;
  }

  .title-wrapper {
    display: flex;
    align-items: center;
    width: 100%;
    margin-bottom: 30px;
  }

  .options-title {
    font-size: 2.2rem;
    font-weight: 800;
    text-align: left;
    margin-left: 5%;
    color: #fff;
    line-height: 1.1;
    margin-bottom: 0;
    flex-shrink: 0;
  }

  /* Estilos para la imagen de las líneas */
  .decoration-lines {
    flex-grow: 1;
    height: auto;
    max-height: 40px;
    margin-left: 30px;
    margin-right: 5%;
    object-fit: cover;
    opacity: 0.6;
  }

  .cards-layout {
    display: flex;
    flex-direction: column;
    width: 100%;
    overflow: visible;
  }

  /* CONFIGURACIÓN DE FILAS */
  .card-row {
    display: flex;
    align-items: stretch; /* ESTIRAR para que el icono tenga la misma altura visual */
    width: 100%;
  }

  /* ICONOS LATERALES (CUADRADOS PLANOS) */
  .side-icon {
    width: auto;
    position: relative;
    z-index: 5;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .side-icon img {
    width: 140px;
    height: 140px;
    padding: 30px;
    border-radius: 0;
    object-fit: contain;
    filter: none;
  }

  .left-icon img {
    background-color: #0F2F44;
    border-radius: 30px 0 0 30px;
  }

  .right-icon img {
    background-color: #5ce07d; /* Verde */
    border-radius: 0;
    padding: 120px;
    margin: 0;
  }

  /* TARJETA BASE */
  .puzzle-card {
    flex: 1;
    padding: 60px 60px;
    position: relative;
    box-shadow: none;
    z-index: 5;
  }

  /* --- FILA SUPERIOR (VERDE) --- */
  .row-left {
    z-index: 2;
    position: relative;
    /* El margen negativo acerca la fila de abajo */
    margin-bottom: -70px;
  }

  .card-green {
    background-color: #5ce07d;
    color: #fff;
    border-radius: 100px 0 0 100px;
    margin-right: 0;
    margin-left: -1px;
  }

  /* Contenedor flexible para el contenido interno */
  .card-content {
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    justify-content: center;
  }

  /* IMPORTANTE: Evita que el botón se estire en la tarjeta verde */
  .card-green .card-content {
    align-items: flex-start;
  }

  /* En la tarjeta azul, mantenemos alineación a la derecha */
  .card-blue .card-content.text-right-content {
    align-items: flex-end;
  }

  /* Header con el botón de atrás */
  .benefits-header {
    display: flex;
    align-items: center;
    gap: 15px;
    margin-bottom: 20px;
  }

  .benefits-header.right-header {
    justify-content: flex-end; /* Alinear a la derecha en tarjeta azul */
  }

  /* Botón círculo de regreso */
  .btn-back-circle {
    background: rgba(255,255,255,0.2);
    border: 2px solid white;
    border-radius: 50%;
    width: 40px;
    height: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    cursor: pointer;
    transition: all 0.3s ease;
  }

  .btn-back-circle:hover {
    background: white;
    color: #0F2F44;
  }

  /* Títulos en la vista de beneficios */
  .benefits-view h3 {
    margin-bottom: 0;
    font-size: 1.6rem;
  }

  /* Lista de beneficios */
  .benefits-list {
    list-style: none;
    padding: 0;
    margin: 0;
    text-align: left;
  }

  .benefits-list li {
    display: flex;
    align-items: flex-start;
    gap: 12px;
    margin-bottom: 15px;
    font-size: 0.95rem;
    line-height: 1.4;
  }

  /* Icono Check SVG */
  .check-icon {
    width: 20px;
    height: 20px;
    stroke: #0F2F44; /* Oscuro por defecto (tarjeta verde) */
    min-width: 20px;
    margin-top: 2px;
  }

  /* Color específico para checks en tarjeta azul */
  .card-blue .check-icon {
    stroke: #5ce07d;
  }

  /* Estilos específicos para alineación derecha (Tarjeta Azul) */
  .right-aligned .benefits-list {
    text-align: right;
  }
  .right-aligned .benefits-list li {
    flex-direction: row-reverse; /* Invierte orden: Texto - Check */
  }

  /* === ANIMACIÓN DE ENTRADA (Slide Fade) === */
  .slide-fade-enter-active,
  .slide-fade-leave-active {
    transition: all 0.3s ease-out;
  }

  .slide-fade-enter-from {
    opacity: 0;
    transform: translateY(20px);
  }

  .slide-fade-leave-to {
    opacity: 0;
    transform: translateY(-20px);
  }
  /* --- FILA INFERIOR (AZUL) --- */
  .row-right {
    z-index: 1;
    position: relative;
    padding-top: 70px;
  }

  .card-blue {
    background-color: #0F2F44;
    color: #fff;
    border-radius: 0 100px 100px 0;
    margin-left: 0;
    text-align: right;
    margin-right: -90px;
    padding-right: 200px;
    z-index: 1000;
  }

  .left-icon {
      align-items: flex-end;
      z-index: 1;
      border-radius: 0;
      padding: 120px;
      margin: 0;
      background-color: #0F2F44;
  }

  /* TIPOGRAFÍA Y BOTONES (SIN CAMBIOS) */
  .puzzle-card h3 {
    font-size: 2.2rem;
    font-weight: 800;
    margin-bottom: 20px;
    text-transform: uppercase;
  }

  .puzzle-card p {
    font-size: 1.1rem;
    line-height: 1.6;
    margin-bottom: 30px;
    opacity: 0.95;
    max-width: 90%;
  }

  .text-right-content {
    display: flex;
    flex-direction: column;
    align-items: flex-end;
    margin-left: auto;
  }

  .btn-pill.white {
    background: white;
    color: inherit;
    border: none;
    padding: 14px 40px;
    border-radius: 999px;
    font-weight: 700;
    font-size: 1rem;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: 0 5px 15px rgba(0,0,0,0.1);
  }

  .card-green .btn-pill.white { color: #5ce07d; }
  .card-blue .btn-pill.white { color: #24465f; }

  .btn-pill.white:hover {
    transform: translateY(-3px);
    box-shadow: 0 10px 20px rgba(0,0,0,0.2);
  }

  /* TRANSICIONES */
  .fade-enter-active, .fade-leave-active { transition: opacity 0.3s ease, transform 0.3s ease; }
  .fade-enter-from, .fade-leave-to { opacity: 0; transform: translateY(10px); }

/* RESPONSIVE */
@media (max-width: 992px) {
  .cards-layout {
    overflow: visible;
  }

  /* 1. APILAR VERTICALMENTE */
  .card-row {
    flex-direction: column;
    width: 100%;
    align-items: center;
    gap: 0; /* Separación entre la tarjeta verde y la azul */
  }

  /* Para la fila azul (donde el icono está a la derecha en desktop),
     invertimos el orden para que en móvil el icono quede ARRIBA de la tarjeta */
  .row-right {
    flex-direction: column-reverse;
    padding-top: 0; /* Reset del padding de desktop */
    margin-top: -50px;
  }

  .row-left {
    margin-bottom: 0; /* Reset del margen negativo de desktop */
  }

  /* 2. ICONOS EN MÓVIL */
  .side-icon {
    width: auto;
    /* Margen negativo para que el icono "muerda" la parte superior de la tarjeta */
    margin-bottom: -50px;
    z-index: 20; /* Siempre encima */
    display: flex;
    justify-content: center;
  }

  .side-icon img {
    width: 100px;
    height: 100px;
    padding: 25px;
    /* En móvil se ven mejor circulares para centrar, o puedes dejarlos cuadrados con border-radius pequeño */
    border-radius: 50%;
  }

  .left-icon{
    padding: 0;
    background-color: transparent;
  }

  .right-icon{
    z-index: 1000;
  }

  /* Colores de fondo de los iconos en móvil (Coinciden con su tarjeta) */
  .left-icon img {
    background-color: #5ce07d; /* Verde */
    position: relative;
    top: 60px;
    padding: 0;
    margin-bottom: 5px;
  }

  .right-icon img {
    background-color: #0F2F44; /* Azul */
    padding: 0;
    position: relative;
    top: 60px;
    margin-bottom: 5px;
  }

  /* 3. TARJETAS */
  .puzzle-card {
    width: 90%; /* Ancho cómodo en móvil */
    margin: 0 auto;
    /* Padding superior grande para dejar espacio al icono */
    padding: 80px 30px 40px;
    text-align: center;
  }

  /* RESETEOS IMPRESCINDIBLES DE DESKTOP */
  .card-green {
    margin-left: 0;  /* Quitamos el margen negativo lateral */
    padding-left: 30px; /* Quitamos el padding extra */
  }

  .card-blue {
    margin-right: 0; /* Quitamos el margen negativo lateral */
    padding-right: 30px; /* Quitamos el padding extra */
    text-align: center;
  }

  /* Centrar textos y botones */
  .text-right-content {
    align-items: center;
    margin-left: 0;
  }

  .puzzle-card h3 {
    font-size: 1.8rem;
  }

  .puzzle-card p {
    max-width: 100%;
    font-size: 1rem;
  }

  .options-title {
    text-align: center;
    margin-left: 0;
    font-size: 1.8rem;
    margin-bottom: 40px;
  }

  /* Centrar contenido en móvil */
  .card-green .card-content,
  .card-blue .card-content.text-right-content {
    align-items: center;
  }

  /* Resetear alineación derecha en móvil */
  .right-aligned .benefits-list {
    text-align: left;
  }
  .right-aligned .benefits-list li {
    flex-direction: row;
  }
  .benefits-header.right-header {
    justify-content: flex-start;
    flex-direction: row-reverse;
  }
}

@media (max-width: 768px) {
  .hero-top {
    height: 75vh;
    min-height: 500px;
    clip-path: polygon(0 0, 100% 0, 100% 100%, 50% 92%, 0 100%);
  }
  .intro-block {
    margin-bottom: 40px;
  }
}

</style>
