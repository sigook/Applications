<template>
  <section class="app">
    <div class="app__inner">
      <div class="app__top">
        <div class="app__copy">
          <span class="app__eyebrow">Sigook® App</span>
          <h2 class="app__title">
            Your workforce,<br />in your <span class="app__title-accent">pocket</span>
          </h2>
          <p class="app__subtitle">
            Smarter Staffing,<br />Powered by Technology
          </p>

          <div class="app__badges">
            <a
              href="https://apps.apple.com/us/app/sigook/id1446736193"
              target="_blank"
              rel="noopener noreferrer"
              class="app__badge"
              aria-label="Download Sigook on the App Store"
            >
              <img
                src="@/assets/images/landing/app-download/app-store-apple.webp"
                alt=""
                class="app__badge-icon app__badge-icon--apple"
                loading="lazy"
                decoding="async"
              />
              <span class="app__badge-text">
                <span class="app__badge-small">Download on the</span>
                <span class="app__badge-big">App Store</span>
              </span>
            </a>

            <a
              href="https://play.google.com/store/apps/details?id=com.sigook.sigook&hl=es_EC"
              target="_blank"
              rel="noopener noreferrer"
              class="app__badge"
              aria-label="Get Sigook® on Google Play"
            >
              <img
                src="@/assets/images/landing/app-download/app-store-google.webp"
                alt=""
                class="app__badge-icon"
                loading="lazy"
                decoding="async"
              />
              <span class="app__badge-text">
                <span class="app__badge-small">DOWNLOAD ON THE</span>
                <span class="app__badge-big">Play Store</span>
              </span>
            </a>
          </div>
        </div>

        <div class="app__phone-wrap" aria-hidden="true">
          <span class="app__phone-halo"></span>
          <img
            src="@/assets/images/landing/app-download/app-hand-phone.webp"
            alt=""
            class="app__phone-img"
            width="1024"
            height="1536"
            loading="lazy"
            decoding="async"
          />
        </div>
      </div>

      <div class="app__bottom">
        <div class="app__bottom-inner">
          <span class="app__bottom-eyebrow">Workforce platform</span>
          <h2 class="app__features-title">
            All-in-One Staffing &amp;<br />Workforce Platform
          </h2>
          <div class="app__accent-line"></div>

          <ul class="app__features">
            <li v-for="feature in features" :key="feature">
              <LandingIcon name="check" class="app__check" />
              <span>{{ feature }}</span>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import LandingIcon from '@/components/landing/shared/icons/LandingIcon.vue'

const features = [
  'One platform. Total workforce visibility.',
  'Real-time attendance and supervisor-approved shifts.',
  'Instant, accurate timesheets—no manual processing.',
  'Mobile-first for workers, web-based for clients.',
  'From onboarding to invoicing and payroll, fully connected.',
]
</script>

<style scoped>
.app {
  position: relative;
  min-height: 1380px;
  margin-top: -200px;
  padding-top: 200px;
  margin-bottom: -200px;
  padding-bottom: 200px;
  overflow: hidden;
  font-family: var(--font-family);
  z-index: 1;
  color: #fff;
}

.app__inner {
  position: relative;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 var(--gutter-desktop);
  z-index: 1;
}

.app__top {
  display: grid;
  grid-template-columns: 1.05fr 1fr;
  align-items: center;
  gap: 48px;
  padding: 90px 0 40px;
}

.app__copy {
  position: relative;
  z-index: 2;
}

.app__eyebrow {
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

.app__title {
  font-size: 48px;
  font-weight: 700;
  line-height: 1.05;
  color: #fff;
  margin: 0 0 16px;
  letter-spacing: -0.5px;
}

.app__title-accent {
  color: var(--c-brand-cyan);
}

.app__subtitle {
  font-size: 17px;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.82);
  margin: 0 0 40px;
  max-width: 380px;
}

.app__badges {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  gap: 14px;
}

.app__badge {
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

.app__badge:hover {
  transform: translateY(-2px);
  background: rgba(255, 255, 255, 0.14);
  border-color: rgba(255, 255, 255, 0.3);
  box-shadow: 0 10px 24px -8px rgba(0, 0, 0, 0.45);
}

.app__badge-icon {
  width: 56px;
  height: 56px;
  object-fit: contain;
  flex-shrink: 0;
}

.app__badge-icon--apple {
  transform: scale(2.90);
  transform-origin: center;
}

.app__badge-text {
  display: flex;
  flex-direction: column;
  line-height: 1.1;
}

.app__badge-small {
  font-size: 10px;
  font-weight: 400;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  opacity: 0.85;
}

.app__badge-big {
  font-size: 17px;
  font-weight: 600;
}

.app__phone-wrap {
  position: relative;
  display: flex;
  justify-content: center;
  align-items: flex-start;
  height: 100%;
}

.app__phone-halo {
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

.app__phone-img {
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

.app__bottom {
  position: relative;
}

.app__bottom-inner {
  position: relative;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 36px 0 200px;
  z-index: 1;
}

.app__bottom-eyebrow {
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

.app__features-title {
  font-size: 34px;
  font-weight: 700;
  line-height: 1.15;
  color: #fff;
  margin: 0 0 16px;
  max-width: 560px;
  letter-spacing: -0.3px;
}

.app__accent-line {
  width: 60px;
  height: 3px;
  background: var(--c-brand-red);
  border-radius: 2px;
  margin: 0 0 28px;
}

.app__features {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  column-gap: 32px;
  row-gap: 14px;
  max-width: 980px;
}

.app__features li {
  display: flex;
  align-items: flex-start;
  gap: 14px;
  font-size: 16px;
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.95);
}

.app__check {
  flex-shrink: 0;
  width: 22px;
  height: 22px;
  padding: 3px;
  border-radius: 50%;
  background: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  margin-top: 2px;
}

@media (max-width: 1023px), (max-height: 899px) {
  .app {
    min-height: 0;
    margin-top: -120px;
    padding-top: 120px;
  }

  .app__inner {
    padding: 0 var(--gutter-mobile);
  }

  .app__top {
    grid-template-columns: 1fr;
    gap: 24px;
    padding: 60px 0 32px;
    text-align: center;
  }

  .app__copy { order: 2; }

  .app__phone-wrap {
    order: 1;
    height: 340px;
  }

  .app__phone-halo {
    width: 340px;
    height: 340px;
    top: 0;
  }

  .app__phone-img {
    width: 260px;
    animation: none;
  }

  .app__title {
    font-size: 36px;
  }

  .app__subtitle {
    font-size: 15px;
    margin-left: auto;
    margin-right: auto;
  }

  .app__badges {
    justify-content: center;
    gap: 10px;
    max-width: 440px;
    margin-left: auto;
    margin-right: auto;
  }

  .app__badge {
    flex: 1 1 0;
    gap: 8px;
    padding: 10px 12px;
    min-width: 0;
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    background: rgba(255, 255, 255, 0.6);
  }

  .app__badge-icon {
    width: 32px;
    height: 32px;
  }

  .app__badge-small {
    font-size: 8px;
    letter-spacing: 0.3px;
  }

  .app__badge-big {
    font-size: 13px;
  }

  .app__bottom-inner {
    padding: 32px 0 100px;
  }

  .app__features-title {
    font-size: 28px;
  }

  .app__features {
    grid-template-columns: 1fr;
  }

  .app__features li {
    font-size: 15px;
  }
}
</style>
