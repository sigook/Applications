import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import type { VueWrapper } from '@vue/test-utils'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import { scrollTarget } from '@/testing/scrollTarget'
import HeroSection from './HeroSection.vue'

function activeDot(wrapper: VueWrapper) {
  return wrapper.findAll('.hero__dot').findIndex((d) => d.classes().includes('hero__dot--active'))
}

describe('HeroSection', () => {
  beforeEach(() => {
    vi.useFakeTimers()
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it('advances the slide every five seconds and wraps around', async () => {
    const wrapper = mountWithPlugins(HeroSection)
    expect(activeDot(wrapper)).toBe(0)

    await vi.advanceTimersByTimeAsync(5000)
    expect(activeDot(wrapper)).toBe(1)

    await vi.advanceTimersByTimeAsync(10000)
    expect(activeDot(wrapper)).toBe(0)
  })

  it('jumps to a dot and restarts the timer', async () => {
    const wrapper = mountWithPlugins(HeroSection)
    await vi.advanceTimersByTimeAsync(4000)

    await wrapper.findAll('.hero__dot')[2].trigger('click')
    expect(activeDot(wrapper)).toBe(2)

    await vi.advanceTimersByTimeAsync(4000)
    expect(activeDot(wrapper)).toBe(2)
    await vi.advanceTimersByTimeAsync(1000)
    expect(activeDot(wrapper)).toBe(0)
  })

  it('scrolls to the employers section', async () => {
    const scroll = scrollTarget('for-employers')
    const wrapper = mountWithPlugins(HeroSection)

    await wrapper.find('.hero__cta').trigger('click')

    expect(scroll).toHaveBeenCalledWith({ behavior: 'smooth' })
  })

  it('ignores the scroll without an employers section', async () => {
    const wrapper = mountWithPlugins(HeroSection)

    await wrapper.find('.hero__cta').trigger('click')

    expect(wrapper.find('.hero__cta').exists()).toBe(true)
  })
})
