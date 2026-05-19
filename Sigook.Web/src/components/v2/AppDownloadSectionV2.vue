<template>
  <section class="app-v2">
    <!-- Desktop wave backgrounds (Figma desktop: full-bleed, top: 480px) -->
    <img src="@/assets/images/v2/app-wave-blue.svg"  class="app-v2__wave app-v2__wave--cobalt" alt="" aria-hidden="true" />
    <img src="@/assets/images/v2/app-wave-cyan.svg"  class="app-v2__wave app-v2__wave--cyan"   alt="" aria-hidden="true" />

    <!-- Mobile wave backgrounds (Figma mobile node 425:10372 — different SVG shape/size) -->
    <img src="@/assets/images/v2/app-wave-blue-mob.svg" class="app-v2__wave-mob app-v2__wave-mob--cobalt" alt="" aria-hidden="true" />
    <img src="@/assets/images/v2/app-wave-cyan-mob.svg" class="app-v2__wave-mob app-v2__wave-mob--cyan"   alt="" aria-hidden="true" />

    <!-- 1440px reference grid — all content positioned relative to this -->
    <div class="app-v2__inner">

      <!-- Cyan ellipse behind phone (Figma node 425:5803: left 753px, top 156px, 566×549) -->
      <img src="@/assets/images/v2/app-cyan-ellipse.svg" class="app-v2__cyan-ellipse" alt="" aria-hidden="true" />

      <!-- Download block — white area, left column -->
      <div class="app-v2__download">
        <h2 class="app-v2__dl-title">Download our App</h2>
        <p class="app-v2__dl-sub">
          Smarter Staffing,<br>Powered by Technology
        </p>
        <a href="#" class="btn btn--primary btn--sm app-v2__dl-btn" @click.prevent>
          Download
          <ArrowIconV2 :width="32" :height="11" :stroke-width="1.5" />
        </a>
      </div>

      <!--
        Blue zone wrapper — on desktop: invisible static container (absolute children
        still position relative to app-v2__inner). On mobile: blue background block
        that groups the platform section + stores pill into one solid blue area.
      -->
      <div class="app-v2__blue-zone">
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

        <!-- Stores pill — bottom center (desktop: absolute; mobile: in-flow inside blue zone) -->
        <img
          src="@/assets/images/v2/app-stores-pill.svg"
          alt="Download on App Store and Google Play"
          class="app-v2__stores"
        />
      </div><!-- /blue-zone -->

      <!-- Hand holding iPhone photo (Figma: left 602px, top 0, 460×767, rotate -0.88deg) -->
      <div class="app-v2__phone" aria-hidden="true">
        <img src="@/assets/images/v2/app-hand-phone.png" alt="" class="app-v2__phone-img" />
      </div>

      <!-- Device graphic -->
      <img src="@/assets/images/v2/app-phone-ellipse.svg"      class="app-v2__device-ellipse"     alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/app-phone-graphic-blue.png" class="app-v2__device-phone-blue"  alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/app-phone-graphic.png"      class="app-v2__device-phone"       alt="" aria-hidden="true" />

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

/* --- Blue zone wrapper — desktop: no styles (transparent static container) --- */
.app-v2__blue-zone {
  /* Desktop: position static so absolutely-positioned children still anchor to app-v2__inner */
}

/* --- Mobile wave images — hidden on desktop --- */
.app-v2__wave-mob {
  display: none;
}

/* ============================================================
   Mobile  ≤ 1023px  —  Figma ref node 425:10372 (440px canvas)
   All positions are section-relative (canvas y origin = 5231px)
   ============================================================ */
@media (max-width: 1023px) {
  /* ── Section shell ── */
  .app-v2 {
    min-height: 1010px; /* total mobile canvas height */
    background: white;
  }

  /* Rectangle 32: solid cyan floor beneath the waves (Figma: top=794px, fills to bottom) */
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

  /* ── Inner canvas — fixed-height absolute grid ── */
  .app-v2__inner {
    width: 100%;
    height: 1010px;
    padding: 0;
  }

  /* ── Blue zone — reset (::before pseudo handles the blue floor) ── */
  .app-v2__blue-zone {
    margin: 0;
    padding: 0;
    background: none;
  }

  /* ── Download block — top-left, overlaps phone on the right ── */
  /* Figma: title center y=73px, subtitle y=116px, button y=161px from section top */
  .app-v2__download {
    position: absolute;
    left: 24px;
    top: 50px;
    width: calc(100% - 48px);
    z-index: 4;
  }

  /* ── Phone hand — right side, extends top-to-wave ── */
  /* Figma: left=83px, top=0, w=339px, h=574px */
  .app-v2__phone {
    display: block;
    position: absolute;
    left: 83px;
    top: 0;
    width: 339px;
    height: 574px;
    z-index: 1; /* behind cyan wave so wave clips the wrist */
  }

  /* ── Cyan ellipse (Ellipse 17) — right side, partial circle effect ── */
  /* Figma: left=314px, top=137px, size=384px */
  .app-v2__cyan-ellipse {
    display: block;
    position: absolute;
    left: 314px;
    top: 137px;
    width: 384px;
    height: 384px;
    z-index: 1;
  }

  /* Desktop waves — hidden on mobile (wrong SVG shape for mobile) */
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
    max-width: none; /* override global img { max-width: 100% } reset */
    height: 408px;
    object-fit: fill;
  }

  /* Figma: Cobalt Wave left=-181px, behind cyan */
  .app-v2__wave-mob--cobalt {
    left: -181px;
    z-index: 2;
  }

  /* Figma: Cyan Wave left=-213px, in front of cobalt */
  .app-v2__wave-mob--cyan {
    left: -213px;
    z-index: 3;
  }

  /* ── Platform block (All-in-One section) ── */
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
    width: 100%;
    margin-bottom: 24px;
  }

  /* Figma: list left=~142px from section edge → 142-24=118px from platform left.
     Offset clears the device graphic (Ellipse 22) which spans x=0→146px. */
  .app-v2__list {
    margin-left: 118px;
    width: auto;
  }

  /* ── Device graphic — Ellipse 22 (navy circle) ── */
  /* Figma: left=-53px, top=729px, size=199px */
  .app-v2__device-ellipse {
    display: block;
    position: absolute;
    left: -53px;
    top: 729px;
    width: 199px;
    height: 199px;
    z-index: 4;
  }

  /* Figma: phone graphic blue — left=29px, top=748px, w=108px, h=156px */
  .app-v2__device-phone-blue {
    display: block;
    position: absolute;
    left: 29px;
    top: 748px;
    width: 108px;
    height: 156px;
    z-index: 5;
  }

  /* Figma: phone graphic — left=38px, top=743px, w=103px, h=155px */
  .app-v2__device-phone {
    display: block;
    position: absolute;
    left: 38px;
    top: 743px;
    width: 103px;
    height: 155px;
    z-index: 6;
  }

  /* ── Stores pill — centered, bottom of canvas ── */
  /* Figma: left=calc(50%-64px), top=961px, w=128px, h=44px */
  .app-v2__stores {
    position: absolute;
    left: 50%;
    transform: translateX(-50%);
    top: 961px;
    width: 128px;
    height: 44px;
    z-index: 4;
    margin: 0;
    display: block;
  }
}
</style>
