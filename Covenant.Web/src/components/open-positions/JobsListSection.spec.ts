import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, type VueWrapper } from '@vue/test-utils'
import { mountAt } from '@/testing/mountWithPlugins'
import { jobService } from '@/services/jobService'
import { useJobs } from '@/composables/useJobs'
import type { Job } from '@/services/types/job.types'
import ApplyNowDialog from '@/components/jobs/ApplyNowDialog.vue'
import JobsListSection from './JobsListSection.vue'

function job(numberId: string, title: string): Job {
  return {
    id: numberId,
    requestId: `r${numberId}`,
    numberId,
    title,
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
}

const jobs = [job('1', 'Forklift Operator'), job('2', 'Welder'), { ...job('3', 'Cleaner'), salary: '$0.00' }]

type ChangeListener = (event: { matches: boolean }) => void

function mockMobile(matches: boolean) {
  const listeners: ChangeListener[] = []
  vi.spyOn(window, 'matchMedia').mockImplementation(
    (query: string) =>
      ({
        matches,
        media: query,
        addEventListener: (_type: string, listener: ChangeListener) => listeners.push(listener),
        removeEventListener: () => undefined,
      }) as unknown as MediaQueryList,
  )
  return {
    change(next: boolean) {
      matches = next
      listeners.forEach((l) => l({ matches: next }))
    },
  }
}

async function settle() {
  for (let i = 0; i < 3; i++) {
    await flushPromises()
    await new Promise((resolve) => setTimeout(resolve, 0))
  }
}

async function mountList(path = '/open-positions') {
  const result = await mountAt(JobsListSection, path, { global: { stubs: { ApplyNowDialog: true } } })
  await settle()
  return result
}

function selectedTitles(wrapper: VueWrapper) {
  return wrapper.findAll('.job-card.is-selected').map((c) => c.find('.card-title').text())
}

describe('JobsListSection', () => {
  beforeEach(() => {
    useJobs().clearJobs()
    vi.spyOn(Element.prototype, 'scrollIntoView').mockImplementation(() => undefined)
  })

  it('lists the jobs, selects the first one and syncs the URL', async () => {
    const getJobs = vi.spyOn(jobService, 'getJobs').mockResolvedValue(jobs)
    const { wrapper, router } = await mountList('/open-positions?jobTitle=weld')

    expect(getJobs).toHaveBeenCalledWith({ jobId: undefined, jobTitle: 'weld', location: undefined })
    expect(wrapper.find('.list-header').text()).toBe('3 Jobs Found')
    expect(wrapper.findAll('.salary-tag')).toHaveLength(2)
    expect(selectedTitles(wrapper)).toEqual(['Forklift Operator'])
    expect(wrapper.find('.detail-card').text()).toContain('Forklift Operator')
    expect(router.currentRoute.value.query).toEqual({ jobTitle: 'weld', jobId: '1' })
  })

  it('selects and scrolls to the deep-linked job', async () => {
    vi.spyOn(jobService, 'getJobs').mockResolvedValue(jobs)
    const { wrapper, router } = await mountList('/open-positions?jobId=2')

    expect(selectedTitles(wrapper)).toEqual(['Welder'])
    expect(router.currentRoute.value.query).toEqual({ jobId: '2' })
    expect(Element.prototype.scrollIntoView).toHaveBeenCalledWith({ behavior: 'smooth', block: 'center' })
  })

  it('falls back to the first job when the deep link is unknown', async () => {
    vi.spyOn(jobService, 'getJobs').mockResolvedValue(jobs)
    const { wrapper, router } = await mountList('/open-positions?jobId=99')

    expect(selectedTitles(wrapper)).toEqual(['Forklift Operator'])
    expect(router.currentRoute.value.query).toEqual({ jobId: '1' })
  })

  it('selects the clicked job and keeps the URL shareable', async () => {
    vi.spyOn(jobService, 'getJobs').mockResolvedValue(jobs)
    const { wrapper, router } = await mountList()

    await wrapper.findAll('.job-card')[1].trigger('click')
    await settle()

    expect(selectedTitles(wrapper)).toEqual(['Welder'])
    expect(router.currentRoute.value.query).toEqual({ jobId: '2' })
  })

  it('retries after an error', async () => {
    const getJobs = vi.spyOn(jobService, 'getJobs').mockRejectedValueOnce(new Error('Server down')).mockResolvedValue(jobs)
    const { wrapper } = await mountList()
    expect(wrapper.find('.error-message').text()).toBe('Server down')

    await wrapper.find('.retry-button').trigger('click')
    await flushPromises()

    expect(getJobs).toHaveBeenCalledTimes(2)
    expect(wrapper.findAll('.job-card')).toHaveLength(3)
  })

  it('shows the empty state and loads every job on demand', async () => {
    const getJobs = vi
      .spyOn(jobService, 'getJobs')
      .mockResolvedValueOnce([])
      .mockImplementation(() => new Promise((resolve) => setTimeout(() => resolve(jobs), 0)))
    const { wrapper, router } = await mountList('/open-positions?jobTitle=zzz')
    expect(wrapper.text()).toContain('No Jobs Available')

    await wrapper.find('.retry-button').trigger('click')
    await settle()

    expect(getJobs).toHaveBeenLastCalledWith(undefined)
    expect(router.currentRoute.value.query).toEqual({ jobId: '1' })
    expect(selectedTitles(wrapper)).toEqual(['Forklift Operator'])
  })

  it('selects the first job of a new search and clears the selection when nothing matches', async () => {
    vi.spyOn(jobService, 'getJobs').mockResolvedValue(jobs)
    const { wrapper } = await mountList('/open-positions?jobId=2')
    const { fetchJobs } = useJobs()

    vi.mocked(jobService.getJobs).mockResolvedValue([jobs[2], jobs[1]])
    await fetchJobs()
    await flushPromises()
    expect(selectedTitles(wrapper)).toEqual(['Cleaner'])

    vi.mocked(jobService.getJobs).mockResolvedValue([])
    await fetchJobs()
    await flushPromises()
    expect(wrapper.text()).toContain('No Jobs Available')
  })

  it('opens the apply dialog for a job and confirms the submission', async () => {
    vi.spyOn(jobService, 'getJobs').mockResolvedValue(jobs)
    const { wrapper } = await mountList()
    const dialog = () => wrapper.findComponent(ApplyNowDialog)

    await wrapper.find('.detail-card .btn-apply-large').trigger('click')
    expect(dialog().props()).toMatchObject({ modelValue: true, selectedJob: jobs[0] })

    dialog().vm.$emit('update:modelValue', false)
    await flushPromises()
    expect(dialog().props()).toMatchObject({ modelValue: false, selectedJob: null })

    await wrapper.find('.detail-card .btn-apply-large').trigger('click')
    dialog().vm.$emit('application-submitted')
    await flushPromises()

    expect(dialog().props()).toMatchObject({ modelValue: false, selectedJob: null })
    expect(document.body.textContent).toContain('Application submitted successfully!')
  })

  it('works as an independent accordion on mobile', async () => {
    const media = mockMobile(true)
    vi.spyOn(jobService, 'getJobs').mockResolvedValue(jobs)
    const { wrapper } = await mountList()
    const cards = () => wrapper.findAll('.job-card')

    expect(wrapper.find('.job-detail-col').exists()).toBe(false)
    expect(wrapper.findAll('.inline-detail')).toHaveLength(1)

    await cards()[1].trigger('click')
    expect(selectedTitles(wrapper)).toEqual(['Forklift Operator', 'Welder'])

    await cards()[0].trigger('click')
    expect(selectedTitles(wrapper)).toEqual(['Welder'])

    media.change(false)
    await flushPromises()
    expect(wrapper.find('.job-detail-col').exists()).toBe(true)

    media.change(true)
    await flushPromises()
    expect(selectedTitles(wrapper)).toEqual(['Welder'])
  })

  it('expands the first job of a new search on mobile', async () => {
    mockMobile(true)
    vi.spyOn(jobService, 'getJobs').mockResolvedValue(jobs)
    const { wrapper } = await mountList()

    vi.mocked(jobService.getJobs).mockResolvedValue([jobs[1]])
    await useJobs().fetchJobs()
    await flushPromises()

    expect(selectedTitles(wrapper)).toEqual(['Welder'])
  })
})
