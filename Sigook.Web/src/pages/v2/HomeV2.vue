<template>
  <main>
    <!-- ── Chunk 2: Hero ── -->
    <section class="hero-v2">
      <!-- Background photo + overlays -->
      <div class="hero-v2__bg" aria-hidden="true">
        <img src="@/assets/images/v2/hero-bg.jpg" alt="" class="hero-v2__bg-img" />
        <div class="hero-v2__overlay"></div>
      </div>

      <!-- Centred content stack -->
      <div class="hero-v2__content">
        <img
          src="@/assets/images/v2/hero-logo.png"
          alt="Sigook Work Factory"
          class="hero-v2__logo"
        />

        <a href="#" class="btn btn--secondary btn--lg hero-v2__cta" @click.prevent>
          Learn More
        </a>

        <p class="hero-v2__tagline">
          Behind Every Great American Company is Great Talent
        </p>

        <div class="hero-v2__dots">
          <span class="hero-v2__dot hero-v2__dot--active"></span>
          <span class="hero-v2__dot"></span>
          <span class="hero-v2__dot"></span>
        </div>
      </div>
    </section>

    <!-- ── Chunk 3a: For Employers ── -->
    <section class="employers-v2">
      <!-- Layered backgrounds (back → front) -->
      <div class="employers-v2__bg-1"></div>
      <div class="employers-v2__bg-2"></div>
      <div class="employers-v2__bg-3"></div>

      <!-- Decorative arrow (centre) -->
      <div class="employers-v2__arrow" aria-hidden="true">
        <svg width="52" height="20" viewBox="0 0 52 20" fill="none" xmlns="http://www.w3.org/2000/svg">
          <line x1="0" y1="10" x2="42" y2="10" stroke="white" stroke-width="2"/>
          <polyline points="34,2 50,10 34,18" stroke="white" stroke-width="2" fill="none" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </div>

      <!-- Text block (right side) -->
      <div class="employers-v2__text">
        <h1 class="v2-h1">FOR EMPLOYERS</h1>
        <div class="sect-line"></div>
        <p class="v2-body">
          Find the right people<br>for your business
        </p>
      </div>
    </section>

    <!-- ── Chunk 3b: For Talents ── -->
    <section class="talents-v2">
      <!-- Decorative arrow (centre) -->
      <div class="talents-v2__arrow" aria-hidden="true">
        <svg width="52" height="20" viewBox="0 0 52 20" fill="none" xmlns="http://www.w3.org/2000/svg">
          <line x1="0" y1="10" x2="42" y2="10" stroke="white" stroke-width="2"/>
          <polyline points="34,2 50,10 34,18" stroke="white" stroke-width="2" fill="none" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </div>

      <!-- Text block (left side, right-aligned) -->
      <div class="talents-v2__text">
        <h1 class="v2-h1">FOR TALENTS</h1>
        <div class="sect-line sect-line--right"></div>
        <p class="v2-body">
          Where great talent<br>meets great opportunities
        </p>
      </div>
    </section>

    <!-- ── Chunk 4: Our Numbers ── -->
    <section class="numbers-v2">
      <!-- Decorative background circles -->
      <div class="numbers-v2__deco numbers-v2__deco--ring-l" aria-hidden="true"></div>
      <div class="numbers-v2__deco numbers-v2__deco--blue-l" aria-hidden="true"></div>
      <div class="numbers-v2__deco numbers-v2__deco--navy"   aria-hidden="true"></div>
      <div class="numbers-v2__deco numbers-v2__deco--pink"   aria-hidden="true"></div>
      <div class="numbers-v2__deco numbers-v2__deco--ring-r" aria-hidden="true"></div>
      <div class="numbers-v2__deco numbers-v2__deco--red"    aria-hidden="true"></div>
      <div class="numbers-v2__deco numbers-v2__deco--blue-r" aria-hidden="true"></div>

      <!-- Stats row (cobalt divider + 4 dots + 3 circles) -->
      <div class="numbers-v2__stats">
        <!-- Dots sitting on the divider line -->
        <span class="numbers-v2__dot numbers-v2__dot--d1" aria-hidden="true"></span>
        <span class="numbers-v2__dot numbers-v2__dot--d2" aria-hidden="true"></span>
        <span class="numbers-v2__dot numbers-v2__dot--d3" aria-hidden="true"></span>
        <span class="numbers-v2__dot numbers-v2__dot--d4" aria-hidden="true"></span>

        <!-- Stat 1 -->
        <div class="numbers-v2__stat">
          <div class="numbers-v2__circle">
            <span class="v2-h2 numbers-v2__number">+330</span>
            <span class="v2-body numbers-v2__label">Clients Served</span>
          </div>
        </div>

        <!-- Stat 2 -->
        <div class="numbers-v2__stat">
          <div class="numbers-v2__circle">
            <span class="v2-h2 numbers-v2__number">+1,700</span>
            <span class="v2-body numbers-v2__label">Job Posted</span>
          </div>
        </div>

        <!-- Stat 3 -->
        <div class="numbers-v2__stat">
          <div class="numbers-v2__circle">
            <span class="v2-h2 numbers-v2__number numbers-v2__number--red">+5,000</span>
            <span class="v2-body numbers-v2__label">Applications</span>
          </div>
        </div>
      </div>
    </section>

    <!-- ── Chunk 5: Why Choose Us ── -->
    <section class="why-v2">
      <!-- Block A — Corporate buildings photo -->
      <div class="why-v2__hero">
        <img src="@/assets/images/v2/why-bg.jpg" alt="" class="why-v2__hero-bg" aria-hidden="true" />
        <div class="why-v2__hero-overlay" aria-hidden="true"></div>
        <div class="why-v2__hero-content">
          <div class="why-v2__hero-left">
            <h2 class="v2-display why-v2__hero-title">Why Choose Us?</h2>
          </div>
          <div class="why-v2__hero-right">
            <p class="v2-body why-v2__hero-body">
              We are a Talent Management Agency focused on providing skilled professionals,
              tailored workforce solutions, and reliable support to ensure every partnership
              runs smoothly and efficiently.
            </p>
          </div>
        </div>
      </div>

      <!-- Block B — Blue content panel -->
      <div class="why-v2__panel">
        <div class="why-v2__panel-inner">

          <!-- Row 1: Map (left) + Nationwide vision (right) -->
          <div class="why-v2__row why-v2__row--top">
            <div class="why-v2__col why-v2__col--map">
              <div class="why-v2__map-wrap">
                <!-- Territory shape (ocean + landmass) -->
                <div class="why-v2__map-container">
                  <svg class="why-v2__map-territory" viewBox="0 0 613 551" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
                    <rect width="613" height="551" fill="#B0CEDD"/>
                    <!-- USA + Canada simplified landmass -->
                    <path fill="#D4E8F4" stroke="#98B8CA" stroke-width="2"
                      d="M 95,470
                         C 65,420 48,380 38,310
                         C 30,260 32,235 20,205
                         L 0,180 L 0,85
                         L 65,45 L 180,18 L 300,8 L 410,18
                         L 490,52 L 545,110 L 580,175 L 590,230
                         L 572,272 L 555,305 L 540,330 L 525,352
                         L 508,378 L 496,410 L 480,445 L 462,468
                         L 448,490 L 438,515 L 425,535
                         L 405,535 L 388,518
                         L 368,508 L 338,505 L 298,506
                         L 258,520 L 232,510 L 210,490
                         L 182,474 L 155,480 L 122,466
                         Z"/>
                  </svg>
                  <!-- Red pins overlay — white areas become territory color via multiply -->
                  <img src="@/assets/images/v2/usa-map.png" alt="USA/Canada presence map" class="why-v2__map" />
                </div>
                <!-- Hawaii inset (SVG — source PNG is transparent) -->
                <div class="why-v2__hawaii-wrap">
                  <svg class="why-v2__hawaii-svg" viewBox="0 0 300 150" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
                    <rect width="300" height="150" fill="#B0CEDD"/>
                    <ellipse cx="10"  cy="50"  rx="8"  ry="9"  fill="#7AADCA"/>
                    <ellipse cx="32"  cy="42"  rx="18" ry="16" fill="#7AADCA"/>
                    <ellipse cx="105" cy="55"  rx="22" ry="14" fill="#7AADCA"/>
                    <ellipse cx="168" cy="62"  rx="20" ry="8"  fill="#7AADCA"/>
                    <ellipse cx="178" cy="80"  rx="10" ry="8"  fill="#7AADCA"/>
                    <ellipse cx="210" cy="65"  rx="25" ry="16" fill="#7AADCA"/>
                    <ellipse cx="212" cy="92"  rx="9"  ry="6"  fill="#7AADCA"/>
                    <ellipse cx="278" cy="108" rx="18" ry="30" fill="#7AADCA"/>
                  </svg>
                </div>
              </div>
            </div>
            <div class="why-v2__col why-v2__col--nationwide">
              <h2 class="v2-h2 why-v2__col-title">
                Nationwide vision<br>with local focus
              </h2>
              <p class="v2-body why-v2__col-body">
                We combine the reach of a nationwide recruitment network with the
                dedication and understanding of local expertise, ensuring tailored
                solutions that truly meet each client's unique needs.
              </p>
              <a href="#" class="btn btn--primary btn--sm why-v2__btn" @click.prevent>
                Learn More
                <svg class="why-v2__btn-arrow" width="32" height="11" viewBox="0 0 32 11" fill="none" aria-hidden="true">
                  <line x1="0" y1="5.5" x2="26" y2="5.5" stroke="white" stroke-width="1.5"/>
                  <polyline points="20,1 30,5.5 20,10" stroke="white" stroke-width="1.5" fill="none" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
              </a>
            </div>
          </div>

          <!-- Row 2: Market Leaders (left) + Recruitment Graphic (right) -->
          <div class="why-v2__row why-v2__row--bottom">
            <div class="why-v2__col why-v2__col--leaders">
              <h2 class="v2-h2 why-v2__col-title why-v2__col-title--right">
                We are Market<br>Leaders &amp; Innovators
              </h2>
              <p class="v2-body why-v2__col-body why-v2__col-body--right">
                At Sigook Work Factory, we pride ourselves on being trusted market leaders
                and forward-thinking innovators. By combining industry expertise with
                modern strategies, we create meaningful connections between businesses and
                top talent. Our commitment to excellence ensures that both employers and
                job seekers achieve success in a rapidly changing world.
              </p>
              <div class="why-v2__col-btn-right">
                <a href="#" class="btn btn--primary btn--sm why-v2__btn" @click.prevent>
                  Learn More
                  <svg class="why-v2__btn-arrow" width="32" height="11" viewBox="0 0 32 11" fill="none" aria-hidden="true">
                    <line x1="0" y1="5.5" x2="26" y2="5.5" stroke="white" stroke-width="1.5"/>
                    <polyline points="20,1 30,5.5 20,10" stroke="white" stroke-width="1.5" fill="none" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                </a>
              </div>
            </div>
            <div class="why-v2__col why-v2__col--graphic">
              <!-- Recruitment graphic: white illustration (SVG) on blue bg -->
              <svg class="why-v2__graphic-svg" viewBox="0 0 480 360" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
                <!-- Main magnifying glass circle -->
                <circle cx="280" cy="165" r="130" fill="none" stroke="white" stroke-width="12" opacity="0.9"/>
                <!-- Handle -->
                <line x1="375" y1="265" x2="440" y2="330" stroke="white" stroke-width="16" stroke-linecap="round" opacity="0.9"/>
                <!-- Person inside circle (head + body) -->
                <circle cx="280" cy="140" r="28" fill="white" opacity="0.9"/>
                <path d="M220 210 Q220 185 280 185 Q340 185 340 210" fill="white" opacity="0.9"/>
                <!-- Smaller person left-1 -->
                <circle cx="90" cy="155" r="20" fill="white" opacity="0.75"/>
                <path d="M48 210 Q48 193 90 193 Q132 193 132 210" fill="white" opacity="0.75"/>
                <!-- Smaller person left-2 -->
                <circle cx="145" cy="175" r="18" fill="white" opacity="0.65"/>
                <path d="M108 225 Q108 210 145 210 Q182 210 182 225" fill="white" opacity="0.65"/>
                <!-- Sparkle stars -->
                <!-- Star 1: top-right -->
                <g transform="translate(420,60)" stroke="white" stroke-width="2.5" stroke-linecap="round" opacity="0.8">
                  <line x1="0" y1="-14" x2="0" y2="14"/><line x1="-14" y1="0" x2="14" y2="0"/>
                  <line x1="-8" y1="-8" x2="8" y2="8"/><line x1="8" y1="-8" x2="-8" y2="8"/>
                </g>
                <!-- Star 2: top center -->
                <g transform="translate(240,20)" stroke="white" stroke-width="2" stroke-linecap="round" opacity="0.6">
                  <line x1="0" y1="-10" x2="0" y2="10"/><line x1="-10" y1="0" x2="10" y2="0"/>
                  <line x1="-6" y1="-6" x2="6" y2="6"/><line x1="6" y1="-6" x2="-6" y2="6"/>
                </g>
                <!-- Star 3: left -->
                <g transform="translate(30,90)" stroke="white" stroke-width="2" stroke-linecap="round" opacity="0.6">
                  <line x1="0" y1="-10" x2="0" y2="10"/><line x1="-10" y1="0" x2="10" y2="0"/>
                  <line x1="-6" y1="-6" x2="6" y2="6"/><line x1="6" y1="-6" x2="-6" y2="6"/>
                </g>
                <!-- Star 4: bottom left -->
                <g transform="translate(60,300)" stroke="white" stroke-width="2" stroke-linecap="round" opacity="0.5">
                  <line x1="0" y1="-8" x2="0" y2="8"/><line x1="-8" y1="0" x2="8" y2="0"/>
                  <line x1="-5" y1="-5" x2="5" y2="5"/><line x1="5" y1="-5" x2="-5" y2="5"/>
                </g>
              </svg>
            </div>
          </div>

        </div>
      </div>
    </section>

    <!-- ══════════════════════════════════════════
         CHUNK 6 — CERTIFIED SECTION
    ══════════════════════════════════════════ -->
    <section class="certified-v2">
      <!-- Background photo + overlay -->
      <div class="certified-v2__bg">
        <img src="@/assets/images/v2/certified-bg.jpg" alt="" class="certified-v2__photo" />
        <div class="certified-v2__overlay"></div>
      </div>
      <!-- Centered content -->
      <div class="certified-v2__content">
        <h2 class="certified-v2__title">Smarter staffing for growing teams</h2>
        <p class="certified-v2__body">
          We support growing businesses with flexible staffing and recruitment solutions, combining experienced recruiters, streamlined processes, and reliable talent to meet evolving workforce needs.
        </p>
        <div class="certified-v2__badge">Grow With Us</div>
      </div>
    </section>

    <!-- ── Chunk 7: App Download ── -->
    <section class="app-v2">
      <!-- Wave background (covers bottom 60% of section) -->
      <div class="app-v2__wave-bg" aria-hidden="true">
        <svg viewBox="0 0 1440 580" xmlns="http://www.w3.org/2000/svg" preserveAspectRatio="none">
          <!-- Cobalt wave (back layer, offset right) -->
          <path d="M349 0 H52 V580 H1492 V226 H1166C811 226 645 0 349 0Z" fill="#2C5EBA"/>
          <!-- Cyan wave (front layer) -->
          <path d="M297 0 H0 V580 H1440 V226 H1114C759 226 593 0 297 0Z" fill="#00ADEF"/>
        </svg>
      </div>

      <!-- Content -->
      <div class="app-v2__inner">
        <!-- Left: text blocks -->
        <div class="app-v2__left">
          <!-- White area: download CTA -->
          <div class="app-v2__download">
            <h2 class="app-v2__dl-title">Download our APP</h2>
            <p class="app-v2__dl-sub">
              Smarter Staffing,<br>Powered by Technology
            </p>
            <a href="#" class="btn btn--primary btn--md app-v2__dl-btn" @click.prevent>
              Download
              <svg class="app-v2__btn-arrow" width="32" height="11" viewBox="0 0 32 11" fill="none" aria-hidden="true">
                <line x1="0" y1="5.5" x2="26" y2="5.5" stroke="white" stroke-width="1.5"/>
                <polyline points="20,1 30,5.5 20,10" stroke="white" stroke-width="1.5" fill="none" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
            </a>
          </div>

          <!-- Blue area: platform features -->
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

        <!-- Right: phone mockup (spans full section height) -->
        <div class="app-v2__right" aria-hidden="true">
          <div class="app-v2__phone-circle"></div>
          <img
            src="@/assets/images/v2/app-phone.png"
            alt="Sigook mobile app"
            class="app-v2__phone-img"
          />
        </div>
      </div>

      <!-- Device icon circle (bottom-left, absolute) -->
      <div class="app-v2__device" aria-hidden="true">
        <img src="@/assets/images/v2/app-phone-graphic.png" alt="" class="app-v2__device-img" />
      </div>
    </section>

    <!-- ── Chunks 8–10 placeholder ── -->
    <section style="padding: 60px; text-align: center; color: #999;">
      Chunks 8–10 coming soon
    </section>
  </main>
