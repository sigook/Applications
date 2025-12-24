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
                <label for="location">City or Postal Code</label>
                <input
                  type="text"
                  id="location"
                  v-model="filters.location"
                  placeholder="Enter your current location"
                />
              </div>

              <div class="form-row">
                <div class="form-group half">
                  <label for="jobType">Job Type</label>
                  <div class="select-wrapper">
                    <select id="jobType" v-model="filters.jobType">
                      <option value="" disabled selected>Select One</option>
                      <option value="full-time">Full Time</option>
                      <option value="part-time">Part Time</option>
                      <option value="contract">Contract</option>
                      <option value="temporary">Temporary</option>
                    </select>
                  </div>
                </div>

                <div class="form-group half">
                  <label for="country">Select Country</label>
                  <div class="select-wrapper country-select">
                    <span class="flag-icon">🇨🇦</span>
                    <select id="country" v-model="filters.country">
                      <option value="canada">Canada</option>
                      <option value="usa">USA</option>
                    </select>
                  </div>
                </div>
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

// Definimos la estructura de los filtros
interface SearchFilters {
  jobTitle: string;
  location: string;
  jobType: string;
  country: string;
}

// Estado reactivo del formulario
const filters = reactive<SearchFilters>({
  jobTitle: '',
  location: '',
  jobType: '',
  country: 'canada' // Valor por defecto
});

// Definimos evento para comunicar al padre que se debe buscar
const emit = defineEmits<{
  (e: 'search', filters: SearchFilters): void
}>();

const handleSearch = () => {
  // Emitimos los filtros actuales al componente padre (View)
  // El padre se encargará de llamar a la API con estos datos
  emit('search', { ...filters });
}
</script>

<style scoped>
.search-hero {
  background-color: #05162d; /* Azul oscuro del tema */
  width: 100%;
  min-height: 85vh; /* Altura considerable para el hero */
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  /* Curva inferior izquierda grande */
  border-bottom-left-radius: 250px;
  padding: 120px 0 100px; /* Padding top para compensar navbar */
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
  width: 360px; /* Ancho fijo para que quepa en el círculo */
  box-shadow: 0 10px 30px rgba(0,0,0,0.15);
  text-align: center;
}

.form-header h3 {
  font-size: 1.5rem;
  font-weight: 700;
  color: #171717;
  margin-bottom: 5px;
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
  width: 100%;
  padding: 10px 15px;
  border: 1px solid #eee;
  border-radius: 8px;
  font-size: 0.9rem;
  background: #fafafa;
  outline: none;
  transition: border-color 0.3s;
}

.form-group input:focus,
.form-group select:focus {
  border-color: #32d26a;
}

/* Fila de selects */
.form-row {
  display: flex;
  gap: 15px;
}
.form-group.half {
  flex: 1;
}

/* Select de país con bandera */
.country-select {
  position: relative;
}
.country-select select {
  padding-right: 30px; /* Espacio para la bandera si fuera imagen de fondo */
  text-align: right; /* Alineación del texto para dar espacio a la izq si quieres */
  appearance: none; /* Reset nativo */
}
.flag-icon {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  pointer-events: none;
  font-size: 1.2rem;
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
  }

  .decorative-ring {
    top: -5%; left: -5%;
    width: 110%; height: 110%;
  }
}

@media (max-width: 600px) {
  .hero-graphic {
    width: 340px;
    height: 340px;
  }

  .form-card {
    width: 280px;
    padding: 20px;
  }

  .hero-title {
    font-size: 2.5rem;
  }

  .decorative-ring {
    display: none; /* Ocultar anillo en móvil si molesta */
  }

  .search-hero {
    border-bottom-left-radius: 50px;
  }
}
</style>
