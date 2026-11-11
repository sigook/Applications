import { beforeEach, describe, expect, it, vi } from 'vitest'
import { jobService } from '@/services/jobService'
import type { Job } from '@/services/types/job.types'
import { useJobs } from './useJobs'

const job: Job = {
  id: '1',
  requestId: 'r1',
  numberId: '100',
  title: 'Forklift Operator',
  salary: '$20.00',
  location: 'Toronto',
  type: 'Temporary',
  description: '',
  requirements: '',
  responsibilities: '',
  shift: '',
  createdAt: '2026-11-01',
  createdBy: null,
  agencyId: null,
  companyType: null,
}

describe('useJobs', () => {
  beforeEach(() => {
    useJobs().clearJobs()
  })

  it('loads jobs with the given filters', async () => {
    const getJobs = vi.spyOn(jobService, 'getJobs').mockResolvedValue([job])
    const { fetchJobs, jobs, hasJobs, jobsCount, loading } = useJobs()

    await fetchJobs({ jobTitle: 'Forklift' })

    expect(getJobs).toHaveBeenCalledWith({ jobTitle: 'Forklift' })
    expect(jobs.value).toEqual([job])
    expect(hasJobs.value).toBe(true)
    expect(jobsCount.value).toBe(1)
    expect(loading.value).toBe(false)
  })

  it('exposes the error message and empties the list when loading fails', async () => {
    vi.spyOn(jobService, 'getJobs').mockRejectedValue(new Error('Network error.'))
    const { fetchJobs, jobs, error } = useJobs()

    await fetchJobs()

    expect(error.value).toBe('Network error.')
    expect(jobs.value).toEqual([])
  })

  it('falls back to a generic message for non-Error failures', async () => {
    vi.spyOn(jobService, 'getJobs').mockRejectedValue('boom')
    const { fetchJobs, error, clearError } = useJobs()

    await fetchJobs()
    expect(error.value).toBe('Failed to fetch jobs. Please try again later.')

    clearError()
    expect(error.value).toBeNull()
  })
})
