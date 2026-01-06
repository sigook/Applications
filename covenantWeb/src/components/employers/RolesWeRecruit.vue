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

// Asegúrate de que esta ruta sea correcta según tu estructura de carpetas
import rolesData from '../../assets/json/RolesEmployersData.json'

interface Role {
  title: string;
  image: string;
  icon: string;
  description: string;
}

const activeIndex = ref<number | null>(null);

const toggleCard = (index: number) => {
  activeIndex.value = activeIndex.value === index ? null : index;
}

const getRoleImage = (imageName: string) => {
  return new URL(`../../assets/images/roles/${imageName}`, import.meta.url).href
}

const getRoleIcon = (iconName: string) => {
  return new URL(`../../assets/images/roles/icons/${iconName}`, import.meta.url).href
}

const roles: Role[] = rolesData;
</script>

<style scoped>
.roles-section {
  background-color: #0F2F44;
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
  font-weight: 300;
  text-transform: capitalize;
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
  cursor: pointer;
  z-index: 1;
  box-shadow: 0 10px 30px rgba(0,0,0,0.4);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.role-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 40px rgba(0,0,0,0.5);
}

.role-card:hover .role-card__img {
  transform: scale(1.05);
}

/* Formas de las tarjetas */
.role-card--left {
  align-self: flex-start;
  border-radius: 0 150px 150px 0;
  box-shadow: 0 50px 0 #010914;
}

.role-card--right {
  align-self: flex-end;
  border-radius: 150px 0 0 150px;
  box-shadow: 0 50px 0 #184461;
}

/* LÓGICA INTERNA DE CARAS */
.role-card__inner {
  position: relative;
  width: 100%;
  height: 100%;
  border-radius: inherit;
  overflow: hidden;
  -webkit-mask-image: -webkit-radial-gradient(white, black);
  mask-image: radial-gradient(white, black);
  transform: translateZ(0);
}

.role-card__face {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  transition: opacity 0.5s ease, transform 0.5s ease;
}

/* --- CARA FRONTAL --- */
.role-card__face--front {
  display: flex;
  align-items: center;
  z-index: 2;
  opacity: 1;
}

/* --- CARA TRASERA --- */
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

/* IMAGEN Y OVERLAY */
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
  padding: 0 80px;
  display: flex;
  align-items: center;
  color: white;
  gap: 30px;
}

/* Tarjeta Izquierda: Contenido alineado a la derecha (Centro pantalla) */
.role-card--left .role-card__content {
  justify-content: flex-end;
  text-align: right;
  flex-direction: row;
}

/* Tarjeta Derecha: Contenido alineado a la izquierda (Centro pantalla) */
.role-card--right .role-card__content {
  justify-content: flex-end;
  flex-direction: row-reverse;
  text-align: left;
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

.info-content {
  width: 100%;
  padding: 0 60px;
  display: flex;
  align-items: center;
  gap: 40px;
  color: white;
}

/* Tarjeta Izquierda: Icono al borde izquierdo (pantalla), Texto al centro */
.role-card--left .info-content {
  flex-direction: row-reverse;
  text-align: right;
}

/* Tarjeta Derecha: Icono al borde derecho (pantalla), Texto al centro */
.role-card--right .info-content {
  flex-direction: row;
  text-align: left;
}

.info-text {
  font-size: 1.1rem;
  line-height: 1.5;
  flex: 1;
}

.info-icon {
  width: 110px;
  height: 110px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  border-radius: 50%;
}

.info-icon img {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

@media (max-width: 768px) {

  .role-card {
    width: 100%;
    height: 200px;
    margin: 0 auto 25px auto;
    align-self: auto;
    box-shadow: 0 10px 25px rgba(0,0,0,0.3);
  }

  .role-card--left, .role-card--right {
    align-self: auto;
  }

  .role-card--left {
    box-shadow: 0 50px 0 #010914;
  }

  .role-card--right {
    box-shadow: 0 50px 0 #184461;
  }

  .role-card__inner {
    border-radius: inherit !important;
    overflow: hidden;
  }

  .role-card__content {
    padding: 0 30px;
    gap: 15px;
  }

  .role-card__title {
    font-size: 1.3rem;
  }

  .role-card__arrow {
    width: 40px;
    height: 40px;
  }

  .role-card__arrow svg {
    width: 20px;
    height: 20px;
  }

  .info-content {
    padding: 0 30px;
    gap: 20px;
  }

  .info-text {
    font-size: 0.9rem;
    flex: 1;
    margin: 0 !important;
  }

  .info-icon {
    width: 60px;
    height: 60px;
  }
}
</style>
