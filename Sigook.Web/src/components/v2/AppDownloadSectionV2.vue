<template>
  <section class="app-v2">
    <!-- Wave backgrounds: same assets, shown/hidden via media query -->
    <img src="@/assets/images/v2/app-download/app-wave-cobalt.png" class="app-v2__wave app-v2__wave--cobalt" alt="" aria-hidden="true" />
    <img src="@/assets/images/v2/app-download/app-wave-cyan.png"   class="app-v2__wave app-v2__wave--cyan"   alt="" aria-hidden="true" />
    <img src="@/assets/images/v2/app-download/app-wave-cobalt.png" class="app-v2__wave-mob app-v2__wave-mob--cobalt" alt="" aria-hidden="true" />
    <img src="@/assets/images/v2/app-download/app-wave-cyan.png"   class="app-v2__wave-mob app-v2__wave-mob--cyan"   alt="" aria-hidden="true" />

    <!-- 1440px reference grid -->
    <div class="app-v2__inner">
      <img src="@/assets/images/v2/app-download/app-cyan-ellipse.svg" class="app-v2__cyan-ellipse" alt="" aria-hidden="true" />

      <!-- Download block — white area, left column -->
      <div class="app-v2__download">
        <h2 class="app-v2__dl-title">Download our App</h2>
        <p class="app-v2__dl-sub">
          Smarter Staffing,<br>Powered by Technology
        </p>
        <a :href="downloadHref" class="btn btn--primary btn--sm app-v2__dl-btn">
          Download
          <ArrowIconV2 :width="32" :height="11" :stroke-width="1.5" />
        </a>
      </div>

      <!--
        Blue zone wrapper: desktop = transparent static container so absolute children
        anchor to app-v2__inner. Mobile = groups platform + stores into the blue area.
      -->
      <div class="app-v2__blue-zone">
        <div class="app-v2__platform">
          <h2 class="app-v2__pl-title">
            All-in-One Staffing &amp;<br>Workforce Platform
          </h2>
          <ul class="app-v2__list">
            <li v-for="feature in features" :key="feature">{{ feature }}</li>
          </ul>
        </div>

        <img
          src="@/assets/images/v2/app-download/app-stores-pill.svg"
          alt="Download on App Store and Google Play"
          class="app-v2__stores"
        />
      </div>

      <!-- Hand holding iPhone photo -->
      <div class="app-v2__phone" aria-hidden="true">
        <img src="@/assets/images/v2/app-download/app-hand-phone.png" alt="" class="app-v2__phone-img" />
      </div>

      <!-- Device graphic -->
      <img src="@/assets/images/v2/app-download/app-phone-ellipse.svg"      class="app-v2__device-ellipse"    alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/app-download/app-phone-graphic-blue.png" class="app-v2__device-phone-blue" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/app-download/app-phone-graphic.png"      class="app-v2__device-phone"      alt="" aria-hidden="true" />
    </div>
  </section>
</template>

<script setup lang="ts">
import ArrowIconV2 from '@/components/v2/ArrowIconV2.vue'

withDefaults(defineProps<{
  downloadHref?: string
}>(), {
  downloadHref: '#'
})

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
   App Section V2  —  Figma reference: node 425:5801
   Canvas size: 1440 × 1379 px
   ============================================================ */

.app-v2 {
  position: relative;
  overflow: hidden;
  background: white;
  min-height: 1380px;
  z-index: 1;
  font-family: 'Poppins', sans-serif;
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
  font-size: 30px;
  font-weight: 700;
  line-height: 1.15;
  color: var(--c-ink);
  margin: 0 0 12px;
}

.app-v2__dl-sub {
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
  font-size: 30px;
  font-weight: 700;
  line-height: 1.15;
  color: white;
  margin: 0 0 48px;
}

.app-v2__list {
  font-size: 15px;
  line-height: 1.6;
  color: white;
  padding-left: 20px;
  margin: 0;
}

