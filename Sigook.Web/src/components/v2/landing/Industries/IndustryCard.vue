<template>
  <article
    class="industry-card"
    :class="[
      `industry-card--${industry.tone}`,
      { 'industry-card--expanded': expanded },
    ]"
  >
    <!-- Layer 0 — solid brand gradient (always present; revealed when expanded) -->
    <div class="industry-card__base" aria-hidden="true"></div>

    <!-- Layer 1 — photo cover (fades out when expanded) -->
    <div
      class="industry-card__photo"
      :style="photoStyle"
      aria-hidden="true"
    ></div>

    <!-- Layer 2 — navy overlay on top of photo (fades out with photo) -->
    <div class="industry-card__overlay" aria-hidden="true"></div>

    <!-- Ghost numeral 01-11 -->
    <span class="industry-card__index" aria-hidden="true">
      {{ formatIndex(index) }}
    </span>

    <!-- Content -->
    <div class="industry-card__content">
      <IndustryIcon :name="industry.key" class="industry-card__icon" />

      <h3 class="industry-card__title">{{ industry.title }}</h3>

      <!-- Tagline (collapsed copy) — fades when expanded -->
      <p class="industry-card__tagline">{{ industry.tagline }}</p>

      <!-- Expanded body — fades in, max-height transitions -->
      <div class="industry-card__expanded">
        <p class="industry-card__body">{{ industry.body }}</p>

        <span class="industry-card__positions-label">Common roles</span>
        <ul class="industry-card__positions">
          <li
            v-for="position in industry.positions"
            :key="position"
          >{{ position }}</li>
        </ul>
      </div>

      <button
        class="industry-card__toggle"
        type="button"
        :aria-expanded="expanded"
        @click="$emit('toggle')"
      >
        <span>{{ expanded ? 'Show less' : 'View details' }}</span>
        <span class="industry-card__toggle-icon" aria-hidden="true">{{
          expanded ? '−' : '+'
        }}</span>
      </button>
    </div>
  </article>
</template>

<script lang="ts">
import type { IndustryIconName } from '@/components/v2/landing/shared/IndustryIcon.vue'

/**
 * Tone names match PrimaryCard / DualCta audience section — keeps the
 * Industries page anchored to the same color vocabulary as the Home page's
 * "Find Work / Find Talent" cards.
 */
export type IndustryTone = 'navy' | 'red'

/**
 * Industry data shape — exported so the parent panel can tightly type its
 * data array. Photo path is optional; cards gracefully degrade to a pure
 * brand gradient if the image is missing.
 */
export interface Industry {
  readonly key: IndustryIconName
  readonly title: string
  readonly tagline: string
  readonly body: string
  readonly positions: readonly string[]
  readonly photo?: string
  readonly tone: IndustryTone
}
</script>

<script setup lang="ts">
/**
 * IndustryCard — expandable card that transitions from "photo + gradient
 * overlay" (collapsed) to "solid brand gradient + detailed info" (expanded).
 *
 * The parent panel owns the expanded state (accordion behavior: only one
 * card open at a time) and toggles via the `toggle` emit.
 *
 * Visual rhythm:
 *  • Asymmetric brand radius alternates per index (handled here, not parent)
 *  • Ghost numeral in the background echoes the magazine-style indices
 *    used in WhyWorkWithUs and FocusAreas panels
 *  • Cyan / red tone drives both the gradient and the icon's accent
 */
import { computed } from 'vue'
import IndustryIcon from '@/components/v2/landing/shared/IndustryIcon.vue'

const props = defineProps<{
  industry: Industry
  expanded: boolean
  index: number
}>()

defineEmits<{
  toggle: []
}>()

const photoStyle = computed(() => ({
  backgroundImage: props.industry.photo
    ? `url(${props.industry.photo})`
    : 'none',
}))

function formatIndex(idx: number): string {
  return String(idx + 1).padStart(2, '0')
}
</script>

<style scoped>
/* ── Card shell ─────────────────────────────────────────────────────────── */
.industry-card {
  position: relative;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  border: 1px solid rgba(255, 255, 255, 0.10);
  isolation: isolate;
  min-height: clamp(380px, 38vw, 460px);
  cursor: pointer;
  transition:
    border-color 0.4s ease,
    transform 0.4s cubic-bezier(0.22, 1, 0.36, 1);
}

.industry-card:hover {
  border-color: rgba(255, 255, 255, 0.22);
  transform: translateY(-4px);
}

/* Alternating asymmetric brand radius — odd cards: TL+BR, even: TR+BL */
.industry-card:nth-child(odd) {
  border-radius:
    clamp(40px, 5.5vw, 72px) 0
    clamp(40px, 5.5vw, 72px) 0;
}

.industry-card:nth-child(even) {
  border-radius:
    0 clamp(40px, 5.5vw, 72px)
    0 clamp(40px, 5.5vw, 72px);
}

/* ── Layer 0 — solid brand gradient base (always present) ───────────────── */
.industry-card__base {
  position: absolute;
  inset: 0;
  z-index: 0;
}

/* Gradient backgrounds match the DualCta (Home audience section) so the
   Industries page and the Home audience cards share the same color story. */
.industry-card--navy .industry-card__base {
  background: linear-gradient(
    135deg,
    rgba(15, 47, 68, 0.95) 0%,
    rgba(21, 117, 187, 0.75) 100%
  );
}

