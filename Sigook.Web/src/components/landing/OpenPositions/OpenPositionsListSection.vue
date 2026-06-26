<template>
  <section id="op-list" class="op-list">

    <div class="op-list__body" :class="{ 'op-list__body--empty': !showList }">
      <p v-if="loading" class="op-list__empty">Loading open roles…</p>

      <p v-else-if="error" class="op-list__empty">
        {{ error }}
        <button type="button" class="op-list__empty-reset" @click="fetchJobs()">Retry</button>
      </p>

      <p v-else-if="filtered.length === 0" class="op-list__empty">
        No openings match your search.
        <button type="button" class="op-list__empty-reset" @click="resetFilters">Clear search</button>.
      </p>

      <ol v-else class="op-list__items" aria-label="Open positions">
        <li
          v-for="job in filtered"
          :key="job.numberId"
          class="op-list__item-wrap"
          :data-job-id="job.numberId"
        >
          <button
            type="button"
            class="op-list__item"
            :class="{
              'op-list__item--active': isActive(job.numberId),
              'op-list__item--expanded-mobile': isExpanded(job.numberId),
            }"
            @click="selectJob(job.numberId)"
            :aria-expanded="isExpanded(job.numberId)"
            :aria-controls="`op-detail-${job.numberId}`"
          >
            <div class="op-list__item-head">
              <h3 class="op-list__item-title">{{ job.title }}</h3>
              <span class="op-list__item-number">{{ job.numberId }}</span>
            </div>
            <div class="op-list__item-meta">
              <span class="op-list__item-location">{{ job.location }}</span>
              <template v-if="showSalary(job)">
                <span class="op-list__item-dot" aria-hidden="true">·</span>
                <span class="op-list__item-salary">{{ job.salary }}</span>
              </template>
            </div>
          </button>

          <div
            v-if="isExpanded(job.numberId)"
            :id="`op-detail-${job.numberId}`"
            class="op-list__inline-detail"
          >
            <JobDetail :job="job" />
          </div>
        </li>
      </ol>

      <aside v-if="showList && selectedJob" class="op-list__detail" aria-label="Job detail">
        <JobDetail :job="selectedJob" />
      </aside>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, nextTick, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import JobDetail from '@/components/landing/OpenPositions/JobDetail.vue'
import { useJobs } from '@/composables/useJobs'
import type { JobViewModel } from '@/types/website'

const route = useRoute()
const router = useRouter()

const { jobs, loading, error, fetchJobs } = useJobs()

const query = ref('')
const selectedId = ref<string>('')
const urlSyncReady = ref(false)

const expandedIds = ref<string[]>([])
const isMobile = ref(
  typeof window !== 'undefined' && window.matchMedia('(max-width: 1023px)').matches,
)

function isExpanded(numberId: string): boolean {
  return expandedIds.value.includes(numberId)
}

function isActive(numberId: string): boolean {
  return isMobile.value ? isExpanded(numberId) : selectedId.value === numberId
}

function readJobIdFromRoute(): string | undefined {
  const value = route.query.jobId
  if (typeof value === 'string') return value
  if (Array.isArray(value)) return typeof value[0] === 'string' ? value[0] : undefined
  return undefined
}

const filtered = computed(() => {
  const q = query.value.trim().toLowerCase()
  if (!q) return jobs.value
  return jobs.value.filter((j) => j.title.toLowerCase().includes(q))
})

const showList = computed(() => !loading.value && !error.value && filtered.value.length > 0)

const selectedJob = computed(() =>
  filtered.value.find((j) => j.numberId === selectedId.value) ?? filtered.value[0],
)

function showSalary(job: JobViewModel): boolean {
  return !!job.salary && job.salary !== '$0.00'
}

function toggleExpanded(numberId: string): void {
  if (expandedIds.value.includes(numberId)) {
    expandedIds.value = expandedIds.value.filter((id) => id !== numberId)
  } else {
    expandedIds.value = [...expandedIds.value, numberId]
    selectedId.value = numberId
  }
}

