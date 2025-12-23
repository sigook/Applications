<template>
  <div class="open-positions-page">
    <MainNavbar />

    <SearchHero @search="fetchJobs" />

    <section class="results-placeholder" style="padding: 100px 20px; text-align: center;">
      <h2>Job Listings Loading Area</h2>
      <p style="color: #666;">(API connection coming soon...)</p>

      <div v-if="lastSearch" style="margin-top: 20px; background: #f0f0f0; padding: 20px; display: inline-block; text-align: left;">
        <strong>Debug Last Search:</strong>
        <pre>{{ lastSearch }}</pre>
      </div>
    </section>

    <MainFooter />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'
import SearchHero from '@/components/open-positions/SearchHero.vue'

// Variable reactiva para guardar la búsqueda (solo para debug por ahora)
const lastSearch = ref<any>(null);

// Función que se ejecutará cuando el usuario de click en "Search"
const fetchJobs = (filters: any) => {
  console.log('Filters received from SearchHero:', filters);
  lastSearch.value = filters;

  // AQUÍ HAREMOS EL LLAMADO A LA API (GET) MÁS ADELANTE
  // Ejemplo: axios.get('/api/jobs', { params: filters })...
}
</script>

<style scoped>
.open-positions-page {
  min-height: 100vh;
  background-color: #f9f9f9; /* Fondo claro para la parte de abajo */
}
</style>
