<template>
  <section class="app-v2">
    <div class="app-v2__inner">
      <!-- TOP: CTA + phone hero -->
      <div class="app-v2__top">
        <div class="app-v2__copy">
          <span class="app-v2__eyebrow">Sigook App</span>
          <h2 class="app-v2__title">
            Your workforce,<br />in your <span class="app-v2__title-accent">pocket</span>
          </h2>
          <p class="app-v2__subtitle">
            Smarter Staffing,<br />Powered by Technology
          </p>

          <div class="app-v2__badges">
            <a href="#" class="app-v2__badge" aria-label="Download on the App Store">
              <img
                src="@/assets/images/v2/app-download/app-store-apple.png"
                alt=""
                class="app-v2__badge-icon app-v2__badge-icon--apple"
              />
              <span class="app-v2__badge-text">
                <span class="app-v2__badge-small">Download on the</span>
                <span class="app-v2__badge-big">App Store</span>
              </span>
            </a>

            <a href="#" class="app-v2__badge" aria-label="Get it on Google Play">
              <img
                src="@/assets/images/v2/app-download/app-store-google.png"
                alt=""
                class="app-v2__badge-icon"
              />
              <span class="app-v2__badge-text">
                <span class="app-v2__badge-small">DOWNLOAD ON THE</span>
                <span class="app-v2__badge-big">Play Store</span>
              </span>
            </a>
          </div>
        </div>

        <div class="app-v2__phone-wrap" aria-hidden="true">
          <span class="app-v2__phone-halo"></span>
          <img
            src="@/assets/images/v2/app-download/app-hand-phone.png"
            alt=""
            class="app-v2__phone-img"
          />
        </div>
      </div>

      <!-- BOTTOM: Features -->
      <div class="app-v2__bottom">
        <div class="app-v2__bottom-inner">
          <span class="app-v2__bottom-eyebrow">Workforce platform</span>
          <h2 class="app-v2__features-title">
            All-in-One Staffing &amp;<br />Workforce Platform
          </h2>
          <div class="app-v2__accent-line"></div>

          <ul class="app-v2__features">
            <li v-for="feature in features" :key="feature">
              <svg
                class="app-v2__check"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2.5"
                stroke-linecap="round"
                stroke-linejoin="round"
                aria-hidden="true"
              >
                <polyline points="20 6 9 17 4 12" />
              </svg>
              <span>{{ feature }}</span>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const features = [
  'One platform. Total workforce visibility.',
  'Real-time attendance and supervisor-approved shifts.',
  'Instant, accurate timesheets—no manual processing.',
  'Mobile-first for workers, web-based for clients.',
  'From onboarding to invoicing and payroll, fully connected.',
]
</script>

<style scoped>
/* ============================================================
   App Download — background lives in GlobalBackgroundV2.
   This section keeps its content + overlap extensions.
   ============================================================ */

.app-v2 {
  position: relative;
  min-height: 1380px;
  /* Extend up behind CertifiedSection AND down behind TestimonialsSection so
     the wedges left by their asymmetric corners reveal GlobalBackground instead
     of a colour mismatch. */
  margin-top: -200px;
  padding-top: 200px;
  margin-bottom: -200px;
  padding-bottom: 200px;
  overflow: hidden;
  font-family: var(--font-family);
  z-index: 1;
  color: #fff;
}

/* ── Inner container ──────────────────────────────────────── */
.app-v2__inner {
  position: relative;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 var(--gutter-desktop);
  z-index: 1;
}

/* ── TOP: CTA + phone ─────────────────────────────────────── */
.app-v2__top {
  display: grid;
  grid-template-columns: 1.05fr 1fr;
  align-items: center;
  gap: 48px;
  padding: 90px 0 40px;
}

.app-v2__copy {
  position: relative;
  z-index: 2;
}

.app-v2__eyebrow {
  display: inline-block;
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 2.5px;
  text-transform: uppercase;
  color: #fff;
  background: var(--c-brand-red);
  padding: 8px 18px;
  border-radius: var(--r-pill);
  margin-bottom: 24px;
  box-shadow: 0 8px 18px -6px rgba(229, 45, 39, 0.55);
}

.app-v2__title {
  font-size: 48px;
  font-weight: 700;
  line-height: 1.05;
  color: #fff;
  margin: 0 0 16px;
  letter-spacing: -0.5px;
}

.app-v2__title-accent {
  color: var(--c-brand-cyan);
}

.app-v2__subtitle {
  font-size: 17px;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.82);
  margin: 0 0 40px;
  max-width: 380px;
}

/* ── Store badges ─────────────────────────────────────────── */
.app-v2__badges {
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  gap: 14px;
}

.app-v2__badge {
  display: inline-flex;
  align-items: center;
  gap: 14px;
  padding: 14px 24px;
  background: rgba(255, 255, 255, 0.466);
  border: 1px solid rgba(255, 255, 255, 0.39);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
  color: #fff;
  border-radius: 14px;
  text-decoration: none;
  transition: transform 0.2s ease, box-shadow 0.2s ease, background 0.2s ease, border-color 0.2s ease;
  min-width: 210px;
}

