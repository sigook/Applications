<template>
  <section class="app-v2">
    <!-- Full-bleed wave backgrounds (Figma: cobalt blue + cyan wave, top: 480px) -->
    <img src="@/assets/images/v2/app-wave-blue.svg"  class="app-v2__wave app-v2__wave--cobalt" alt="" aria-hidden="true" />
    <img src="@/assets/images/v2/app-wave-cyan.svg"  class="app-v2__wave app-v2__wave--cyan"   alt="" aria-hidden="true" />

    <!-- 1440px reference grid — all content positioned relative to this -->
    <div class="app-v2__inner">

      <!-- Cyan ellipse behind phone (Figma node 425:5803: left 753px, top 156px, 566×549) -->
      <img src="@/assets/images/v2/app-cyan-ellipse.svg" class="app-v2__cyan-ellipse" alt="" aria-hidden="true" />

      <!-- Download block — white area, left column -->
      <div class="app-v2__download">
        <h2 class="app-v2__dl-title">Download our APP</h2>
        <p class="app-v2__dl-sub">
          Smarter Staffing,<br>Powered by Technology
        </p>
        <a href="#" class="btn btn--primary btn--sm app-v2__dl-btn" @click.prevent>
          Download
          <ArrowIconV2 :width="32" :height="11" :stroke-width="1.5" />
        </a>
      </div>

      <!-- Platform block — blue wave area, left column -->
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
      </div>

      <!-- Hand holding iPhone photo (Figma: left 602px, top 0, 460×767, rotate -0.88deg) -->
      <div class="app-v2__phone" aria-hidden="true">
        <img src="@/assets/images/v2/app-hand-phone.png" alt="" class="app-v2__phone-img" />
      </div>

      <!-- Device graphic -->
      <img src="@/assets/images/v2/app-phone-ellipse.svg"      class="app-v2__device-ellipse"     alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/app-phone-graphic-blue.png" class="app-v2__device-phone-blue"  alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/app-phone-graphic.png"      class="app-v2__device-phone"       alt="" aria-hidden="true" />

      <!-- Stores pill — bottom center -->
      <img
        src="@/assets/images/v2/app-stores-pill.svg"
        alt="Download on App Store and Google Play"
        class="app-v2__stores"
      />

    </div>
  </section>
</template>

<script setup lang="ts">
import ArrowIconV2 from '@/components/v2/ArrowIconV2.vue'
</script>

<style scoped>
/* ============================================================
   App Section V2  —  Figma reference: node 425:5801
   Canvas size: 1440 × 1379 px
   ============================================================ */

.app-v2 {
  position: relative;
  overflow: hidden;
  background: white;
  min-height: 1380px;
  z-index: 1;
}

/* --- Wave backgrounds (full-bleed, start at 480px from section top) --- */
.app-v2__wave {
  position: absolute;
  top: 480px;
  left: 0;
  width: 100%;
  height: 899px;
  object-fit: fill;
  pointer-events: none;
}

/* Figma node 425:5802: cobalt wave has left: 52px offset */
.app-v2__wave--cobalt { z-index: 1; left: 52px; }
.app-v2__wave--cyan   { z-index: 2; }

/* --- Inner 1440px layout reference grid --- */
.app-v2__inner {
  position: relative;
  width: 1440px;
  max-width: 100%;
  margin: 0 auto;
  height: 1380px;
}

/* --- Cyan ellipse behind phone (Figma node 425:5803) --- */
.app-v2__cyan-ellipse {
  position: absolute;
  left: 753px;
  top: 156px;
  width: 566px;
  height: 549px;
  z-index: 1;
  object-fit: contain;
}

/* --- Download block (white area) --- */
.app-v2__download {
  position: absolute;
  left: 193px;
  top: 204px;
  width: 362px;
  z-index: 3;
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
  margin: 0 0 26px;
}

.app-v2__dl-btn {
  display: inline-flex;
  align-items: center;
  gap: 10px;
}

/* --- Platform block (blue wave area) --- */
.app-v2__platform {
  position: absolute;
  left: 220px;
  top: 580px;
  width: 520px;
  z-index: 3;
}

.app-v2__pl-title {
  font-family: 'Poppins', sans-serif;
  font-size: 30px;
  font-weight: 700;
  line-height: 1.15;
  color: white;
  text-transform: none;
  margin: 0 0 48px;
  width: 100%;
}

.app-v2__list {
  font-family: 'Poppins', sans-serif;
  font-size: 15px;
  line-height: 1.6;
  color: white;
  padding-left: 20px;
  margin: 0;
  width: 100%;
}

.app-v2__list li {
  margin-bottom: 0;
}

/* --- Hand+phone image — sits between cobalt wave (z:1) and cyan wave (z:2) --- */
.app-v2__phone {
  position: absolute;
  left: 600px;
  top: 0;
  width: 450px;
  z-index: 1; /* behind cyan wave (z:2) so the wave covers the wrist naturally */
}

.app-v2__phone-img {
  width: 100%;
  height: auto;
  display: block;
  transform: rotate(-0.88deg);
  transform-origin: top center;
}

/* --- Device graphic — half-circle cropped at left edge, logo in visible right half --- */
.app-v2__device-ellipse {
  position: absolute;
  left: -128px;
  top: 690px;
  width: 255px;
  height: 255px;
  z-index: 4;
  object-fit: contain;
}

.app-v2__device-phone-blue {
  position: absolute;
  left: 8px;
  top: 726px;
  width: 126px;
  height: 182px;
  opacity: 0.5;
  z-index: 5;
  object-fit: contain;
}

.app-v2__device-phone {
  position: absolute;
  left: 12px;
  top: 740px;
  width: auto;
  height: 160px;
  z-index: 6;
  object-fit: contain;
}

/* --- Stores pill (bottom-center) --- */
.app-v2__stores {
  position: absolute;
  left: 666px;
  top: 940px;
  width: 152px;
  height: auto;
  z-index: 3;
  display: block;
}

/* --- Mobile (<= 1023px) --- */
@media (max-width: 1023px) {
  .app-v2 {
    min-height: unset;
  }

  .app-v2__wave {
    top: unset;
    bottom: 0;
    height: 55%;
  }

  .app-v2__wave--cobalt {
    left: 0;
  }

  .app-v2__inner {
    width: 100%;
    height: auto;
    padding: 60px 24px 240px;
  }

  .app-v2__download,
  .app-v2__platform {
    position: relative;
    left: unset;
    top: unset;
    width: 100%;
  }

  .app-v2__platform {
    margin-top: 60px;
  }

  .app-v2__pl-title {
    width: auto;
    margin-bottom: 24px;
  }

  .app-v2__list {
    margin-left: 0;
    width: auto;
  }

  .app-v2__cyan-ellipse,
  .app-v2__phone,
  .app-v2__device-ellipse,
  .app-v2__device-phone-blue,
  .app-v2__device-phone {
    display: none;
  }

  .app-v2__stores {
    position: relative;
    left: unset;
    top: unset;
    margin-top: 32px;
  }
}
</style>
