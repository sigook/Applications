<template>
  <section class="roles-section">
    <div class="container">
      <h2 class="roles-section__title">We look for:</h2>

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
                <p class="info-text">{{ role.description }}</p>

                <div class="icon-wrapper">
                  <div class="info-icon">
                    <img :src="getRoleIcon(role.icon)" alt="icon" />
                  </div>
                  <button class="plus-btn" @click.stop="openDetail(index)">
                    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
                  </button>
                </div>
              </div>
            </div>

            <div class="role-card__face role-card__face--detail">

              <div class="detail-header-shape">
                <div class="detail-icon">
                   <img :src="getRoleIcon(role.icon)" alt="icon" />
                </div>
              </div>

              <div class="detail-content">
                <h4 class="detail-title">Roles our clients hire for {{ role.title }}</h4>
                <ul class="detail-list">
                  <li v-for="(item, i) in role.detailList" :key="i">{{ item }}</li>
                </ul>
              </div>

              <button class="back-btn" @click.stop="closeDetail()">
                 <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="15 18 9 12 15 6"></polyline></svg>
              </button>
            </div>

          </div>
        </div>
      </div>

    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'

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

// Click en el botón + (Abre la capa de detalle)
const openDetail = (index: number) => {
  detailIndex.value = index;
}

// Click en volver (dentro de detalle)
const closeDetail = () => {
  detailIndex.value = null;
  // Mantenemos activeIndex para que vuelva al dorso "Back", o null para cerrar todo.
  // activeIndex.value = null;
}

const getRoleImage = (imageName: string) => {
  return new URL(`../../assets/images/roles/${imageName}`, import.meta.url).href
}

const getRoleIcon = (iconName: string) => {
  return new URL(`../../assets/images/roles/icons/${iconName}`, import.meta.url).href
}

// --- DATA ---
// Lista genérica para rellenar los roles que no tenían captura específica
const genericList = [
  "Specialized Technicians",
  "Operational Managers",
  "Support Staff",
  "Project Coordinators",
  "Safety Supervisors",
  "Quality Control Specialists"
];

const roles = [
  {
    title: 'Automotive',
    image: 'automotive.png',
    icon: 'car.svg',
    description: "Drive your career forward in the automotive industry. Join the teams that keep the world in motion — one vehicle at a time.",
    detailList: genericList
  },
  {
    title: 'Aviation',
    image: 'aviation.png',
    icon: 'plane.svg',
    description: "Take your aviation career to new heights. Whether you're a ground handler, technician, or logistics professional, we connect you with trusted employers.",
    detailList: genericList
  },
  {
    title: 'Construction',
    image: 'construction.png',
    icon: 'crane.svg',
    description: "Build your future with confidence. We help you find opportunities with companies that value craftsmanship, teamwork, and safety on every job site.",
    detailList: genericList
  },
  {
    title: 'Engineering',
    image: 'engineering.png',
    icon: 'gear.svg',
    description: "Design, innovate, and build the future. We connect top talent in IT, civil works, oil and gas, and industrial sectors with companies leading large-scale projects.",
    detailList: genericList
  },
  {
    title: 'IT / AI',
    image: 'it-ai.png',
    icon: 'chip.svg',
    description: "Step into the fast-paced world of IT and AI, where your skills can power the next digital breakthrough. Join teams that are redefining the future.",
    detailList: genericList
  },
  {
    title: 'Financial/Insurance',
    image: 'finance.png',
    icon: 'money.svg',
    description: "Grow your professional career in finance and insurance. We connect you with organizations that rely on accuracy, integrity, and expertise.",
    // LISTA ESPECÍFICA DE LA CAPTURA
    detailList: [
      "Accounting Clerks and Bookkeepers",
      "Financial Analysts",
      "Claims Processors and Adjusters",
      "Underwriting Assistants",
      "Payroll and Billing Specialists",
      "Data Entry and Compliance Clerks",
      "Credit and Collections Coordinators",
      "Customer Service Representatives",
      "Administrative Assistants",
      "Office Managers"
    ]
  },
  {
    title: 'Legal/Accounting',
    image: 'legal.png',
    icon: 'doc.svg',
    description: "Grow your professional career. We connect you with organizations—helping you contribute to financial stability and long-term success.",
    detailList: genericList
  },
  {
    title: 'Logistics, 3PL/4PL',
    image: 'logistics.png',
    icon: 'truck.svg',
    description: "Drive your logistics career forward. We connect you with opportunities that reward reliability, performance, and growth.",
    detailList: genericList
  },
  {
    title: 'Manufacturing',
    image: 'manufacturing.png',
    icon: 'factory.svg',
    description: "Shape the future of production. Join leading manufacturers looking for dependable, skilled professionals to ensure quality and efficiency.",
    detailList: genericList
  },
  {
    title: 'Retail',
    image: 'retail.png',
    icon: 'store.svg',
    description: "Turn your talent into success. Whether in customer service, merchandising, or management, we help you join retailers who value excellence.",
    detailList: genericList
  },
  {
    title: 'Transportation',
    image: 'transportation.png',
    icon: 'transport.svg',
    description: "Move your career in the right direction. We connect professional drivers and logistics experts with reliable employers who prioritize safety.",
    detailList: genericList
  },
];
</script>

