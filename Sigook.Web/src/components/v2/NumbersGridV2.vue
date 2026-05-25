<template>
  <div ref="gridRef" class="numbers-grid-v2" :class="{ 'is-visible': visible }">
    <article
      v-for="(stat, idx) in stats"
      :key="stat.label"
      class="numbers-grid-v2__stat"
      :class="[
        `numbers-grid-v2__stat--${stat.tone}`,
        `numbers-grid-v2__stat--slot-${idx}`,
      ]"
    >
      <span class="numbers-grid-v2__num">
        <span class="numbers-grid-v2__plus" aria-hidden="true">+</span>{{ stat.value }}
      </span>

      <span class="numbers-grid-v2__lbl">{{ stat.label }}</span>
    </article>
  </div>
</template>

<script lang="ts">
/**
 * Shared types for NumbersGridV2 — exported so parents can tightly type their
 * stat arrays. Tones match the canonical Home WhyChooseUs / Numbers feature
 * card variants.
 */
export type StatTone = 'blue' | 'cyan' | 'red'

export interface Stat {
  readonly value: string
  readonly label: string
  readonly tone: StatTone
}
</script>

<script setup lang="ts">
/**
 * NumbersGridV2 — stat cards that share the Home page's canonical card
 * vocabulary: triple gradient background, radial corner glow, asymmetric
 * brand radius. Renders the brand duality (cyan / blue / red) the same way
 * Home does, just with stat content instead of feature copy.
 *
 * Data lives in the parent — pass an array of Stat via the `stats` prop.
 * The component renders as many cards as items provided; staggered fade-in
 * uses an IntersectionObserver on the grid wrapper.
 */
import { ref, onMounted, onUnmounted } from 'vue'

defineProps<{
  stats: readonly Stat[]
}>()

const gridRef = ref<HTMLElement | null>(null)
const visible = ref(false)
let observer: IntersectionObserver | null = null

onMounted(() => {
  if (!gridRef.value) return
  observer = new IntersectionObserver(
    (entries) => { visible.value = entries[0].isIntersecting },
    { threshold: 0.15, rootMargin: '0px 0px -10% 0px' }
  )
  observer.observe(gridRef.value)
})

onUnmounted(() => observer?.disconnect())
</script>

<style scoped>
/* ── Grid shell ─────────────────────────────────────────────────────────── */
.numbers-grid-v2 {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: clamp(16px, 1.8vw, 28px);
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
  font-family: var(--font-family);
}

/* ── Stat card shell — same glass + shadow pattern as Home Numbers ──────── */
.numbers-grid-v2__stat {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: clamp(8px, 1vw, 14px);
  padding:
    clamp(32px, 4vw, 56px)
    clamp(20px, 2.5vw, 32px);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.22);
  box-shadow: 0 18px 44px rgba(0, 0, 0, 0.30);
  isolation: isolate;
  overflow: hidden;
  text-align: center;

  /* Entrance fade with staggered delays — matches Home Numbers behavior */
  opacity: 0;
  transform: translateY(40px) scale(0.92);
  transition:
    opacity 0.65s ease-out,
    transform 0.65s cubic-bezier(0.22, 1, 0.36, 1);
}

.numbers-grid-v2.is-visible .numbers-grid-v2__stat { opacity: 1; transform: translateY(0) scale(1); }
.numbers-grid-v2.is-visible .numbers-grid-v2__stat--slot-0 { transition-delay: 0.10s; }
.numbers-grid-v2.is-visible .numbers-grid-v2__stat--slot-1 { transition-delay: 0.22s; }
.numbers-grid-v2.is-visible .numbers-grid-v2__stat--slot-2 { transition-delay: 0.34s; }
.numbers-grid-v2.is-visible .numbers-grid-v2__stat--slot-3 { transition-delay: 0.46s; }
.numbers-grid-v2.is-visible .numbers-grid-v2__stat--slot-4 { transition-delay: 0.58s; }
.numbers-grid-v2.is-visible .numbers-grid-v2__stat--slot-5 { transition-delay: 0.70s; }