</template>

<script setup lang="ts">
// No state needed for static hero
</script>

<style scoped>
/* ── Hero wrapper ── */
.hero-v2 {
  position: relative;
  width: 100%;
  height: 865px;
  overflow: hidden;
}

/* ── Background ── */
.hero-v2__bg {
  position: absolute;
  inset: 0;
}

.hero-v2__bg-img {
  position: absolute;
  width: 100%;
  height: 296%;
  top: -20%;
  left: 0;
  object-fit: cover;
  object-position: center top;
}

.hero-v2__overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(90deg, rgba(15, 47, 68, 0.35) 0%, rgba(15, 47, 68, 0.35) 100%),
    linear-gradient(180deg, rgba(15, 47, 68, 0) 71.55%, rgb(15, 47, 68) 100%);
}

/* ── Content stack ── */
.hero-v2__content {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-top: 353px; /* logo top = y:353 in Figma */
}

/* ── Logo ── */
.hero-v2__logo {
  width: 365px;
  height: 147px;
  object-fit: contain;
  object-position: center;
  display: block;
}

/* ── CTA Button ── */
.hero-v2__cta {
  margin-top: 77px; /* 577 - (353+147) */
  border-color: var(--c-brand-blue) !important;
  box-shadow: 0 4px 2px rgba(0, 0, 0, 0.25);
}

