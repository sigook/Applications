<template>
  <article class="job-detail">
    <header class="job-detail__head">
      <h3 class="job-detail__title">{{ job.title }}</h3>

      <div class="job-detail__meta">
        <span class="job-detail__meta-item job-detail__meta-item--location">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0z" />
            <circle cx="12" cy="10" r="3" />
          </svg>
          {{ job.location }}
        </span>

        <span v-if="showSalary" class="job-detail__meta-item job-detail__meta-item--salary">
          {{ job.salary }}
        </span>

        <span v-if="job.type" class="job-detail__chip">
          {{ job.type }}
        </span>
      </div>

      <button
        type="button"
        class="job-detail__apply"
        :aria-label="`Apply for ${job.title}`"
        @click="onApply"
      >
        <span>Apply for this role</span>
        <span class="job-detail__apply-arrow" aria-hidden="true">→</span>
      </button>

    </header>

    <section v-if="job.description" class="job-detail__section">
      <h4 class="job-detail__section-title">Description</h4>
      <div class="job-detail__html" v-html="job.description"></div>
    </section>

    <section v-if="job.shift" class="job-detail__section">
      <h4 class="job-detail__section-title">Schedule</h4>
      <p class="job-detail__text">{{ job.shift }}</p>
    </section>

    <section v-if="job.responsibilities" class="job-detail__section">
      <h4 class="job-detail__section-title">Responsibilities</h4>
      <div class="job-detail__html" v-html="job.responsibilities"></div>
    </section>

    <section v-if="job.requirements" class="job-detail__section">
      <h4 class="job-detail__section-title">Requirements</h4>
      <div class="job-detail__html" v-html="job.requirements"></div>
    </section>

    <footer class="job-detail__footer">
      <span class="job-detail__posted">
        <template v-if="formattedPosted">Posted {{ formattedPosted }} · </template>ref {{ job.numberId }}
      </span>
    </footer>
  </article>
</template>

<script setup lang="ts">
/**
 * JobDetail — the right-column / inline detail panel for a single job.
 *
 * Consumes a JobViewModel from the live API (`@/types/website`). Owns no
 * state — purely presentational. Used by OpenPositionsListSection in two modes:
 *   - Desktop: sticky right column
 *   - Mobile : inline expanded under the active list item
 */
import { computed } from 'vue'
import type { JobViewModel } from '@/types/website'
import { useWorkerRegisterModal } from '@/components/v2/landing/shared/forms/useWorkerRegisterModal'

const props = defineProps<{
  job: JobViewModel
}>()

const registerModal = useWorkerRegisterModal()

function onApply(): void {
  registerModal.open({
    jobTitle: props.job.title,
    jobNumber: props.job.numberId,
    requestId: props.job.requestId,
  })
}

const showSalary = computed(() => !!props.job.salary && props.job.salary !== '$0.00')

const formattedPosted = computed(() => {
  const date = new Date(props.job.createdAt)
  if (Number.isNaN(date.getTime())) return ''
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
})
</script>

<style scoped>
/* ── Shell ──────────────────────────────────────────────────────────────── */
.job-detail {
  --jd-fg: rgba(255, 255, 255, 0.82);
  --jd-fg-strong: #fff;
  --jd-fg-muted: rgba(255, 255, 255, 0.5);
  --jd-divider: rgba(255, 255, 255, 0.1);

  position: relative;
  padding: clamp(26px, 3.2vw, 40px) clamp(24px, 3vw, 40px);
  background: linear-gradient(180deg,
    rgba(255, 255, 255, 0.09) 0%,
    rgba(255, 255, 255, 0.03) 100%);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.18);
  border-radius:
    clamp(20px, 2.4vw, 28px) clamp(20px, 2.4vw, 28px)
    clamp(20px, 2.4vw, 28px) clamp(40px, 5vw, 56px);
  color: var(--jd-fg);
  font-family: var(--font-family);
  display: flex;
  flex-direction: column;
  gap: clamp(16px, 2vw, 24px);
  isolation: isolate;
  overflow: hidden;
}

/* Decorative bottom-left glow */
.job-detail::before {
  content: '';
  position: absolute;
  pointer-events: none;
  bottom: 0;
  left: 0;
  width: clamp(160px, 18vw, 220px);
  height: clamp(160px, 18vw, 220px);
  z-index: 0;
  border-radius: 0 clamp(40px, 5vw, 56px) 0 0;
  background: radial-gradient(circle at bottom left,
    rgba(0, 173, 239, 0.26) 0%,
    rgba(0, 173, 239, 0.08) 45%,
    transparent 72%);
}

/* ── Header — title + meta + apply ──────────────────────────────────────── */
.job-detail__head {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  gap: clamp(12px, 1.5vw, 18px);
}

