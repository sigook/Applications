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
            'is-active': activeIndex === index
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
                <div class="info-icon">
                  <img :src="getRoleIcon(role.icon)" alt="icon" />
                </div>
              </div>
            </div>

          </div>
        </div>
      </div>

    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// 1. IMPORTAR LOS DATOS DEL JSON
// Ajusta la ruta relativa (../../) según donde esté ubicado tu componente
import rolesData from '../../assets/json/RolesEmployersData.json'

// (Opcional) Definir interfaz para TypeScript para mayor seguridad
interface Role {
  title: string;
  image: string;
  icon: string;
  description: string;
}

// --- ESTADO Y LÓGICA ---
const activeIndex = ref<number | null>(null);

const toggleCard = (index: number) => {
  activeIndex.value = activeIndex.value === index ? null : index;
}

// Función para cargar la IMAGEN DE FONDO (Igual que antes)
const getRoleImage = (imageName: string) => {
  return new URL(`../../assets/images/roles/${imageName}`, import.meta.url).href
}

// Función NUEVA para cargar el ICONO desde la subcarpeta "icons"
const getRoleIcon = (iconName: string) => {
  // Asume ruta: src/assets/images/roles/icons/nombre.svg
  return new URL(`../../assets/images/roles/icons/${iconName}`, import.meta.url).href
}

// --- DATA ---
// Asignamos los datos importados a la constante roles
const roles: Role[] = rolesData;
</script>

<style scoped>
/* SE MANTIENEN TUS ESTILOS EXACTOS */
.roles-section {
  background-color: #05162d;
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
  gap: 25px;
}

.role-card {
  position: relative;
  width: 65%;
  height: 240px;
  overflow: hidden;
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
  transform: scale(1.05);
}

.role-card--left {
  align-self: flex-start;
  border-radius: 0 150px 150px 0;
  text-align: left;
  box-shadow: 0 50px 0 #010914;
}

.role-card--right {
  align-self: flex-end;
  border-radius: 150px 0 0 150px;
  text-align: right;
  box-shadow: 0 50px 0 #0F2F44;
}

/* LÓGICA INTERNA DE CARAS */
.role-card__inner {
  position: relative;
  width: 100%;
  height: 100%;
}

.role-card__face {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  transition: opacity 0.5s ease, transform 0.5s ease;
}

.role-card__face--front {
  display: flex;
  align-items: center;
  z-index: 2;
  opacity: 1;
}

.role-card--right .role-card__face--front {
  flex-direction: row-reverse;
}

.role-card__face--back {
  background-color: #0d2644;
  z-index: 1;
  opacity: 0;
  transform: translateY(20px);
  display: flex;
  align-items: center;
  justify-content: center;
}

.role-card.is-active .role-card__face--front {
  opacity: 0;
  transform: scale(0.95);
  pointer-events: none;
}

.role-card.is-active .role-card__face--back {
  opacity: 1;
  transform: translateY(0);
  z-index: 3;
}

/* CONTENIDO */
.role-card__bg-wrapper {
  position: absolute;
  inset: 0;
  z-index: 0;
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
  background: linear-gradient(to right, rgba(12, 34, 59, 0.9) 0%, rgba(12, 34, 59, 0.3) 100%);
}

.role-card--right .role-card__overlay {
  background: linear-gradient(to left, rgba(12, 34, 59, 0.9) 0%, rgba(12, 34, 59, 0.3) 100%);
}

.role-card__content {
  position: relative;
  z-index: 2;
  width: 100%;
  padding: 0 60px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: white;
}

.role-card__title {
  font-size: 2rem;
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
  flex-shrink: 0;
}

.role-card:hover .role-card__arrow {
  background: #32d26a;
  color: white;
}

.role-card--right .role-card__arrow svg {
  transform: rotate(180deg);
}

/* CONTENIDO TRASERO */
.info-content {
  width: 100%;
  padding: 0 60px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  color: white;
}

.role-card--right .info-content {
  flex-direction: row-reverse;
}

.info-text {
  font-size: 1.1rem;
  line-height: 1.5;
  flex: 1;
  margin-right: 40px;
}

.role-card--right .info-text {
  margin-right: 0;
  margin-left: 40px;
}

.info-icon {
  width: 70px;
  height: 70px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

/* NUEVO: ESTILO PARA LA IMAGEN DEL ICONO */
.info-icon img {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

@media (max-width: 768px) {
  .role-card {
    width: 100%;
    height: 180px;
    border-radius: 20px !important;
    margin-bottom: 20px;
  }

  .role-card__content, .info-content {
    padding: 0 30px;
  }

  .role-card__title {
    font-size: 1.5rem;
  }

  .info-text {
    font-size: 0.9rem;
    margin-right: 20px;
  }
  .role-card--right .info-text {
    margin-left: 20px;
  }
  .info-icon {
    width: 40px;
    height: 40px;
  }
}
</style>