/* --- Hand+phone image — sits between cobalt wave (z:1) and cyan wave (z:2) --- */
.app-v2__phone {
  position: absolute;
  left: 600px;
  top: 0;
  width: 450px;
  z-index: 1; /* behind cyan wave so the wave covers the wrist naturally */
}

.app-v2__phone-img {
  width: 100%;
  height: auto;
  display: block;
  transform: rotate(-0.88deg);
  transform-origin: top center;
}

/* --- Device graphic — half-circle cropped at left edge --- */
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

/* --- Mobile wave images — hidden on desktop --- */
.app-v2__wave-mob {
  display: none;
}

/* ============================================================
   Mobile  ≤ 1023px  —  Figma ref node 425:10372 (440px canvas)
   ============================================================ */
@media (max-width: 1023px) {
  .app-v2 {
    min-height: 1010px;
  }

  /* Rectangle 32: solid cyan floor beneath the waves (Figma: top=794px) */
  .app-v2::before {
    content: '';
    position: absolute;
    left: 0;
    right: 0;
    top: 794px;
    bottom: 0;
    background: var(--c-brand-cyan, #00ADEF);
    z-index: 0;
    pointer-events: none;
  }

  .app-v2__inner {
    width: 100%;
    height: 1010px;
    padding: 0;
  }

  /* Figma: title center y=73px, button y=161px from section top */
  .app-v2__download {
    position: absolute;
    left: 24px;
    top: 50px;
    width: calc(100% - 48px);
    z-index: 4;
  }

  /* Figma: left=83px, top=0, w=339px, h=574px */
  .app-v2__phone {
    left: 83px;
    top: 0;
    width: 339px;
    height: 574px;
    z-index: 1;
  }

  /* Figma: left=314px, top=137px, size=384px */
  .app-v2__cyan-ellipse {
    left: 314px;
    top: 137px;
    width: 384px;
    height: 384px;
    z-index: 1;
  }

  .app-v2__wave {
    display: none;
  }

  /* Mobile waves — SVG paths designed for 440px canvas, stretched to 654px wide */
  /* Figma: both waves top=402px from section top */
  .app-v2__wave-mob {
    display: block;
    position: absolute;
    top: 402px;
    width: 654px;
    max-width: none; /* override global img { max-width: 100% } */
    height: 408px;
    object-fit: fill;
  }

  /* Figma: Cobalt left=-181px, behind cyan */
  .app-v2__wave-mob--cobalt { left: -181px; z-index: 2; }
  /* Figma: Cyan left=-213px, in front of cobalt */
  .app-v2__wave-mob--cyan   { left: -213px; z-index: 3; }

  /* Figma: title center y=648px → top-edge ~618px */
  .app-v2__platform {
    position: absolute;
    left: 24px;
    top: 618px;
    width: calc(100% - 48px);
    margin-top: 0;
    z-index: 4;
  }

  .app-v2__pl-title {
    margin-bottom: 24px;
  }

  /* Offset clears the device graphic (Ellipse 22) which spans x=0→146px */
  .app-v2__list {
    margin-left: 118px;
    width: auto;
  }

  /* Figma: left=-53px, top=729px, size=199px */
  .app-v2__device-ellipse {
    left: -53px;
    top: 729px;
    width: 199px;
    height: 199px;
    z-index: 4;
  }

  /* Figma: left=29px, top=748px, w=108px, h=156px */
  .app-v2__device-phone-blue {
    left: 29px;
    top: 748px;
    width: 108px;
    height: 156px;
    z-index: 5;
  }

  /* Figma: left=38px, top=743px, w=103px, h=155px */
  .app-v2__device-phone {
    left: 38px;
    top: 743px;
    width: 103px;
    height: 155px;
    z-index: 6;
  }

  /* Figma: centered, top=961px, w=128px, h=44px */
  .app-v2__stores {
    position: absolute;
    left: 50%;
    transform: translateX(-50%);
    top: 961px;
    width: 128px;
    height: 44px;
    z-index: 4;
    margin: 0;
  }
}
</style>