/* ── Tagline ── */
.hero-v2__tagline {
  margin-top: 64px; /* 697 - (577+56) */
  font-family: var(--font-family);
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  color: #fff;
  text-align: center;
  max-width: 221px;
}

/* ── Slider dots ── */
.hero-v2__dots {
  display: flex;
  align-items: center;
  gap: 8px;
  background-color: #fff;
  border: 1px solid rgba(170, 206, 220, 0.3);
  padding: 4px 10px;
  border-radius: 30px;
  margin-top: 10px; /* 761 - (697+54) */
}

.hero-v2__dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: var(--c-neutral-muted);
  flex-shrink: 0;
}

.hero-v2__dot--active {
  width: 10px;
  height: 10px;
  background-color: var(--c-brand-blue);
}

/* ─────────────────────────────────────────────
   Shared decorative elements
───────────────────────────────────────────── */
.sect-line {
  width: 113px;
  height: 2px;
  background: #fff;
  margin: 16px 0 20px;
}

/* right-aligned variant (talents) */
.sect-line--right {
  margin-left: auto;
}

/* ─────────────────────────────────────────────
   Employers
───────────────────────────────────────────── */
.employers-v2 {
  position: relative;
  width: 100%;
  height: 659px;
  background: var(--c-brand-navy); /* shows between rounded-corner gaps */
  overflow: hidden;
}

