import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { jobService } from '@/services/jobService'
import type { Job, JobFilters } from '@/services/types/job.types'

export const useJobsStore = defineStore('jobs', () => {
  // State
  const jobs = ref<Job[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const hasJobs = computed(() => jobs.value.length > 0)
  const jobsCount = computed(() => jobs.value.length)

  // Actions
  async function fetchJobs(filters?: JobFilters) {
    loading.value = true
    error.value = null

    try {
      jobs.value = await jobService.getJobs(filters)
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch jobs. Please try again later.'
      jobs.value = []
    } finally {
      loading.value = false
    }
  }

  function clearJobs() {
    jobs.value = []
    error.value = null
  }

  function clearError() {
    error.value = null
  }

  return {
    // State
    jobs,
    loading,
    error,
    // Getters
    hasJobs,
    jobsCount,
    // Actions
    fetchJobs,
    clearJobs,
    clearError,
  }
})
