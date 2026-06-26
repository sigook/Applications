<template>
  <footer class="footer">
    <div class="footer__accent" aria-hidden="true"></div>

    <div class="footer__inner">
      <div class="footer__main">
        <div class="footer__brand">
          <img
            src="@/assets/images/v2/footer/footer-logo.png"
            alt="Sigook Work Factory"
            class="footer__logo"
          />
          <p class="footer__tagline">
            Staffing and workforce solutions for growing businesses across the United States.
          </p>
          <SocialLinks class="footer__social" />
        </div>

        <div class="footer__cols">
          <nav class="footer__col" aria-label="Company">
            <span class="footer__col-eyebrow">Company</span>
            <router-link to="/about"             class="footer__link">About Us</router-link>
            <router-link to="/open-positions"    class="footer__link">Open Positions</router-link>
            <router-link to="/industries"        class="footer__link">Industries</router-link>
            <router-link to="/special-projects"  class="footer__link">Special Projects</router-link>
            <router-link to="/partner"           class="footer__link">Become a Partner</router-link>
            <a href="#sp-contact"                   class="footer__link">Contact</a>
          </nav>

          <nav class="footer__col" aria-label="Services">
            <span class="footer__col-eyebrow">Services</span>
            <router-link to="/employers" class="footer__link">Temporary Staffing</router-link>
            <router-link to="/payroll"   class="footer__link">Payroll Services</router-link>
            <router-link to="/talents"   class="footer__link">Workforce Solutions</router-link>
            <router-link to="/certified" class="footer__link">Compliance</router-link>
          </nav>

          <div class="footer__col">
            <span class="footer__col-eyebrow">Contact</span>
            <a href="mailto:info@sigook.com" class="footer__link">info@sigook.com</a>
            <a href="tel:+13055550123"       class="footer__link">+1 (305) 555-0123</a>
            <span class="footer__link footer__link--plain">Florida, USA</span>
          </div>
        </div>
      </div>

      <div class="footer__divider" aria-hidden="true"></div>

      <div class="footer__bottom">
        <p class="footer__copyright">
          &copy; {{ currentYear }} Sigook Work Factory Inc. All rights reserved.
        </p>
        <div class="footer__legal">
          <router-link to="/privacy-policy" class="footer__legal-link">Privacy Policy</router-link>
          <router-link to="/terms-and-conditions"          class="footer__legal-link">Terms and Conditions</router-link>
          <router-link to="/disclaimer"     class="footer__legal-link">Disclaimer</router-link>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
import SocialLinks from '@/components/landing/shared/SocialLinks.vue'

const currentYear = new Date().getFullYear()
</script>

<style scoped>
.footer {
  position: relative;
  z-index: 1;
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

.footer__accent {
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

.footer__inner {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 72px var(--gutter-desktop) 36px;
  display: flex;
  flex-direction: column;
  gap: 48px;
}

.footer__main {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 64px;
}

.footer__cols {
  display: contents;
}

.footer__brand {
  display: flex;
  flex-direction: column;
  gap: 18px;
  flex-shrink: 0;
  max-width: 260px;
}

.footer__logo {
  width: 180px;
  height: 72px;
  object-fit: contain;
  object-position: left center;
  filter: brightness(0) invert(1);
}

.footer__tagline {
  font-family: var(--font-family);
  font-size: 14px;
  font-weight: 400;
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.70);
  margin: 0;
}

.footer__col {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.footer__col-eyebrow {
  display: inline-block;
  font-family: var(--font-family);
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: 6px;
}

.footer__link {
  font-family: var(--font-family);
  font-size: 14px;
  font-weight: 400;
  line-height: 1.4;
  color: rgba(255, 255, 255, 0.72);
  text-decoration: none;
  transition: color 0.25s ease, transform 0.25s ease;
  width: fit-content;
}

.footer__link:hover {
  color: var(--c-brand-cyan);
  transform: translateX(2px);
}

.footer__link--plain,
.footer__link--plain:hover {
  cursor: default;
  color: rgba(255, 255, 255, 0.72);
  transform: none;
}

.footer__divider {
  height: 1px;
  background: rgba(255, 255, 255, 0.14);
  width: 100%;
}

.footer__bottom {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-family: var(--font-family);
  font-size: 12px;
  font-weight: 400;
  color: rgba(255, 255, 255, 0.55);
}

.footer__copyright {
  margin: 0;
}

.footer__legal {
  display: flex;
  gap: 28px;
}

.footer__legal-link {
  color: rgba(255, 255, 255, 0.55);
  text-decoration: none;
  transition: color 0.25s ease;
}

.footer__legal-link:hover {
  color: var(--c-brand-cyan);
}

@media (max-width: 1023px) {
  .footer {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    background: linear-gradient(
      180deg,
      rgba(9, 48, 85, 0.55) 0%,
      rgba(9, 48, 85, 0.80) 100%
    );
  }

  .footer__inner {
    padding: 56px 28px 36px;
    gap: 36px;
  }

  .footer__main {
    flex-direction: column;
    gap: 36px;
  }

  .footer__brand {
    max-width: 100%;
  }

  .footer__logo {
    width: 150px;
    height: 60px;
  }

  .footer__cols {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
    gap: 28px 16px;
    width: 100%;
  }

  .footer__bottom {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
}
</style>