/* Layer 1 – deepest blue (#13629e) */
.employers-v2__bg-1 {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 659px;
  background: #13629e;
  border-radius: 0 125px 150px 0;
}

/* Layer 2 – brand blue (#1575bb), inset 29px from top */
.employers-v2__bg-2 {
  position: absolute;
  left: 0;
  top: 29px;
  width: 100%;
  height: 630px;
  background: var(--c-brand-blue);
  border-radius: 0 130px 150px 0;
}

/* Layer 3 – cyan (#00adef), inset 57px from top — frontmost */
.employers-v2__bg-3 {
  position: absolute;
  left: 0;
  top: 57px;
  width: 100%;
  height: 602px;
  background: var(--c-brand-cyan);
  border-radius: 0 130px 150px 0;
}

/* Decorative arrow — centred at x≈703px, y≈178px */
.employers-v2__arrow {
  position: absolute;
  left: 703px;
  top: 178px;
  transform: translate(-50%, -50%);
  z-index: 2;
  display: flex;
}

/* Text block — starts at x=939px */
.employers-v2__text {
  position: absolute;
  left: 939px;
  top: 105px; /* H1 top (Figma centre=127px, h≈44px → 127-22=105) */
  z-index: 2;
  color: #fff;
}

.employers-v2__text .v2-h1 {
  color: #fff;
  margin: 0;
  white-space: nowrap;
}

