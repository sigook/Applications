<template>
  <header class="nav" :class="{ 'nav--solid': isScrolled }">
    <div class="nav__inner">
      <router-link
        to="/"
        class="nav__logo"
        :class="{ 'nav__logo--hidden': heroLogoVisible }"
      >
        <img src="@/assets/images/logo-white-v2.png" alt="Sigook" />
      </router-link>

      <nav class="nav__links" aria-label="Primary">
        <router-link
          v-for="link in navLinks"
          :key="link.label"
          :to="link.to"
          class="nav__link"
          exact-active-class="nav__link--active"
        >
          <span>{{ link.label }}</span>
        </router-link>
      </nav>

      <div class="nav__actions">
        <b-button
          native-type="button"
          class="nav__cta nav__cta--ghost"
          :loading="!authReady"
          :disabled="!authReady"
          @click="onSignIn"
        >
          {{ ctaLabel }}
        </b-button>
      </div>

      <button
        class="nav__hamburger"
        :class="{ 'nav__hamburger--open': mobileOpen }"
        @click="mobileOpen = !mobileOpen"
        aria-label="Toggle menu"
        :aria-expanded="mobileOpen"
      >
        <span /><span /><span />
      </button>
    </div>

    <transition name="nav-drawer">
      <div
        v-if="mobileOpen"
        class="nav__overlay"
        @click.self="mobileOpen = false"
      >
        <aside
          ref="drawerRef"
          class="nav__drawer"
          role="dialog"
          aria-modal="true"
          aria-label="Navigation menu"
        >
          <div class="nav__drawer-head">
            <span class="nav__drawer-title">Menu</span>
            <button
              class="nav__drawer-close"
              type="button"
              aria-label="Close menu"
              @click="mobileOpen = false"
            >
<LandingIcon name="close" />
            </button>
          </div>

          <nav class="nav__drawer-links" aria-label="Primary mobile">
            <router-link
              v-for="link in navLinks"
              :key="link.label"
              :to="link.to"
              class="nav__drawer-link"
              exact-active-class="nav__drawer-link--active"
              @click="mobileOpen = false"
            >{{ link.label }}</router-link>
          </nav>

          <div class="nav__drawer-actions">
            <b-button
              native-type="button"
              class="nav__cta nav__cta--ghost"
              :loading="!authReady"
              :disabled="!authReady"
              @click="onSignIn"
            >{{ ctaLabel }}</b-button>
          </div>
        </aside>
      </div>
    </transition>
  </header>
</template>

<script setup lang="ts">
import '@/assets/scss/tokens.scss';
import LandingIcon from '@/components/landing/shared/icons/LandingIcon.vue';
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useSecurityStore } from '@/stores/security';
import menu from '@/security/menu';
import { useFocusTrap } from '@/composables/useFocusTrap';
import { useBodyScrollLock } from '@/composables/useBodyScrollLock';

interface NavLink {
  label: string;
  to: string;
}

const router = useRouter();
const route = useRoute();
const securityStore = useSecurityStore();

const isScrolled = ref(false);
const mobileOpen = ref(false);
const drawerRef = ref<HTMLElement | null>(null);
const { lock, unlock } = useBodyScrollLock();
useFocusTrap(mobileOpen, drawerRef);

const navLinks: readonly NavLink[] = [
  { label: 'Open Positions', to: '/open-positions' },
  { label: 'Talents', to: '/talents' },
  { label: 'Employers', to: '/employers' },
  { label: 'Industries', to: '/industries' },
  { label: 'Special Projects', to: '/special-projects' },
  { label: 'About Us', to: '/about' },
];

const heroLogoVisible = ref(false);
let heroLogoObserver: IntersectionObserver | null = null;

function observeHeroLogo(): void {
  heroLogoObserver?.disconnect();
  heroLogoObserver = null;
  heroLogoVisible.value = false;
  nextTick(() => {
    const el = document.querySelector('.hero__logo');
    if (!el) return;
    heroLogoObserver = new IntersectionObserver(
      ([entry]) => { heroLogoVisible.value = entry.isIntersecting; },
      { rootMargin: '-80px 0px 0px 0px', threshold: 0 },
    );
    heroLogoObserver.observe(el);
  });
}

