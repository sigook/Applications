<template>
  <header class="navbar" :class="{ 'navbar--scrolled': isScrolled }">
    <div class="navbar__inner">
      <!-- MENU DESKTOP -->
      <nav class="navbar__menu">
        <RouterLink to="/" class="navbar__link" active-class="navbar__link--active">HOME</RouterLink>
        <RouterLink to="/open-positions" class="navbar__link" active-class="navbar__link--active">
          OPEN POSITIONS
        </RouterLink>
        <RouterLink to="/industries" class="navbar__link" active-class="navbar__link--active">
          INDUSTRIES
        </RouterLink>
        <RouterLink to="/about" class="navbar__link" active-class="navbar__link--active">
          ABOUT US
        </RouterLink>
        <RouterLink to="/employers" class="navbar__link" active-class="navbar__link--active">EMPLOYERS</RouterLink>
        <RouterLink to="/talents" class="navbar__link" active-class="navbar__link--active">TALENTS</RouterLink>
        <RouterLink to="/become-partner" class="navbar__link" active-class="navbar__link--active">
          BECOME A PARTNER
        </RouterLink>
        <RouterLink to="/licensed-certified" class="navbar__link" active-class="navbar__link--active">
          LICENSED &amp; CERTIFIED
        </RouterLink>
      </nav>

      <!-- BOTÓN HAMBURGUESA (solo mobile) -->
      <button class="navbar__toggle" @click="toggleMobile" aria-label="Open menu">
        <span :class="{ 'navbar__toggle-line--open': isMobileOpen }"></span>
        <span :class="{ 'navbar__toggle-line--open': isMobileOpen }"></span>
        <span :class="{ 'navbar__toggle-line--open': isMobileOpen }"></span>
      </button>
    </div>

    <!-- MENU MOBILE -->
    <transition name="nav-slide">
      <!-- ⬅ CAMBIO: overlay a pantalla completa -->
      <div
        v-if="isMobileOpen"
        class="navbar__mobile-overlay"
        @click.self="closeMobile"
      >
        <nav class="navbar__mobile">
          <RouterLink @click="closeMobile" to="/" class="navbar__mobile-link">Home</RouterLink>
          <RouterLink @click="closeMobile" to="/open-positions" class="navbar__mobile-link">
            Open Positions
          </RouterLink>
          <RouterLink @click="closeMobile" to="/industries" class="navbar__mobile-link">
            Industries
          </RouterLink>
          <RouterLink @click="closeMobile" to="/about" class="navbar__mobile-link">
            About Us
          </RouterLink>
          <RouterLink @click="closeMobile" to="/employers" class="navbar__mobile-link">
            Employers
          </RouterLink>
          <RouterLink @click="closeMobile" to="/talents" class="navbar__mobile-link">
            Talents
          </RouterLink>
          <RouterLink @click="closeMobile" to="/become-partner" class="navbar__mobile-link">
            Become a Partner
          </RouterLink>
          <RouterLink @click="closeMobile" to="/licensed-certified" class="navbar__mobile-link">
            Licensed &amp; Certified
          </RouterLink>
        </nav>
      </div>
    </transition>
  </header>
</template>

