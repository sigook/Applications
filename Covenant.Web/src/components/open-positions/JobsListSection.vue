<template>
  <section class="jobs-section" id="jobs-results">
    <div class="container">

      <!-- LOADING STATE with Vuetify -->
      <div v-if="loading" class="loading-container">
        <v-progress-circular
          indeterminate
          color="primary"
          :size="70"
          :width="7"
        ></v-progress-circular>
        <p>Loading jobs...</p>
      </div>

      <!-- ERROR STATE -->
      <div v-else-if="error" class="error-container">
        <p class="error-message">{{ error }}</p>
        <button @click="fetchJobs()" class="retry-button">Retry</button>
      </div>

      <!-- EMPTY STATE (no jobs available) -->
      <div v-else-if="jobs.length === 0" class="empty-state-container">
        <div class="empty-state-icon">📋</div>
        <h3 class="empty-state-title">No Jobs Available</h3>
        <p class="empty-state-message">
          There are currently no job openings matching your search criteria.
          Please try adjusting your filters or check back later.
        </p>
        <button @click="viewAllJobs()" class="retry-button">View All Jobs</button>
      </div>

      <!-- JOBS LAYOUT -->
      <div v-else class="jobs-layout">

        <div class="jobs-list-col">
          <div class="list-header">
            <span>{{ jobs.length }} Jobs Found</span>
          </div>

          <div class="scrollable-list">
            <div v-for="job in jobs" :key="job.numberId" class="job-item">
              <div
                class="job-card"
                :class="{ 'is-selected': isActive(job.numberId) }"
                :data-job-id="job.numberId"
                @click="selectJob(job)"
              >
                <h3 class="card-title">{{ job.title }}</h3>
                <p class="card-location">{{ job.location }}</p>

                <div class="card-footer">
                  <span class="salary-tag" v-if="job.salary && job.salary !== '$0.00'">{{ job.salary }}</span>
                  <span class="id-tag">#{{ job.numberId }}</span>
                </div>
              </div>

              <!-- Mobile: independent inline accordion under the tapped card. -->
              <div v-if="isMobile && isExpanded(job.numberId)" class="inline-detail">
                <JobDetailCard :job="job" @apply="handleApplyClick" />
              </div>
            </div>
          </div>
        </div>

        <!-- Desktop: single master-detail side panel. -->
        <div v-if="!isMobile" class="job-detail-col">
          <div v-if="selectedJob" class="detail-card">
            <JobDetailCard :job="selectedJob" @apply="handleApplyClick" />
          </div>

          <div v-else class="empty-state">
            <p>Select a job to view details</p>
          </div>
        </div>

      </div>
    </div>

    <!-- Apply Now Dialog -->
    <ApplyNowDialog
      v-model="showApplyDialog"
      :selected-job="jobToApply"
      @application-submitted="onApplicationSubmitted"
    />

    <!-- Success Snackbar -->
    <v-snackbar
      v-model="successSnackbar"
      color="success"
      location="top"
      :timeout="5000"
    >
      ✅ Application submitted successfully! We'll contact you soon.
    </v-snackbar>
  </section>
</template>

<script setup lang="ts">
import { ref, nextTick, onMounted, onUnmounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useJobs } from '@/composables/useJobs'
import ApplyNowDialog from '@/components/jobs/ApplyNowDialog.vue'
import JobDetailCard from '@/components/open-positions/JobDetailCard.vue'
import type { Job } from '@/services/types/job.types'

const route = useRoute()
const router = useRouter()

// Use jobs composable for API integration
const { jobs, loading, error, fetchJobs } = useJobs()

// Estado para el trabajo seleccionado (desktop master-detail panel)
const selectedJob = ref<Job | null>(null)

// Mobile independent accordion state (desktop uses selectedJob)
const expandedIds = ref<string[]>([])
const isMobile = ref(
  typeof window !== 'undefined' && window.matchMedia('(max-width: 900px)').matches,
)

const isExpanded = (numberId: string): boolean => expandedIds.value.includes(numberId)
const isActive = (numberId: string): boolean =>
  isMobile.value ? isExpanded(numberId) : selectedJob.value?.numberId === numberId

// Estado para el trabajo al que se está aplicando (para el modal)
const jobToApply = ref<Job | null>(null)

// Estado para el dialog y snackbar
const showApplyDialog = ref(false)
const successSnackbar = ref(false)

