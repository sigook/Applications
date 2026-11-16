import { describe, expect, it, vi } from 'vitest'
import api from './api'
import { jobService } from './jobService'

describe('jobService', () => {
  it('asks for Canadian and US jobs along with the filters', async () => {
    const get = vi.spyOn(api, 'get').mockResolvedValue({ data: [] })

    const jobs = await jobService.getJobs({ location: 'Toronto' })

    expect(jobs).toEqual([])
    expect(get).toHaveBeenCalledWith('/api/website/jobs', {
      params: { location: 'Toronto', countries: ['USA', 'CA'] },
    })
  })
})
