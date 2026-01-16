<template>
  <header
    class="navbar"
    :class="{
      'navbar--scrolled': isScrolled,
      'navbar--light-theme': isLightPage
    }"
  >
    <div class="navbar__inner">

      <RouterLink
        to="/"
        class="navbar__logo-link"
        :class="{ 'desktop-hidden': isHomePage }"
      >
        <img
          :src="currentLogo"
          alt="Covenant Group"
          class="navbar__logo-img"
        />
      </RouterLink>

      <nav class="navbar__menu">
        <RouterLink
          to="/"
          class="navbar__link"
          active-class="navbar__link--active"
          :class="{ 'desktop-hidden': !isHomePage }"
        >HOME</RouterLink>
        <RouterLink to="/open-positions" class="navbar__link" active-class="navbar__link--active">OPEN POSITIONS</RouterLink>
        <RouterLink to="/industries" class="navbar__link" active-class="navbar__link--active">INDUSTRIES</RouterLink>
        <RouterLink to="/about" class="navbar__link" active-class="navbar__link--active">ABOUT US</RouterLink>
        <RouterLink to="/employers" class="navbar__link" active-class="navbar__link--active">EMPLOYERS</RouterLink>
        <RouterLink to="/talents" class="navbar__link" active-class="navbar__link--active">TALENTS</RouterLink>
        <RouterLink to="/become-partner" class="navbar__link" active-class="navbar__link--active">BECOME A PARTNER</RouterLink>
        <RouterLink to="/licensed-certified" class="navbar__link" active-class="navbar__link--active">LICENSED & CERTIFIED</RouterLink>
      </nav>

      <button class="navbar__toggle" @click="toggleMobile" aria-label="Open menu">
        <span :class="{ 'navbar__toggle-line--open': isMobileOpen }"></span>
        <span :class="{ 'navbar__toggle-line--open': isMobileOpen }"></span>
        <span :class="{ 'navbar__toggle-line--open': isMobileOpen }"></span>
      </button>

    </div>

    <transition name="nav-slide">
      <div
        v-if="isMobileOpen"
        class="navbar__mobile-overlay"
        @click.self="closeMobile"
      >
        <nav class="navbar__mobile">
          <RouterLink @click="closeMobile" to="/" class="navbar__mobile-link">Home</RouterLink>
          <RouterLink @click="closeMobile" to="/open-positions" class="navbar__mobile-link">Open Positions</RouterLink>
          <RouterLink @click="closeMobile" to="/industries" class="navbar__mobile-link">Industries</RouterLink>
          <RouterLink @click="closeMobile" to="/about" class="navbar__mobile-link">About Us</RouterLink>
          <RouterLink @click="closeMobile" to="/employers" class="navbar__mobile-link">Employers</RouterLink>
          <RouterLink @click="closeMobile" to="/talents" class="navbar__mobile-link">Talents</RouterLink>
          <RouterLink @click="closeMobile" to="/become-partner" class="navbar__mobile-link">Become a Partner</RouterLink>
          <RouterLink @click="closeMobile" to="/licensed-certified" class="navbar__mobile-link">Licensed & Certified</RouterLink>
        </nav>
      </div>
    </transition>
  </header>
</template>

<script setup lang="ts">
  import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
  import { RouterLink, useRoute } from 'vue-router'

  // --- IMPORTS DE IMAGENES ---
  import logoWhite from '@/assets/images/logo-covenant-white.svg'
  import logoColor from '@/assets/images/logo-covenant-footer.svg'

  const route = useRoute()
  const isScrolled = ref<boolean>(false)
  const isMobileOpen = ref<boolean>(false)

  // Detectar si estamos en la página blanca
  const isLightPage = computed(() => route.path === '/become-partner')
  // Detectar si estamos en el Home
  const isHomePage = computed(() => route.path === '/')

  // Logo cambia según la página
  const currentLogo = computed(() => {
    return isLightPage.value ? logoColor : logoWhite
  })

  const onScroll = (): void => {
    // Aunque ya no se fija, mantenemos esto por si quieres
    // que el fondo se oscurezca un poco mientras desaparece al hacer scroll
    isScrolled.value = window.scrollY > 40
  }

  const toggleMobile = (): void => {
    isMobileOpen.value = !isMobileOpen.value
  }

  const closeMobile = (): void => {
    isMobileOpen.value = false
  }

  onMounted(() => window.addEventListener('scroll', onScroll))
  onBeforeUnmount(() => window.removeEventListener('scroll', onScroll))
</script>

<style scoped>
/* ================== NAVBAR PRINCIPAL ================== */
.navbar {
  position: absolute; /* Se queda arriba, scrollea con la página */
  top: 0;
  left: 0;
  right: 0;
  z-index: 50;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 30px 4vw;
  color: #fff;
  background: linear-gradient(
    to bottom,
    rgba(0, 0, 0, 0.25),
    rgba(0, 0, 0, 0)
  );
  transition: background 0.3s ease, padding 0.3s ease, color 0.3s ease;
}

.navbar--scrolled {
  background: rgba(0, 0, 0, 0.85);
  padding-block: 10px;
  /* position: fixed;  <--- ELIMINADO: Así la barra no persigue al usuario */
}