function selectJob(numberId: string): void {
  if (isMobile.value) {
    toggleExpanded(numberId)
    return
  }
  selectedId.value = numberId
}

function resetFilters(): void {
  query.value = ''
  if (filtered.value.length > 0) selectedId.value = filtered.value[0].numberId
}

watch(filtered, (list) => {
  if (list.length === 0) {
    selectedId.value = ''
  } else if (!list.some((j) => j.numberId === selectedId.value)) {
    selectedId.value = list[0].numberId
  }
  if (expandedIds.value.length > 0) {
    const present = new Set(list.map((j) => j.numberId))
    const next = expandedIds.value.filter((id) => present.has(id))
    if (next.length !== expandedIds.value.length) expandedIds.value = next
  }
})

watch(selectedId, (id) => {
  if (!urlSyncReady.value || !id) return
  if (readJobIdFromRoute() === id) return
  router.replace({ query: { ...route.query, jobId: id } })
})

watch(
  () => route.query.jobId,
  (value) => {
    const id = typeof value === 'string' ? value : Array.isArray(value) ? value[0] : undefined
    if (!id || id === selectedId.value) return
    if (jobs.value.some((j) => j.numberId === id)) selectedId.value = id
  },
)

async function scrollToSelected(numberId: string): Promise<void> {
  await nextTick()
  const el = document.querySelector(`[data-job-id="${numberId}"]`)
  if (el) el.scrollIntoView({ behavior: 'smooth', block: 'center' })
}

function updateIsMobile(): void {
  isMobile.value = window.matchMedia('(max-width: 1023px)').matches
}

let mobileMql: MediaQueryList | null = null

onMounted(async () => {
  mobileMql = window.matchMedia('(max-width: 1023px)')
  mobileMql.addEventListener('change', updateIsMobile)
  updateIsMobile()

  const initialJobId = readJobIdFromRoute()
  await fetchJobs()
  await nextTick()

  const matched = !!initialJobId && jobs.value.some((j) => j.numberId === initialJobId)
  if (matched) selectedId.value = initialJobId as string

  if (isMobile.value && selectedId.value) {
    expandedIds.value = [selectedId.value]
  }

  urlSyncReady.value = true

  if (selectedId.value && readJobIdFromRoute() !== selectedId.value) {
    router.replace({ query: { ...route.query, jobId: selectedId.value } })
  }

  if (matched) await scrollToSelected(selectedId.value)
})

onUnmounted(() => {
  mobileMql?.removeEventListener('change', updateIsMobile)
})
</script>

<style scoped>
.op-list {
  position: relative;
  width: 100%;
  margin-top: clamp(-360px, -33vh, -240px);
  padding:
    clamp(16px, 2vw, 28px)
    clamp(20px, 3vw, 64px)
    clamp(80px, 10vw, 130px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(36px, 5vw, 56px);
  z-index: 5;
  isolation: isolate;
  font-family: var(--font-family);
}

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
  min-height: clamp(420px, 52vh, 640px);
}

.op-list__body--empty {
  grid-template-columns: 1fr;
  justify-items: center;
  align-content: center;
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
  border: 1px solid var(--c-glass-border-soft);
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

.op-list__inline-detail {
  display: none;
}

@media (max-width: 1023px) {
  .op-list__body {
    grid-template-columns: 1fr;
  }

  .op-list__detail {
    display: none;
  }

  .op-list__inline-detail {
    display: block;
    margin-top: 10px;
    animation: op-list-expand 0.35s ease;
  }

  .op-list__items {
    max-height: none;
    padding-right: 0;
  }

  .op-list__search {
    width: 100%;
    min-width: 0;
  }
}

@media (max-width: 599px) {
  .op-list {
    margin-top: clamp(-190px, -22vh, -120px);
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
