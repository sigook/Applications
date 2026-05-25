<template>
  <section id="op-list" class="op-list">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="op-list__surface" aria-hidden="true"></div>

    <header class="op-list__header">
      <EyebrowPill variant="white" class="op-list__eyebrow">
        Open Roles
      </EyebrowPill>

      <h2 class="op-list__heading">
        {{ JOBS.length }} jobs hiring now.
        <span class="op-list__heading-accent">Skim, click, apply.</span>
      </h2>

      <p class="op-list__subtitle">
        The full list of roles we're actively recruiting for. Filter by track,
        search by title, then dig into the role on the right.
      </p>

      <!-- Toolbar — chip filters + local search ─────────────────────────── -->
      <div class="op-list__toolbar">
        <nav class="op-list__chips" aria-label="Filter by track">
          <button
            v-for="chip in JOB_TRACK_FILTERS"
            :key="chip.key"
            type="button"
            class="op-list__chip"
            :class="{ 'op-list__chip--active': activeTrack === chip.key }"
            @click="selectTrack(chip.key)"
          >
            {{ chip.label }}
          </button>
        </nav>

        <label class="op-list__search">
          <span class="op-list__search-icon" aria-hidden="true">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="11" cy="11" r="7" />
              <path d="m21 21-4.3-4.3" />
            </svg>
          </span>
          <input
            v-model="query"
            type="search"
            placeholder="Search by title…"
            aria-label="Search jobs by title"
            class="op-list__search-input"
          />
        </label>
      </div>
    </header>

    <!-- Body — master-detail grid (desktop) / accordion (mobile) ─────────── -->
    <div class="op-list__body" :class="{ 'op-list__body--empty': filtered.length === 0 }">
      <!-- Empty state -->
      <p v-if="filtered.length === 0" class="op-list__empty">
        No openings match your filters. Try a broader track or
        <button type="button" class="op-list__empty-reset" @click="resetFilters">reset all filters</button>.
      </p>

      <!-- Job list (left column on desktop, full width on mobile) -->
      <ol v-else class="op-list__items" aria-label="Open positions">
        <li
          v-for="(job, idx) in filtered"
          :key="job.id"
          class="op-list__item-wrap"
        >
          <button
            type="button"
            class="op-list__item"
            :class="{
              'op-list__item--active': job.id === selectedId,
              'op-list__item--expanded-mobile': job.id === selectedId,
            }"
            @click="selectJob(job.id, idx)"
            :aria-expanded="job.id === selectedId"
            :aria-controls="`op-detail-${job.id}`"
          >
            <div class="op-list__item-head">
              <h3 class="op-list__item-title">{{ job.title }}</h3>
              <span class="op-list__item-number">{{ job.jobNumber }}</span>
            </div>
            <div class="op-list__item-meta">
              <span class="op-list__item-location">{{ formatLocation(job.location) }}</span>
              <span class="op-list__item-dot" aria-hidden="true">·</span>
              <span class="op-list__item-salary">{{ formatSalary(job.salary) }}</span>
            </div>
          </button>

          <!-- Mobile inline detail — only renders when this item is the
               selected one. Desktop hides this (detail lives in the right
               column). -->
          <div
            v-if="job.id === selectedId"
            :id="`op-detail-${job.id}`"
            class="op-list__inline-detail"
          >
            <JobDetail :job="job" />
          </div>
        </li>
      </ol>

      <!-- Desktop sticky detail panel — visible at ≥1024px only -->
      <aside v-if="selectedJob" class="op-list__detail" aria-label="Job detail">
        <JobDetail :job="selectedJob" />
      </aside>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * OpenPositionsListSection — master-detail job-board layout.
 *
 * Desktop (≥1024px): two columns — sticky job list on the left, detail on
 * the right. Click a job → detail updates in place.
 *
 * Mobile (<1024px): the right-column detail is hidden by CSS; clicking a
 * job expands the detail INLINE beneath the row (accordion pattern). No
 * scroll position is lost.
 *
 * Filtering is local — chip filters (All / Professional / Industrial) plus
 * a free-text title search. Default selection: the first filtered job, so
 * the detail panel is never empty.
 */
import { ref, computed } from 'vue'
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'
import JobDetail from '@/components/v2/landing/OpenPositions/JobDetail.vue'
import {
  JOBS,
  JOB_TRACK_FILTERS,
  filterJobs,
  formatLocation,
  formatSalary,
  type JobTrack,
} from '@/data/jobs'

const activeTrack = ref<JobTrack | 'all'>('all')
const query = ref('')
const selectedId = ref<string>(JOBS[0].id)

const filtered = computed(() => filterJobs(JOBS, activeTrack.value, query.value))

const selectedJob = computed(() =>
  filtered.value.find((j) => j.id === selectedId.value) ?? filtered.value[0],
)