.employers-v2__text .v2-body {
  color: #fff;
  margin: 0;
}

/* ─────────────────────────────────────────────
   Talents
───────────────────────────────────────────── */
.talents-v2 {
  position: relative;
  width: 100%;
  height: 354px;
  background: var(--c-brand-red);
  border-radius: 150px 0 150px 0; /* rounded-tl-150 rounded-br-150 */
  margin-top: -312px; /* overlap lower portion of Employers */
  z-index: 3;
  overflow: hidden;
}

/* Decorative arrow — centred at x≈692px, y≈162px */
.talents-v2__arrow {
  position: absolute;
  left: 692px;
  top: 162px;
  transform: translate(-50%, -50%);
  z-index: 2;
  display: flex;
}

/* Text block — right-edge at x=520px, top starts at y=83px */
.talents-v2__text {
  position: absolute;
  left: 259px;
  width: 261px;
  top: 83px; /* H1 top (Figma centre=105px, h≈44px → 105-22=83) */
  text-align: right;
  z-index: 2;
  color: #fff;
}

.talents-v2__text .v2-h1 {
  color: #fff;
  margin: 0;
  white-space: nowrap;
}

.talents-v2__text .v2-body {
  color: #fff;
  margin: 0;
}

/* ── Mobile adjustments ── */
@media (max-width: 1023px) {
  /* Hero */
  .hero-v2 {
    height: 100svh;
    min-height: 600px;
  }

  .hero-v2__content {
    padding-top: 140px;
    padding-left: 24px;
    padding-right: 24px;
  }

  .hero-v2__logo {
    width: 220px;
    height: 90px;
  }

  .hero-v2__cta {
    margin-top: 48px;
  }

  .hero-v2__tagline {
    margin-top: 32px;
    font-size: 14px;
    max-width: 280px;
  }

  /* Employers – stack text below center */
  .employers-v2 {
    height: auto;
    min-height: 420px;
    padding-bottom: 48px;
  }

  .employers-v2__bg-1 { height: 100%; }
  .employers-v2__bg-2 { height: calc(100% - 29px); }
  .employers-v2__bg-3 { height: calc(100% - 57px); }

  .employers-v2__arrow {
    left: 50%;
    top: 56px;
  }

  .employers-v2__text {
    left: 24px;
    right: 24px;
    top: 120px;
  }

  /* Talents – restore full height */
  .talents-v2 {
    height: auto;
    min-height: 300px;
    padding: 80px 24px 48px;
    margin-top: -80px;
    border-radius: 80px 0 80px 0;
  }

  .talents-v2__text {
    position: relative;
    left: auto;
    width: 100%;
    top: auto;
    text-align: left;
  }

  .talents-v2__text .sect-line--right {
    margin-left: 0;
  }

  .talents-v2__arrow {
    left: 50%;
    top: 40px;
  }
}