.industry-card--red .industry-card__base {
  background: linear-gradient(
    135deg,
    rgba(229, 45, 39, 0.92) 0%,
    rgba(229, 45, 39, 0.70) 100%
  );
}

/* ── Layer 1 — photo cover (visible when collapsed) ─────────────────────── */
.industry-card__photo {
  position: absolute;
  inset: 0;
  z-index: 1;
  background-size: cover;
  background-position: center;
  transition: opacity 0.55s cubic-bezier(0.22, 1, 0.36, 1);
}

.industry-card--expanded .industry-card__photo {
  opacity: 0;
}

/* ── Layer 2 — brand-tone overlay (visible when collapsed) ──────────────── */
/* Top-to-bottom gradient that matches the card's tone (navy or red) so the
   photo reads through the brand color instead of a generic dark veil. Goes
   from semi-transparent at the top (lets the photo breathe) to nearly opaque
   at the bottom (anchors the title + tagline). Fades out on expand to
   reveal the solid `.industry-card__base` gradient underneath. */
.industry-card__overlay {
  position: absolute;
  inset: 0;
  z-index: 2;
  transition: opacity 0.55s cubic-bezier(0.22, 1, 0.36, 1);
}

.industry-card--navy .industry-card__overlay {
  background: linear-gradient(
    180deg,
    rgba(15, 47, 68, 0.55) 0%,
    rgba(21, 117, 187, 0.65) 55%,
    rgba(15, 47, 68, 0.92) 100%
  );
}

.industry-card--red .industry-card__overlay {
  background: linear-gradient(
    180deg,
    rgba(229, 45, 39, 0.55) 0%,
    rgba(229, 45, 39, 0.75) 55%,
    rgba(176, 30, 26, 0.92) 100%
  );
}

.industry-card--expanded .industry-card__overlay {
  opacity: 0;
}

/* ── Ghost numeral ──────────────────────────────────────────────────────── */
.industry-card__index {
  position: absolute;
  top: clamp(-4px, -0.5vw, 4px);
  right: clamp(10px, 1.6vw, 22px);
  z-index: 3;
  font-family: var(--font-family);
  font-size: clamp(80px, 10vw, 140px);
  font-weight: 800;
  line-height: 0.85;
  letter-spacing: -0.04em;
  color: rgba(255, 255, 255, 0.10);
  pointer-events: none;
  user-select: none;
}

/* ── Content ────────────────────────────────────────────────────────────── */
.industry-card__content {
  position: relative;
  z-index: 4;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: clamp(12px, 1.4vw, 18px);
  padding: clamp(32px, 3.5vw, 48px) clamp(28px, 3vw, 40px);
  flex: 1;
  font-family: var(--font-family);
}

.industry-card__icon {
  font-size: clamp(40px, 4.5vw, 56px);
  color: #fff;
  filter: drop-shadow(0 4px 12px rgba(0, 0, 0, 0.25));
}

.industry-card__title {
  font-size: clamp(22px, 2.4vw, 30px);
  font-weight: 700;
  line-height: 1.2;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0;
  text-shadow: 0 2px 14px rgba(0, 0, 0, 0.40);
}

.industry-card__tagline {
  font-size: clamp(13px, 1.15vw, 15px);
  font-weight: 400;
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.82);
  margin: 0;
  max-height: 200px;
  opacity: 1;
  overflow: hidden;
  transition:
    opacity 0.3s ease,
    max-height 0.5s ease;
}

.industry-card--expanded .industry-card__tagline {
  opacity: 0;
  max-height: 0;
}

/* ── Expanded body ──────────────────────────────────────────────────────── */
.industry-card__expanded {
  width: 100%;
  max-height: 0;
  opacity: 0;
  overflow: hidden;
  transition:
    max-height 0.5s ease,
    opacity 0.3s ease 0.15s;
}

.industry-card--expanded .industry-card__expanded {
  max-height: 600px;
  opacity: 1;
}

.industry-card__body {
  font-size: clamp(13px, 1.15vw, 15px);
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.92);
  margin: 0 0 clamp(16px, 1.8vw, 22px);
}

.industry-card__positions-label {
  display: inline-block;
  font-size: clamp(10px, 0.8vw, 11px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.65);
  margin-bottom: clamp(8px, 1vw, 12px);
}

.industry-card__positions {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.industry-card__positions li {
  position: relative;
  padding-left: 16px;
  font-size: clamp(13px, 1.1vw, 14px);
  color: rgba(255, 255, 255, 0.88);
  line-height: 1.4;
}

.industry-card__positions li::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0.55em;
  width: 6px;
  height: 6px;
  background: rgba(255, 255, 255, 0.75);
  border-radius: 50%;
}

/* ── Toggle button ──────────────────────────────────────────────────────── */
.industry-card__toggle {
  margin-top: auto;
  align-self: flex-start;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: clamp(8px, 1vw, 11px) clamp(18px, 2vw, 24px);
  background: rgba(255, 255, 255, 0.10);
  border: 1px solid rgba(255, 255, 255, 0.35);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(12px, 1vw, 13px);
  font-weight: 600;
  letter-spacing: 0.04em;
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    transform 0.25s ease;
}

.industry-card__toggle:hover {
  background: rgba(255, 255, 255, 0.18);
  border-color: rgba(255, 255, 255, 0.55);
  transform: translateY(-1px);
}

.industry-card__toggle-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 16px;
  height: 16px;
  font-size: 16px;
  font-weight: 700;
  line-height: 1;
}
</style>
