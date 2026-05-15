<template>
  <header class="nav-v2" :class="{ 'nav-v2--solid': isScrolled }">
    <div class="nav-v2__inner">
      <!-- Logo -->
      <router-link to="/v2/home" class="nav-v2__logo">
        <img src="@/assets/images/logo-white-v2.png" alt="Sigook" />
      </router-link>

      <!-- Desktop nav links -->
      <nav class="nav-v2__links">
        <router-link
          v-for="link in navLinks"
          :key="link.label"
          :to="link.to"
          class="nav-v2__link"
          exact-active-class="nav-v2__link--active"
        >{{ link.label }}</router-link>
      </nav>

      <!-- Desktop buttons -->
      <div class="nav-v2__actions">
        <router-link to="/v2/sign-up" class="btn btn--cyan btn--sm">Sign Up</router-link>
        <router-link to="/v2/sign-in" class="btn btn--secondary btn--sm nav-v2__btn-signin">Sign In</router-link>
      </div>

      <!-- Mobile hamburger -->
      <button
        class="nav-v2__hamburger"
        :class="{ 'nav-v2__hamburger--open': mobileOpen }"
        @click="mobileOpen = !mobileOpen"
        aria-label="Toggle menu"
      >
        <span /><span /><span />
      </button>
    </div>

    <!-- Mobile drawer -->
    <transition name="nav-drawer">
      <div v-if="mobileOpen" class="nav-v2__drawer">
        <router-link
          v-for="link in navLinks"
          :key="link.label"
          :to="link.to"
          class="nav-v2__drawer-link"
          @click="mobileOpen = false"
        >{{ link.label }}</router-link>
        <div class="nav-v2__drawer-actions">
          <router-link to="/v2/sign-up" class="btn btn--cyan btn--sm" @click="mobileOpen = false">Sign Up</router-link>
          <router-link to="/v2/sign-in" class="btn btn--secondary btn--sm" @click="mobileOpen = false">Sign In</router-link>
        </div>
      </div>
    </transition>
  </header>
</template>

<script setup lang="ts">
import '@/assets/css/tailwind.css';
import { ref, onMounted, onUnmounted } from 'vue';

const isScrolled = ref(false);
const mobileOpen = ref(false);

const navLinks = [
  { label: 'Open Positions', to: '/v2/open-positions' },
  { label: 'Industries',     to: '/v2/industries' },
  { label: 'News',           to: '/v2/news' },
  { label: 'About Us',       to: '/v2/about' },
  { label: 'Employers',      to: '/v2/employers' },
  { label: 'Talents',        to: '/v2/talents' },
];

function onScroll() {
  isScrolled.value = window.scrollY > 80;
}

onMounted(() => {
  window.addEventListener('scroll', onScroll, { passive: true });
});

onUnmounted(() => {
  window.removeEventListener('scroll', onScroll);
});
</script>

<style scoped>
/* ── Base layout ── */
.nav-v2 {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 100;
  width: 100%;
  transition: background-color 0.3s ease, box-shadow 0.3s ease;
}

.nav-v2--solid {
  background-color: var(--c-brand-blue);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.06);
}

/* ── Inner container ── */
.nav-v2__inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 80px;
  padding: 0 80px;
  max-width: 1440px;
  margin: 0 auto;
}

/* ── Logo ── */
.nav-v2__logo {
  flex-shrink: 0;
  display: block;
  line-height: 0;
}

.nav-v2__logo img {
  height: 44px;
  width: auto;
  max-width: 160px;
  object-fit: contain;
}

/* Sign In button: white border visible on both transparent and solid navbar */
.nav-v2__btn-signin {
  border-color: rgba(255, 255, 255, 0.55) !important;
}

/* ── Desktop nav links ── */
.nav-v2__links {
  display: flex;
  align-items: center;
  gap: 28px;
  flex-shrink: 0;
}

.nav-v2__link {
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 500;
  letter-spacing: 0.78px;
  white-space: nowrap;
  text-decoration: none;
  color: rgba(255, 255, 255, 0.5);
  transition: color 0.2s ease;
  cursor: pointer;
}

.nav-v2--solid .nav-v2__link {
  color: rgba(255, 255, 255, 0.75);
}

.nav-v2__link--active,
.nav-v2__link:hover {
  color: #fff;
}

/* ── Desktop buttons ── */
.nav-v2__actions {
  display: flex;
  align-items: center;
  gap: 20px;
  flex-shrink: 0;
}

/* ── Hamburger (mobile) ── */
.nav-v2__hamburger {
  display: none;
  flex-direction: column;
  justify-content: space-between;
  width: 24px;
  height: 18px;
  background: transparent;
  border: none;
  padding: 0;
  cursor: pointer;
  flex-shrink: 0;
}

.nav-v2__hamburger span {
  display: block;
  width: 24px;
  height: 2px;
  background-color: #fff;
  border-radius: 1px;
  transition: transform 0.25s ease, opacity 0.25s ease;
}

.nav-v2__hamburger--open span:nth-child(1) {
  transform: translateY(8px) rotate(45deg);
}
.nav-v2__hamburger--open span:nth-child(2) {
  opacity: 0;
}
.nav-v2__hamburger--open span:nth-child(3) {
  transform: translateY(-8px) rotate(-45deg);
}

/* ── Mobile drawer ── */
.nav-v2__drawer {
  display: flex;
  flex-direction: column;
  gap: 0;
  background-color: var(--c-brand-navy);
  padding: 16px 24px 24px;
}

.nav-v2__drawer-link {
  font-family: var(--font-family);
  font-size: 15px;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.85);
  text-decoration: none;
  padding: 14px 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
  display: block;
  transition: color 0.2s ease;
}

.nav-v2__drawer-link:hover {
  color: #fff;
}

.nav-v2__drawer-actions {
  display: flex;
  gap: 16px;
  margin-top: 20px;
}

/* ── Drawer transition ── */
.nav-drawer-enter-active,
.nav-drawer-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}
.nav-drawer-enter-from,
.nav-drawer-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}

/* ── Responsive ── */
@media (max-width: 1023px) {
  .nav-v2__inner {
    height: 64px;
    padding: 0 24px;
  }

  .nav-v2__logo img {
    height: 34px;
    width: auto;
    max-width: 120px;
  }

  .nav-v2__links,
  .nav-v2__actions {
    display: none;
  }

  .nav-v2__hamburger {
    display: flex;
  }
}
</style>