function selectTrack(key: JobTrack | 'all'): void {
  activeTrack.value = key
  // Keep selection valid — if the current job is filtered out, jump to
  // the first item in the new filtered list.
  if (filtered.value.length > 0 && !filtered.value.some((j) => j.id === selectedId.value)) {
    selectedId.value = filtered.value[0].id
  }
}

function selectJob(id: string, _idx: number): void {
  // Toggle on mobile — clicking the active item collapses it.
  if (window.matchMedia('(max-width: 1023px)').matches && selectedId.value === id) {
    // Find a different job to keep the right-column detail filled (it's
    // hidden on mobile anyway, but desktop wants a non-empty panel).
    selectedId.value = filtered.value.find((j) => j.id !== id)?.id ?? id
    return
  }
  selectedId.value = id
}

function resetFilters(): void {
  activeTrack.value = 'all'
  query.value = ''
  if (filtered.value.length > 0) selectedId.value = filtered.value[0].id
}
</script>

<style scoped>
/* ── Panel shell ────────────────────────────────────────────────────────── */
.op-list {
  position: relative;
  width: 100%;
  margin-top: clamp(-180px, -10vw, -80px);
  padding:
    clamp(140px, 14vw, 200px)
    clamp(20px, 3vw, 64px)
    clamp(100px, 12vw, 160px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(36px, 5vw, 56px);
  z-index: 5;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
  overflow: hidden;
  isolation: isolate;
  font-family: var(--font-family);
}

.op-list__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

/* ── Header ─────────────────────────────────────────────────────────────── */
.op-list__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 980px;
  width: 100%;
  margin: 0 auto;
  gap: clamp(18px, 2.4vw, 24px);
}

.op-list__eyebrow {
  margin-bottom: clamp(8px, 1vw, 12px);
}

.op-list__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0;
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.op-list__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.op-list__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 580px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Toolbar — chips + search ───────────────────────────────────────────── */
.op-list__toolbar {
  display: grid;
  grid-template-columns: 1fr auto;
  align-items: center;
  gap: clamp(14px, 2vw, 22px);
  width: 100%;
  margin-top: clamp(10px, 1.4vw, 16px);
}

.op-list__chips {
  display: flex;
  flex-wrap: wrap;
  gap: clamp(8px, 1vw, 12px);
}

.op-list__chip {
  font-size: clamp(12px, 1vw, 13px);
  font-weight: 600;
  letter-spacing: 0.04em;
  padding: clamp(8px, 0.9vw, 11px) clamp(16px, 1.7vw, 22px);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.20);
  color: rgba(255, 255, 255, 0.80);
  backdrop-filter: blur(10px) saturate(140%);
  -webkit-backdrop-filter: blur(10px) saturate(140%);
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease;
}

.op-list__chip:hover {
  background: rgba(255, 255, 255, 0.12);
  border-color: rgba(255, 255, 255, 0.35);
  color: #fff;
}

.op-list__chip--active {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  box-shadow: 0 6px 16px rgba(0, 173, 239, 0.30);
}

.op-list__chip--active:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
}

.op-list__search {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 0 14px;
  height: clamp(40px, 4vw, 46px);
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.20);
  border-radius: 999px;
  min-width: clamp(220px, 26vw, 300px);
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    box-shadow 0.25s ease;
}

.op-list__search:hover {
  background: rgba(255, 255, 255, 0.10);
  border-color: rgba(255, 255, 255, 0.35);
}

.op-list__search:focus-within {
  background: rgba(255, 255, 255, 0.12);
  border-color: var(--c-brand-cyan);
  box-shadow: 0 0 0 3px rgba(0, 173, 239, 0.20);
}

.op-list__search-icon {
  display: inline-flex;
  width: 16px;
  height: 16px;
  color: rgba(255, 255, 255, 0.55);
}

.op-list__search-icon svg {
  width: 100%;
  height: 100%;
}

.op-list__search-input {
  flex: 1;
  background: transparent;
  border: 0;
  outline: none;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(12px, 1vw, 13px);
  font-weight: 500;
  min-width: 0;
}

.op-list__search-input::placeholder {
  color: rgba(255, 255, 255, 0.50);
}

/* Remove the native search cancel button — we don't style it */
.op-list__search-input::-webkit-search-cancel-button { display: none; }

/* ── Body — master-detail layout ────────────────────────────────────────── */
.op-list__body {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: minmax(280px, 1fr) minmax(0, 1.5fr);
  gap: clamp(20px, 2.6vw, 32px);
  width: 100%;
  max-width: 1280px;
  margin: 0 auto;
  align-items: start;
}

.op-list__body--empty {
  grid-template-columns: 1fr;
  justify-items: center;
}

.op-list__empty {
  font-size: clamp(14px, 1.2vw, 16px);
  color: rgba(255, 255, 255, 0.78);
  text-align: center;
  margin: 0;
  padding: clamp(32px, 4vw, 48px);
}

