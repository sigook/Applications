import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import { locationService } from '@/services/locationService'
import HomeContact from './HomeContact.vue'

describe('HomeContact', () => {
  beforeEach(() => {
    vi.useFakeTimers()
    vi.spyOn(locationService, 'getCountries').mockResolvedValue([])
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it('rotates the testimonials every six seconds', async () => {
    const wrapper = mountWithPlugins(HomeContact)
    const first = wrapper.find('.hc-bg-layer.layer-1')
    expect(first.classes()).toContain('bg-active')

    await vi.advanceTimersByTimeAsync(6000)
    expect(wrapper.find('.hc-bg-layer.layer-2').classes()).toContain('bg-active')

    await vi.advanceTimersByTimeAsync(6000)
    expect(first.classes()).toContain('bg-active')
  })

  it('jumps to the testimonial of the clicked dot', async () => {
    const wrapper = mountWithPlugins(HomeContact)

    await wrapper.findAll('.hc-hero__dot')[1].trigger('click')

    expect(wrapper.findAll('.hc-hero__dot')[1].classes()).toContain('hc-hero__dot--active')
    expect(wrapper.text()).toContain('Ottawa, Ontario')
  })
})