const ctaLabel = computed(() => (securityStore.user ? 'Go to Portal' : 'Sign In'));

const authReady = computed(() => securityStore.isReady);

async function onSignIn(): Promise<void> {
  mobileOpen.value = false;
  if (securityStore.user) {
    const homePageUrl = menu.getDefaultHomePageUrlBaseOnRoles(securityStore.userRoles);
    router.push(homePageUrl);
  } else {
    await securityStore.signIn();
  }
}

function onScroll() {
  isScrolled.value = window.scrollY > 80;
}

function onKeydown(e: KeyboardEvent): void {
  if (e.key === 'Escape') mobileOpen.value = false;
}

watch(mobileOpen, (open) => {
  if (open) {
    lock();
    document.addEventListener('keydown', onKeydown);
  } else {
    unlock();
    document.removeEventListener('keydown', onKeydown);
  }
});

onMounted(() => {
  window.addEventListener('scroll', onScroll, { passive: true });
  observeHeroLogo();
});

watch(() => route.fullPath, observeHeroLogo);

onUnmounted(() => {
  window.removeEventListener('scroll', onScroll);
  document.removeEventListener('keydown', onKeydown);
  heroLogoObserver?.disconnect();
});
</script>

<style scoped>
.nav {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 100;
  width: 100%;
  transition:
    background 0.35s ease,
    border-color 0.35s ease,
    box-shadow 0.35s ease;
}

.nav--solid {
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

.nav__inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 80px;
  padding: 0 80px;
  width: 100%;
  max-width: 1440px;
  margin: 0 auto;
  gap: 32px;
}

.nav__logo {
  flex-shrink: 0;
  display: block;
  line-height: 0;
  transition: opacity 0.45s ease;
}

.nav__logo--hidden {
  opacity: 0;
  pointer-events: none;
}

.nav__logo img {
  height: 44px;
  width: auto;
  max-width: 160px;
  object-fit: contain;
  transition: transform 0.25s ease;
}

.nav__logo:hover img {
  transform: scale(1.04);
}

@media (prefers-reduced-motion: reduce) {
  .nav__logo {
    transition: opacity 0.2s ease;
    clip-path: none;
  }
  .nav__logo--hidden {
    clip-path: none;
  }
}

.nav__links {
  display: flex;
  align-items: center;
  gap: 26px;
  flex-shrink: 0;
}

.nav__link {
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

.nav__link::after {
  content: '';
  position: absolute;
  left: 50%;
  bottom: 2px;
  width: 0;
  height: 2px;
  background: var(--c-brand-cyan);
  border-radius: 2px;
  transform: translateX(-50%);
  transition: width 0.3s var(--ease-brand);
}

.nav__link:hover {
  color: #fff;
}

.nav__link:hover::after {
  width: 70%;
}

.nav__link--active {
  color: #fff;
}

.nav__link--active::after {
  width: 100%;
}

.nav__actions {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}

.nav__cta {
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

.nav__cta--ghost {
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(10px) saturate(150%);
  -webkit-backdrop-filter: blur(10px) saturate(150%);
  border: 1.5px solid rgba(255, 255, 255, 0.55);
  color: #fff;
}

.nav__cta--ghost:hover {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-navy);
  transform: translateY(-1px);
}

.nav__cta--primary {
  background: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  border: 1.5px solid var(--c-brand-cyan);
  box-shadow: 0 6px 18px rgba(0, 173, 239, 0.30);
}

.nav__cta--primary:hover {
  background: var(--c-brand-cyan-strong);
  border-color: var(--c-brand-cyan-strong);
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(0, 173, 239, 0.40);
}

.nav__hamburger {
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

.nav__hamburger span {
  display: block;
  width: 26px;
  height: 2px;
  background-color: #fff;
  border-radius: 2px;
  transition: transform 0.25s ease, opacity 0.25s ease, background-color 0.25s ease;
}

.nav__hamburger--open span:nth-child(1) {
  transform: translateY(8px) rotate(45deg);
  background-color: var(--c-brand-cyan);
}

.nav__hamburger--open span:nth-child(2) {
  opacity: 0;
}

.nav__hamburger--open span:nth-child(3) {
  transform: translateY(-8px) rotate(-45deg);
  background-color: var(--c-brand-cyan);
}

.nav__overlay {
  position: fixed;
  top: 0;
  bottom: 0;
  left: 0;
  width: 100vw;
  z-index: 200;
  background: rgba(5, 25, 45, 0.55);
  -webkit-backdrop-filter: blur(2px);
  backdrop-filter: blur(2px);
  display: flex;
  justify-content: flex-end;
}

.nav__drawer {
  position: relative;
  display: flex;
  flex-direction: column;
  width: min(360px, 84vw);
  height: 100vh;
  height: 100dvh;
  padding: 18px 22px calc(28px + env(safe-area-inset-bottom));
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.98) 0%,
    rgba(9, 48, 85, 0.96) 100%
  );
  -webkit-backdrop-filter: blur(24px) saturate(160%);
  backdrop-filter: blur(24px) saturate(160%);
  border-left: 1px solid rgba(255, 255, 255, 0.12);
  box-shadow: -24px 0 48px rgba(0, 0, 0, 0.40);
  overflow-y: auto;
}

