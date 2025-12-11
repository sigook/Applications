<template>
  <section class="roles-section">
    <div class="container">
      <h2 class="roles-section__title">Roles We Recruit:</h2>

      <div class="roles-list">
        <div
          v-for="(role, index) in roles"
          :key="index"
          class="role-card"
          :class="{ 'role-card--right': index % 2 !== 0, 'role-card--left': index % 2 === 0 }"
        >
          <div class="role-card__bg-wrapper">
            <img src="@/assets/images/hero-bg.png" :alt="role.title" class="role-card__img" />
            <div class="role-card__overlay"></div>
          </div>

          <div class="role-card__content">
            <h3 class="role-card__title">{{ role.title }}</h3>

            <div class="role-card__arrow">
              <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg>
            </div>
          </div>
        </div>
      </div>

    </div>
  </section>
</template>

<script setup lang="ts">
// Data de ejemplo (Asegúrate de tener las imágenes)
const roles = [
  { title: 'Automotive', image: 'automotive.jpg' },
  { title: 'Aviation', image: 'aviation.jpg' },
  { title: 'Construction', image: 'construction.jpg' },
  { title: 'IT / AI', image: 'it-ai.jpg' },
  { title: 'Financial/Insurance', image: 'finance.jpg' },
  { title: 'Legal/Accounting', image: 'legal.jpg' },
  { title: 'Logistics, 3PL/4PL', image: 'logistics.jpg' },
  { title: 'Manufacturing', image: 'manufacturing.jpg' },
  { title: 'Retail', image: 'retail.jpg' },
  { title: 'Transportation', image: 'transportation.jpg' },
];
</script>

<style scoped>
.roles-section {
  background-color: #05162d; /* Fondo oscuro continuo */
  padding: 80px 0 120px;
  overflow: hidden;
}

.container {
  max-width: 100%;
  margin: 0;
  padding: 0;
}

.roles-section__title {
  text-align: center;
  color: white;
  margin-bottom: 60px;
  font-size: 2.2rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.roles-list {
  display: flex;
  flex-direction: column;
  gap: 25px; /* Espacio vertical entre tarjetas */
}

/* === ESTILOS BASE DE LA TARJETA === */
.role-card {
  position: relative;
  width: 65%; /* Ocupan el 85% del ancho para dejar espacio al lado contrario */
  height: 240px; /* AUMENTADO: Mucho más grandes que antes */
  display: flex;
  align-items: center;
  overflow: hidden; /* Para que la imagen respete los bordes redondeados */
  cursor: pointer;
  box-shadow: 0 10px 30px rgba(0,0,0,0.4);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  z-index: 1;
}

.role-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 40px rgba(0,0,0,0.5);
}

.role-card:hover .role-card__img {
  transform: scale(1.05); /* Zoom suave en la imagen */
}

/* === INTERCALADO (ZIG ZAG) === */

/* IMPARES (Izquierda) */
.role-card--left {
  align-self: flex-start; /* Se pega a la izquierda */
  /* Redondeado solo en el lado derecho (Top-Right, Bottom-Right) */
  border-radius: 0 150px 150px 0;
  text-align: left;
}

/* PARES (Derecha) */
.role-card--right {
  align-self: flex-end; /* Se pega a la derecha */
  /* Redondeado solo en el lado izquierdo (Top-Left, Bottom-Left) */
  border-radius: 150px 0 0 150px;
  /* Invertimos el contenido para que el texto quede "adentro" */
  flex-direction: row-reverse;
  text-align: right;
}

/* === IMAGEN DE FONDO === */
.role-card__bg-wrapper {
  position: absolute;
  inset: 0;
  z-index: 1;
}

.role-card__img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.role-card__overlay {
  position: absolute;
  inset: 0;
  /* Gradiente para que el texto sea legible */
  background: linear-gradient(to right, rgba(12, 34, 59, 0.9) 0%, rgba(12, 34, 59, 0.3) 100%);
}

/* Invertimos el gradiente para las tarjetas derechas */
.role-card--right .role-card__overlay {
  background: linear-gradient(to left, rgba(12, 34, 59, 0.9) 0%, rgba(12, 34, 59, 0.3) 100%);
}

/* === CONTENIDO (TEXTO E ICONO) === */
.role-card__content {
  position: relative;
  z-index: 2;
  width: 100%;
  padding: 0 60px; /* Espacio interno generoso */
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: white;
}

.role-card__title {
  font-size: 2rem; /* Texto más grande */
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  text-shadow: 0 2px 4px rgba(0,0,0,0.5);
}

.role-card__arrow {
  background: rgba(255,255,255,0.2);
  backdrop-filter: blur(5px);
  width: 50px;
  height: 50px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.3s;
}

.role-card:hover .role-card__arrow {
  background: #32d26a; /* Verde al hover */
  color: white;
}

/* Rotar flecha si está a la derecha */
.role-card--right .role-card__arrow svg {
  transform: rotate(180deg);
}

/* === RESPONSIVE === */
@media (max-width: 768px) {
  .role-card {
    width: 100%; /* En móvil ocupan todo el ancho */
    height: 180px;
    border-radius: 20px !important; /* Bordes simples en móvil */
    margin-bottom: 20px;
  }

  .role-card__content {
    padding: 0 30px;
  }

  .role-card__title {
    font-size: 1.5rem;
  }
}
</style>