/* ─────────────────────────────────────────────
   Our Numbers
───────────────────────────────────────────── */
.numbers-v2 {
  position: relative;
  width: 100%;
  height: 800px;
  overflow: hidden;
  /* light blue-gray gradient matching Figma */
  background:
    linear-gradient(180deg, rgba(245,245,245,0.3) 10.77%, rgba(201,201,201,0.3) 81.051%),
    linear-gradient(90deg, rgba(170,206,220,0.3) 0%, rgba(170,206,220,0.3) 100%),
    #ffffff;
}

/* ── Decorative circles ── */
.numbers-v2__deco {
  position: absolute;
  border-radius: 50%;
  pointer-events: none;
  z-index: 0;
}

/* Large stroke ring — left */
.numbers-v2__deco--ring-l {
  width: 311px; height: 311px;
  top: 161px; left: 79px;
  border: 1.5px solid rgba(0,0,0,0.1);
  background: transparent;
}
/* Large solid blue — left */
.numbers-v2__deco--blue-l {
  width: 298px; height: 298px;
  top: 140px; left: 266px;
  background: var(--c-brand-sky-blue);
  opacity: 0.85;
}
/* Small navy circle */
.numbers-v2__deco--navy {
  width: 99px; height: 99px;
  top: 196px; left: 225px;
  background: var(--c-brand-navy);
  opacity: 0.7;
}
/* Small rose/salmon circle */
.numbers-v2__deco--pink {
  width: 110px; height: 110px;
  top: 220px; left: 754px;
  background: #f8b8b6;
  opacity: 0.7;
}
/* Large stroke ring — right */
.numbers-v2__deco--ring-r {
  width: 390px; height: 390px;
  top: 180px; left: 1160px;
  border: 1.5px solid rgba(0,0,0,0.1);
  background: transparent;
}
/* Small red dot — right */
.numbers-v2__deco--red {
  width: 90px; height: 90px;
  top: 183px; left: 1082px;
  background: var(--c-brand-red);
  opacity: 0.85;
}
/* Large solid blue — lower right */
.numbers-v2__deco--blue-r {
  width: 241px; height: 241px;
  top: 321px; left: 919px;
  background: var(--c-brand-sky-blue);
  opacity: 0.85;
}

/* ── Stats row ── */
.numbers-v2__stats {
  position: absolute;
  /* Figma: circles start at x=225, row width=990 (3×212 + 2×177 gap) */
  top: 507px;
  left: 225px;
  width: 990px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  z-index: 1;
}

/* Cobalt divider line — extends 88px left and 91px right of the row */
.numbers-v2__stats::before {
  content: '';
  position: absolute;
  top: 50%;
  left: -88px;
  right: -91px;
  height: 4px;
  margin-top: -2px;
  background: var(--c-brand-cobalt);
  z-index: 0;
}

