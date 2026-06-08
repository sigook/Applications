<template>
  <section class="op-hero">
    <HeroBackground :image="heroImage" focal="center 35%" />
    <!-- Magnifier — reuses the Home top-left anchor (Open Positions is the
         page closest in spirit to Home: discovery + search). Anchors are
         "1 per page" by default; this is the intentional exception. -->
    <DecoMagnifier class="op-hero__magnifier" />

    <div class="op-hero__content">
      <EyebrowPill variant="cyan" class="op-hero__eyebrow">
        Open Positions
      </EyebrowPill>

      <h1 class="op-hero__heading">
        Find your next move.
        <span class="op-hero__heading-accent">Today's openings.</span>
      </h1>

      <p class="op-hero__subtitle">
        Browse vetted roles across the United States — from skilled trades
        to professional positions. Updated weekly.
      </p>

      <!-- Search form — glass V2 instead of the Figma white floating card -->
      <form class="op-hero__search" @submit.prevent="onSearch">
        <div class="op-hero__field">
          <label class="op-hero__label" for="op-job-title">Job title</label>
          <input
            id="op-job-title"
            v-model="form.title"
            type="text"
            class="op-hero__input"
            placeholder="e.g. Forklift Operator"
            autocomplete="off"
          />
        </div>

        <div class="op-hero__field">
          <label class="op-hero__label" for="op-city">City or postal code</label>
          <input
            id="op-city"
            v-model="form.location"
            type="text"
            class="op-hero__input"
            placeholder="e.g. Miami, FL"
            autocomplete="off"
          />
        </div>

        <button type="submit" class="op-hero__submit">
          <span>Search</span>
          <span class="op-hero__submit-arrow" aria-hidden="true">→</span>
        </button>
      </form>

      <ul class="op-hero__stats" aria-label="Why apply with us">
        <li class="op-hero__stat">
          <span class="op-hero__stat-value">Weekly</span>
          <span class="op-hero__stat-label">Fresh roles added</span>
        </li>
        <li class="op-hero__stat">
          <span class="op-hero__stat-value">Vetted</span>
          <span class="op-hero__stat-label">Employers only</span>
        </li>
        <li class="op-hero__stat">
          <span class="op-hero__stat-value">Fast</span>
          <span class="op-hero__stat-label">Apply in minutes</span>
        </li>
      </ul>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Open Positions hero — window-style intro with the search form glass-baked
 * directly into the layout (replaces the Figma floating white card).
 *
 * On submit the form queries the live /api/WebSite/jobs feed by title/city
 * (the only server-side filters available) and scrolls to the list section,
 * which also has a local title search for in-place refinement.
 *
 * Magnifier anchor: top-left (reuses Home's anchor by design — both pages
 * are search/discovery surfaces).
 */
import { reactive } from 'vue'
import DecoMagnifier from '@/components/v2/landing/shared/DecoMagnifier.vue'
import HeroBackground from '@/components/v2/landing/shared/HeroBackground.vue'
import heroImage from '@/assets/images/v2/hero/open-positions.jpg'
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'
import { useJobs } from '@/composables/useJobs'

interface SearchForm {
  title: string
  location: string
}

const form = reactive<SearchForm>({
  title: '',
  location: '',
})

const { fetchJobs } = useJobs()

function onSearch(): void {
  fetchJobs({
    jobTitle: form.title || undefined,
    location: form.location || undefined,
  })
  const target = document.getElementById('op-list')
  target?.scrollIntoView({ behavior: 'smooth', block: 'start' })
}
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.op-hero {
  position: relative;
  width: 100%;
  min-height: max(100vh, 1120px);
  overflow: hidden;
  isolation: isolate;
}

/* ── Magnifier — top-left anchor (mirrors Home) ─────────────────────────── */
.op-hero__magnifier {
  top: clamp(14%, 16vw, 22%);
  left: clamp(6%, 8vw, 12%);
}

/* ── Content stack ──────────────────────────────────────────────────────── */
.op-hero__content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: max(100vh, 1120px);
  padding:
    clamp(80px, 12vw, 140px)
    clamp(20px, 3vw, 32px)
    clamp(120px, 16vw, 220px);
  text-align: center;
  max-width: 1100px;
  margin: 0 auto;
  font-family: var(--font-family);
}

.op-hero__eyebrow {
  margin-bottom: clamp(20px, 3vw, 28px);
}

