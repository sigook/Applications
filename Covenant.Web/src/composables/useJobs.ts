import { storeToRefs } from 'pinia'
import { useJobsStore } from '@/stores/jobs'

/**
 * Composable for using jobs functionality
 * Provides reactive access to jobs store state and actions
 */
export function useJobs() {
  const store = useJobsStore()

  // Convert store state to refs for reactivity
  const { jobs, loading, error, hasJobs, jobsCount } = storeToRefs(store)

  return {
    // State
    jobs,
    loading,
    error,
    // Getters
    hasJobs,
    jobsCount,
    // Actions
    fetchJobs: store.fetchJobs,
    clearJobs: store.clearJobs,
    clearError: store.clearError,
  }
}