// Mobile: independent toggle; desktop: single selection. URL stays shareable.
const selectJob = (job: Job) => {
  if (isMobile.value) {
    if (expandedIds.value.includes(job.numberId)) {
      expandedIds.value = expandedIds.value.filter((id) => id !== job.numberId)
      return
    }
    expandedIds.value = [...expandedIds.value, job.numberId]
  }

  selectedJob.value = job
  router.replace({ query: { ...route.query, jobId: job.numberId } })
}

// Función para ver todos los trabajos (limpia filtros y query)
const viewAllJobs = () => {
  // Limpiar querystring
  router.replace({ query: {} })

  // Cargar todos los trabajos sin filtros
  fetchJobs()
}

// Función para abrir el dialog de Apply para un trabajo específico
const handleApplyClick = (job: Job) => {
  jobToApply.value = job
  showApplyDialog.value = true
}

// Handler cuando se completa la aplicación
const onApplicationSubmitted = () => {
  showApplyDialog.value = false
  jobToApply.value = null // Limpiar el job al cerrar
  successSnackbar.value = true
}

// Scroll hacia el job card seleccionado dentro de la lista
const scrollToSelectedJob = async (jobNumberId: string) => {
  await nextTick()
  const jobCard = document.querySelector(`.job-card[data-job-id="${jobNumberId}"]`) as HTMLElement | null
  if (jobCard) {
    // Scroll de la lista interna para centrar la card
    jobCard.scrollIntoView({ behavior: 'smooth', block: 'center' })

    // Scroll de la página para centrar la sección de jobs
    const jobsSection = document.getElementById('jobs-results')
    if (jobsSection) {
      jobsSection.scrollIntoView({ behavior: 'smooth', block: 'start' })
    }
  }
}

const updateIsMobile = (): void => {
  isMobile.value = window.matchMedia('(max-width: 900px)').matches
}
let mobileMql: MediaQueryList | null = null

// Skip the initial results watch so a deep-link selection survives mount.
let ready = false

// Al montar el componente, cargamos trabajos desde la API
onMounted(async () => {
  mobileMql = window.matchMedia('(max-width: 900px)')
  mobileMql.addEventListener('change', updateIsMobile)
  updateIsMobile()

  // Obtenemos filtros de la URL (incluyendo jobId si existe)
  const filters = {
    jobId: route.query.jobId as string | undefined,
    jobTitle: route.query.jobTitle as string | undefined,
    location: route.query.location as string | undefined
  }

  await fetchJobs(filters)

  // Select the deep-linked job when its jobId matches; otherwise default to the
  // first job. Either way, keep the URL's jobId in sync so it stays shareable.
  if (jobs.value.length > 0) {
    const jobFromUrl = filters.jobId
      ? jobs.value.find(j => j.numberId === filters.jobId)
      : undefined
    const initialJob = jobFromUrl ?? jobs.value[0]

    selectedJob.value = initialJob
    if (isMobile.value) expandedIds.value = [initialJob.numberId]
    if (route.query.jobId !== initialJob.numberId) {
      router.replace({ query: { ...route.query, jobId: initialJob.numberId } })
    }
    if (jobFromUrl) scrollToSelectedJob(initialJob.numberId)
  }

  await nextTick()
  ready = true
})

onUnmounted(() => {
  mobileMql?.removeEventListener('change', updateIsMobile)
})

// On new search results, default to the first job and sync the URL's jobId.
watch(jobs, (newJobs) => {
  if (!ready) return
  if (newJobs.length > 0) {
    const first = newJobs[0]
    selectedJob.value = first
    if (isMobile.value) expandedIds.value = [first.numberId]
    if (route.query.jobId !== first.numberId) {
      router.replace({ query: { ...route.query, jobId: first.numberId } })
    }
  } else {
    selectedJob.value = null
  }
  const present = new Set(newJobs.map((j) => j.numberId))
  expandedIds.value = expandedIds.value.filter((id) => present.has(id))
})

// Limpiar jobToApply cuando se cierra el dialog
watch(showApplyDialog, (newValue) => {
  if (!newValue) {
    jobToApply.value = null
  }
})
</script>

<style scoped>
*{
  box-sizing: border-box;
}

.jobs-section {
  padding: 60px 0 100px;
  background-color: #f9f9f9;
  border-radius: 0 0 100px 100px;
}