.nav__drawer-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 10px;
}

.nav__drawer-title {
  font-family: var(--font-family);
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
}

.nav__drawer-close {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  margin-right: -6px;
  border: none;
  border-radius: 50%;
  background: var(--c-glass-fill);
  color: #fff;
  cursor: pointer;
  transition: background 0.2s ease, transform 0.25s ease;
}

.nav__drawer-close:hover {
  background: rgba(255, 255, 255, 0.14);
  transform: rotate(90deg);
}

.nav__drawer-close svg {
  width: 20px;
  height: 20px;
}

.nav__drawer-links {
  display: flex;
  flex-direction: column;
  margin-top: 4px;
}

.nav__drawer-link {
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

.nav__drawer-link::before {
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

.nav__drawer-link:hover,
.nav__drawer-link--active {
  color: #fff;
  padding-left: 22px;
}

.nav__drawer-link:hover::before,
.nav__drawer-link--active::before {
  width: 3px;
}

.nav__drawer-actions {
  display: flex;
  gap: 12px;
  margin-top: auto;
  padding-top: 24px;
}

.nav__drawer-actions .nav__cta {
  flex: 1;
}

.nav-drawer-enter-active,
.nav-drawer-leave-active {
  transition: opacity 0.3s ease;
}
.nav-drawer-enter-active .nav__drawer,
.nav-drawer-leave-active .nav__drawer {
  transition: transform 0.35s var(--ease-brand);
}
.nav-drawer-enter-from,
.nav-drawer-leave-to {
  opacity: 0;
}
.nav-drawer-enter-from .nav__drawer,
.nav-drawer-leave-to .nav__drawer {
  transform: translateX(100%);
}

@media (prefers-reduced-motion: reduce) {
  .nav-drawer-enter-active .nav__drawer,
  .nav-drawer-leave-active .nav__drawer {
    transition: none;
  }
}

@media (max-width: 1199px) {
  .nav__inner {
    height: 64px;
    padding: 0 24px;
    gap: 16px;
  }

  .nav__logo img {
    height: 34px;
    width: auto;
    max-width: 120px;
  }

  .nav__links,
  .nav__actions {
    display: none !important;
  }

  .nav__hamburger {
    display: flex;
    margin-left: auto;
  }
}

@media (max-width: 1023px) {
  .nav--solid {
    background: linear-gradient(
      180deg,
      rgba(9, 48, 85, 0.94) 0%,
      rgba(9, 48, 85, 0.80) 100%
    );
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    box-shadow: 0 12px 24px rgba(0, 0, 0, 0.25);
  }

  .nav__cta--ghost {
    background: var(--c-glass-fill-active);
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
  }

  .nav__overlay {
    -webkit-backdrop-filter: none;
    backdrop-filter: none;
  }

  .nav__drawer {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    box-shadow: -24px 0 24px rgba(0, 0, 0, 0.40);
  }
}
</style>