.app-v2__badge:hover {
  transform: translateY(-2px);
  background: rgba(255, 255, 255, 0.14);
  border-color: rgba(255, 255, 255, 0.3);
  box-shadow: 0 10px 24px -8px rgba(0, 0, 0, 0.45);
}

.app-v2__badge-icon {
  width: 56px;
  height: 56px;
  object-fit: contain;
  flex-shrink: 0;
}

.app-v2__badge-icon--apple {
  transform: scale(2.90);
  transform-origin: center;
}

.app-v2__badge-text {
  display: flex;
  flex-direction: column;
  line-height: 1.1;
}

.app-v2__badge-small {
  font-size: 10px;
  font-weight: 400;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  opacity: 0.85;
}

.app-v2__badge-big {
  font-size: 17px;
  font-weight: 600;
}

/* ── Phone hero with cyan halo ─────────────────────────────── */
.app-v2__phone-wrap {
  position: relative;
  display: flex;
  justify-content: center;
  align-items: flex-start;
  height: 100%;
}

.app-v2__phone-halo {
  position: absolute;
  width: 460px;
  height: 460px;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  background: radial-gradient(
    circle,
    rgba(0, 173, 239, 0.55) 0%,
    rgba(0, 173, 239, 0.25) 38%,
    rgba(0, 173, 239, 0) 72%
  );
  border-radius: 50%;
  filter: blur(4px);
  z-index: 0;
}

.app-v2__phone-img {
  position: relative;
  width: 340px;
  max-width: 100%;
  height: auto;
  z-index: 1;
  transform: rotate(-1.5deg);
  transform-origin: top center;
  animation: phone-float 6s ease-in-out infinite;
  filter: drop-shadow(0 30px 40px rgba(0, 0, 0, 0.45));
  -webkit-mask-image: linear-gradient(to bottom, #000 78%, transparent 100%);
          mask-image: linear-gradient(to bottom, #000 78%, transparent 100%);
}

@keyframes phone-float {
  0%, 100% { transform: rotate(-1.5deg) translateY(0); }
  50%      { transform: rotate(-1.5deg) translateY(-12px); }
}

/* ── BOTTOM: Features ─────────────────────────────────────── */
.app-v2__bottom {
  position: relative;
}

.app-v2__bottom-inner {
  position: relative;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 36px 0 200px;
  z-index: 1;
}

.app-v2__bottom-eyebrow {
  display: inline-block;
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 2.5px;
  text-transform: uppercase;
  color: var(--c-brand-navy);
  background: var(--c-brand-cyan);
  padding: 8px 18px;
  border-radius: var(--r-pill);
  margin-bottom: 24px;
  box-shadow: 0 8px 18px -6px rgba(0, 173, 239, 0.45);
}

.app-v2__features-title {
  font-size: 34px;
  font-weight: 700;
  line-height: 1.15;
  color: #fff;
  margin: 0 0 16px;
  max-width: 560px;
  letter-spacing: -0.3px;
}

.app-v2__accent-line {
  width: 60px;
  height: 3px;
  background: var(--c-brand-red);
  border-radius: 2px;
  margin: 0 0 28px;
}

.app-v2__features {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  column-gap: 32px;
  row-gap: 14px;
  max-width: 980px;
}

.app-v2__features li {
  display: flex;
  align-items: flex-start;
  gap: 14px;
  font-size: 16px;
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.95);
}

.app-v2__check {
  flex-shrink: 0;
  width: 22px;
  height: 22px;
  padding: 3px;
  border-radius: 50%;
  background: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  margin-top: 2px;
}

/* ============================================================
   Mobile  ≤ 1023px
   ============================================================ */
@media (max-width: 1023px) {
  .app-v2 {
    min-height: 0;
    margin-top: -120px;
    padding-top: 120px;
  }

  .app-v2__inner {
    padding: 0 var(--gutter-mobile);
  }

  .app-v2__top {
    grid-template-columns: 1fr;
    gap: 24px;
    padding: 60px 0 32px;
    text-align: center;
  }

  .app-v2__copy { order: 2; }

  .app-v2__phone-wrap {
    order: 1;
    height: 340px;
  }

  .app-v2__phone-halo {
    width: 340px;
    height: 340px;
    top: 0;
  }

  .app-v2__phone-img {
    width: 260px;
  }

  .app-v2__title {
    font-size: 36px;
  }

  .app-v2__subtitle {
    font-size: 15px;
    margin-left: auto;
    margin-right: auto;
  }

  .app-v2__badges {
    justify-content: center;
    gap: 10px;
  }

  .app-v2__badge {
    flex: 1 1 0;
    gap: 8px;
    padding: 10px 12px;
    min-width: 0;
  }

  .app-v2__badge-icon {
    width: 32px;
    height: 32px;
  }

  .app-v2__badge-small {
    font-size: 8px;
    letter-spacing: 0.3px;
  }

  .app-v2__badge-big {
    font-size: 13px;
  }

  .app-v2__bottom-inner {
    padding: 32px 0 100px;
  }

  .app-v2__features-title {
    font-size: 28px;
  }

  .app-v2__features {
    grid-template-columns: 1fr;
  }

  .app-v2__features li {
    font-size: 15px;
  }
}
</style>
