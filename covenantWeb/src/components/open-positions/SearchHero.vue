<template>
  <section class="search-hero">
    <div class="container hero-inner">

      <div class="hero-text" data-aos="fade-right">
        <h1 class="hero-title">Search Jobs</h1>
        <p class="hero-subtitle">Discover your next career move</p>
      </div>

      <div class="hero-graphic" data-aos="zoom-in">

        <div class="decorative-ring"></div>

        <div class="green-circle">

          <div class="form-card">
            <div class="form-header">
              <h3>Open Positions</h3>
              <p>Filter by title, location, and type</p>
            </div>

            <form @submit.prevent="handleSearch" class="search-form">

              <div class="form-group">
                <label for="jobTitle">Job Title</label>
                <input
                  type="text"
                  id="jobTitle"
                  v-model="filters.jobTitle"
                  placeholder="Enter your dreamed position"
                />
              </div>

              <div class="form-group">
                <label for="location">City</label>
                <input
                  type="text"
                  id="location"
                  v-model="filters.location"
                  placeholder="Enter city name"
                />
              </div>

              <button type="submit" class="btn-search">
                Search
              </button>

            </form>
          </div>
        </div>
      </div>

    </div>
  </section>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useJobsStore } from '@/stores/jobs'

// Definimos la estructura de los filtros
interface SearchFilters {
  jobTitle: string;
  location: string;
}

// Estado reactivo del formulario
const filters = reactive<SearchFilters>({
  jobTitle: '',
  location: ''
})

const router = useRouter()

// Store de jobs para llamar a la API
const jobsStore = useJobsStore()

const handleSearch = () => {
  // Limpiar el querystring (especialmente jobId)
  router.replace({ query: {} })

  // Llamamos directamente al store con los filtros
  jobsStore.fetchJobs({
    jobTitle: filters.jobTitle || undefined,
    location: filters.location || undefined
  })

  // Scroll suave a los resultados
  const resultsSection = document.getElementById('jobs-results')
  if (resultsSection) {
    resultsSection.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }
}
</script>

<style scoped>
* {
  box-sizing: border-box;
}

.search-hero {
  background-color: #0F2F44;
  width: 100%;
  min-height: 85vh;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  border-bottom-left-radius: 250px;
  padding: 120px 0 100px;
  overflow: hidden;
}

.hero-inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  max-width: 1200px;
  padding: 0 20px;
  gap: 40px;
}

/* === TEXTO IZQUIERDO === */
.hero-text {
  max-width: 500px;
  color: white;
}

.hero-title {
  font-size: 3.5rem;
  font-weight: 800;
  margin-bottom: 15px;
  line-height: 1.1;
}

.hero-subtitle {
  font-size: 1.2rem;
  opacity: 0.9;
  font-weight: 300;
}

/* === GRÁFICO DERECHO === */
.hero-graphic {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 700px;
  height: 700px;
}

/* Anillo Blanco Decorativo */
.decorative-ring {
  position: absolute;
  width: 110%;
  height: 110%;
  border: 2px solid #ffffff;
  border-radius: 50%;
  z-index: 2;
  top: -5%;
  left: 7%;
  pointer-events: none; /* Para que no bloquee clicks */
}

/* Círculo Verde */
.green-circle {
  background-color: #5ce07d;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  z-index: 1;
  box-shadow: 0 20px 50px rgba(0,0,0,0.3);
}

/* === TARJETA DEL FORMULARIO === */
.form-card {
  background: white;
  border-radius: 12px;
  padding: 30px;
  width: 380px; /* Aumenté un poco el ancho para que respire mejor */
  max-width: 100%; /* Seguridad para pantallas pequeñas */
  box-shadow: 0 10px 30px rgba(0,0,0,0.15);
  text-align: center;
  position: relative;
  z-index: 10;
}

.form-header h3 {
  font-size: 1.5rem;
  font-weight: 700;
  color: #171717;
  margin-bottom: 5px;
  margin-top: 0;
}
.form-header p {
  font-size: 0.85rem;
  color: #888;
  margin-bottom: 25px;
}

/* Estilos de inputs */
.form-group {
  margin-bottom: 15px;
  text-align: left;
  width: 100%; /* Asegura que el grupo ocupe el espacio disponible */
}

.form-group label {
  display: block;
  font-size: 0.8rem;
  font-weight: 700;
  color: #171717;
  margin-bottom: 6px;
}

.form-group input,
.form-group select {
  width: 100%; /* Ocupa el 100% del padre */
  padding: 10px 15px;
  border: 1px solid #eee;
  border-radius: 8px;
  font-size: 0.9rem;
  background: #fafafa;
  outline: none;
  transition: border-color 0.3s;
  /* CORRECCIÓN CRÍTICA: Asegura que padding y borde estén dentro del ancho */
  box-sizing: border-box;
}

.form-group input:focus,
.form-group select:focus {
  border-color: #32d26a;
  background: #fff;
}


/* Botón Search */
.btn-search {
  width: 100%;
  background-color: #5ce07d;
  color: #05162d;
  border: none;
  padding: 12px;
  font-weight: 700;
  border-radius: 999px; /* Pill shape */
  cursor: pointer;
  margin-top: 10px;
  transition: transform 0.2s, background-color 0.2s;
  box-sizing: border-box;
}

.btn-search:hover {
  background-color: #4bd16f;
  transform: translateY(-2px);
}

/* === RESPONSIVE === */
@media (max-width: 1024px) {
  .hero-inner {
    flex-direction: column;
    text-align: center;
  }

  .search-hero {
    min-height: auto;
    padding-bottom: 80px;
    border-bottom-left-radius: 100px;
  }

  .hero-graphic {
    width: 450px;
    height: 450px;
    margin-top: 40px;
  }

  .decorative-ring {
    top: -5%; left: -5%;
    width: 110%; height: 110%;
  }
}

@media (max-width: 600px) {
  .hero-graphic {
    width: 320px;
    height: 320px;
  }

  /* Ajuste de la tarjeta en móvil */
  .form-card {
    width: 280px;
    padding: 20px;
  }

  .hero-title {
    font-size: 2.5rem;
  }

  .decorative-ring {
    display: none;
  }

  .search-hero {
    border-bottom-left-radius: 50px;
    padding-top: 100px;
  }
}
</style>
