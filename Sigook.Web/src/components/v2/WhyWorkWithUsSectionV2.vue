<template>
  <section class="why-work-v2">
    <!-- Section title -->
    <h2 class="why-work-v2__title">Why work with us?</h2>

    <!-- Panel 1: We are Local (text left, photo right) -->
    <div class="why-work-v2__panel why-work-v2__panel--local">
      <!-- Decorative CSS circles (Figma decorative ellipses came as empty PNGs) -->
      <span class="why-work-v2__ellipse why-work-v2__ellipse--local-mid why-work-v2__deco-red" aria-hidden="true"></span>
      <span class="why-work-v2__ellipse why-work-v2__ellipse--local-sm why-work-v2__deco-cyan" aria-hidden="true"></span>
      <!-- Main photo -->
      <img
        src="@/assets/images/v2/about/local-ellipse-38.png"
        alt=""
        class="why-work-v2__ellipse why-work-v2__ellipse--local-main why-work-v2__photo"
        aria-hidden="true"
      />
      <h3 class="why-work-v2__panel-title why-work-v2__panel-title--local">We are Local</h3>
      <p class="why-work-v2__panel-text why-work-v2__panel-text--local">
        Most of our employers are based in the Greater Toronto Area which gives us the
        opportunity to build a strong relationship with them, know their needs and as a
        result find the right job for you.
      </p>
    </div>

    <!-- Decorative stray ellipse 44 between panels 1 and 2 -->
    <span class="why-work-v2__deco-44 why-work-v2__deco-cyan" aria-hidden="true"></span>

    <!-- Panel 2: You get hired quicker (photo left bleed, text right) -->
    <div class="why-work-v2__panel why-work-v2__panel--quicker">
      <span class="why-work-v2__ellipse why-work-v2__ellipse--quicker-mid why-work-v2__deco-red" aria-hidden="true"></span>
      <span class="why-work-v2__ellipse why-work-v2__ellipse--quicker-sm why-work-v2__deco-cyan" aria-hidden="true"></span>
      <img
        src="@/assets/images/v2/about/quicker-ellipse-39.png"
        alt=""
        class="why-work-v2__ellipse why-work-v2__ellipse--quicker-main why-work-v2__photo"
        aria-hidden="true"
      />
      <h3 class="why-work-v2__panel-title why-work-v2__panel-title--quicker">
        You get hired quicker!
      </h3>
      <p class="why-work-v2__panel-text why-work-v2__panel-text--quicker">
        We created
        <a href="https://www.sigook.com/" target="_blank" rel="noopener">SIGOOK™</a>,
        a self-serve platform where employers can hire you within a few clicks. Once you
        are part of the platform database, you might get hire today and start working
        tomorrow!
      </p>
    </div>

    <!-- Panel 3: Our Experience (text left, photo right) -->
    <div class="why-work-v2__panel why-work-v2__panel--experience">
      <span class="why-work-v2__ellipse why-work-v2__ellipse--exp-mid why-work-v2__deco-red" aria-hidden="true"></span>
      <img
        src="@/assets/images/v2/about/exp-ellipse-40.png"
        alt=""
        class="why-work-v2__ellipse why-work-v2__ellipse--exp-main why-work-v2__photo"
        aria-hidden="true"
      />
      <span class="why-work-v2__ellipse why-work-v2__ellipse--exp-sm why-work-v2__deco-cyan" aria-hidden="true"></span>
      <span class="why-work-v2__ellipse why-work-v2__ellipse--exp-xs why-work-v2__deco-red" aria-hidden="true"></span>
      <h3 class="why-work-v2__panel-title why-work-v2__panel-title--experience">Our Experience</h3>
      <p class="why-work-v2__panel-text why-work-v2__panel-text--experience">
        COVENANT GROUP Staffing Agency has been in the staffing industry for over 10
        years. We know exactly what our clients need. We use our expertise to recruit
        and match workers to our client's requirements.
      </p>
    </div>
  </section>
</template>

<script setup lang="ts">
// Why work with us section — Figma node 425:7795
// Mobile dimensions: 1277px tall; photo collages extend off-canvas on each side.
</script>

<style scoped>
/* ============================================================
   Why work with us (mobile-first)
   Figma node: 425:7795 (1277px total height)
   All child positions relative to section top (canvas y=1917 → local y=0)
   ============================================================ */

.why-work-v2 {
  position: relative;
  width: 100%;
  height: 1277px;
  overflow: hidden;
  background: var(--c-bg);
  font-family: var(--font-family);
  color: var(--c-ink);
}

/* ── Section title — Figma: x=77, y=0, w=287, 30px Bold ────────────── */
.why-work-v2__title {
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  margin: 0;
  font-size: 30px;
  font-weight: 700;
  line-height: 1.5; /* 45px height per Figma */
  color: var(--c-ink);
  text-align: center;
  white-space: nowrap;
  z-index: 2;
}

/* ── Shared ellipse styles ─────────────────────────────────────────── */
.why-work-v2__ellipse {
  position: absolute;
  display: block;
  pointer-events: none;
  border-radius: 50%;
  z-index: 0;
}

/* Photo ellipses sit ABOVE decorative circles so the colored deco peeks
   out around the edges (creating the brand "petal cluster" framing) */
.why-work-v2__photo {
  object-fit: cover;
  z-index: 1;
}

/* Decorative solid colour circles (Figma assets came empty) */
.why-work-v2__deco-red { background-color: var(--c-brand-red); }
.why-work-v2__deco-cyan { background-color: var(--c-brand-cyan); }

.why-work-v2__panel {
  position: absolute;
  left: 0;
  right: 0;
}

