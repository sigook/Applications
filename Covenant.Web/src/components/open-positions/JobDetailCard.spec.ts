import { describe, expect, it } from 'vitest'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import type { Job } from '@/services/types/job.types'
import JobDetailCard from './JobDetailCard.vue'

const job: Job = {
  id: '1',
  requestId: 'r1',
  numberId: '100',
  title: 'Forklift Operator',
  salary: '$20.00',
  location: 'Toronto',
  type: 'Temporary',
  description: '<p>Move pallets</p>',
  requirements: '',
  responsibilities: '<ul><li>Load trucks</li></ul>',
  shift: 'Nights',
  createdAt: '2026-11-01',
  createdBy: null,
  agencyId: null,
  companyType: null,
}

describe('JobDetailCard', () => {
  it('renders the job header and only the sections that have content', () => {
    const wrapper = mountWithPlugins(JobDetailCard, { props: { job } })

    expect(wrapper.find('.detail-title').text()).toContain('Forklift Operator')
    expect(wrapper.find('.detail-id').text()).toBe('#100')
    expect(wrapper.find('.salary').text()).toBe('$20.00 / hr')
    expect(wrapper.text()).toContain('Schedule')
    expect(wrapper.text()).toContain('Responsibilities')
    expect(wrapper.text()).not.toContain('Requirements')
  })

  it('hides a zero salary', () => {
    const wrapper = mountWithPlugins(JobDetailCard, { props: { job: { ...job, salary: '$0.00' } } })

    expect(wrapper.find('.salary').exists()).toBe(false)
  })

  it('emits apply with the job from both buttons', async () => {
    const wrapper = mountWithPlugins(JobDetailCard, { props: { job } })

    const buttons = wrapper.findAll('.btn-apply-large')
    await buttons[0].trigger('click')
    await buttons[1].trigger('click')

    expect(wrapper.emitted('apply')).toEqual([[job], [job]])
  })
})