<style scoped>
/* ========================================= */
/* === TUS ESTILOS EXISTENTES (BASE) === */
/* ========================================= */
.roles-section {
  background-color: #05162d;
  padding: 80px 0 120px;
  overflow: hidden;
  border-radius: 0 180px 0 0;
}

.container { max-width: 100%; margin: 0; padding: 0; }
.roles-section__title {
  text-align: center; color: white; margin-bottom: 60px;
  font-size: 2.2rem; font-weight: 700; text-transform: uppercase; letter-spacing: 1px;
}
.roles-list { display: flex; flex-direction: column; gap: 25px; }

.role-card {
  position: relative; width: 65%; height: 240px;
  cursor: pointer; box-shadow: 0 10px 30px rgba(0,0,0,0.4);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  z-index: 1;
}
.role-card:hover { transform: translateY(-5px); box-shadow: 0 15px 40px rgba(0,0,0,0.5); }
.role-card:hover .role-card__img { transform: scale(1.05); }

.role-card--left {
  align-self: flex-start; border-radius: 0 150px 150px 0; text-align: left; box-shadow: 0 50px 0 #010914;
}
.role-card--right {
  align-self: flex-end; border-radius: 150px 0 0 150px; text-align: right; box-shadow: 0 50px 0 #0F2F44;
}

/* Inner Wrapper */
.role-card__inner { position: relative; width: 100%; height: 100%; }

/* Common Face Styles */
.role-card__face {
  position: absolute; inset: 0; width: 100%; height: 100%;
  transition: opacity 0.5s ease, transform 0.5s ease;
  overflow: hidden; /* Importante para el recorte de formas */
}
/* Heredar borderRadius del padre */
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
  flex-direction: row; /* Layout horizontal para título y lista */
  padding: 0;
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
  top: 0;
  right: 0;
  width: 120px;
  height: 100px;
  background-color: #05162d; /* Azul oscuro */
  border-bottom-left-radius: 60px; /* Curva suave */
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2;
}
/* Ajuste para tarjetas izquierdas (si quisieras cambiar lado, pero mantenemos diseño consistente) */
/* Nota: En la captura el icono siempre está a la derecha arriba */

.detail-icon {
  width: 40px; height: 40px;
}
.detail-icon img {
  width: 100%; height: 100%; object-fit: contain;
  filter: brightness(0) saturate(100%) invert(80%) sepia(35%) saturate(860%) hue-rotate(86deg) brightness(98%) contrast(89%);
  /* Filtro aproximado para volver el icono verde si es SVG/PNG blanco, o dejarlo natural si ya es verde */
}

.detail-content {
  padding: 30px 40px;
  padding-right: 130px; /* Espacio para no chocar con el icono azul */
  width: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  text-align: left;
}

.detail-title {
  font-size: 1.2rem;
  font-weight: 800;
  margin-bottom: 15px;
  line-height: 1.2;
}

.detail-list {
  list-style: disc;
  padding-left: 20px;
  margin: 0;
  font-size: 0.85rem;
  line-height: 1.6;
  font-weight: 500;

  /* Layout columnas para la lista */
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 5px 20px;
}

.back-btn {
  position: absolute;
  bottom: 20px;
  left: 20px;
  background: transparent;
  border: none;
  cursor: pointer;
  color: #05162d;
  opacity: 0.5;
  transition: opacity 0.3s;
}
.back-btn:hover { opacity: 1; }