<script setup lang="ts">
  import { ref, onMounted, onBeforeUnmount } from 'vue'
  import { RouterLink } from 'vue-router'

  const isScrolled = ref<boolean>(false)
  const isMobileOpen = ref<boolean>(false)

  const onScroll = (): void => {
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
.navbar {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  z-index: 50;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 36px 4vw;
  color: #fff;
  background: linear-gradient(
    to bottom,
    rgba(0, 0, 0, 0.55),
    rgba(0, 0, 0, 0)
  );
  transition: background 0.3s ease, padding 0.3s ease;
}

/* cuando se hace scroll puedes dejarlo más compacto */
.navbar--scrolled {
  background: rgba(0, 0, 0, 0.5);
  padding-block: 10px;
}

.navbar__inner {
  width: 100%;
  max-width: 1280px;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* ================== MENU DESKTOP ================== */

.navbar__menu {
  display: flex;
  flex-wrap: wrap; /* evita desbordes en resoluciones menores */
  justify-content: center;
  gap: clamp(4rem, 1.6vw, 5rem); /* gap se adapta al ancho */
}

.navbar__link {
  position: relative;
  padding: 0 4px 12px;
  text-transform: uppercase;
  font-size: clamp(0.7rem, 0.8rem + 0.1vw, 0.85rem); /* ajusta tamaño según pantalla */
  letter-spacing: 0.08em;
  color: #ffffff;
  text-decoration: none;
  white-space: nowrap; /* evita cortes de palabras raros */
}

.navbar__link::after {
  content: "";
  position: absolute;
  left: 50%;
  bottom: 0;
  transform: translateX(-50%) scaleX(0);
  transform-origin: center;
  width: 110%;
  height: 2px;
  background: #ffffff;
  opacity: 0;
  transition:
    transform 0.25s ease,
    opacity 0.25s ease;
}

.navbar__link:hover::after,
.navbar__link--active::after {
  transform: translateX(-50%) scaleX(1);
  opacity: 1;
}

/* ================== BOTÓN HAMBURGUESA ================== */

.navbar__toggle {
  display: none; /* se muestra solo en mobile */
  margin-left: auto;
  width: 32px;
  height: 24px;
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 0;
}

.navbar__toggle span {
  display: block;
  height: 2px;
  width: 100%;
  background: #ffffff;
  border-radius: 999px;
  transition: transform 0.25s ease, opacity 0.25s ease;
}

.navbar__toggle span + span {
  margin-top: 6px;
}

/* opcional: animación simple cuando el menú está abierto */
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
.navbar__mobile {
  position: absolute;          /* ⬅ importante: ahora relativo al overlay */
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
  color: #ffffff;
  text-decoration: none;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  font-size: 0.9rem;
  padding: 14px 0;
  text-align: right;
}

/* ⬅ CAMBIO: línea exactamente debajo del link, ocupando el ancho del panel
   y con más grosor */
.navbar__mobile-link::after {
  content: "";
  position: absolute;
  left: 185px;
  right: 0;                                /* ya no “cortamos” a la mitad */
  bottom: 0;
  height: 2px;                             /* más gruesa */
  background-color: rgba(255, 255, 255, 0.5);
}

/* botón inferior */
.navbar__mobile-cta {
  /* ⬅ CAMBIO: botón ocupa ancho del panel y queda bien abajo */
  align-self: stretch;
  margin-top: auto;
  padding: 14px 18px;
  border-radius: 999px;
  border: none;
  background: #34d06b;
  color: #04141f;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  cursor: pointer;
}

/* ================== TRANSICIÓN DEL PANEL MOBILE ================== */

.nav-slide-enter-from,
.nav-slide-leave-to {
  opacity: 0;
  transform: translateX(40px);
}

.nav-slide-enter-active,
.nav-slide-leave-active {
  transition: opacity 0.25s ease, transform 0.25s ease;
}

/* ⬅ CAMBIO: overlay que ocupa toda la pantalla y detecta el click fuera del panel */
.navbar__mobile-overlay {
  position: fixed;
  inset: 0;
  z-index: 40;
  background: rgba(0, 0, 0, 0.55); /* opcional */
}
/* ================== RESPONSIVE ================== */

/* En pantallas medianas ajustamos un poco padding y gap */
@media (max-width: 1100px) {
  .navbar {
    padding-inline: 3vw;
  }

  .navbar__menu {
    gap: clamp(0.4rem, 1.1vw, 1rem);
  }
}

/* En tablets y mobile: ocultamos el menú desktop y mostramos hamburguesa */
@media (max-width: 768px) {
  .navbar {
    padding-inline: 4vw;
  }

  .navbar__inner {
    justify-content: flex-end;
  }

  .navbar__menu {
    display: none;
  }

  .navbar__toggle {
    display: block;
  }
}
</style>



