import api from './api'
import type { Job, JobFilters } from './types/job.types'

export const jobService = {
  /**
   * Get jobs from the API
   * @param filters - Optional filters for job search
   * @returns Promise with array of jobs
   */
  async getJobs(filters?: JobFilters): Promise<Job[]> {
    const response = await api.get<Job[]>('/api/website/jobs', {
        params: {
          ...filters,
          countries: ['USA', 'CA']
        },
      })
      return response.data
  },
}
