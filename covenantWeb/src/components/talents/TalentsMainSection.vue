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
            <span class="hero-subtitle">• Find Work •</span>
            TALENTS
        </h1>
        <div class="switch-box">
             <button class="switch-btn" :class="{ 'active': activeTab === 'professional' }" @click="activeTab = 'professional'">Professional</button>
             <button class="switch-btn" :class="{ 'active': activeTab === 'industrial' }" @click="activeTab = 'industrial'">Industrial</button>
        </div>
      </div>
    </div>

    <div class="dynamic-content-wrapper" :class="`bg-${activeTab}`">

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

          <button class="btn-contact-outline">
            OPEN POSITIONS
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="9 18 15 12 9 6"></polyline></svg>
          </button>

        </div>
      </transition>

      <div class="hiring-options container">
        <div class="title-wrapper">
            <h2 class="options-title" v-if="activeTab === 'professional'">Your Career,<br>Your Way</h2>
            <h2 class="options-title" v-else>Your Career,<br>Your Way</h2>

            <img :src="imgDecoration" alt="" class="decoration-lines" />
        </div>

        <transition name="fade" mode="out-in">
          <div :key="activeTab" class="cards-layout">

            <div class="card-row row-left">
              <div class="side-icon left-icon" :class="{ 'hidden-mobile': showBenefitsLeft }">
                <img :src="getCardIcon(currentContent.iconLeft)" alt="Icon Left" />
              </div>

              <div class="puzzle-card card-green" :class="{ 'sharp-mobile': showBenefitsLeft }">
                <transition name="slide-fade" mode="out-in">

                  <div v-if="!showBenefitsLeft" key="front" class="card-content split-layout">
                    <div class="col-title-btn">
                      <h3>{{ currentContent.card1Title }}</h3>
                      <button class="btn-pill white" @click="showBenefitsLeft = true">&lt;&lt; Benefits</button>
                    </div>
                    <div class="col-text">
                      <p>{{ currentContent.card1Desc }}</p>
                    </div>
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
              <div class="puzzle-card card-blue" :class="{ 'sharp-mobile': showBenefitsRight }">
                <transition name="slide-fade" mode="out-in">

                  <div v-if="!showBenefitsRight" key="front" class="card-content text-right-content split-layout">
                    <div class="col-text">
                      <p>{{ currentContent.card2Desc }}</p>
                    </div>
                    <div class="col-title-btn">
                      <h3>{{ currentContent.card2Title }}</h3>
                      <button class="btn-pill white" @click="showBenefitsRight = true">Benefits &gt;&gt;</button>
                    </div>
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

              <div class="side-icon right-icon" :class="{ 'hidden-mobile': showBenefitsRight }">
                <img :src="getCardIcon(currentContent.iconRight)" alt="Icon Right" />
              </div>
            </div>

            </div>
        </transition>
      </div>
    </div>

  </section>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import imgProfessional from '@/assets/images/hero-talents-professional.png'
import imgIndustrial from '@/assets/images/hero-talents-industrial.png'
import imgDecoration from '@/assets/images/lines-decoration.png'
import talentsJson from '../../assets/json/TalentsData.json'

interface TalentBenefit {
  title: string;
  text: string;
}

interface TalentContent {
  introText: string;
  card1Title: string;
  card1Desc: string;
  card1Benefits: TalentBenefit[]; // Nuevo
  card2Title: string;
  card2Desc: string;
  card2Benefits: TalentBenefit[]; // Nuevo
  iconLeft: string;
  iconRight: string;
}

const route = useRoute()
const activeTab = ref<'professional' | 'industrial'>('professional');

const checkQueryParam = () => {
  const type = route.query.type as string | undefined
  if (type === 'professional' || type === 'industrial') {
    activeTab.value = type
  }
}

// Chequear al montar
onMounted(() => {
  checkQueryParam()
})

// Chequear cambios en la URL
watch(() => route.query.type, () => {
  checkQueryParam()
})

// ESTADO PARA SABER SI MOSTRAMOS BENEFICIOS
const showBenefitsLeft = ref(false);
const showBenefitsRight = ref(false);

// IMPORTANTE: Cuando cambiamos de pestaña (Professional <-> Industrial), reseteamos las tarjetas
watch(activeTab, () => {
  showBenefitsLeft.value = false;
  showBenefitsRight.value = false;
});