/* ── Dots on the divider ── */
.numbers-v2__dot {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: var(--c-brand-cobalt);
  z-index: 1;
}
/* Figma x-centers (section-relative): 159, 526, 914, 1303
   Relative to stats container left=225: 159-225-22=-88, 526-225-22=279, 914-225-22=667, 1303-225-22=1056 */
.numbers-v2__dot--d1 { left: -88px;  }
.numbers-v2__dot--d2 { left: 279px;  }
.numbers-v2__dot--d3 { left: 667px;  }
.numbers-v2__dot--d4 { left: 1056px; }

/* ── Single stat ── */
.numbers-v2__stat {
  position: relative;
  z-index: 2;
}

/* ── White circle ── */
.numbers-v2__circle {
  width: 212px;
  height: 212px;
  border-radius: 50%;
  background: #fff;
  box-shadow: 0 4px 24px rgba(0,0,0,0.10);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.numbers-v2__number {
  display: block;
  color: var(--c-ink);
  margin: 0;
  font-size: var(--text-h2-size);    /* 30px */
  font-weight: var(--text-h2-weight); /* 700 */
  line-height: var(--text-h2-lh);    /* 1.15 */
}

.numbers-v2__number--red {
  color: var(--c-brand-red);
}

.numbers-v2__label {
  display: block;
  color: var(--c-ink);
  margin: 4px 0 0;
  font-size: var(--text-body-size);    /* 15px */
  font-weight: var(--text-body-weight); /* 400 */
  line-height: var(--text-body-lh);    /* 1.6 */
}

/* ── Mobile: stack vertically ── */
@media (max-width: 1023px) {
  .numbers-v2 {
    height: auto;
    padding: 80px 24px;
  }

  .numbers-v2__stats {
    position: relative;
    top: auto;
    left: auto;
    width: 100%;
    flex-direction: column;
    align-items: center;
    gap: 32px;
  }

  .numbers-v2__stats::before { display: none; }
  .numbers-v2__dot           { display: none; }
  .numbers-v2__deco          { display: none; }
}

/* ─────────────────────────────────────────────
   Why Choose Us
───────────────────────────────────────────── */
.why-v2 {
  position: relative;
}

/* ── Block A: Photo ── */
.why-v2__hero {
  position: relative;
  width: 100%;
  height: 490px;
  border-radius: 150px 0 150px 0; /* cut-tl-br */
  overflow: hidden;
}

.why-v2__hero-bg {
  position: absolute;
  width: 100%;
  height: 200%;
  top: -30%;
  left: 0;
  object-fit: cover;
  object-position: center 30%;
}

.why-v2__hero-overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(180deg, rgba(16,52,75,0.23) 0%, rgba(15,47,68,0.65) 100%),
    linear-gradient(180deg, rgba(69,147,86,0.4) 0%, rgba(102,102,102,0.4) 100%);
}

.why-v2__hero-content {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  height: 100%;
  padding: 0 80px;
  gap: 94px;
}

.why-v2__hero-left {
  flex: 0 0 480px;
  text-align: right;
}

.why-v2__hero-title {
  color: #fff;
  margin: 0;
}

.why-v2__hero-right {
  flex: 1;
}

.why-v2__hero-body {
  color: #fff;
  margin: 0;
}

/* ── Block B: Blue panel ── */
.why-v2__panel {
  position: relative;
  background: var(--c-brand-blue);
  border-radius: 150px 0 150px 0;
  margin-top: -196px;
  z-index: 1;
  padding-bottom: 120px;
  overflow: hidden;
}

.why-v2__panel-inner {
  max-width: 1440px;
  margin: 0 auto;
  padding: 248px 80px 0;
}

/* ── Rows ── */
.why-v2__row {
  display: flex;
  align-items: flex-start;
  gap: 92px;
}

.why-v2__row--bottom {
  margin-top: 72px;
}

/* ── Columns ── */
.why-v2__col {
  flex: 1;
}

.why-v2__col--map {
  flex: 0 0 575px;
}

.why-v2__col--nationwide {
  padding-top: 48px;
}

.why-v2__col--leaders {
  flex: 0 0 575px;
}

.why-v2__col--graphic {
  display: flex;
  align-items: center;
  padding-top: 24px;
}

/* ── Map images ── */
.why-v2__map-wrap {
  position: relative;
}

.why-v2__map-container {
  position: relative;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 4px 16px rgba(0,0,0,0.18);
}

.why-v2__map-territory {
  width: 100%;
  display: block;
}

.why-v2__map {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  mix-blend-mode: multiply;
  object-fit: cover;
}

