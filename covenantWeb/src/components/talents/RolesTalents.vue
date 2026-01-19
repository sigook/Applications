<template>
  <section class="roles-section">
    <div class="container">
      <h2 class="roles-section__title">Roles We Recruit:</h2>

      <div class="roles-list">
        <div
          v-for="(role, index) in roles"
          :key="index"
          class="role-card"
          :class="{
            'role-card--right': index % 2 !== 0,
            'role-card--left': index % 2 === 0,
            'is-active': activeIndex === index,
            'is-detailed': detailIndex === index
          }"
          @click="toggleCard(index)"
        >
          <div class="role-card__inner">

            <div class="role-card__face role-card__face--front">
              <div class="role-card__bg-wrapper">
                <img :src="getRoleImage(role.image)" :alt="role.title" class="role-card__img" />
                <div class="role-card__overlay"></div>
              </div>

              <div class="role-card__content">
                <h3 class="role-card__title">{{ role.title }}</h3>
                <div class="role-card__arrow">
                  <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg>
                </div>
              </div>
            </div>

            <div class="role-card__face role-card__face--back">
              <div class="info-content">
                <div class="info-icon">
                  <img :src="getRoleIcon(role.icon)" alt="icon" />
                </div>

                <p class="info-text">{{ role.description }}</p>

                <button class="plus-btn" @click.stop="openDetail(index)">
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
                </button>
              </div>
            </div>

            <div class="role-card__face role-card__face--detail">

              <div class="detail-header-shape">
                <div class="detail-icon">
                   <img :src="getRoleIcon(role.icon)" alt="icon" />
                </div>
              </div>

              <div class="detail-content">
                <h4 class="detail-title">Roles our clients hire for <br /> {{ role.title }}</h4>
                <ul class="detail-list">
                  <li v-for="(item, i) in role.detailList" :key="i">{{ item }}</li>
                </ul>
              </div>

              <!-- Back button removed -->
            </div>

          </div>
        </div>
      </div>

    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// 1. IMPORTAR EL JSON
import rolesTalentsData from '../../assets/json/RolesTalentsData.json'

interface RoleTalent {
  title: string;
  image: string;
  icon: string;
  description: string;
  detailList: string[];
}

// --- ESTADO ---
const activeIndex = ref<number | null>(null);
const detailIndex = ref<number | null>(null);

// Click en la tarjeta general (Alterna entre Frente y Dorso, o cierra si está abierto)
const toggleCard = (index: number) => {
  if (detailIndex.value === index) {
    // Si está en detalle, volver a la normalidad (cerrar todo o volver al dorso)
    detailIndex.value = null;
    activeIndex.value = null; // Cerramos todo para reiniciar ciclo
  } else {
    // Toggle normal frente/dorso
    activeIndex.value = activeIndex.value === index ? null : index;
    detailIndex.value = null; // Asegurar que detalle esté cerrado al abrir
  }
}

// Click en el botón +
const openDetail = (index: number) => {
  detailIndex.value = index;
}

const getRoleImage = (imageName: string) => {
  return new URL(`../../assets/images/roles/${imageName}`, import.meta.url).href
}

const getRoleIcon = (iconName: string) => {
  return new URL(`../../assets/images/roles/icons/${iconName}`, import.meta.url).href
}

// --- DATA ---
// 2. ASIGNAR LOS DATOS IMPORTADOS
const roles: RoleTalent[] = rolesTalentsData;
</script>

<style scoped>
/* ========================================= */
/* === TUS ESTILOS EXISTENTES (BASE) === */
/* ========================================= */
.roles-section {
  background-color: #0F2F44;
  padding: 80px 0 120px;
  overflow: hidden;
  border-radius: 0 180px 0 0;
}

.container { max-width: 100%; margin: 0; padding: 0; }
.roles-section__title {
  text-align: center; color: white; margin-bottom: 60px;
  font-size: 2.2rem; font-weight: 300; text-transform: capitalize; letter-spacing: 1px;
}
.roles-list { display: flex; flex-direction: column; gap: 25px; }

.role-card {
  position: relative; width: 65%; height: 240px;
  cursor: pointer; box-shadow: 0 10px 30px rgba(0,0,0,0.4);
  transition: all 0.5s cubic-bezier(0.25, 0.8, 0.25, 1); /* Transición suave para todo (ancho, bordes, etc) */
  z-index: 1;
}
.role-card:hover { transform: translateY(-5px); box-shadow: 0 15px 40px rgba(0,0,0,0.5); }
.role-card:hover .role-card__img { transform: scale(1.05); }

/* ESTADO EXPANDIDO (DETALLE) */
.role-card.is-detailed {
  width: 100%; /* Ocupar todo el ancho */
  height: 400px; /* Altura ajustada */
  border-radius: 150px 0 150px 0; /* Solo bordes Sup-Izq e Inf-Der redondeados */
  z-index: 10; /* Elevar sobre los demás */
  align-self: center; /* Centrar para evitar saltos extraños desde left/right */
}
/* Forzar herencia de bordes en el inner y faces */
.role-card.is-detailed .role-card__inner,
.role-card.is-detailed .role-card__face {
  border-radius: 150px 0 150px 0;
}

