<!-- src/components/layout/MainFooter.vue -->
<template>
  <footer class="footer">
    <!-- PANEL GRIS CURVO -->
    <div class="footer__panel">
      <div class="footer__grid">
        <!-- COLUMNA CONTACTO -->
        <div class="footer__col footer__col--contact">
          <h3 class="footer__title">
            Contact Us!
          </h3>
          <span class="footer__divider footer__divider--large"></span>

          <div class="footer__block">
            <p class="footer__label">Business Hours:</p>
            <p>Monday - Friday: 9:00 to 17:00</p>
          </div>

          <div class="footer__block">
            <p class="footer__label">Phone:</p>
            <a href="tel:+16476604440">+1 (647) 660 4440</a>
          </div>

          <div class="footer__block">
            <p class="footer__label">Email:</p>
            <a href="mailto:info@covenantgroupl.com">
              info@covenantgroupl.com
            </a>
          </div>
        </div>

        <!-- NAV 1 -->
        <div class="footer__col footer__nav-1">
          <span class="footer__divider"></span>
          <RouterLink class="footer__link" to="/">Home</RouterLink>
          <RouterLink class="footer__link" to="/about-us">About Us</RouterLink>
          <RouterLink class="footer__link" to="/industries">Industries</RouterLink>
        </div>

        <!-- NAV 2 -->
        <div class="footer__col footer__nav-2">
          <div class="footer__block">
            <p class="footer__section">For Business</p>
            <RouterLink class="footer__sub-link" to="/industries?type=professional">
              Professional
            </RouterLink>
            <RouterLink class="footer__sub-link" to="/industries?type=industrial">
              Industrial
            </RouterLink>
          </div>
          <div class="footer__block">
            <p class="footer__section">For Talents</p>
            <RouterLink class="footer__sub-link" to="/open-positions?type=professional">
              Professional
            </RouterLink>
            <RouterLink class="footer__sub-link" to="/open-positions?type=industrial">
              Industrial
            </RouterLink>
          </div>

        </div>

        <!-- NAV 3 -->
        <div class="footer__col footer__block footer__nav-3">
          <RouterLink class="footer__link" to="/news">News</RouterLink>
          <RouterLink class="footer__link" to="/become-a-partner">
            Become a Partner
          </RouterLink>
          <RouterLink class="footer__link" to="/download-app">
            Download Our App
          </RouterLink>
        </div>
      </div>
    </div>

    <!-- ZONA BLANCA CON LOGO Y COPYRIGHT -->
    <div class="footer__bottom">
      <img
        src="@/assets/images/logo-covenant-footer.svg"
        alt="Covenant Group"
        class="footer__logo"
      />
      <p class="footer__copy">© 2025 COVENANT GROUP LTD</p>
    </div>
  </footer>
</template>

<script setup>
import { RouterLink } from 'vue-router'
</script>

<style scoped>
*{
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.footer {
  background: #ffffff;
  font-family: Poppins, sans-serif;
}

/* PANEL GRIS CURVO */
.footer__panel {
  background: #c9c9c9;
  border-radius: 0 0 220px 0;
  padding: 60px 80px 70px;
  position: relative;
  left: 50%;
  right: 50%;
  margin-left: -50vw;
  margin-right: -50vw;
  width: 100vw;
}

/* GRID PRINCIPAL */
.footer__grid {
  max-width: 100rem;
  margin: 0 auto;
  display: grid;
  grid-template-columns: minmax(260px, 1.8fr) repeat(4, minmax(120px, 0.7fr));
  column-gap: 80px;
}

/* COLUMNA CONTACTO */
.footer__title {
  margin: 0 0 14px;
  font-size: 2rem;
  font-weight: 700;
  color: #ffffff;
}

.footer__col--contact {
  margin-left: 5rem;
  color: #f5f5f5;
}

.footer__block {
  margin-top: 18px;
  font-size: 0.9rem;
}

.footer__label {
  margin: 0 0 4px;
  font-weight: 600;
}

.footer a {
  color: #f5f5f5;
  text-decoration: none;
}

.footer a:hover {
  text-decoration: underline;
}

/* DIVISORES */
.footer__divider {
  display: block;
  width: 36px;
  height: 2px;
  background: rgba(255, 255, 255, 0.7);
  margin-bottom: 18px;
}

.footer__divider--large {
  width: 60px;
}

/* NAV COLUMNS */
.footer__col {
  color: #f5f5f5;
  font-size: 0.9rem;
}

.footer__link {
  display: block;
  margin-bottom: 10px;
}

.footer__section {
  margin: 0 0 6px;
  font-weight: 600;
}

.footer__sub-link {
  display: block;
  font-size: 0.8rem;
  margin-bottom: 4px;
}

/* ZONA BLANCA INFERIOR */
.footer__bottom {
  padding: 40px 20px 32px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.footer__logo {
  height: 60px;
  object-fit: contain;
  margin-bottom: 2rem;
}

.footer__copy {
  margin: 0;
  font-size: 0.8rem;
  color: #777777;
}

.footer__nav-3{
  display: flex;
  flex-direction: column;
  gap: 10px;
}

/* RESPONSIVE */
@media (max-width: 1024px) {
  .footer__panel {
    padding: 50px 32px 60px;
    border-radius: 0 0 140px 0;
  }

  .footer__grid {
    column-gap: 40px;
    grid-template-columns: minmax(260px, 1.3fr) repeat(3, minmax(120px, 0.8fr));
  }
}

@media (max-width: 768px) {
  .footer__grid {
    grid-template-columns: 1fr 1fr;
    row-gap: 32px;
  }

  .footer__panel {
    border-radius: 0 0 80px 0;
  }
}

@media (max-width: 540px) {
  .footer__grid {
    grid-template-columns: 1fr;
  }

  .footer__nav-1, .footer__nav-2, .footer__nav-3{
    display: none;
  }
}
</style>