.navbar__inner {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

/* ================== LOGO ================== */
.navbar__logo-link {
  display: flex;
  align-items: center;
  z-index: 60;
}

.navbar__logo-img {
  height: 40px;
  width: auto;
  transition: height 0.3s ease;
}

/* ================== TEMA CLARO (Become a Partner) ================== */
.navbar--light-theme {
  color: #04141f;
  background: transparent;
}

.navbar--light-theme.navbar--scrolled {
  background: rgba(255, 255, 255, 0.95);
  box-shadow: 0 4px 6px rgba(0,0,0,0.05);
}

.navbar--light-theme .navbar__toggle span {
  background: #04141f;
}

/* ================== MENU DESKTOP ================== */
.navbar__menu {
  display: flex;
  flex-wrap: nowrap; /* Cambio de wrap a nowrap para asegurar una sola línea */
  gap: clamp(1rem, 1.2vw, 2.5rem); /* Reduje un poco el gap mínimo */
}

.navbar__link {
  position: relative;
  padding: 0 4px 8px;
  text-transform: uppercase;
  font-size: clamp(0.65rem, 0.75rem + 0.1vw, 0.85rem); /* Ajuste leve para pantallas medianas */
  letter-spacing: 0.05em; /* Reduje ligeramente espacio para que quepa mejor */
  color: inherit;
  text-decoration: none;
  font-weight: 500;
  white-space: nowrap;
}

.navbar__link::after {
  content: "";
  position: absolute;
  left: 50%;
  bottom: 0;
  transform: translateX(-50%) scaleX(0);
  width: 100%;
  height: 2px;
  background: currentColor;
  transition: transform 0.25s ease;
}

.navbar__link:hover::after,
.navbar__link--active::after {
  transform: translateX(-50%) scaleX(1);
}

/* ================== BOTÓN HAMBURGUESA ================== */
.navbar__toggle {
  display: none;
  width: 32px;
  height: 24px;
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 0;
  z-index: 60;
}

.navbar__toggle span {
  display: block;
  height: 2px;
  width: 100%;
  background: #ffffff;
  border-radius: 999px;
  transition: all 0.25s ease;
}

.navbar__toggle span + span {
  margin-top: 6px;
}

/* Forzamos blanco cuando el menú está abierto */
.navbar__toggle-line--open {
  background: #ffffff !important;
}

.navbar__toggle-line--open:nth-child(1) {
  transform: translateY(8px) rotate(45deg);
}
.navbar__toggle-line--open:nth-child(2) {
  opacity: 0;
}
.navbar__toggle-line--open:nth-child(3) {
  transform: translateY(-8px) rotate(-45deg);
}

/* ================== MENU MOBILE ================== */
.navbar__mobile-overlay {
  position: fixed;
  inset: 0;
  z-index: 40;
  background: rgba(0, 0, 0, 0.55);
}

.navbar__mobile {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  width: 72%;
  max-width: 360px;
  background: linear-gradient(
    to left,
    rgba(4, 20, 31, 0.98) 0%,
    rgba(4, 20, 31, 0.92) 55%,
    rgba(4, 20, 31, 0.0) 100%
  );
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  padding: 90px 32px 40px 24px;
  box-sizing: border-box;
}

.navbar__mobile-link {
  position: relative;
  width: 100%;
  color: #ffffff !important;
  text-decoration: none;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  font-size: 0.9rem;
  padding: 14px 0;
  text-align: right;
}

.navbar__mobile-link::after {
  content: "";
  position: absolute;
  left: 185px;
  right: 0;
  bottom: 0;
  height: 2px;
  background-color: rgba(255, 255, 255, 0.5);
}

/* ================== TRANSICIÓN ================== */
.nav-slide-enter-from,
.nav-slide-leave-to {
  opacity: 0;
  transform: translateX(40px);
}

.nav-slide-enter-active,
.nav-slide-leave-active {
  transition: opacity 0.25s ease, transform 0.25s ease;
}

/* ================== UTILIDADES Y LAYOUT DESKTOP ================== */
@media (min-width: 991px) {
  .desktop-hidden {
    display: none !important;
  }

  /* Centrar todo el contenido en desktop */
  .navbar__inner {
    justify-content: center;
    gap: 50px; /* Separación entre Logo y Menú */
  }

  /* Reducir gap entre links (override del clamp base) */
  .navbar__menu {
    gap: 50px; 
  }
}

/* ================== RESPONSIVE ================== */



/* Ajustes específicos para Portátiles / Laptops de baja resolución (aprox 1024px - 1280px) */
@media (max-width: 1280px) and (min-width: 991px) {
  .navbar {
    padding-inline: 2vw; /* Reducir padding lateral para ganar espacio */
  }
  
  .navbar__logo-img {
    height: 32px; /* Logo un poco más pequeño */
  }

  .navbar__link {
    font-size: 0.7rem; /* Fuente más pequeña para que quepa todo */
    letter-spacing: 0.04em;
  }

  .navbar__menu {
    gap: clamp(1rem, 1.5vw, 2rem); /* Gap ajustado: ni muy pegado ni desbordado */
    margin-left: 20px; /* Separación explícita del logo por si acaso */
  }
}

/* Ajuste para llenar espacio a partir de 1281px (evita romper layout en 1024-1280px) */
@media (min-width: 1281px) {
  .navbar__menu {
    gap: 50px; 
  }
}

/* Cambiado de 1100px a 990px para mostrar menú desktop en laptops */
@media (max-width: 990px) {
  .navbar__menu {
    display: none;
  }
  .navbar__toggle {
    display: block;
  }
}

@media (max-width: 768px) {
  .navbar {
    padding-inline: 4vw;
  }
  .navbar__logo-img {
    height: 30px;
  }
}
</style>