.op-list__empty-reset {
  background: none;
  border: 0;
  color: var(--c-brand-cyan);
  font-weight: 700;
  cursor: pointer;
  text-decoration: underline;
  text-underline-offset: 2px;
  font: inherit;
  padding: 0;
}

/* ── Job items list ─────────────────────────────────────────────────────── */
.op-list__items {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: clamp(10px, 1.2vw, 14px);
  max-height: clamp(560px, 70vh, 760px);
  overflow-y: auto;
  padding-right: 6px;

  /* Scrollbar — subtle white */
  scrollbar-width: thin;
  scrollbar-color: rgba(255, 255, 255, 0.25) transparent;
}

.op-list__items::-webkit-scrollbar { width: 6px; }
.op-list__items::-webkit-scrollbar-track { background: transparent; }
.op-list__items::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.25);
  border-radius: 999px;
}

.op-list__item-wrap {
  display: flex;
  flex-direction: column;
}

.op-list__item {
  display: flex;
  flex-direction: column;
  gap: 6px;
  text-align: left;
  padding: clamp(14px, 1.6vw, 18px) clamp(16px, 1.8vw, 22px);
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.14);
  border-radius:
    clamp(14px, 1.6vw, 18px) clamp(14px, 1.6vw, 18px)
    clamp(14px, 1.6vw, 18px) clamp(28px, 3vw, 36px);
  color: #fff;
  cursor: pointer;
  width: 100%;
  font: inherit;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    transform 0.25s ease;
}

.op-list__item:hover {
  background: rgba(255, 255, 255, 0.10);
  border-color: rgba(255, 255, 255, 0.28);
  transform: translateX(2px);
}

.op-list__item--active {
  background: rgba(0, 173, 239, 0.18);
  border-color: rgba(0, 173, 239, 0.55);
  box-shadow: 0 8px 24px rgba(0, 173, 239, 0.25);
}

.op-list__item--active:hover {
  background: rgba(0, 173, 239, 0.22);
  border-color: var(--c-brand-cyan);
  transform: translateX(2px);
}

.op-list__item-head {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  gap: 12px;
}

.op-list__item-title {
  font-size: clamp(14px, 1.3vw, 16px);
  font-weight: 700;
  line-height: 1.25;
  margin: 0;
  color: #fff;
}

.op-list__item-number {
  font-size: clamp(11px, 0.95vw, 12px);
  font-weight: 500;
  color: rgba(255, 255, 255, 0.50);
  flex-shrink: 0;
}

.op-list__item-meta {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 6px;
  font-size: clamp(11px, 0.95vw, 12px);
  font-weight: 500;
  color: rgba(255, 255, 255, 0.70);
  letter-spacing: 0.02em;
}

.op-list__item-location {
  color: rgba(255, 255, 255, 0.85);
}

.op-list__item-dot {
  opacity: 0.6;
}

.op-list__item-salary {
  color: var(--c-brand-cyan);
  font-weight: 700;
}

.op-list__item--active .op-list__item-salary {
  color: #fff;
}

/* ── Detail (desktop right column) ──────────────────────────────────────── */
.op-list__detail {
  position: sticky;
  top: 90px;
  max-height: calc(100vh - 110px);
  overflow-y: auto;
  padding-right: 4px;
  scrollbar-width: thin;
  scrollbar-color: rgba(255, 255, 255, 0.25) transparent;
}

.op-list__detail::-webkit-scrollbar { width: 6px; }
.op-list__detail::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.25);
  border-radius: 999px;
}

/* ── Inline detail (mobile accordion) — hidden by default ───────────────── */
.op-list__inline-detail {
  display: none;
}

/* ── Responsive — switch to accordion under 1024px ──────────────────────── */
@media (max-width: 1023px) {
  .op-list__body {
    grid-template-columns: 1fr;
  }

  /* Hide the right-column detail; show inline detail under the active item */
  .op-list__detail {
    display: none;
  }

  .op-list__inline-detail {
    display: block;
    margin-top: 10px;
    animation: op-list-expand 0.35s ease;
  }

  /* Remove list scroll on mobile — the page scrolls naturally */
  .op-list__items {
    max-height: none;
    padding-right: 0;
  }

  /* Toolbar stacks: chips on top, search below */
  .op-list__toolbar {
    grid-template-columns: 1fr;
  }

  .op-list__chips {
    justify-content: center;
  }

  .op-list__search {
    width: 100%;
    min-width: 0;
  }
}

@keyframes op-list-expand {
  from { opacity: 0; transform: translateY(-6px); }
  to   { opacity: 1; transform: translateY(0); }
}

@media (prefers-reduced-motion: reduce) {
  .op-list__inline-detail { animation: none; }
}
</style>
