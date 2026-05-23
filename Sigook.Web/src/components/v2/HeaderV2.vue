<template>
  <header class="nav-v2" :class="{ 'nav-v2--solid': isScrolled }">
    <div class="nav-v2__inner">
      <!-- Logo -->
      <router-link to="/v2/home" class="nav-v2__logo">
        <img src="@/assets/images/logo-white-v2.png" alt="Sigook" />
      </router-link>

      <!-- Desktop nav links -->
      <nav class="nav-v2__links" aria-label="Primary">
        <router-link
          v-for="link in navLinks"
          :key="link.label"
          :to="link.to"
          class="nav-v2__link"
          exact-active-class="nav-v2__link--active"
        >
          <span>{{ link.label }}</span>
        </router-link>
      </nav>

      <!-- Desktop actions -->
      <div class="nav-v2__actions">
        <router-link to="/v2/sign-in" class="nav-v2__cta nav-v2__cta--ghost">
          Sign In
        </router-link>
      </div>

      <!-- Mobile hamburger -->
      <button
        class="nav-v2__hamburger"
        :class="{ 'nav-v2__hamburger--open': mobileOpen }"
        @click="mobileOpen = !mobileOpen"
        aria-label="Toggle menu"
        :aria-expanded="mobileOpen"
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
          exact-active-class="nav-v2__drawer-link--active"
          @click="mobileOpen = false"
        >{{ link.label }}</router-link>
        <div class="nav-v2__drawer-actions">
          <router-link
            to="/v2/sign-in"
            class="nav-v2__cta nav-v2__cta--ghost"
            @click="mobileOpen = false"
          >Sign In</router-link>
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
  { label: 'Open Positions',   to: '/v2/open-positions' },
  { label: 'Industries',       to: '/v2/industries' },
  { label: 'News',             to: '/v2/news' },
  { label: 'About Us',         to: '/v2/about' },
  { label: 'Employers',        to: '/v2/employers' },
  { label: 'Talents',          to: '/v2/talents' },
  { label: 'Special Projects', to: '/v2/special-projects' },
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
/* ── Base layout ─────────────────────────────────────────────────────────── */
.nav-v2 {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 100;
  width: 100%;
  transition:
    background 0.35s ease,
    backdrop-filter 0.35s ease,
    border-color 0.35s ease,
    box-shadow 0.35s ease;
}

/* Scrolled state: glass overlay matching DualCta / Footer vocabulary */
.nav-v2--solid {
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.78) 0%,
    rgba(9, 48, 85, 0.62) 100%
  );
  backdrop-filter: blur(16px) saturate(160%);
  -webkit-backdrop-filter: blur(16px) saturate(160%);
  border-bottom: 1px solid rgba(255, 255, 255, 0.10);
  box-shadow: 0 12px 32px rgba(0, 0, 0, 0.25);
}

/* ── Inner container ─────────────────────────────────────────────────────── */
.nav-v2__inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 80px;
  padding: 0 80px;
  max-width: 1440px;
  margin: 0 auto;
  gap: 32px;
}

/* ── Logo ────────────────────────────────────────────────────────────────── */
.nav-v2__logo {
  flex-shrink: 0;
  display: block;
  line-height: 0;
  transition: transform 0.25s ease;
}

.nav-v2__logo:hover {
  transform: scale(1.04);
}

.nav-v2__logo img {
  height: 44px;
  width: auto;
  max-width: 160px;
  object-fit: contain;
}

/* ── Desktop nav links — cyan underline indicator ──────────────────────── */
.nav-v2__links {
  display: flex;
  align-items: center;
  gap: 26px;
  flex-shrink: 0;
}

.nav-v2__link {
  position: relative;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 500;
  letter-spacing: 0.06em;
  white-space: nowrap;
  text-decoration: none;
  color: rgba(255, 255, 255, 0.65);
  padding: 8px 2px;
  transition: color 0.25s ease;
  cursor: pointer;
}

/* Underline indicator pseudo */
.nav-v2__link::after {
  content: '';
  position: absolute;
  left: 50%;
  bottom: 2px;
  width: 0;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
  transform: translateX(-50%);
  transition: width 0.3s cubic-bezier(0.22, 1, 0.36, 1);
}