.role-card--left {
  align-self: flex-start; border-radius: 0 150px 150px 0; text-align: left; box-shadow: 0 50px 0 #162731;
}
.role-card--right {
  align-self: flex-end; border-radius: 150px 0 0 150px; text-align: right; box-shadow: 0 50px 0 #184461;
}

/* Inner Wrapper */
.role-card__inner {
  position: relative;
  width: 100%;
  height: 100%;
  border-radius: inherit;
  overflow: hidden;
  -webkit-mask-image: -webkit-radial-gradient(white, black);
  mask-image: radial-gradient(white, black);
  transform: translateZ(0);
  transition: border-radius 0.5s ease;
}

/* Common Face Styles */
.role-card__face {
  position: absolute; inset: 0; width: 100%; height: 100%;
  transition: opacity 0.5s ease, transform 0.5s ease, border-radius 0.5s ease;
  overflow: hidden; /* Importante para el recorte de formas */
  border-radius: inherit;
}
/* Heredar borderRadius del padre (Ya no es necesario explícito aquí si usamos inherit, pero mantenemos por especificidad en estados normales) */
.role-card--left .role-card__face { border-radius: 0 150px 150px 0; }
.role-card--right .role-card__face { border-radius: 150px 0 0 150px; }

/* --- 1. FRONT FACE --- */
.role-card__face--front { display: flex; align-items: center; z-index: 2; opacity: 1; }
.role-card--right .role-card__face--front { flex-direction: row-reverse; }

/* --- 2. BACK FACE --- */
.role-card__face--back {
  background-color: #0d2644; z-index: 1; opacity: 0; transform: translateY(20px);
  display: flex; align-items: center; justify-content: center;
}

/* ESTADOS ACTIVOS (Frente vs Dorso) */
.role-card.is-active .role-card__face--front { opacity: 0; transform: scale(0.95); pointer-events: none; }
.role-card.is-active .role-card__face--back { opacity: 1; transform: translateY(0); z-index: 3; }

/* ========================================= */
/* === 3. DETAIL FACE (NUEVO) === */
/* ========================================= */
.role-card__face--detail {
  background-color: #5ce07d; /* Verde brillante de la captura */
  z-index: 1;
  opacity: 0;
  transform: translateX(100%); /* Entra deslizando desde el lado */
  display: flex;
  /* flex-direction: row;  <-- Layout horizontal */
  color: #05162d; /* Texto oscuro */
}

/* Estado Detalle Activo */
.role-card.is-detailed .role-card__face--front,
.role-card.is-detailed .role-card__face--back {
  opacity: 0; pointer-events: none;
}
.role-card.is-detailed .role-card__face--detail {
  opacity: 1;
  transform: translateX(0);
  z-index: 4; /* Encima de todo */
}

/* Diseño interno de la tarjeta de detalle */

/* Forma decorativa superior derecha (Azul oscura con icono) */
.detail-header-shape {
  position: absolute;
  top: 50%; /* Centrado vertical */
  right: 0;
  transform: translateY(-50%);
  width: 160px;
  height: 140px; /* Más alto */
  background-color: #05162d; /* Azul oscuro */
  border-radius: 100px 0 0 100px; /* Curva borde izquierdo */
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2;
  box-shadow: -5px 0 10px rgba(0,0,0,0.1);
}

.detail-icon {
  width: 50px; height: 50px;
}
.detail-icon img {
  width: 100%; height: 100%; object-fit: contain;
  filter: brightness(0) saturate(100%) invert(80%) sepia(35%) saturate(860%) hue-rotate(86deg) brightness(98%) contrast(89%);
}

.detail-content {
  width: 100%;
  height: 100%;
  padding: 0 140px 0 60px; /* Espacio derecho para el icono */
  display: flex;
  flex-direction: row; /* Horizontal */
  align-items: center; /* Centrado vertical */
  justify-content: space-between;
  gap: 40px;
}

.detail-title {
  flex: 0 0 40%; /* Título ocupa 40% */
  font-size: 1.8rem;
  font-weight: 800;
  margin-bottom: 0;
  line-height: 1.1;
  text-align: left;
}

.detail-list {
  flex: 1;
  list-style: disc;
  padding-left: 20px;
  margin: 0;
  font-size: 0.9rem;
  line-height: 1.6;
  font-weight: 600;
  text-align: left; /* Asegurar alineación izquierda incluso en cartas 'right' */

  /* Layout columnas para la lista */
  display: flex;
  flex-direction: column;
  gap: 5px; 
  /* Scroll si es necesario */
  max-height: 300px; 
  overflow-y: auto;
}