.why-work-v2__panel-title {
  position: absolute;
  margin: 0;
  font-size: 22px;
  font-weight: 700;
  line-height: 1.5;
  color: var(--c-ink);
  z-index: 1;
}

.why-work-v2__panel-text {
  position: absolute;
  margin: 0;
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  color: var(--c-ink);
  z-index: 1;
}

.why-work-v2__panel-text a {
  color: inherit;
  text-decoration: underline;
}

/* ── Panel 1: We are Local — y=64 from section top ─────────────────── */
.why-work-v2__ellipse--local-main { top: 64px;  left: 246px; width: 417px; height: 417px; }
.why-work-v2__ellipse--local-mid  { top: 159px; left: 233px; width: 300px; height: 300px; }
.why-work-v2__ellipse--local-sm   { top: 81px;  left: 262px; width: 123px; height: 123px; }
.why-work-v2__panel-title--local  { top: 145px; left: 39px;  }
.why-work-v2__panel-text--local   { top: 187px; left: 39px;  width: 181px; }

/* ── Decorative ellipse 44 between panels 1 & 2 — y=774 ───────────── */
.why-work-v2__deco-44 {
  position: absolute;
  top: 774px;
  left: 133px;
  width: 41px;
  height: 41px;
  border-radius: 50%;
  display: block;
  pointer-events: none;
  z-index: 0;
}

/* ── Panel 2: You get hired quicker — y=432 (overlaps panel 1) ────── */
/* Photo bleeds off-canvas to the LEFT (negative x) */
.why-work-v2__ellipse--quicker-main { top: 432px; left: -234px; width: 417px; height: 417px; }
.why-work-v2__ellipse--quicker-mid  { top: 441px; left: -159px; width: 356px; height: 356px; }
.why-work-v2__ellipse--quicker-sm   { top: 613px; left: -67px;  width: 241px; height: 241px; }
.why-work-v2__panel-title--quicker  { top: 501px; left: 221px; max-width: 162px; }
.why-work-v2__panel-text--quicker   { top: 573px; left: 221px; width: 180px; }

/* ── Panel 3: Our Experience — y=839 (overlaps panel 2) ───────────── */
.why-work-v2__ellipse--exp-main { top: 839px;  left: 246px; width: 417px; height: 417px; }
.why-work-v2__ellipse--exp-mid  { top: 860px;  left: 235px; width: 417px; height: 417px; }
.why-work-v2__ellipse--exp-sm   { top: 1199px; left: 323px; width: 78px;  height: 78px;  }
.why-work-v2__ellipse--exp-xs   { top: 1228px; left: 311px; width: 28px;  height: 28px;  }
.why-work-v2__panel-title--experience { top: 912px; left: 39px;  }
.why-work-v2__panel-text--experience  { top: 954px; left: 39px;  width: 181px; }

/* ============================================================
   Desktop (≥1024px) — Figma node 425:6657 (1718.4×1809.5)
   Section starts at canvas y=2400; positions below are section-local.
   Layout pattern: title centered top, then 3 alternating panels:
     • Panel 1 (Local): photo right, text left
     • Panel 2 (Quicker): photo left (bleeds -171px), text right
     • Panel 3 (Experience): photo right, text left
   ============================================================ */
@media (min-width: 1024px) {
  .why-work-v2 {
    height: 1810px;
  }

  .why-work-v2__title {
    font-size: 50px;
    line-height: 1.5; /* 75px (Figma title height) */
  }

  .why-work-v2__panel-title {
    font-size: 30px;
    line-height: 1.5;
  }

  /* ── Panel 1: We are Local — photo right ─────────────────────── */
  .why-work-v2__ellipse--local-main { top: 98px;  left: 828px; width: 719px; height: 719px; }
  .why-work-v2__ellipse--local-mid  { top: 262px; left: 806px; width: 517px; height: 517px; }
  .why-work-v2__ellipse--local-sm   { top: 127px; left: 856px; width: 212px; height: 212px; }
  .why-work-v2__panel-title--local  { top: 294px; left: 454px; width: 265px; }
  .why-work-v2__panel-text--local   { top: 395px; left: 257px; width: 462px; }

  /* Stray decoration ellipse 44 — desktop position from Figma */
  .why-work-v2__deco-44 {
    top: 1171px; /* 3571 - 2400 */
    left: 492px;
    width: 74px;
    height: 74px;
  }

  /* ── Panel 2: Quicker — photo left bleed, text right ─────────── */
  .why-work-v2__ellipse--quicker-main { top: 553px; left: -171px; width: 754px; height: 754px; }
  .why-work-v2__ellipse--quicker-mid  { top: 569px; left: -35px;  width: 643px; height: 643px; }
  .why-work-v2__ellipse--quicker-sm   { top: 880px; left: 131px;  width: 436px; height: 436px; }
  .why-work-v2__panel-title--quicker  { top: 867px; left: 719px; max-width: 479px; width: auto; }
  .why-work-v2__panel-text--quicker   { top: 923px; left: 719px; width: 479px; }

  /* ── Panel 3: Experience — photo right ────────────────────────── */
  .why-work-v2__ellipse--exp-main { top: 1120px; left: 816px;  width: 689px; height: 689px; }
  .why-work-v2__ellipse--exp-mid  { top: 1085px; left: 1524px; width: 689px; height: 689px; }
  .why-work-v2__ellipse--exp-sm   { top: 1681px; left: 962px;  width: 129px; height: 129px; }
  .why-work-v2__ellipse--exp-xs   { top: 1729px; left: 942px;  width: 46px;  height: 46px;  }
  .why-work-v2__panel-title--experience { top: 1352px; left: 466px; width: 253px; }
  .why-work-v2__panel-text--experience  { top: 1447px; left: 257px; width: 462px; }
}
</style>
