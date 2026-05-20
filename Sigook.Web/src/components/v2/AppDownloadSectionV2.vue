<template>
  <section class="app-v2">
    <!-- Decorative geometric shapes: solid + outlined circles, varied sizes -->
    <div class="app-v2__shapes" aria-hidden="true">
      <!-- Large rings (background depth) -->
      <span class="app-v2__shape app-v2__shape--ring-cyan shape-xl-1"></span>
      <span class="app-v2__shape app-v2__shape--ring-red  shape-xl-2"></span>
      <span class="app-v2__shape app-v2__shape--ring-cyan shape-xl-3"></span>

      <!-- Medium rings -->
      <span class="app-v2__shape app-v2__shape--ring-red  shape-md-1"></span>
      <span class="app-v2__shape app-v2__shape--ring-cyan shape-md-2"></span>
      <span class="app-v2__shape app-v2__shape--ring-red  shape-md-3"></span>

      <!-- Small dots -->
      <span class="app-v2__shape app-v2__shape--dot-cyan shape-sm-1"></span>
      <span class="app-v2__shape app-v2__shape--dot-red  shape-sm-2"></span>
      <span class="app-v2__shape app-v2__shape--dot-cyan shape-sm-3"></span>
      <span class="app-v2__shape app-v2__shape--dot-red  shape-sm-4"></span>
      <span class="app-v2__shape app-v2__shape--dot-cyan shape-sm-5"></span>
      <span class="app-v2__shape app-v2__shape--dot-red  shape-sm-6"></span>

      <!-- Tiny accents -->
      <span class="app-v2__shape app-v2__shape--dot-cyan shape-xs-1"></span>
      <span class="app-v2__shape app-v2__shape--dot-red  shape-xs-2"></span>
    </div>

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

      <!-- BOTTOM: Features (same background, no panel) -->
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
   App Download — modern reimagined v2
   Unified dark background, geometric decorative shapes.
   Total height 1380px (compat with TestimonialsSection -700 overlap)
   ============================================================ */

.app-v2 {
  position: relative;
  min-height: 1380px;
  background:
    radial-gradient(circle at 80% 20%, rgba(0, 173, 239, 0.18) 0%, transparent 45%),
    linear-gradient(
      180deg,
      var(--c-brand-navy) 0%,
      var(--c-brand-navy-deep) 22%,
      var(--c-brand-deep) 50%,
      var(--c-brand-navy-deep) 78%,
      var(--c-brand-navy) 100%
    );
  /* Extend up behind CertifiedSection (z:2) AND down behind TestimonialsSection
     (z:3) to fill the wedges left by their asymmetric border-radii. Adjacent
     sections stay on top via z-index; only the curve-exposed areas get filled
     with matching navy — no more colour jumps at either edge. */
  margin-top: -200px;
  padding-top: 200px;
  margin-bottom: -200px;
  padding-bottom: 200px;
  overflow: hidden;
  font-family: var(--font-family);
  z-index: 1;
  color: #fff;
}

/* ── Decorative shapes (solid dots + outlined rings) ────────── */
.app-v2__shapes {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
}

.app-v2__shape {
  position: absolute;
  border-radius: 50%;
}

.app-v2__shape--dot-cyan { background: var(--c-brand-cyan); }
.app-v2__shape--dot-red  { background: var(--c-brand-red); }
.app-v2__shape--ring-cyan {
  background: transparent;
  border: 2.5px solid var(--c-brand-cyan);
}
.app-v2__shape--ring-red {
  background: transparent;
  border: 2.5px solid var(--c-brand-red);
}

/* ── Large outlined rings (visual depth) ───────────────────── */
.shape-xl-1 { width: 220px; height: 220px; top: 80px;   left: -60px; opacity: 0.55; animation: drift-a 18s ease-in-out infinite; }
.shape-xl-2 { width: 180px; height: 180px; top: 980px;  left: 86%;   opacity: 0.55; animation: drift-b 22s ease-in-out infinite; }
.shape-xl-3 { width: 280px; height: 280px; top: 540px;  left: 38%;   opacity: 0.28; animation: drift-a 26s ease-in-out infinite; }

/* ── Medium rings ──────────────────────────────────────────── */
.shape-md-1 { width: 76px;  height: 76px;  top: 180px;  left: 4%;    opacity: 0.85; animation: drift-b 14s ease-in-out infinite; }
.shape-md-2 { width: 56px;  height: 56px;  top: 760px;  left: 14%;   opacity: 0.75; animation: drift-a 12s ease-in-out infinite; }
.shape-md-3 { width: 44px;  height: 44px;  top: 1200px; left: 58%;   opacity: 0.85; animation: drift-b 16s ease-in-out infinite; }

/* ── Small solid dots ──────────────────────────────────────── */
.shape-sm-1 { width: 16px;  height: 16px;  top: 260px;  left: 22%;   opacity: 1;    animation: drift-a 10s ease-in-out infinite; }
.shape-sm-2 { width: 14px;  height: 14px;  top: 130px;  left: 78%;   opacity: 1;    animation: drift-b 11s ease-in-out infinite; }
.shape-sm-3 { width: 18px;  height: 18px;  top: 660px;  left: 52%;   opacity: 1;    animation: drift-a 13s ease-in-out infinite; }
.shape-sm-4 { width: 12px;  height: 12px;  top: 920px;  left: 28%;   opacity: 1;    animation: drift-b 9s  ease-in-out infinite; }
.shape-sm-5 { width: 14px;  height: 14px;  top: 1090px; left: 8%;    opacity: 0.95; animation: drift-a 12s ease-in-out infinite; }
.shape-sm-6 { width: 12px;  height: 12px;  top: 1280px; left: 46%;   opacity: 1;    animation: drift-b 15s ease-in-out infinite; }

/* ── Tiny accent dots ──────────────────────────────────────── */
.shape-xs-1 { width: 7px;   height: 7px;   top: 420px;  left: 90%;   opacity: 1;    animation: drift-b 8s  ease-in-out infinite; }
.shape-xs-2 { width: 8px;   height: 8px;   top: 820px;  left: 64%;   opacity: 1;    animation: drift-a 11s ease-in-out infinite; }

@keyframes drift-a {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(14px, -18px); }
}
@keyframes drift-b {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(-12px, 16px); }
}

/* ── Inner container ────────────────────────────────────────── */
.app-v2__inner {
  position: relative;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 var(--gutter-desktop);
  z-index: 1;
}

/* ── TOP: CTA + phone ───────────────────────────────────────── */
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

/* Solid pill eyebrow — high contrast on dark bg */
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

/* ── Store badges (bigger icons, more presence) ────────────── */
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

/* Apple logo PNG has whitespace around the glyph — scale up to visually match Play Store */
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

/* ── Phone hero with cyan halo, masked at bottom to blend ──── */
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

/* ── BOTTOM: Features (same unified bg, no panel) ──────────── */
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
    /* Mobile: smaller overlap (corners are smaller on mobile too) */
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

  .app-v2__copy {
    order: 2;
  }

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

  /* Revert badge sizing on mobile — fit two in a row */
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

  /* Tame the decorative shapes on mobile — keep variety but smaller */
  .shape-xl-3 { display: none; }
  .shape-xl-1 { width: 140px; height: 140px; }
  .shape-xl-2 { width: 110px; height: 110px; }
  .shape-md-1 { width: 48px; height: 48px; }
  .shape-md-2 { width: 36px; height: 36px; }
  .shape-md-3 { width: 32px; height: 32px; }
}
</style>