.why-v2__hawaii-wrap {
  display: inline-block;
  border-radius: 4px;
  border: 1px solid rgba(255,255,255,0.35);
  margin-top: 10px;
  overflow: hidden;
  width: 172px;
}

.why-v2__hawaii-svg {
  width: 100%;
  display: block;
}

/* ── Recruitment graphic SVG ── */
.why-v2__graphic-svg {
  width: 100%;
  max-width: 480px;
  height: auto;
  display: block;
}

/* ── Column text ── */
.why-v2__col-title {
  color: #fff;
  margin: 0;
}

.why-v2__col-title--right {
  text-align: right;
}

.why-v2__col-body {
  color: rgba(255,255,255,0.9);
  margin: 20px 0 24px;
}

.why-v2__col-body--right {
  text-align: right;
}

.why-v2__col-btn-right {
  text-align: right;
}

/* ── Learn More buttons ── */
.why-v2__btn {
  display: inline-flex;
  align-items: center;
  gap: 10px;
}

.why-v2__btn-arrow {
  flex-shrink: 0;
}

/* ── Mobile ── */
@media (max-width: 1023px) {
  .why-v2__hero {
    height: auto;
    min-height: 280px;
    border-radius: 80px 0 80px 0;
    padding: 60px 24px;
  }

  .why-v2__hero-bg {
    height: 100%;
    top: 0;
  }

  .why-v2__hero-content {
    flex-direction: column;
    align-items: flex-start;
    padding: 0;
    height: auto;
    gap: 20px;
  }

  .why-v2__hero-left {
    flex: none;
    width: 100%;
    text-align: left;
  }

  .why-v2__hero-title {
    font-size: 32px;
  }

  .why-v2__hero-right {
    width: 100%;
  }

  .why-v2__panel {
    border-radius: 80px 0 80px 0;
    margin-top: -80px;
  }

  .why-v2__panel-inner {
    padding: 120px 24px 0;
  }

  .why-v2__row {
    flex-direction: column;
    gap: 40px;
  }

  .why-v2__col--map,
  .why-v2__col--leaders {
    flex: none;
    width: 100%;
  }

  .why-v2__col--nationwide {
    padding-top: 0;
  }

  .why-v2__col-title--right,
  .why-v2__col-body--right,
  .why-v2__col-btn-right {
    text-align: left;
  }
}

/* ═══════════════════════════════════════════
   CHUNK 6 — CERTIFIED SECTION
═══════════════════════════════════════════ */

.certified-v2 {
  position: relative;
  width: 100%;
  height: 577px;
  overflow: hidden;
  border-radius: 0 150px 0 150px; /* top-right and bottom-left */
}

/* ── Background photo + overlay ── */
.certified-v2__bg {
  position: absolute;
  inset: 0;
}
.certified-v2__photo {
  position: absolute;
  width: 100%;
  height: 200%;
  top: -47%;
  left: 0;
  object-fit: cover;
  object-position: center 40%;
}
.certified-v2__overlay {
  position: absolute;
  inset: 0;
  background: rgba(26, 117, 187, 0.45);
}

/* ── Content: centered column ── */
.certified-v2__content {
  position: relative;
  z-index: 1;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: 122px 80px 0;
}

.certified-v2__title {
  font-family: 'Poppins', sans-serif;
  font-size: 30px;
  font-weight: 300; /* Light */
  line-height: 1.2;
  color: #fff;
  margin: 0 0 56px;
  max-width: 643px;
  text-transform: none;
}

.certified-v2__body {
  font-family: 'Poppins', sans-serif;
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  color: #fff;
  margin: 0 0 40px;
  max-width: 578px;
}

.certified-v2__badge {
  background: var(--neutral-muted, #99B4BF);
  color: #fff;
  font-family: 'Poppins', sans-serif;
  font-size: 15px;
  font-weight: 400;
  line-height: 1.6;
  padding: 12px 60px;
  border-radius: 20px;
  letter-spacing: 0.02em;
}

/* ────────────────────────────────────────────────
   Chunk 7 — App Download
──────────────────────────────────────────────── */
.app-v2 {
  position: relative;
  overflow: hidden;
  background: white;
  min-height: 960px;
}

.app-v2__wave-bg {
  position: absolute;
  left: 0; right: 0;
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

/* Left column */
.app-v2__left {
  flex: 0 0 46%;
  display: flex;
  flex-direction: column;
}

.app-v2__download {
  padding-top: 180px;
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

/* Right column: phone visual */
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

/* Device icon circle (bottom-left) */
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