.nav-v2__link:hover {
  color: #fff;
}

.nav-v2__link:hover::after {
  width: 70%;
}

.nav-v2__link--active {
  color: #fff;
}

.nav-v2__link--active::after {
  width: 100%;
}

/* ── Desktop actions ─────────────────────────────────────────────────────── */
.nav-v2__actions {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}

/* CTAs — glass ghost + solid cyan primary */
.nav-v2__cta {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  height: 38px;
  padding: 0 22px;
  border-radius: 999px;
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.04em;
  text-decoration: none;
  cursor: pointer;
  transition:
    background 0.25s ease,
    color 0.25s ease,
    border-color 0.25s ease,
    transform 0.25s ease;
}

/* Sign In — glass ghost pill (matches Hero CTA language) */
.nav-v2__cta--ghost {
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(10px) saturate(150%);
  -webkit-backdrop-filter: blur(10px) saturate(150%);
  border: 1.5px solid rgba(255, 255, 255, 0.55);
  color: #fff;
}

.nav-v2__cta--ghost:hover {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-navy);
  transform: translateY(-1px);
}

/* Sign Up — solid cyan (primary action) */
.nav-v2__cta--primary {
  background: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  border: 1.5px solid var(--c-brand-cyan);
  box-shadow: 0 6px 18px rgba(0, 173, 239, 0.30);
}

.nav-v2__cta--primary:hover {
  background: #0098d6;
  border-color: #0098d6;
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(0, 173, 239, 0.40);
}

/* ── Hamburger (mobile) ──────────────────────────────────────────────────── */
.nav-v2__hamburger {
  display: none;
  flex-direction: column;
  justify-content: space-between;
  width: 26px;
  height: 18px;
  background: transparent;
  border: none;
  padding: 0;
  cursor: pointer;
  flex-shrink: 0;
}

.nav-v2__hamburger span {
  display: block;
  width: 26px;
  height: 2px;
  background-color: #fff;
  border-radius: 2px;
  transition: transform 0.25s ease, opacity 0.25s ease, background-color 0.25s ease;
}

.nav-v2__hamburger--open span:nth-child(1) {
  transform: translateY(8px) rotate(45deg);
  background-color: var(--c-brand-cyan);
}

.nav-v2__hamburger--open span:nth-child(2) {
  opacity: 0;
}

.nav-v2__hamburger--open span:nth-child(3) {
  transform: translateY(-8px) rotate(-45deg);
  background-color: var(--c-brand-cyan);
}

/* ── Mobile drawer — glass overlay (matches navbar scrolled state) ──────── */
.nav-v2__drawer {
  display: flex;
  flex-direction: column;
  gap: 0;
  background: rgba(9, 48, 85, 0.92);
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  border-bottom: 1px solid rgba(255, 255, 255, 0.10);
  padding: 8px 24px 24px;
}

.nav-v2__drawer-link {
  position: relative;
  font-family: var(--font-family);
  font-size: 15px;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.78);
  text-decoration: none;
  padding: 16px 0 16px 16px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
  display: block;
  transition: color 0.2s ease, padding-left 0.25s ease;
}

.nav-v2__drawer-link::before {
  content: '';
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 0;
  height: 18px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
  transition: width 0.25s ease;
}

.nav-v2__drawer-link:hover,
.nav-v2__drawer-link--active {
  color: #fff;
  padding-left: 22px;
}

.nav-v2__drawer-link:hover::before,
.nav-v2__drawer-link--active::before {
  width: 3px;
}

.nav-v2__drawer-actions {
  display: flex;
  gap: 12px;
  margin-top: 22px;
}

.nav-v2__drawer-actions .nav-v2__cta {
  flex: 1;
}

/* ── Drawer transition ──────────────────────────────────────────────────── */
.nav-drawer-enter-active,
.nav-drawer-leave-active {
  transition: opacity 0.25s ease, transform 0.25s ease;
}
.nav-drawer-enter-from,
.nav-drawer-leave-to {
  opacity: 0;
  transform: translateY(-12px);
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .nav-v2__inner {
    height: 64px;
    padding: 0 24px;
    gap: 16px;
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
