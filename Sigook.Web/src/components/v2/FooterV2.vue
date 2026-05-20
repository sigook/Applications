<template>
  <footer class="footer-v2">
    <div class="footer-v2__inner">
      <!-- Main row: brand column + 3 link columns -->
      <div class="footer-v2__main">
        <!-- Brand column -->
        <div class="footer-v2__brand">
          <img
            src="@/assets/images/v2/footer-logo.png"
            alt="Sigook Work Factory"
            class="footer-v2__logo"
          />
          <p class="footer-v2__tagline">
            Staffing and workforce solutions for growing businesses across Canada and the US.
          </p>
        </div>

        <!-- Nav columns: auto-fit grid → 2 cols on small mobile, 3 on wider -->
        <div class="footer-v2__cols">
          <nav class="footer-v2__col" aria-label="Company">
            <p class="footer-v2__col-heading">Company</p>
            <router-link to="/v2/about"            class="footer-v2__link">About Us</router-link>
            <router-link to="/v2/open-positions"  class="footer-v2__link">Open Positions</router-link>
            <router-link to="/v2/industries"      class="footer-v2__link">Industries</router-link>
            <router-link to="/v2/special-projects" class="footer-v2__link">Special Projects</router-link>
            <router-link to="/v2/partner"         class="footer-v2__link">Become a Partner</router-link>
            <a href="#sp-contact"                 class="footer-v2__link">Contact</a>
          </nav>

          <nav class="footer-v2__col" aria-label="Services">
            <p class="footer-v2__col-heading">Services</p>
            <router-link to="/v2/employers" class="footer-v2__link">Temporary Staffing</router-link>
            <router-link to="/v2/payroll"   class="footer-v2__link">Payroll Services</router-link>
            <router-link to="/v2/talents"   class="footer-v2__link">Workforce Solutions</router-link>
            <router-link to="/v2/about"     class="footer-v2__link">HR Consulting</router-link>
            <router-link to="/v2/certified" class="footer-v2__link">Compliance</router-link>
          </nav>

          <div class="footer-v2__col">
            <p class="footer-v2__col-heading">Contact</p>
            <a href="mailto:info@sigook.com" class="footer-v2__link">info@sigook.com</a>
            <a href="tel:+16475550123"       class="footer-v2__link">+1 (647) 555-0123</a>
            <span class="footer-v2__link footer-v2__link--plain">Toronto, ON, Canada</span>
          </div>
        </div>
      </div>

      <!-- Divider -->
      <div class="footer-v2__divider" aria-hidden="true"></div>

      <!-- Bottom row: copyright + legal links -->
      <div class="footer-v2__bottom">
        <p class="footer-v2__copyright">
          &copy; {{ currentYear }} Sigook Work Factory Inc. All rights reserved.
        </p>
        <div class="footer-v2__legal">
          <router-link to="/v2/privacy-policy" class="footer-v2__legal-link">Privacy Policy</router-link>
          <router-link to="/v2/terms"           class="footer-v2__legal-link">Terms of Use</router-link>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
const currentYear = new Date().getFullYear()
</script>

<style scoped>
/* ── Shell ─────────────────────────────────────────────────────────────────── */
.footer-v2 {
  background: var(--c-bg);
}

/* ── Inner container ───────────────────────────────────────────────────────── */
.footer-v2__inner {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 64px var(--gutter-desktop) 40px;
  display: flex;
  flex-direction: column;
  gap: 48px;
}

/* ── Main row ──────────────────────────────────────────────────────────────── */
.footer-v2__main {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
}

/* Transparent on desktop — 3 cols become direct children of main flex row */
.footer-v2__cols {
  display: contents;
}

/* ── Brand column ──────────────────────────────────────────────────────────── */
.footer-v2__brand {
  display: flex;
  flex-direction: column;
  gap: 16px;
  flex-shrink: 0;
}

.footer-v2__logo {
  width: 180px;
  height: 72px;
  object-fit: contain;
  object-position: left center;
}

.footer-v2__tagline {
  font-size: 13px;
  font-weight: 300;
  line-height: normal;
  color: var(--c-ink-2);
  max-width: 220px;
  margin: 0;
}

/* ── Link columns ──────────────────────────────────────────────────────────── */
.footer-v2__col {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.footer-v2__col-heading {
  font-size: 13px;
  font-weight: 700;
  color: var(--c-ink);
  line-height: normal;
  margin: 0;
}

.footer-v2__link {
  font-size: 13px;
  font-weight: 400;
  color: var(--c-ink-2);
  line-height: normal;
  text-decoration: none;
  transition: color 0.2s;
}

.footer-v2__link:hover {
  color: var(--c-brand-blue);
}

.footer-v2__link--plain {
  cursor: default;
}

.footer-v2__link--plain:hover {
  color: var(--c-ink-2);
}

/* ── Divider ───────────────────────────────────────────────────────────────── */
.footer-v2__divider {
  height: 1px;
  background: var(--c-line);
  width: 100%;
}

/* ── Bottom row ────────────────────────────────────────────────────────────── */
.footer-v2__bottom {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 12px;
  color: var(--c-ink-2);
}

.footer-v2__copyright {
  margin: 0;
}

.footer-v2__legal {
  display: flex;
  gap: 24px;
}

.footer-v2__legal-link {
  color: var(--c-ink-2);
  text-decoration: none;
  transition: color 0.2s;
}

.footer-v2__legal-link:hover {
  color: var(--c-brand-blue);
}

/* ── Mobile ────────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .footer-v2__inner {
    padding: 48px 32px 40px;
    gap: 32px;
  }

  .footer-v2__main {
    flex-direction: column;
    gap: 32px;
  }

  .footer-v2__logo {
    width: 150px;
    height: 60px;
  }

  /* Auto-fit: 2 cols on narrow mobile, 3 cols when there's enough room */
  .footer-v2__cols {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(130px, 1fr));
    gap: 24px 16px;
    width: 100%;
  }

  .footer-v2__bottom {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }
}
</style>