/* ========================================= */
/* === CONTENIDO EXISTENTE (Frente/Dorso) === */
/* ========================================= */
.role-card__bg-wrapper { position: absolute; inset: 0; z-index: 0; }
.role-card__img { width: 100%; height: 100%; object-fit: cover; transition: transform 0.5s ease; }
.role-card__overlay { position: absolute; inset: 0; background: linear-gradient(to right, rgba(12, 34, 59, 0.9) 0%, rgba(12, 34, 59, 0.3) 100%); }
.role-card--right .role-card__overlay { background: linear-gradient(to left, rgba(12, 34, 59, 0.9) 0%, rgba(12, 34, 59, 0.3) 100%); }

.role-card__content { position: relative; z-index: 2; width: 100%; padding: 0 60px; display: flex; justify-content: space-between; align-items: center; color: white; }
.role-card__title { font-size: 2rem; font-weight: 800; text-transform: uppercase; letter-spacing: 0.5px; text-shadow: 0 2px 4px rgba(0,0,0,0.5); }
.role-card__arrow { background: rgba(255,255,255,0.2); backdrop-filter: blur(5px); width: 50px; height: 50px; border-radius: 50%; display: flex; align-items: center; justify-content: center; transition: background 0.3s; flex-shrink: 0; }
.role-card:hover .role-card__arrow { background: #32d26a; color: white; }
.role-card--right .role-card__arrow svg { transform: rotate(180deg); }

/* DORSO */
.info-content { width: 100%; padding: 0 60px; display: flex; align-items: center; justify-content: space-between; color: white; }
.role-card--right .info-content { flex-direction: row-reverse; }

.info-text { font-size: 1.1rem; line-height: 1.5; flex: 1; margin-right: 40px; }
.role-card--right .info-text { margin-right: 0; margin-left: 40px; }

.icon-wrapper { display: flex; flex-direction: column; align-items: center; gap: 15px; flex-shrink: 0; }
.info-icon { width: 70px; height: 70px; display: flex; align-items: center; justify-content: center; }
.info-icon img { width: 100%; height: 100%; object-fit: contain; }

.plus-btn {
  width: 30px; height: 30px; border-radius: 50%; border: 1px solid #32d26a; background: transparent; color: #32d26a; display: flex; align-items: center; justify-content: center; cursor: pointer; transition: all 0.3s ease;
}
.plus-btn:hover { background: #32d26a; color: #12223b; transform: scale(1.1); }

/* RESPONSIVE */
/* === RESPONSIVE (CORREGIDO) === */
@media (max-width: 768px) {
  .role-card {
    width: 100%;
    /* CORRECCIÓN IMPORTANTE: */
    /* Cambiamos height: auto por una altura fija suficiente para el contenido */
    height: 280px;
    /* min-height ya no es necesario si fijamos height, pero lo dejamos por seguridad */
    min-height: 280px;

    border-radius: 20px !important;
    margin-bottom: 20px;
    overflow: hidden; /* Asegura que la imagen no se salga de los bordes redondeados */
  }

  /* Aseguramos que las caras respeten el borde del móvil */
  .role-card__face {
    border-radius: 20px !important;
  }

  /* Ajustes de padding para que quepa la información en pantalla pequeña */
  .role-card__content,
  .info-content,
  .detail-content {
    padding: 20px 25px;
  }

  .detail-content {
    padding-top: 60px; /* Espacio para el icono de esquina */
    padding-right: 25px;
  }

  /* Ajuste de los elementos decorativos de la tarjeta de detalle */
  .detail-header-shape {
    width: 80px;
    height: 60px;
    border-bottom-left-radius: 40px;
  }

  .detail-icon {
    width: 30px;
    height: 30px;
  }

  .role-card__title {
    font-size: 1.4rem;
  }

  /* Ajustamos la lista de detalles para que quepa bien */
  .detail-list {
    grid-template-columns: 1fr; /* Una sola columna en móvil */
    font-size: 0.8rem;
    gap: 4px;
    max-height: 160px; /* Evitar desbordamiento si la lista es larga */
    overflow-y: auto; /* Permitir scroll si es mucho texto */
  }

  /* Ajustes de la cara trasera (Info) */
  .info-text {
    font-size: 0.9rem;
    margin-right: 15px;
    /* Limitamos las líneas para que no se corte el botón */
    display: -webkit-box;
    -webkit-line-clamp: 6;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .role-card--right .info-text {
    margin-left: 15px;
  }

  .info-icon {
    width: 40px;
    height: 40px;
  }
}
</style>