/* ── Heading ────────────────────────────────────────────────────────────── */
.op-hero__heading {
  font-size: clamp(32px, 5.5vw, 60px);
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(20px, 3vw, 32px);
  max-width: 920px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.op-hero__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.op-hero__subtitle {
  font-size: clamp(14px, 1.4vw, 17px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(36px, 5vw, 56px);
  max-width: 680px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

/* ── Search form — glass V2 (replaces white floating card) ──────────────── */
.op-hero__search {
  position: relative;
  z-index: 2;
  display: grid;
  /* 2 inputs in one row on desktop, plus the submit button on the right. */
  grid-template-columns: minmax(0, 1.4fr) minmax(0, 1.2fr) auto;
  gap: clamp(12px, 1.4vw, 16px);
  width: 100%;
  max-width: 1080px;
  padding: clamp(20px, 2.2vw, 28px);
  background: linear-gradient(180deg,
    rgba(255, 255, 255, 0.10) 0%,
    rgba(255, 255, 255, 0.04) 100%);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.22);
  border-radius:
    clamp(20px, 2.5vw, 28px) clamp(20px, 2.5vw, 28px)
    clamp(20px, 2.5vw, 28px) clamp(40px, 5vw, 56px);
  box-shadow: 0 20px 48px rgba(0, 0, 0, 0.30);
  margin-bottom: clamp(36px, 4.8vw, 56px);
  text-align: left;
}

.op-hero__field {
  display: flex;
  flex-direction: column;
  gap: 6px;
  min-width: 0;
}

.op-hero__label {
  font-size: clamp(10px, 0.85vw, 11px);
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
}

.op-hero__input {
  width: 100%;
  height: clamp(44px, 4.4vw, 50px);
  padding: 0 14px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.22);
  border-radius: 12px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 500;
  outline: none;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    box-shadow 0.25s ease;
}

.op-hero__input::placeholder {
  color: rgba(255, 255, 255, 0.45);
}

.op-hero__input:hover {
  background: rgba(255, 255, 255, 0.10);
  border-color: rgba(255, 255, 255, 0.40);
}

.op-hero__input:focus {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 3px rgba(0, 173, 239, 0.20);
}

/* ── Submit button — red pill matching Figma intent ─────────────────────── */
.op-hero__submit {
  /* Sits below the labels but at input height — align to end of the grid */
  align-self: end;
  height: clamp(44px, 4.4vw, 50px);
  padding: 0 clamp(22px, 2.4vw, 28px);
  background: var(--c-brand-red);
  border: 1px solid var(--c-brand-red);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 700;
  letter-spacing: 0.04em;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  white-space: nowrap;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    transform 0.25s ease,
    box-shadow 0.25s ease;
}

.op-hero__submit:hover {
  background: #c92622;
  border-color: #c92622;
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(229, 45, 39, 0.35);
}

.op-hero__submit-arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}

.op-hero__submit:hover .op-hero__submit-arrow {
  transform: translateX(3px);
}

/* ── Stats strip ────────────────────────────────────────────────────────── */
.op-hero__stats {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: clamp(14px, 1.8vw, 22px);
}

.op-hero__stat {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6px;
  padding: clamp(12px, 1.6vw, 18px) clamp(18px, 2.2vw, 26px);
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.18);
  border-radius: clamp(14px, 1.8vw, 18px);
  backdrop-filter: blur(10px) saturate(150%);
  -webkit-backdrop-filter: blur(10px) saturate(150%);
  min-width: clamp(130px, 13vw, 170px);
}

.op-hero__stat-value {
  font-size: clamp(22px, 2.8vw, 30px);
  font-weight: 700;
  line-height: 1;
  letter-spacing: -0.02em;
  color: var(--c-brand-cyan);
  text-shadow: 0 4px 16px rgba(0, 0, 0, 0.40);
}

.op-hero__stat-label {
  font-size: clamp(10px, 0.9vw, 11px);
  font-weight: 600;
  letter-spacing: 0.16em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
  text-align: center;
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .op-hero {
    min-height: 100svh;
  }

  .op-hero__content {
    min-height: 100svh;
  }

  /* Stack form inputs 2x2, submit takes a row of its own */
  .op-hero__search {
    grid-template-columns: 1fr 1fr;
  }

  .op-hero__submit {
    grid-column: 1 / -1;
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 599px) {
  /* Single column on phones */
  .op-hero__search { grid-template-columns: 1fr; }

  .op-hero__stats {
    flex-direction: column;
    width: 100%;
    max-width: 280px;
    gap: 10px;
  }

  .op-hero__stat {
    flex-direction: row;
    justify-content: space-between;
    width: 100%;
    min-width: 0;
  }

  .op-hero__stat-label { text-align: right; }
}
</style>
