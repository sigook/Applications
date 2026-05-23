<template>
  <section class="contact-v2">
    <!-- Decorative geometric shapes — same vocabulary as AppDownload -->
    <div class="contact-v2__shapes" aria-hidden="true">
      <!-- Large rings (background depth) -->
      <span class="contact-v2__shape contact-v2__shape--ring-cyan shape-xl-1"></span>
      <span class="contact-v2__shape contact-v2__shape--ring-red  shape-xl-2"></span>
      <span class="contact-v2__shape contact-v2__shape--ring-cyan shape-xl-3"></span>

      <!-- Medium rings -->
      <span class="contact-v2__shape contact-v2__shape--ring-red  shape-md-1"></span>
      <span class="contact-v2__shape contact-v2__shape--ring-cyan shape-md-2"></span>
      <span class="contact-v2__shape contact-v2__shape--ring-red  shape-md-3"></span>

      <!-- Small solid dots -->
      <span class="contact-v2__shape contact-v2__shape--dot-cyan shape-sm-1"></span>
      <span class="contact-v2__shape contact-v2__shape--dot-red  shape-sm-2"></span>
      <span class="contact-v2__shape contact-v2__shape--dot-cyan shape-sm-3"></span>
      <span class="contact-v2__shape contact-v2__shape--dot-red  shape-sm-4"></span>
      <span class="contact-v2__shape contact-v2__shape--dot-cyan shape-sm-5"></span>

      <!-- Tiny accents -->
      <span class="contact-v2__shape contact-v2__shape--dot-cyan shape-xs-1"></span>
      <span class="contact-v2__shape contact-v2__shape--dot-red  shape-xs-2"></span>
    </div>

    <div class="contact-v2__inner">
      <h1 class="contact-v2__title">Get in Touch!</h1>
      <p class="contact-v2__lead">Your next hire starts here</p>

      <GetInTouchFormV2 />
    </div>
  </section>
</template>

<script setup lang="ts">
import GetInTouchFormV2 from '@/components/v2/GetInTouchFormV2.vue'
</script>

<style scoped>
/* ── Section shell — same dark gradient as AppDownload for visual continuity ── */
.contact-v2 {
  position: relative;
  background:
    radial-gradient(circle at 80% 20%, rgba(0, 173, 239, 0.18) 0%, transparent 45%),
    linear-gradient(
      180deg,
      var(--c-brand-navy) 0%,
      var(--c-brand-navy-deep) 22%,
      var(--c-brand-deep) 55%,
      var(--c-brand-blue) 100%
    );
  border-radius: 0 0 20px 20px;
  overflow: hidden;
  font-family: var(--font-family);
  color: #fff;
  /* Extend up behind Testimonials to fill the wedge revealed by its rounded
     bottom-left corner — same technique AppDownload uses against its neighbors.
     Content stays in place because padding-top compensates the negative margin. */
  margin-top: -200px;
  padding-top: 200px;
  z-index: 2;
}

/* ── Decorative geometric shapes (matches AppDownload vocabulary) ────────── */
.contact-v2__shapes {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
}

.contact-v2__shape {
  position: absolute;
  border-radius: 50%;
}

.contact-v2__shape--dot-cyan { background: var(--c-brand-cyan); }
.contact-v2__shape--dot-red  { background: var(--c-brand-red); }
.contact-v2__shape--ring-cyan {
  background: transparent;
  border: 2.5px solid var(--c-brand-cyan);
}
.contact-v2__shape--ring-red {
  background: transparent;
  border: 2.5px solid var(--c-brand-red);
}

/* Large outlined rings (visual depth) */
.shape-xl-1 { width: 220px; height: 220px; top: 60px;  left: -60px;  opacity: 0.55; animation: drift-a 18s ease-in-out infinite; }
.shape-xl-2 { width: 180px; height: 180px; top: 760px; left: 86%;    opacity: 0.55; animation: drift-b 22s ease-in-out infinite; }
.shape-xl-3 { width: 280px; height: 280px; top: 400px; left: 80%;    opacity: 0.28; animation: drift-a 26s ease-in-out infinite; }

/* Medium rings */
.shape-md-1 { width: 76px;  height: 76px;  top: 200px; left: 6%;     opacity: 0.85; animation: drift-b 14s ease-in-out infinite; }
.shape-md-2 { width: 56px;  height: 56px;  top: 600px; left: 12%;    opacity: 0.75; animation: drift-a 12s ease-in-out infinite; }
.shape-md-3 { width: 44px;  height: 44px;  top: 940px; left: 60%;    opacity: 0.85; animation: drift-b 16s ease-in-out infinite; }

/* Small solid dots */
.shape-sm-1 { width: 16px; height: 16px; top: 280px; left: 22%; opacity: 1;    animation: drift-a 10s ease-in-out infinite; }
.shape-sm-2 { width: 14px; height: 14px; top: 140px; left: 76%; opacity: 1;    animation: drift-b 11s ease-in-out infinite; }
.shape-sm-3 { width: 18px; height: 18px; top: 520px; left: 50%; opacity: 1;    animation: drift-a 13s ease-in-out infinite; }
.shape-sm-4 { width: 12px; height: 12px; top: 820px; left: 30%; opacity: 1;    animation: drift-b 9s  ease-in-out infinite; }
.shape-sm-5 { width: 14px; height: 14px; top: 980px; left: 8%;  opacity: 0.95; animation: drift-a 12s ease-in-out infinite; }

/* Tiny accent dots */
.shape-xs-1 { width: 7px; height: 7px; top: 380px; left: 88%; opacity: 1; animation: drift-b 8s  ease-in-out infinite; }
.shape-xs-2 { width: 8px; height: 8px; top: 720px; left: 66%; opacity: 1; animation: drift-a 11s ease-in-out infinite; }

@keyframes drift-a {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(14px, -18px); }
}
@keyframes drift-b {
  0%, 100% { transform: translate(0, 0); }
  50%      { transform: translate(-12px, 16px); }
}

/* ── Inner container ───────────────────────────────────────────────────────── */
.contact-v2__inner {
  position: relative;
  z-index: 1;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 90px var(--gutter-desktop) 100px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

/* ── Heading ───────────────────────────────────────────────────────────────── */
.contact-v2__title {
  color: #fff;
  font-family: var(--font-family);
  font-size: 48px;
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.015em;
  text-align: center;
  text-transform: none;
  margin: 0 0 12px;
}

.contact-v2__lead {
  color: rgba(255, 255, 255, 0.85);
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 500;
  line-height: 1.5;
  text-align: center;
  margin: 0 0 40px;
}

/* ── Mobile ────────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .contact-v2 {
    min-height: unset;
    /* Smaller overlap on mobile — corners are smaller too */
    margin-top: -120px;
    padding-top: 120px;
  }

  .contact-v2__inner {
    padding: 64px var(--gutter-mobile) 72px;
  }

  .contact-v2__title {
    font-size: 32px;
  }

  .contact-v2__lead {
    font-size: 15px;
    margin-bottom: 32px;
  }

  /* Tame decorative shapes on mobile */
  .shape-xl-3 { display: none; }
  .shape-xl-1 { width: 140px; height: 140px; }
  .shape-xl-2 { width: 110px; height: 110px; left: 78%; }
  .shape-md-1 { width: 48px;  height: 48px; }
  .shape-md-2 { width: 36px;  height: 36px; }
  .shape-md-3 { width: 32px;  height: 32px; }
}
</style>