.jobs-layout {
  display: flex;
  gap: 20px; /* Reducido de 30px */
  align-items: flex-start;
  padding: 0 60px; /* Espacio lateral en desktop */
}

/* === COLUMNA IZQUIERDA (LISTA) === */
.jobs-list-col {
  width: 25%; /* Reducido de 35% */
  flex-shrink: 0;
  margin: 0; /* Margen eliminado para reducir espacio */
}

.list-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 20px;
  font-size: 0.9rem;
  color: #666;
  font-weight: 600;
}

.sort-link {
  color: #32d26a;
  cursor: pointer;
}

.scrollable-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
  /* Altura fija con scroll para la lista, así el detalle se mantiene visible */
  height: 800px;
  overflow-y: auto;
  padding-right: 5px;
}

/* Estilos de la Tarjeta Pequeña */
.job-card {
  background: white;
  padding: 20px;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 1px solid transparent;
  box-shadow: 0 2px 5px rgba(0,0,0,0.05);
}

.job-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0,0,0,0.1);
}

.job-card.is-selected {
  border-color: #32d26a;
  background-color: #05162d;
}

.job-card.is-selected .card-title,
.job-card.is-selected .card-location,
.job-card.is-selected .salary-tag,
.job-card.is-selected .id-tag {
  color: white;
}

.card-title {
  font-size: 1rem;
  font-weight: 700;
  color: #05162d;
  margin-bottom: 5px;
}

.card-location {
  font-size: 0.85rem;
  color: #888;
  margin-bottom: 12px;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.8rem;
}

.salary-tag {
  color: #32d26a;
  font-weight: 700;
}

.id-tag {
  color: #ccc;
}

/* === COLUMNA DERECHA (DETALLE) === */
.job-detail-col {
  flex: 1;
  margin: 0; /* Margen eliminado */
}

.detail-card {
  background: #eaeaea;
  padding: 40px;
  border-radius: 12px;
  /* Sticky para que siga al usuario si la lista es muy larga */
  position: sticky;
  top: 100px;
  max-height: 800px;
  overflow-y: auto; /* Scroll interno también para el detalle si es muy largo */
}

.job-item {
  display: flex;
  flex-direction: column;
}

/* Mobile inline accordion detail */
.inline-detail {
  background: #eaeaea;
  border-radius: 12px;
  padding: 24px;
  margin-top: 12px;
}

/* LOADING AND ERROR STATES */
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 100px 20px;
  gap: 20px;
}

.loading-container p {
  font-size: 1.1rem;
  color: #666;
  font-weight: 600;
}

.error-container {
  text-align: center;
  padding: 100px 20px;
}

.error-message {
  color: #d32f2f;
  font-size: 1.1rem;
  margin-bottom: 20px;
  font-weight: 600;
}

.retry-button {
  background-color: #32d26a;
  color: white;
  border: none;
  padding: 12px 30px;
  border-radius: 999px;
  cursor: pointer;
  font-weight: 700;
  font-size: 1rem;
  transition: background-color 0.3s, transform 0.2s;
}

.retry-button:hover {
  background-color: #28a755;
  transform: translateY(-2px);
}

/* EMPTY STATE */
.empty-state-container {
  text-align: center;
  padding: 100px 20px;
  max-width: 500px;
  margin: 0 auto;
}

.empty-state-icon {
  font-size: 5rem;
  margin-bottom: 20px;
  opacity: 0.5;
}

.empty-state-title {
  font-size: 1.8rem;
  font-weight: 700;
  color: #05162d;
  margin-bottom: 15px;
}

.empty-state-message {
  font-size: 1rem;
  color: #666;
  line-height: 1.6;
  margin-bottom: 30px;
}

/* RESPONSIVE */
@media (max-width: 900px) {
  .jobs-layout {
    flex-direction: column;
    padding: 0; /* Reset en móvil */
  }

  .jobs-list-col {
    width: 100%;
    margin: 0;
    padding: 0 20px;
  }

  .job-detail-col {
    margin: 0;
    width: 100%;
    padding: 0 20px;
  }

  .scrollable-list {
    height: auto;
    max-height: none;
    overflow: visible;
  }

  .detail-card {
    position: static;
    max-height: none;
    overflow-y: visible;
  }
}
</style>
