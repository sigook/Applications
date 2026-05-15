<template>
  <section class="numbers-v2">
    <!--
      numbers-v2__canvas
      • max-width 1440px, width 100% → horizontally centered on any viewport
      • height 960px → stat circle centers sit at y=480 = 960/2 (vertically centered)
      • Decorative elements: fixed px → clip at edge on narrow viewports (intentional)
      • Main content (circles/divider/dots): % positions → scale with viewport
    -->
    <div class="numbers-v2__canvas">

      <!-- ── Decorative circles (fixed px, decorative clipping ok) ───── -->
      <img src="@/assets/images/v2/deco-circle-3.svg" class="numbers-v2__deco numbers-v2__deco--c3" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/deco-circle-1.svg" class="numbers-v2__deco numbers-v2__deco--c1" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/deco-circle-2.svg" class="numbers-v2__deco numbers-v2__deco--c2" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/deco-circle-7.svg" class="numbers-v2__deco numbers-v2__deco--c7" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/deco-circle-6.svg" class="numbers-v2__deco numbers-v2__deco--c6" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/deco-circle-4.svg" class="numbers-v2__deco numbers-v2__deco--c4" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/deco-circle-5.svg" class="numbers-v2__deco numbers-v2__deco--c5" alt="" aria-hidden="true" />

      <!-- ── Decorative lines (fixed px) ────────────────────────────── -->
      <img src="@/assets/images/v2/deco-line-a.svg" class="numbers-v2__deco numbers-v2__deco--l1" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/deco-line-a.svg" class="numbers-v2__deco numbers-v2__deco--l2" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/deco-line-b.svg" class="numbers-v2__deco numbers-v2__deco--l3" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/deco-line-a.svg" class="numbers-v2__deco numbers-v2__deco--l4" alt="" aria-hidden="true" />

      <!-- ── Decorative dot grids (fixed px) ────────────────────────── -->
      <img src="@/assets/images/v2/deco-dots.svg" class="numbers-v2__deco numbers-v2__deco--dots-r" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/deco-dots.svg" class="numbers-v2__deco numbers-v2__deco--dots-l" alt="" aria-hidden="true" />

      <!-- ── Cobalt divider bar (% width + left) ────────────────────── -->
      <div class="numbers-v2__divider" aria-hidden="true"></div>

      <!-- ── Dots on the divider (% left) ──────────────────────────── -->
      <img src="@/assets/images/v2/stat-dot.svg"        class="numbers-v2__sdot numbers-v2__sdot--1" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/stat-dot.svg"        class="numbers-v2__sdot numbers-v2__sdot--2" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/stat-dot.svg"        class="numbers-v2__sdot numbers-v2__sdot--3" alt="" aria-hidden="true" />
      <img src="@/assets/images/v2/stat-dot-filled.svg" class="numbers-v2__sdot numbers-v2__sdot--4" alt="" aria-hidden="true" />

      <!-- ── Stat circles (% left, vertically centered) ─────────────── -->
      <div class="numbers-v2__stat numbers-v2__stat--clients">
        <div class="numbers-v2__circle">
          <span class="numbers-v2__num">+330</span>
          <span class="numbers-v2__lbl">Clients Served</span>
        </div>
      </div>

      <div class="numbers-v2__stat numbers-v2__stat--jobs">
        <div class="numbers-v2__circle">
          <span class="numbers-v2__num">+1,700</span>
          <span class="numbers-v2__lbl">Job Posted</span>
        </div>
      </div>

      <div class="numbers-v2__stat numbers-v2__stat--apps">
        <div class="numbers-v2__circle">
          <span class="numbers-v2__num numbers-v2__num--accent">+5,000</span>
          <span class="numbers-v2__lbl">Applications</span>
        </div>
      </div>

    </div><!-- /canvas -->
  </section>
</template>

<script setup lang="ts">
// Static content — no props needed
</script>

<style scoped>
/* ── Section shell — full-bleed background ──────────────────────────── */
.numbers-v2 {
  position: relative;
  /* No explicit z-index — avoids creating a stacking context that could conflict.
     DOM order (Numbers before WhyChooseUs) + WhyChooseUs z-index:1 puts WhyChooseUs on top. */
  margin-top: -177px;        /* slides under Talents: Talents ends at y=1550, Numbers starts at y=1373 */
  width: 100%;
  height: 1351px;            /* Figma: Section - Our Numbers height = 1351px */
  /* overflow: hidden moved to .numbers-v2__canvas to avoid stacking-context side-effects */
  background:
    linear-gradient(180deg, rgba(245, 245, 245, 0.3) 10.77%, rgba(201, 201, 201, 0.3) 81.051%),
    linear-gradient(90deg, rgba(170, 206, 220, 0.3) 0%, rgba(170, 206, 220, 0.3) 100%),
    #ffffff;
}

/*
  ── Inner canvas ─────────────────────────────────────────────────────
  Horizontally centered at max 1440px.
  height: 1351px (matches section) → position: relative for children.

  Figma section: y=1373, height=1351.
  Stat circles at document y=2092 → section-relative top = 2092−1373 = 719px.
  Divider at document y=1983 → section-relative top = 1983−1373 = 610px.
*/
.numbers-v2__canvas {
  position: relative;
  max-width: 1440px;
  width: 100%;
  height: 1351px;
  margin: 0 auto;
  overflow: hidden; /* clips decorative elements that exceed 1440px — moved here from .numbers-v2 */
}

/* ── Shared deco base ────────────────────────────────────────────────── */
.numbers-v2__deco {
  position: absolute;
  pointer-events: none;
  display: block;
}

