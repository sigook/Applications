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
import { useSeoMeta } from '@unhead/vue'
import MainNavbar from '@/components/layout/MainNavbar.vue'
import MainFooter from '@/components/layout/MainFooter.vue'
import SearchHero from '@/components/open-positions/SearchHero.vue'
import JobsListSection from '@/components/open-positions/JobsListSection.vue'

useSeoMeta({
  title: 'Open Positions - Covenant Group',
  description: 'Browse current job openings at Covenant Group. Find temporary, contract, and permanent positions in skilled trades, industrial, and professional sectors across Canada.',
  ogTitle: 'Open Positions - Covenant Group',
  ogDescription: 'Browse current job openings. Find temporary, contract, and permanent positions across Canada.',
})

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
  background: #F9F9F9;
  background: linear-gradient(180deg,rgba(249, 249, 249, 1) 50%, rgba(15, 47, 68, 1) 50%);
}
</style>
