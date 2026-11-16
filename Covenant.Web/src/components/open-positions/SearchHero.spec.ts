import { describe, expect, it, vi } from 'vitest'
import { flushPromises } from '@vue/test-utils'
import { mountAt } from '@/testing/mountWithPlugins'
import { scrollTarget } from '@/testing/scrollTarget'
import { jobService } from '@/services/jobService'
import SearchHero from './SearchHero.vue'

describe('SearchHero', () => {
  it('searches with the filters, clears the query and scrolls to the results', async () => {
    const getJobs = vi.spyOn(jobService, 'getJobs').mockResolvedValue([])
    const scroll = scrollTarget('jobs-results')
    const { wrapper, router } = await mountAt(SearchHero, '/open-positions?jobId=5')

    await wrapper.find('#jobTitle').setValue('Forklift')
    await wrapper.find('#location').setValue('Toronto')
    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(getJobs).toHaveBeenCalledWith({ jobTitle: 'Forklift', location: 'Toronto' })
    expect(router.currentRoute.value.query).toEqual({})
    expect(scroll).toHaveBeenCalledWith({ behavior: 'smooth', block: 'start' })
  })

  it('sends empty filters as undefined', async () => {
    const getJobs = vi.spyOn(jobService, 'getJobs').mockResolvedValue([])
    const { wrapper } = await mountAt(SearchHero, '/open-positions')

    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(getJobs).toHaveBeenCalledWith({ jobTitle: undefined, location: undefined })
  })
})
