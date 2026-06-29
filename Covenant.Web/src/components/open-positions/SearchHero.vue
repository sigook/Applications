<template>
  <section class="search-hero">
    <div class="container hero-inner">

      <div class="hero-text" data-aos="fade-right">
        <h1 class="hero-title">Search Jobs</h1>
        <p class="hero-subtitle">Discover your next career move</p>
      </div>

      <div class="hero-graphic" data-aos="zoom-in">

        <div class="decorative-ring"></div>
        <div class="green-circle"></div>

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
  </section>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useJobs } from '@/composables/useJobs'

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

const { fetchJobs } = useJobs()

const handleSearch = () => {
  // Limpiar el querystring (especialmente jobId)
  router.replace({ query: {} })

  // Llamamos al composable con los filtros
  fetchJobs({
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
  min-height: 60vh;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  border-bottom-left-radius: 120px;
  padding: 80px 0 60px;
  overflow: hidden;
}

.hero-inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  max-width: 1160px; /* Ensanchado para separar los elementos */
  padding: 0 120px 0 20px; /* Más padding a la derecha para compensar y mantener el form en el mismo lugar */
  gap: 32px;
}

/* === TEXTO IZQUIERDO === */
.hero-text {
  max-width: 400px;
  color: white;
  position: relative;
  z-index: 5;
}

.hero-title {
  font-size: 2.8rem;
  font-weight: 800;
  margin-bottom: 12px;
  line-height: 1.1;
}

.hero-subtitle {
  font-size: 1rem;
  opacity: 0.9;
  font-weight: 300;
}

/* === GRÁFICO DERECHO === */
.hero-graphic {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 40px; /* Para dar espacio a los círculos salientes por la derecha */
  margin-top: 60px; /* Separarlo del navbar y pegarlo más al borde inferior */
}

/* Anillo Blanco Decorativo */
.decorative-ring {
  position: absolute;
  width: 280px;
  height: 280px;
  border: 1px solid rgba(255, 255, 255, 0.7);
  border-radius: 50%;
  z-index: 10;
  top: 0%;
  right: -50px;
  pointer-events: none; /* Para que no bloquee clicks */
}

/* Círculo Verde */
.green-circle {
  position: absolute;
  background-color: #5ce07d;
  width: 250px;
  height: 250px;
  border-radius: 50%;
  z-index: 2;
  top: -30px;
  right: -50px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.1);
}

/* === TARJETA DEL FORMULARIO === */
.form-card {
  background: white;
  border-radius: 10px;
  padding: 24px;
  width: 304px; /* Aumenté un poco el ancho para que respire mejor */
  max-width: 100%; /* Seguridad para pantallas pequeñas */
  box-shadow: 0 8px 24px rgba(0,0,0,0.15);
  text-align: center;
  position: relative;
  z-index: 10;
}

.form-header h3 {
  font-size: 1.2rem;
  font-weight: 700;
  color: #171717;
  margin-bottom: 4px;
  margin-top: 0;
}
.form-header p {
  font-size: 0.75rem;
  color: #888;
  margin-bottom: 20px;
}

/* Estilos de inputs */
.form-group {
  margin-bottom: 12px;
  text-align: left;
  width: 100%; /* Asegura que el grupo ocupe el espacio disponible */
}

.form-group label {
  display: block;
  font-size: 0.7rem;
  font-weight: 700;
  color: #171717;
  margin-bottom: 5px;
}

.form-group input,
.form-group select {
  width: 100%; /* Ocupa el 100% del padre */
  padding: 8px 12px;
  border: 1px solid #eee;
  border-radius: 6px;
  font-size: 0.8rem;
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
  padding: 10px;
  font-weight: 700;
  border-radius: 999px; /* Pill shape */
  font-size: 0.9rem;
  cursor: pointer;
  margin-top: 8px;
  transition: transform 0.2s, background-color 0.2s;
  box-sizing: border-box;
}

.btn-search:hover {
  background-color: #4bd16f;
  transform: translateY(-2px);
}

/* === RESPONSIVE === */
@media (max-width: 1440px) {
  .search-hero {
    min-height: 40vh; /* Se reduce la altura de la sección para 1440px como se solicitó */
  }
}

@media (max-width: 1024px) {
  .hero-inner {
    flex-direction: column;
    text-align: center;
    padding: 0 20px;
  }

  .search-hero {
    min-height: auto;
    padding-bottom: 64px;
    border-bottom-left-radius: 80px;
  }

  .hero-graphic {
    margin-top: 40px;
    margin-right: 0;
  }

  .decorative-ring {
    top: -20px;
    right: -40px;
  }

  .form-card {
    padding: 16px;
  }

  .form-header h3 {
    font-size: 1.1rem;
  }

  .form-header p {
    margin-bottom: 14px;
  }

  .form-group {
    margin-bottom: 8px;
  }

  .form-group input,
  .form-group select {
    padding: 7px 10px;
  }

  .btn-search {
    padding: 9px;
    margin-top: 4px;
  }
}

@media (max-width: 600px) {
  .hero-graphic {
    margin-top: 30px;
    z-index: 0;
  }

  /* Ajuste de la tarjeta en móvil */
  .form-card {
    width: 280px;
    padding: 16px;
    margin-top: 0;
  }

  .hero-title {
    font-size: 2rem;
  }

  .decorative-ring {
    display: none;
  }

  .green-circle {
    width: 200px;
    height: 200px;
    top: -20px;
    right: -20px;
  }

  .search-hero {
    border-bottom-left-radius: 40px;
    padding-top: 80px;
  }
}
</style>