/* ========================================= */
/* === CONTENIDO EXISTENTE (Frente/Dorso) === */
/* ========================================= */
.role-card__bg-wrapper { position: absolute; inset: 0; z-index: 0; }
.role-card__img { width: 100%; height: 100%; object-fit: cover; transition: transform 0.5s ease; }
.role-card__overlay { position: absolute; inset: 0; background: linear-gradient(to right, rgba(12, 34, 59, 0.9) 0%, rgba(12, 34, 59, 0.3) 100%); }
.role-card--right .role-card__overlay { background: linear-gradient(to left, rgba(12, 34, 59, 0.9) 0%, rgba(12, 34, 59, 0.3) 100%); }

.role-card__content { 
  position: relative; 
  z-index: 2; 
  width: 100%; 
  padding: 0 60px; 
  display: flex; 
  justify-content: flex-end; /* Alineado al final */
  align-items: center; 
  color: white; 
  gap: 30px; /* Separación flecha-titulo */
}

/* LEFT CARD: Title - Arrow (Aligned Right) */
.role-card--left .role-card__content {
  flex-direction: row;
  text-align: right;
}

/* RIGHT CARD: Arrow - Title (Aligned Left -> Flex Reverse at End) */
.role-card--right .role-card__content {
  flex-direction: row-reverse;
  text-align: left;
}

.role-card__title { font-size: 2rem; font-weight: 800; text-transform: uppercase; letter-spacing: 0.5px; text-shadow: 0 2px 4px rgba(0,0,0,0.5); }
.role-card__arrow { background: rgba(255,255,255,0.2); backdrop-filter: blur(5px); width: 50px; height: 50px; border-radius: 50%; display: flex; align-items: center; justify-content: center; transition: background 0.3s; flex-shrink: 0; }
.role-card:hover .role-card__arrow { background: #32d26a; color: white; }
.role-card--right .role-card__arrow svg { transform: rotate(180deg); }

/* DORSO */
.info-content { 
  width: 100%; 
  padding: 0 60px; 
  display: flex; 
  align-items: center; 
  justify-content: space-between; 
  color: white; 
  gap: 20px;
}

/* RIGHT CARD (Redondeado Izq): Icono Izq - Texto - Botón Der */
.role-card--right .info-content {
  flex-direction: row;
  text-align: left;
}

/* LEFT CARD (Redondeado Der): Botón Izq - Texto - Icono Der */
.role-card--left .info-content {
  flex-direction: row-reverse;
  text-align: right;
}

.info-text { font-size: 1.1rem; line-height: 1.5; flex: 1; }

.info-icon { width: 100px; height: 100px; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.info-icon img { width: 100%; height: 100%; object-fit: contain; }

.plus-btn {
  width: 40px; height: 40px; border-radius: 50%; border: 1px solid #32d26a; background: transparent; color: #32d26a; display: flex; align-items: center; justify-content: center; cursor: pointer; transition: all 0.3s ease; flex-shrink: 0;
}
.plus-btn:hover { background: #32d26a; color: #12223b; transform: scale(1.1); }

/* RESPONSIVE */
@media (max-width: 768px) {
  .role-card {
    width: 100%;
    height: 200px;
    min-height: 200px;
    margin: 0 auto 25px auto;
    align-self: auto;
    box-shadow: 0 10px 25px rgba(0,0,0,0.3);
  }

  .role-card--left, .role-card--right {
    align-self: auto;
  }

  /* Sombras de color sólidas en móvil para mantener consistencia */
  .role-card--left {
    box-shadow: 0 50px 0 #162731;
  }
  .role-card--right {
    box-shadow: 0 50px 0 #184461;
  }

  .role-card__inner {
    border-radius: inherit !important;
    overflow: hidden;
  }

  .role-card__content,
  .info-content,
  .detail-content {
    padding: 20px 30px;
  }
  
  /* Ajuste de detalle en móvil: Columna vertical */
  .detail-content {
    flex-direction: column;
    align-items: flex-start;
    padding-top: 60px; /* Espacio para icono */
    padding-right: 25px;
    justify-content: center;
    gap: 15px;
  }

  /* Reset de la forma decorativa en móvil */
  .detail-header-shape {
    top: 20px;
    right: 0;
    transform: none;
    width: 80px;
    height: 60px;
    border-radius: 40px 0 0 40px;
    box-shadow: none;
  }
  
  .detail-icon {
    width: 30px; height: 30px;
  }

  .detail-title {
    font-size: 1.3rem; 
    flex: none; 
    width: 100%;
    margin-bottom: 5px;
  }

  .role-card__title {
    font-size: 1.4rem;
  }

  .role-card__arrow {
    width: 40px; height: 40px;
  }
  .role-card__arrow svg { width: 20px; height: 20px; }

  /* Info Content Responsive */
  .info-text {
    font-size: 0.9rem;
    display: -webkit-box;
    -webkit-line-clamp: 6;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .info-icon { width: 50px; height: 50px; }

  /* Increase height significantly when detailed on mobile */
  .role-card.is-detailed {
    height: 450px !important; /* Altura reducida para móvil */
  }

  /* Allow list to expand */
  .detail-list {
    grid-template-columns: 1fr;
    font-size: 0.8rem;
    gap: 4px;
    max-height: 250px; /* Reducido acorde a la carta */
    overflow-y: auto;
  }
}
</style>
