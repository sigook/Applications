import { ref, computed, type Ref } from 'vue';
import { getJobs } from '@/api/websiteApi';
import type { JobViewModel, JobSearchFilter } from '@/types/website';

const jobs: Ref<JobViewModel[]> = ref([]);
const loading = ref(false);
const error = ref<string | null>(null);

export function useJobs() {
  const hasJobs = computed(() => jobs.value.length > 0);
  const jobsCount = computed(() => jobs.value.length);

  async function fetchJobs(filter: JobSearchFilter = {}): Promise<void> {
    loading.value = true;
    error.value = null;
    try {
      jobs.value = await getJobs(filter);
    } catch {
      error.value = 'We could not load the open positions. Please try again.';
      jobs.value = [];
    } finally {
      loading.value = false;
    }
  }

  function clearError(): void {
    error.value = null;
  }

  return {
    jobs,
    loading,
    error,
    hasJobs,
    jobsCount,
    fetchJobs,
    clearError,
  };
}
