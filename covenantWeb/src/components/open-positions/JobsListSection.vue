<template>
  <section class="jobs-section" id="jobs-results">
    <div class="container">

      <div class="jobs-layout">

        <div class="jobs-list-col">
          <div class="list-header">
            <span>{{ jobs.length }} Jobs Found</span>
            <span class="sort-link">Most Recent &gt;</span>
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
              <button class="btn-apply-large">APPLY</button>
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
              <button class="btn-apply-large">APPLY NOW</button>
            </div>

          </div>

          <div v-else class="empty-state">
            <p>Select a job to view details</p>
          </div>
        </div>

      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// IMPORTANTE: Importamos el JSON directamente.
// Vite se encarga de convertirlo en un objeto JS utilizable.
import jobsData from '@/assets/json/jobs.json'

// Definimos la interfaz para tipado estricto
interface Job {
  id: string;
  numberId: string;
  title: string;
  salary: string;
  location: string;
  type: string;
  description: string;
  requirements: string;
  responsibilities: string;
  shift: string;
  createdAt: string;
}

// Convertimos los datos importados al tipo Job[]
// Si TypeScript se queja, puedes usar 'as unknown as Job[]' pero normalmente lo infiere bien.
const jobs = ref<Job[]>(jobsData as Job[]);

// Estado para el trabajo seleccionado
const selectedJob = ref<Job | null>(null);

// Función para seleccionar trabajo
const selectJob = (job: Job) => {
  selectedJob.value = job;
}

// Al montar el componente, seleccionamos el primero de la lista automáticamente
onMounted(() => {
  if (jobs.value.length > 0) {
    selectedJob.value = jobs.value[0];
  }
})
</script>

<style scoped>
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

/* RESPONSIVE */
@media (max-width: 900px) {
  .jobs-layout {
    flex-direction: column;
  }

  .jobs-list-col {
    width: 100%;
  }

  .scrollable-list {
    max-height: 300px;
  }

  .detail-card {
    position: static;
    max-height: none; /* En móvil dejamos que crezca */
    overflow-y: visible;
  }
}
</style>