// Función de iconos (La misma lógica)
const getIcon = (iconName: string) => {
  return new URL(`../../assets/images/roles/icons/${iconName}`, import.meta.url).href
}

const contentData: Record<string, TalentContent> = talentsJson;

const currentContent = computed(() => contentData[activeTab.value]);



const getCardIcon = (fileName: string) => {
  return new URL(`../../assets/images/employers-icons/${fileName}`, import.meta.url).href
}



</script>

<style scoped>
  /* === 1. FONDO Y ESTRUCTURA PRINCIPAL (Vuelve al color original #0F2F44) === */
  .main-section { width: 100%; position: relative; background-color: #ffffff; }
  .container { max-width: 100%; margin: 0; padding: 0; }

  .hero-top { position: relative; width: 100%; height: 85vh; min-height: 650px; display: flex; justify-content: center; align-items: center; text-align: center; color: white; z-index: 2; padding-bottom: 80px; }
  .hero-bg { position: absolute; inset: 0; z-index: -1; overflow: hidden; clip-path: polygon(0 0, 100% 0, 100% 100%, 50% 72%, 0 100%); transform: translateZ(0); -webkit-mask-image: -webkit-radial-gradient(white, black); border-radius: 0; }
  .hero-img-layer { position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover; z-index: 0; opacity: 0; transition: opacity 0.3s ease-in-out; will-change: opacity; }
  .hero-img-layer.img-active { opacity: 1; }
  .hero-overlay { position: absolute; inset: 0; background: linear-gradient(to bottom, rgba(168, 189, 67, 0.4) 0%, #0F2F44 95%); z-index: 1; pointer-events: none; }

  .hero-content { position: relative; z-index: 3; padding-top: 140px; }
  .hero-title { font-size: 3.5rem; font-weight: 800; margin-bottom: 30px; text-transform: uppercase; text-shadow: 0 4px 10px rgba(0,0,0,0.3); line-height: 1; }
  .hero-subtitle { font-size: 2.5rem; font-weight: 300; text-transform: none; color: #59DC76; display: block; margin-bottom: -5px; letter-spacing: 1px; }

  .switch-box { display: inline-flex; background-color: #0F2F44; border-radius: 999px; border: none; backdrop-filter: none; }
  .switch-btn { background: transparent; border: none; color: #ffffff; padding: 18px 38px; border-radius: 999px; font-weight: 600; font-size: 1rem; cursor: pointer; transition: all 0.3s ease; }
  .switch-btn:hover { color: rgba(255, 255, 255, 0.9); }
  .switch-btn.active { background: #ffffff; color: #0F2F44; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15); }

  /* Fondo oscuro para el contenido dinámico con degradado controlado por clases */
  .dynamic-content-wrapper {
    color: black;
    margin-top: -14vh;
    padding-top: 10px;
    position: relative;
    z-index: 1;
    transition: background 0.5s ease; /* Suavizar cambio */
  }

  .dynamic-content-wrapper.bg-professional {
    background: linear-gradient(180deg, rgba(255, 255, 255, 1) 24%, rgba(15, 47, 68, 1) 24%);
  }

  .dynamic-content-wrapper.bg-industrial {
    background: linear-gradient(180deg, rgba(255, 255, 255, 1) 22%, rgba(15, 47, 68, 1) 22%);
  }

  .intro-icon-circle { background: #32d26a; width: 100px; height: 100px; border-radius: 50%; display: flex; align-items: center; justify-content: center; box-shadow: 0 -5px 20px rgba(0,0,0,0.2); border: 4px solid #32d26a; overflow: hidden; }
  .intro-icon-circle img { width: 75%; height: 75%; object-fit: contain; }
  .intro-icon-wrapper { display: flex; justify-content: center; transform: translateY(-50%); position: relative; z-index: 5; margin-bottom: -40px; }

  .intro-block { text-align: center; max-width: 500px; margin: 0 auto 80px; display: flex; flex-direction: column; align-items: center; }
  .intro-text { font-size: 1.15rem; line-height: 1.7; margin-bottom: 30px; opacity: 0.95; }

  .btn-contact-outline { background: #0F2F44; border: none; color: white; padding: 16px 40px; border-radius: 50px; display: flex; align-items: center; gap: 10px; cursor: pointer; transition: all 0.3s; font-weight: 500; font-size: 1.2rem; }
  .btn-contact-outline:hover {background: #0b2130;}

  /* === 2. ESTILOS DEL "ROMPECABEZAS" (REPLICADOS DE EMPLOYERS) === */
  .hiring-options {
    margin-top: 40px;
    width: 100%;
    padding: 0;
  }

  /* Nuevo contenedor flex para alinear texto e imagen */
  .title-wrapper {
    display: flex;
    align-items: center; /* Centrar verticalmente las lineas con el texto */
    width: 100%;
    margin-bottom: 30px; /* Ajustado */
  }

  .options-title {
    font-size: 2.2rem;
    font-weight: 800;
    text-align: left;
    margin-left: 5%;
    color: #fff;
    line-height: 1.1;
    margin-bottom: 0; /* Quitamos margin-bottom aquí porque lo maneja el wrapper */
    flex-shrink: 0; /* Asegura que el texto no se aplaste */
  }

  /* Estilos para la imagen de las líneas */
  .decoration-lines {
    flex-grow: 1; /* Ocupa todo el espacio restante a la derecha */
    height: auto;
    max-height: 40px; /* Ajusta esto según el grosor de tus lineas */
    margin-left: 30px; /* Espacio entre el texto y las líneas */
    margin-right: 5%; /* Para que no pegue con el borde derecho de la pantalla */
    object-fit: cover; /* O contain, dependiendo de como sea tu PNG */
    opacity: 0.6; /* Opcional: para que se vea sutil como en el diseño */
  }

  .cards-layout {
    display: flex;
    flex-direction: column;
    width: 100%;
    overflow: visible;
  }

  /* FILAS DESKTOP */
  .card-row {
    display: flex;
    align-items: stretch; /* ESTIRAR para que el icono tenga la misma altura visual */
    width: 100%;
  }

  /* ICONOS LATERALES (CUADRADOS PLANOS) */
  .side-icon {
    width: auto;
    position: relative;
    z-index: 20;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .side-icon img {
    width: 200px; /* Tamaño grande re-aplicado */
    height: 200px;
    padding: 40px;
    border-radius: 0;
    object-fit: contain;
    filter: none;
    z-index: 20;
    position: relative; /* Asegurar stacking context */
  }

  .left-icon img {
    background-color: #0F2F44;
    border-radius: 30px 0 0 30px;
    padding: 0;
  }

  .right-icon img {
    background-color: #5ce07d; /* Verde */
    border-radius: 0;
    padding: 180px;
    margin: 0;
    z-index: 2000;
    /* FIX: Permitir que el padding sume al tamaño en lugar de restar */
    box-sizing: content-box;
    width: auto;
    height: auto;
  }

  /* TARJETA BASE */
  .puzzle-card {
    flex: 0 1 92%; /* Reducir ancho levemente para no ocupar todo */
    padding: 60px 100px;
    position: relative;
    box-shadow: none;
    z-index: 5;
  }

  /* --- FILA SUPERIOR (VERDE) --- */
  .row-left {
    z-index: 2;
    position: relative;
    justify-content: flex-end; /* Alinear a la derecha (al centro) */
    /* El margen negativo acerca la fila de abajo */
    margin-bottom: -70px;
  }

  .card-green {
    background-color: #5ce07d;
    color: #fff;
    border-radius: 100px 0 0 100px;
    margin-right: 0;
    margin-left: -1px;
    padding-top: 120px; /* Bajarlos un poco */
    padding-bottom: 30px;
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
    justify-content: flex-start; /* Alinear a la izquierda (al centro) */
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
    padding-top: 40px; /* Subirlos un poco */
    z-index: 1000;
  }

  .left-icon {
      align-items: flex-end;
      z-index: 20; /* Asegurar que se vea sobre la tarjeta */
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
  
  /* === SPLIT LAYOUT (Nuevo) === */
  .split-layout {
    flex-direction: row !important; /* Forzamos fila en desktop */
    justify-content: flex-start; /* Por defecto a la izquierda (Green) */
    align-items: center;
    gap: 50px; /* Separarlos un poquito (aumentado de 20px) */
    text-align: left;
  }

  .col-title-btn {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    gap: 20px;
    flex: 0 0 auto;
  }

  /* Ajuste específico para botón en col de título */
  .col-title-btn .btn-pill {
    margin-top: 5px;
  }

  .col-text {
    flex: 0 0 auto;
    display: flex;
    align-items: center;
    max-width: 240px; /* Baja el ancho del texto */
  }

  /* BLUE CARD SPECIFIC */
  .card-blue .split-layout {
    justify-content: flex-end; /* Azul a la derecha */
    text-align: right;
  }

  .card-blue .col-title-btn {
    align-items: flex-end; /* Botón y título a la derecha */
    order: 2; /* Asegurar orden visual si fuera necesario, pero el HTML manda */
  }
  
  .card-blue .col-text {
    order: 1;
    justify-content: flex-end; /* Texto alineado a la derecha en su caja */
  }

  .card-blue .col-text p {
     margin-left: auto; /* Empujar texto a la derecha */
     margin-right: 0;
  }

/* Ajustes para Laptops pequeñas / Tablets (1024px) */
@media (max-width: 1200px) {
  .puzzle-card {
    padding: 60px 50px; /* Reducir padding lateral */
  }

  .split-layout {
    gap: 25px; /* Reducir separación */
  }

  .col-text {
    max-width: 200px; /* Reducir ancho texto */
  }

  /* Ajustar iconos para 1024px */
  .side-icon img {
    width: 150px;
    height: 150px;
    padding: 30px;
  }

  /* Aumentar un poco más solo el icono de la izquierda */
  .left-icon img {
    width: 200px;
    height: 200px;
  }

  .right-icon img {
    padding: 120px; /* Reducir el padding proporcionalmente */
  }
}

/* RESPONSIVE */
@media (max-width: 992px) {
  .hero-content { padding-top: 80px; }
  .hero-title { font-size: 2.8rem; }
  .hero-subtitle { font-size: 2rem; }

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

  /* 4. AJUSTES SECCIÓN BLANCA INTRO (Nuevo requerimiento) */
  .intro-icon-wrapper {
    margin-bottom: -20px;
    padding-top: 80px; /* Aumentado espacio arriba */
  }

  .intro-text {
    max-width: 80%;
    margin-left: auto;
    margin-right: auto;
  }

  /* Botón con más margen abajo para separar del siguiente bloque */
  .intro-block .btn-contact-outline {
    margin-bottom: 60px;
  }

  /* Ajuste del degradado para móvil - Más porcentaje de blanco */
  .dynamic-content-wrapper.bg-professional {
    background: linear-gradient(180deg, rgba(255, 255, 255, 1) 35%, rgba(15, 47, 68, 1) 35%);
  }

  .dynamic-content-wrapper.bg-industrial {
    background: linear-gradient(180deg, rgba(255, 255, 255, 1) 27%, rgba(15, 47, 68, 1) 27%);
  }

  /* 2. ICONOS EN MÓVIL */
  .side-icon {
    width: auto;
    margin-bottom: -50px;
    z-index: 20;
    display: flex;
    justify-content: center;
    border-radius: 50%;
    transition: all 0.5s ease; /* Transición suave */
  }

  .side-icon.hidden-mobile {
    opacity: 0;
    transform: scale(0.9);
    pointer-events: none;
  }

  .side-icon img {
    width: 100px;
    height: 100px;
    padding: 25px;
    border-radius: 50%;
  }

  /* 3. TARJETAS */
  .puzzle-card {
    width: 100%; /* Ancho completo */
    margin: 0 auto;
    /* Padding superior grande para dejar espacio al icono */
    padding: 140px 30px 40px;
    text-align: center;
    transition: border-radius 0.5s ease;
  }

  .puzzle-card.sharp-mobile {
    border-radius: 0 !important;
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
    width: 100%; /* Ancho completo */
    margin: 0 auto;
    /* Padding superior grande para dejar espacio al icono */
    padding: 140px 30px 40px;
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

  /* Resetear split layout en móvil */
  .split-layout {
    flex-direction: column !important;
    text-align: center !important;
    gap: 20px;
  }
  
  .col-title-btn {
    align-items: center !important;
    flex: auto;
    width: 100%;
  }
  
  .col-text {
    width: 100%;
    justify-content: center !important;
  }
  
  .card-blue .col-title-btn {
    order: 0; /* Título primero en móvil también para azul */
  }
              

  }

  @media (max-width: 768px) {
    .hero-top { height: 75vh; min-height: 500px; clip-path: polygon(0 0, 100% 0, 100% 100%, 50% 92%, 0 100%); }
    .intro-block { margin-bottom: 40px; }

  }
</style>