/* Radial corner glow — placed at the larger asymmetric corner */
.numbers-grid-v2__stat::before {
  content: '';
  position: absolute;
  pointer-events: none;
  width: clamp(120px, 14vw, 200px);
  height: clamp(120px, 14vw, 200px);
  z-index: 0;
}

/* ── Variant: blue (blue dominant + red accent on top-right) ────────────── */
.numbers-grid-v2__stat--blue {
  background: linear-gradient(135deg,
    rgba(21, 117, 187, 0.42) 0%,
    rgba(21, 117, 187, 0.14) 50%,
    rgba(229, 45, 39, 0.32) 100%);
  border-radius:
    clamp(24px, 3vw, 36px) clamp(48px, 6vw, 80px)
    clamp(24px, 3vw, 36px) clamp(48px, 6vw, 80px);
}

.numbers-grid-v2__stat--blue::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(229, 45, 39, 0.55) 0%,
    rgba(229, 45, 39, 0.18) 35%,
    transparent 70%);
  border-radius: 0 clamp(48px, 6vw, 80px) 0 0;
}

/* ── Variant: cyan (triple cyan → blue → red gradient) ──────────────────── */
.numbers-grid-v2__stat--cyan {
  background: linear-gradient(135deg,
    rgba(0, 173, 239, 0.34) 0%,
    rgba(21, 117, 187, 0.22) 50%,
    rgba(229, 45, 39, 0.30) 100%);
  border-radius:
    clamp(48px, 6vw, 80px) clamp(24px, 3vw, 36px)
    clamp(48px, 6vw, 80px) clamp(24px, 3vw, 36px);
}

.numbers-grid-v2__stat--cyan::before {
  top: 0;
  left: 0;
  background: radial-gradient(circle at top left,
    rgba(0, 173, 239, 0.55) 0%,
    rgba(0, 173, 239, 0.18) 35%,
    transparent 70%);
  border-radius: clamp(48px, 6vw, 80px) 0 0 0;
}

/* ── Variant: red (red dominant + blue accent on top-right) ─────────────── */
.numbers-grid-v2__stat--red {
  background: linear-gradient(135deg,
    rgba(229, 45, 39, 0.42) 0%,
    rgba(229, 45, 39, 0.14) 50%,
    rgba(21, 117, 187, 0.32) 100%);
  border-radius:
    clamp(24px, 3vw, 36px) clamp(48px, 6vw, 80px)
    clamp(24px, 3vw, 36px) clamp(48px, 6vw, 80px);
}

.numbers-grid-v2__stat--red::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(21, 117, 187, 0.60) 0%,
    rgba(21, 117, 187, 0.20) 35%,
    transparent 70%);
  border-radius: 0 clamp(48px, 6vw, 80px) 0 0;
}

/* ── Number ─────────────────────────────────────────────────────────────── */
.numbers-grid-v2__num {
  position: relative;
  z-index: 1;
  font-size: clamp(36px, 4.5vw, 60px);
  font-weight: 700;
  line-height: 1;
  letter-spacing: -0.02em;
  color: #fff;
  white-space: nowrap;
  text-shadow: 0 4px 18px rgba(0, 0, 0, 0.30);
}

.numbers-grid-v2__plus {
  display: inline-block;
  margin-right: clamp(2px, 0.2vw, 4px);
  font-weight: 700;
}

/* ── Label ──────────────────────────────────────────────────────────────── */
.numbers-grid-v2__lbl {
  position: relative;
  z-index: 1;
  font-size: clamp(11px, 0.95vw, 13px);
  font-weight: 600;
  letter-spacing: 0.10em;
  text-transform: uppercase;
  line-height: 1.4;
  color: rgba(255, 255, 255, 0.86);
}

/* ── Mobile-only behaviors (grid column shifts) ─────────────────────────── */
@media (max-width: 899px) {
  .numbers-grid-v2 { grid-template-columns: 1fr 1fr; }
}

@media (max-width: 480px) {
  .numbers-grid-v2 { grid-template-columns: 1fr; }
}
</style>
