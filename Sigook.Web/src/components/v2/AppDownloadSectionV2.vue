<template>
  <section class="app-v2">
    <!-- Wave background -->
    <div class="app-v2__wave-bg" aria-hidden="true">
      <svg viewBox="0 0 1440 580" xmlns="http://www.w3.org/2000/svg" preserveAspectRatio="none">
        <path d="M349 0 H52 V580 H1492 V226 H1166C811 226 645 0 349 0Z" fill="#2C5EBA"/>
        <path d="M297 0 H0 V580 H1440 V226 H1114C759 226 593 0 297 0Z" fill="#00ADEF"/>
      </svg>
    </div>

    <div class="app-v2__inner">
      <!-- Left: text blocks -->
      <div class="app-v2__left">
        <div class="app-v2__download">
          <h2 class="app-v2__dl-title">Download our APP</h2>
          <p class="app-v2__dl-sub">
            Smarter Staffing,<br>Powered by Technology
          </p>
          <a href="#" class="btn btn--primary btn--md app-v2__dl-btn" @click.prevent>
            Download
            <ArrowIconV2 class="app-v2__btn-arrow" :width="32" :height="11" :stroke-width="1.5" />
          </a>
        </div>

        <div class="app-v2__platform">
          <h2 class="app-v2__pl-title">
            All-in-One Staffing &amp;<br>Workforce Platform
          </h2>
          <ul class="app-v2__list">
            <li>One platform. Total workforce visibility.</li>
            <li>Real-time attendance and supervisor-approved shifts.</li>
            <li>Instant, accurate timesheets—no manual processing.</li>
            <li>Mobile-first for workers, web-based for clients.</li>
            <li>From onboarding to invoicing and payroll, fully connected.</li>
          </ul>
          <div class="app-v2__badges">
            <img src="@/assets/images/v2/app-store-apple.png" alt="Download on App Store" class="app-v2__badge" />
            <img src="@/assets/images/v2/app-store-google.png" alt="Get it on Google Play" class="app-v2__badge" />
          </div>
        </div>
      </div>

      <!-- Right: phone mockup -->
      <div class="app-v2__right" aria-hidden="true">
        <div class="app-v2__phone-circle"></div>
        <img
          src="@/assets/images/v2/app-phone.png"
          alt="Sigook mobile app"
          class="app-v2__phone-img"
        />
      </div>
    </div>

    <!-- Device icon circle (bottom-left) -->
    <div class="app-v2__device" aria-hidden="true">
      <img src="@/assets/images/v2/app-phone-graphic.png" alt="" class="app-v2__device-img" />
    </div>
  </section>
</template>

<script setup lang="ts">
import ArrowIconV2 from '@/components/v2/ArrowIconV2.vue'
</script>

<style scoped>
.app-v2 {
  position: relative;
  overflow: hidden;
  background: white;
  min-height: 960px;
}

.app-v2__wave-bg {
  position: absolute;
  left: 0;
  right: 0;
  bottom: 0;
  height: 580px;
  z-index: 0;
}

.app-v2__wave-bg svg {
  width: 100%;
  height: 100%;
}

.app-v2__inner {
  position: relative;
  z-index: 1;
  max-width: 1280px;
  margin: 0 auto;
  padding: 0 80px;
  display: flex;
  min-height: 960px;
}

.app-v2__left {
  flex: 0 0 46%;
  display: flex;
  flex-direction: column;
}

.app-v2__download {
  /* Figma: "Download our APP" text sits ~116px below where Certified ends.
     Reduced from 180px to match that gap more closely. */
  padding-top: 100px;
}

.app-v2__dl-title {
  font-family: 'Poppins', sans-serif;
  font-size: 30px;
  font-weight: 700;
  line-height: 1.15;
  color: var(--c-ink);
  text-transform: none;
  margin: 0 0 12px;
}

.app-v2__dl-sub {
  font-family: 'Poppins', sans-serif;
  font-size: 15px;
  line-height: 1.6;
  color: var(--c-ink);
  margin: 0 0 28px;
}

.app-v2__dl-btn {
  display: inline-flex;
  align-items: center;
  gap: 10px;
}

.app-v2__btn-arrow {
  flex-shrink: 0;
}

.app-v2__platform {
  margin-top: 190px;
}

.app-v2__pl-title {
  font-family: 'Poppins', sans-serif;
  font-size: 30px;
  font-weight: 700;
  line-height: 1.15;
  color: white;
  text-transform: none;
  margin: 0 0 24px;
}

.app-v2__list {
  font-family: 'Poppins', sans-serif;
  font-size: 15px;
  line-height: 1.6;
  color: white;
  padding-left: 22px;
  margin: 0 0 36px;
}

.app-v2__list li {
  margin-bottom: 4px;
}

.app-v2__badges {
  display: flex;
  gap: 14px;
  align-items: center;
}

.app-v2__badge {
  height: 38px;
  width: auto;
  object-fit: contain;
}

.app-v2__right {
  flex: 1;
  position: relative;
}

.app-v2__phone-circle {
  position: absolute;
  right: -60px;
  top: 60px;
  width: 520px;
  height: 520px;
  background: #00ADEF;
  border-radius: 50%;
}

.app-v2__phone-img {
  position: absolute;
  left: 10%;
  transform: translateX(0) rotate(-0.88deg);
  top: 10px;
  width: 420px;
  height: 640px;
  object-fit: cover;
  object-position: 82% center;
  z-index: 2;
}

.app-v2__device {
  position: absolute;
  left: -25px;
  bottom: 55px;
  width: 245px;
  height: 245px;
  background: #1A2B38;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  z-index: 2;
}

.app-v2__device-img {
  width: 60%;
  height: auto;
  opacity: 0.9;
}
</style>
