<template>
  <footer class="footer-v2">
    <!-- Cyan accent line at the top — closes the page visually -->
    <div class="footer-v2__accent" aria-hidden="true"></div>

    <div class="footer-v2__inner">
      <!-- Main row: brand column + 3 link columns -->
      <div class="footer-v2__main">
        <!-- Brand column -->
        <div class="footer-v2__brand">
          <img
            src="@/assets/images/v2/footer/footer-logo.png"
            alt="Sigook Work Factory"
            class="footer-v2__logo"
          />
          <p class="footer-v2__tagline">
            Staffing and workforce solutions for growing businesses across Canada and the US.
          </p>
        </div>

        <!-- Nav columns -->
        <div class="footer-v2__cols">
          <nav class="footer-v2__col" aria-label="Company">
            <span class="footer-v2__col-eyebrow">Company</span>
            <router-link to="/v2/about"             class="footer-v2__link">About Us</router-link>
            <router-link to="/v2/open-positions"    class="footer-v2__link">Open Positions</router-link>
            <router-link to="/v2/industries"        class="footer-v2__link">Industries</router-link>
            <router-link to="/v2/special-projects"  class="footer-v2__link">Special Projects</router-link>
            <router-link to="/v2/partner"           class="footer-v2__link">Become a Partner</router-link>
            <a href="#sp-contact"                   class="footer-v2__link">Contact</a>
          </nav>

          <nav class="footer-v2__col" aria-label="Services">
            <span class="footer-v2__col-eyebrow">Services</span>
            <router-link to="/v2/employers" class="footer-v2__link">Temporary Staffing</router-link>
            <router-link to="/v2/payroll"   class="footer-v2__link">Payroll Services</router-link>
            <router-link to="/v2/talents"   class="footer-v2__link">Workforce Solutions</router-link>
            <router-link to="/v2/about"     class="footer-v2__link">HR Consulting</router-link>
            <router-link to="/v2/certified" class="footer-v2__link">Compliance</router-link>
          </nav>

          <div class="footer-v2__col">
            <span class="footer-v2__col-eyebrow">Contact</span>
            <a href="mailto:info@sigook.com" class="footer-v2__link">info@sigook.com</a>
            <a href="tel:+16475550123"       class="footer-v2__link">+1 (647) 555-0123</a>
            <span class="footer-v2__link footer-v2__link--plain">Toronto, ON, Canada</span>
          </div>
        </div>
      </div>

      <div class="footer-v2__divider" aria-hidden="true"></div>

      <div class="footer-v2__bottom">
        <p class="footer-v2__copyright">
          &copy; {{ currentYear }} Sigook Work Factory Inc. All rights reserved.
        </p>
        <div class="footer-v2__legal">
          <router-link to="/v2/privacy-policy" class="footer-v2__legal-link">Privacy Policy</router-link>
          <router-link to="/v2/terms"          class="footer-v2__legal-link">Terms of Use</router-link>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
const currentYear = new Date().getFullYear()
</script>

<style scoped>
/* ── Shell — sits above GlobalBackground (z:1) with subtle dark glass overlay ── */
.footer-v2 {
  position: relative;
  z-index: 1;
  /* Subtle dark glass overlay on top of the global-bg gradient — keeps the
     atmospheric circles visible behind while improving text contrast. */
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.35) 0%,
    rgba(9, 48, 85, 0.65) 100%
  );
  backdrop-filter: blur(8px) saturate(140%);
  -webkit-backdrop-filter: blur(8px) saturate(140%);
  color: #fff;
  font-family: var(--font-family);
}

/* Cyan accent line at the top — gradient fade from edges to center */
.footer-v2__accent {
  height: 1px;
  background: linear-gradient(
    90deg,
    transparent 0%,
    rgba(0, 173, 239, 0.50) 20%,
    rgba(0, 173, 239, 0.80) 50%,
    rgba(0, 173, 239, 0.50) 80%,
    transparent 100%
  );
}

/* ── Inner container ─────────────────────────────────────────────────────── */
.footer-v2__inner {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 72px var(--gutter-desktop) 36px;
  display: flex;
  flex-direction: column;
  gap: 48px;
}

/* ── Main row ────────────────────────────────────────────────────────────── */
.footer-v2__main {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 64px;
}

.footer-v2__cols {
  display: contents;
}

/* ── Brand column ────────────────────────────────────────────────────────── */
.footer-v2__brand {
  display: flex;
  flex-direction: column;
  gap: 18px;
  flex-shrink: 0;
  max-width: 260px;
}

.footer-v2__logo {
  width: 180px;
  height: 72px;
  object-fit: contain;
  object-position: left center;
  /* Force white tint so the brand mark reads on dark bg regardless of source PNG */
  filter: brightness(0) invert(1);
}

.footer-v2__tagline {
  font-family: var(--font-family);
  font-size: 14px;
  font-weight: 400;
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.70);
  margin: 0;
}

/* ── Link columns ────────────────────────────────────────────────────────── */
.footer-v2__col {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.footer-v2__col-eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 6px;
}

.footer-v2__link {
  font-family: var(--font-family);
  font-size: 14px;
  font-weight: 400;
  line-height: 1.4;
  color: rgba(255, 255, 255, 0.72);
  text-decoration: none;
  transition: color 0.25s ease, transform 0.25s ease;
  width: fit-content;
}

.footer-v2__link:hover {
  color: var(--c-brand-cyan);
  transform: translateX(2px);
}

.footer-v2__link--plain,
.footer-v2__link--plain:hover {
  cursor: default;
  color: rgba(255, 255, 255, 0.72);
  transform: none;
}

/* ── Divider ─────────────────────────────────────────────────────────────── */
.footer-v2__divider {
  height: 1px;
  background: rgba(255, 255, 255, 0.14);
  width: 100%;
}

/* ── Bottom row ──────────────────────────────────────────────────────────── */
.footer-v2__bottom {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-family: var(--font-family);
  font-size: 12px;
  font-weight: 400;
  color: rgba(255, 255, 255, 0.55);
}

.footer-v2__copyright {
  margin: 0;
}

.footer-v2__legal {
  display: flex;
  gap: 28px;
}

.footer-v2__legal-link {
  color: rgba(255, 255, 255, 0.55);
  text-decoration: none;
  transition: color 0.25s ease;
}

.footer-v2__legal-link:hover {
  color: var(--c-brand-cyan);
}

/* ── Mobile ──────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .footer-v2__inner {
    padding: 56px 28px 36px;
    gap: 36px;
  }

  .footer-v2__main {
    flex-direction: column;
    gap: 36px;
  }

  .footer-v2__brand {
    max-width: 100%;
  }

  .footer-v2__logo {
    width: 150px;
    height: 60px;
  }

  /* 2 → 3 cols depending on viewport */
  .footer-v2__cols {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
    gap: 28px 16px;
    width: 100%;
  }

  .footer-v2__bottom {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
}
</style>
