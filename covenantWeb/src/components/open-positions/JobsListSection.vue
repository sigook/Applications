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

      <!-- JOBS LAYOUT (existing content) -->
      <div v-else class="jobs-layout">

        <div class="jobs-list-col">
          <div class="list-header">
            <span>{{ jobs.length }} Jobs Found</span>
          </div>

          <div class="scrollable-list">
            <div
              v-for="job in jobs"
              :key="job.numberId"
              class="job-card"
              :class="{ 'is-selected': selectedJob?.numberId === job.numberId }"
              @click="selectJob(job)"
            >
              <h3 class="card-title">{{ job.title }}</h3>
              <p class="card-location">{{ job.location }}</p>

              <div class="card-footer">
                <span class="salary-tag" v-if="job.salary && job.salary !== '$0.00'">{{ job.salary }}</span>
                <span class="id-tag">#{{ job.numberId }}</span>
              </div>
            </div>
          </div>
        </div>

        <div class="job-detail-col">
          <div v-if="selectedJob" class="detail-card">

            <div class="detail-header">
              <h2 class="detail-title">{{ selectedJob.title }}</h2>
              <div class="detail-meta">
                <span class="meta-item location">{{ selectedJob.location }}</span>
                <span class="meta-item salary" v-if="selectedJob.salary !== '$0.00'">{{ selectedJob.salary }} / hr</span>
                <span class="meta-item type">{{ selectedJob.type }}</span>
              </div>
            </div>

            <div class="apply-container">
              <button class="btn-apply-large" @click="handleApplyClick">APPLY NOW</button>
            </div>

            <hr class="divider" />

            <div class="detail-body">

              <div class="content-block description" v-if="selectedJob.description">
                <div v-html="selectedJob.description"></div>
              </div>

              <div class="content-block" v-if="selectedJob.shift">
                <h4>Schedule</h4>
                <p>{{ selectedJob.shift }}</p>
              </div>

              <div class="content-block" v-if="selectedJob.responsibilities">
                <h4>Responsibilities</h4>
                <div class="html-content" v-html="selectedJob.responsibilities"></div>
              </div>

              <div class="content-block" v-if="selectedJob.requirements">
                <h4>Requirements</h4>
                <div class="html-content" v-html="selectedJob.requirements"></div>
              </div>

            </div>

            <div class="detail-footer">
              <button class="btn-apply-large" @click="handleApplyClick">APPLY NOW</button>
            </div>

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
import { ref, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useJobs } from '@/composables/useJobs'
import ApplyNowDialog from '@/components/jobs/ApplyNowDialog.vue'
import type { Job } from '@/services/types/job.types'

const route = useRoute()
const router = useRouter()

// Use jobs composable for API integration
const { jobs, loading, error, fetchJobs } = useJobs()

// Estado para el trabajo seleccionado (para mostrar detalles)
const selectedJob = ref<Job | null>(null)

// Estado para el trabajo al que se está aplicando (para el modal)
const jobToApply = ref<Job | null>(null)

// Estado para el dialog y snackbar
const showApplyDialog = ref(true) // Auto-open on page load
const successSnackbar = ref(false)

// Función para seleccionar trabajo
const selectJob = (job: Job) => {
  selectedJob.value = job

  // Actualizar URL con el jobId (permite deep linking)
  router.replace({
    query: { jobId: job.numberId }
  })
}

// Función para ver todos los trabajos (limpia filtros y query)
const viewAllJobs = () => {
  // Limpiar querystring
  router.replace({ query: {} })

  // Cargar todos los trabajos sin filtros
  fetchJobs()
}

// Función para abrir el dialog de Apply
const handleApplyClick = () => {
  jobToApply.value = selectedJob.value
  showApplyDialog.value = true
}

// Handler cuando se completa la aplicación
const onApplicationSubmitted = () => {
  showApplyDialog.value = false
  jobToApply.value = null // Limpiar el job al cerrar
  successSnackbar.value = true
}

// Al montar el componente, cargamos trabajos desde la API
onMounted(async () => {
  // Obtenemos filtros de la URL (incluyendo jobId si existe)
  const filters = {
    jobId: route.query.jobId as string | undefined,
    jobTitle: route.query.jobTitle as string | undefined,
    location: route.query.location as string | undefined
  }

  await fetchJobs(filters)

  if (jobs.value.length > 0) {
    // Si hay jobId en URL y existe ese job, seleccionarlo
    if (filters.jobId) {
      const jobFromUrl = jobs.value.find(j => j.numberId === filters.jobId)
      selectedJob.value = jobFromUrl || jobs.value[0]
    } else {
      selectedJob.value = jobs.value[0]
    }
  }
})

// Cuando los resultados cambien (por búsqueda), seleccionar el primero automáticamente
watch(jobs, (newJobs) => {
  if (newJobs.length > 0) {
    selectedJob.value = newJobs[0]
  } else {
    selectedJob.value = null
  }
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
}

.jobs-layout {
  display: flex;
  gap: 30px;
  align-items: flex-start;
}

/* === COLUMNA IZQUIERDA (LISTA) === */
.jobs-list-col {
  width: 35%;
  flex-shrink: 0;
  margin: 3rem;
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
  margin: 3rem;
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

.detail-title {
  font-size: 1.8rem;
  font-weight: 800;
  color: #05162d;
  margin-bottom: 10px;
}

.detail-meta {
  font-size: 0.95rem;
  color: #555;
  margin-bottom: 20px;
  font-weight: 600;
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
}

.apply-container {
  margin-bottom: 30px;
}

.btn-apply-large {
  background-color: #32d26a; /* Verde brillante */
  color: white;
  width: 100%;
  border: none;
  padding: 15px;
  font-weight: 800;
  border-radius: 999px;
  font-size: 1rem;
  cursor: pointer;
  transition: background 0.3s;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.btn-apply-large:hover {
  background-color: #28a755;
}

.divider {
  border: 0;
  height: 1px;
  background: #ccc;
  margin: 30px 0;
}

/* Contenido HTML dinámico */
.detail-body {
  color: #333;
  line-height: 1.6;
}

.content-block {
  margin-bottom: 30px;
}

.content-block h4 {
  font-size: 1.1rem;
  font-weight: 700;
  margin-bottom: 10px;
  color: #05162d;
}

/* Estilos profundos para el HTML inyectado (v-html) */
:deep(.html-content ul), :deep(.description ul) {
  padding-left: 20px;
  margin-bottom: 15px;
}

:deep(.html-content li), :deep(.description li) {
  margin-bottom: 8px;
  font-size: 0.95rem;
}

:deep(.description p) {
  margin-bottom: 15px;
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
    max-height: 350px;
  }

  .detail-card {
    position: static;
    max-height: none;
    overflow-y: visible;
  }
}
</style>