/*
  ── Decorative circles — fixed px (Figma-exact, section-relative)
  top = document_y − 1373 (section start y).
  Intentionally clip outside the visible area on narrow viewports.
  These are background decoration; clipping is acceptable and intentional.
*/
.numbers-v2__deco--c3 { width: 298px; height: 298px; left: 266px;  top: 291px; }  /* 1664−1373 */
.numbers-v2__deco--c1 { width: 311px; height: 311px; left:  79px;  top: 334px; }  /* 1707−1373 */
.numbers-v2__deco--c2 { width:  99px; height:  99px; left: 225px;  top: 408px; }  /* 1781−1373 */
.numbers-v2__deco--c7 { width: 390px; height: 390px; left: 1160px; top: 373px; }  /* 1746−1373 */
.numbers-v2__deco--c6 { width:  90px; height:  90px; left: 1082px; top: 380px; }  /* 1753−1373 */
.numbers-v2__deco--c4 { width: 110px; height: 110px; left: 754px;  top: 458px; }  /* 1831−1373 */
.numbers-v2__deco--c5 { width: 241px; height: 241px; left: 919px;  top: 667px; }  /* 2040−1373 */

/* ── Decorative lines — fixed px ────────────────────────────────────── */
/* top = doc_y − 1373; left = doc_x (section x = 0) */
.numbers-v2__deco--l1 { width: 96px; height: 3px; left: 488px; top: 458px; }  /* deco-line-1: 1831−1373 */
.numbers-v2__deco--l2 { width: 96px; height: 3px; left: 512px; top: 440px; }  /* deco-line-2: 1813−1373 */
.numbers-v2__deco--l3 { width: 96px; height: 3px; left: 953px; top: 774px; transform: rotate(180deg); }  /* deco-line-3: 2147−1373 */
.numbers-v2__deco--l4 { width: 96px; height: 3px; left: 987px; top: 737px; transform: rotate(180deg); }  /* deco-line-4: 2110−1373 */

/* ── Dot grids — fixed px ────────────────────────────────────────────── */
/* top = doc_y − 1373 */
.numbers-v2__deco--dots-r { width: 66px; height: 31px; left: 953px; top: 380px; }  /* deco-dots-2: 1753−1373 */
.numbers-v2__deco--dots-l { width: 66px; height: 31px; left: 422px; top: 722px; }  /* deco-dots-1: 2095−1373 */

/*
  ── Cobalt divider — % positions so it scales with viewport width
  Figma: left=137px, width=1169px on 1440px canvas
         → 137/1440 = 9.51%   1169/1440 = 81.18%
  top: 610px = doc_y(1983) − section_y(1373)
*/
.numbers-v2__divider {
  position: absolute;
  left: 9.51%;
  width: 81.18%;
  top: 610px;
  height: 4.5px;
  background: #2C5EBA;
  border-radius: 2px;
  pointer-events: none;
}

/*
  ── Divider dots — % left positions
  Figma: 115px / 504px / 892px / 1281px on 1440px canvas
  top: 591px = doc_y(1963.9) − section_y(1373)
*/
.numbers-v2__sdot {
  position: absolute;
  width: 44px;
  height: 44px;
  top: 591px;
  pointer-events: none;
  display: block;
}

.numbers-v2__sdot--1 { left:  7.99%; }   /*  115/1440 */
.numbers-v2__sdot--2 { left: 35.00%; }   /*  504/1440 */
.numbers-v2__sdot--3 { left: 61.94%; }   /*  892/1440 */
.numbers-v2__sdot--4 { left: 88.96%; }   /* 1281/1440 */

/*
  ── Stat circles — % left, fixed size 212px
  Figma: 225px / 614px / 1003px on 1440px canvas
         → 15.63% / 42.64% / 69.65%
  top: 719px = doc_y(2092) − section_y(1373)

  Verified gap at 1024px (min desktop):
    Apps right edge: 69.65% × 1024 + 212 = 925px < 1024px  ✓ not clipped
*/
.numbers-v2__stat {
  position: absolute;
  top: 719px;
}

.numbers-v2__stat--clients { left: 15.63%; }
.numbers-v2__stat--jobs    { left: 42.64%; }
.numbers-v2__stat--apps    { left: 69.65%; }

/* ── Circle card ─────────────────────────────────────────────────────── */
.numbers-v2__circle {
  width: 212px;
  height: 212px;
  border-radius: 50%;
  background: #ffffff;
  box-shadow: 0 6px 24px rgba(0, 0, 0, 0.10);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 2px;
}

.numbers-v2__num {
  font-family: var(--font-family);
  font-size: 30px;
  font-weight: 700;
  line-height: 1.15;
  color: var(--c-ink);
  display: block;
  text-align: center;
}

.numbers-v2__num--accent {
  color: var(--c-brand-red);
}

.numbers-v2__lbl {
  font-family: var(--font-family);
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  color: var(--c-ink);
  display: block;
  text-align: center;
}

/* ── Mobile (≤ 1023px) ───────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .numbers-v2 {
    height: auto;
    overflow: visible;
    margin-top: 0;     /* no overlap on mobile — stack normally */
    padding: 72px 24px 64px;
  }

  /* Canvas becomes a normal flex column */
  .numbers-v2__canvas {
    height: auto;
    overflow: visible;   /* allow natural mobile layout */
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 32px;
  }

  /* Hide all decorative elements */
  .numbers-v2__deco,
  .numbers-v2__divider,
  .numbers-v2__sdot {
    display: none;
  }

  /* Reset stat circles to flow layout, centered */
  .numbers-v2__stat {
    position: relative;
    top: auto;
    left: auto !important;
    display: flex;
    justify-content: center;
  }
}
</style>