.job-detail__title {
  font-size: clamp(22px, 2.6vw, 30px);
  font-weight: 700;
  line-height: 1.18;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0;
  text-shadow: 0 4px 18px rgba(0, 0, 0, 0.35);
}

.job-detail__meta {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: clamp(8px, 1.2vw, 14px);
  font-size: clamp(12px, 1.05vw, 14px);
}

.job-detail__meta-item {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  color: var(--jd-fg);
  font-weight: 500;
}

.job-detail__meta-item--salary {
  color: var(--c-brand-cyan);
  font-weight: 700;
  letter-spacing: 0.01em;
}

.job-detail__meta-item svg {
  width: 14px;
  height: 14px;
}

/* Type chip */
.job-detail__chip {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 999px;
  font-size: clamp(10px, 0.9vw, 11px);
  font-weight: 700;
  letter-spacing: 0.16em;
  text-transform: uppercase;
  background: rgba(0, 173, 239, 0.14);
  border: 1px solid rgba(0, 173, 239, 0.35);
  color: var(--c-brand-cyan);
}

/* ── Apply CTA — solid red pill (matches Figma) ─────────────────────────── */
.job-detail__apply {
  display: inline-flex;
  align-self: flex-start;
  align-items: center;
  gap: 10px;
  margin-top: clamp(8px, 1vw, 12px);
  padding: clamp(12px, 1.3vw, 14px) clamp(24px, 2.6vw, 32px);
  background: var(--c-brand-red);
  border: 1px solid var(--c-brand-red);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 700;
  letter-spacing: 0.04em;
  text-decoration: none;
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    transform 0.25s ease,
    box-shadow 0.25s ease;
}

.job-detail__apply:hover {
  background: #c92622;
  border-color: #c92622;
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(229, 45, 39, 0.35);
}

.job-detail__apply-arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}

.job-detail__apply:hover .job-detail__apply-arrow {
  transform: translateX(3px);
}

/* ── Sections — Description / Schedule / Responsibilities / Requirements ─── */
.job-detail__section {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  gap: clamp(10px, 1.2vw, 14px);
  padding-top: clamp(16px, 2vw, 22px);
  border-top: 1px solid var(--jd-divider);
}

.job-detail__section-title {
  font-size: clamp(11px, 0.95vw, 12px);
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin: 0;
}

.job-detail__text,
.job-detail__html {
  margin: 0;
  font-size: clamp(13px, 1.1vw, 14px);
  line-height: 1.65;
  color: var(--jd-fg);
}

.job-detail__html :deep(p),
.job-detail__html :deep(span),
.job-detail__html :deep(li),
.job-detail__html :deep(div),
.job-detail__html :deep(em),
.job-detail__html :deep(u) {
  color: var(--jd-fg) !important;
  background: transparent !important;
}

.job-detail__html :deep(strong),
.job-detail__html :deep(b),
.job-detail__html :deep(h1),
.job-detail__html :deep(h2),
.job-detail__html :deep(h3),
.job-detail__html :deep(h4),
.job-detail__html :deep(h5),
.job-detail__html :deep(h6) {
  color: var(--jd-fg-strong) !important;
  font-weight: 700;
}

.job-detail__html :deep(a) {
  color: var(--c-brand-cyan) !important;
  text-decoration: underline;
  text-underline-offset: 2px;
}

.job-detail__html :deep(h1),
.job-detail__html :deep(h2),
.job-detail__html :deep(h3),
.job-detail__html :deep(h4),
.job-detail__html :deep(h5),
.job-detail__html :deep(h6) {
  font-size: clamp(14px, 1.2vw, 16px);
  line-height: 1.4;
  margin: 2px 0;
}

.job-detail__html :deep(p) {
  margin: 0 0 8px;
}

.job-detail__html :deep(p:last-child) {
  margin-bottom: 0;
}

.job-detail__html :deep(p:empty),
.job-detail__html :deep(br + br) {
  display: none;
}

.job-detail__html :deep(ul),
.job-detail__html :deep(ol) {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.job-detail__html :deep(li) {
  position: relative;
  padding-left: 18px;
  line-height: 1.55;
}

.job-detail__html :deep(li)::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0.6em;
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: var(--c-brand-cyan);
}

/* ── Footer — posted date + ref ─────────────────────────────────────────── */
.job-detail__footer {
  position: relative;
  z-index: 1;
  padding-top: clamp(14px, 1.8vw, 18px);
  border-top: 1px solid var(--jd-divider);
}

.job-detail__posted {
  font-size: clamp(11px, 0.95vw, 12px);
  color: var(--jd-fg-muted);
  font-weight: 500;
  letter-spacing: 0.04em;
}
</style>
