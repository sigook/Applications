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

        <!-- Company column -->
        <nav class="footer-v2__col" aria-label="Company">
          <p class="footer-v2__col-heading">Company</p>
          <a href="#" class="footer-v2__link" @click.prevent>About Us</a>
          <a href="#" class="footer-v2__link" @click.prevent>Open Positions</a>
          <a href="#" class="footer-v2__link" @click.prevent>Industries</a>
          <a href="#" class="footer-v2__link" @click.prevent>News</a>
          <a href="#" class="footer-v2__link" @click.prevent>Special Projects</a>
        </nav>

        <!-- Services column -->
        <nav class="footer-v2__col" aria-label="Services">
          <p class="footer-v2__col-heading">Services</p>
          <a href="#" class="footer-v2__link" @click.prevent>For Employers</a>
          <a href="#" class="footer-v2__link" @click.prevent>For Talents</a>
          <a href="#" class="footer-v2__link" @click.prevent>Become a Partner</a>
          <a href="#" class="footer-v2__link" @click.prevent>Licensed &amp; Certified</a>
          <a href="#" class="footer-v2__link" @click.prevent>Payroll Solutions</a>
        </nav>

        <!-- Contact column -->
        <div class="footer-v2__col">
          <p class="footer-v2__col-heading">Contact</p>
          <a href="mailto:info@sigook.com" class="footer-v2__link">info@sigook.com</a>
          <a href="tel:+18001234567" class="footer-v2__link">+1 (800) 123-4567</a>
          <span class="footer-v2__link footer-v2__link--plain">Toronto, Ontario, Canada</span>
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
          <a href="#" class="footer-v2__legal-link" @click.prevent>Privacy Policy</a>
          <a href="#" class="footer-v2__legal-link" @click.prevent>Terms of Use</a>
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
    padding: 48px var(--gutter-mobile) 32px;
    gap: 32px;
  }

  .footer-v2__main {
    flex-direction: column;
    gap: 32px;
  }

  .footer-v2__logo {
    width: 140px;
    height: 56px;
  }

  .footer-v2__bottom {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
}
</style>
