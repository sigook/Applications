<template>
  <div class="open-positions-page">
    <MainNavbar />

    <SearchHero @search="handleSearch" />

    <JobsListSection ref="jobsListRef" />

    <MainFooter />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'
import SearchHero from '@/components/open-positions/SearchHero.vue'
import JobsListSection from '@/components/open-positions/JobsListSection.vue'

// Referencia al componente hijo por si necesitamos filtrarlo desde aquí luego
const jobsListRef = ref(null);

const handleSearch = (filters: any) => {
  console.log("Searching with filters:", filters);
  // Aquí luego llamaremos a la API y pasaremos los datos a JobsListSection
  // O llamaremos un método dentro de jobsListRef para filtrar localmente
}
</script>

<style scoped>
.open-positions-page {
  min-height: 100vh;
  background-color: #f9f9f9;
}
</style>
