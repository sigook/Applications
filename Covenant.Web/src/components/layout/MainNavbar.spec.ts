import { describe, expect, it, vi } from 'vitest'
import { mountAt } from '@/testing/mountWithPlugins'
import MainNavbar from './MainNavbar.vue'

describe('MainNavbar', () => {
  it('hides the desktop logo on the home page', async () => {
    const { wrapper } = await mountAt(MainNavbar, '/')

    expect(wrapper.find('.navbar__logo-link').classes()).toContain('desktop-hidden')
    expect(wrapper.find('.navbar').classes()).not.toContain('navbar--light-theme')
  })

  it('uses the light theme on the partner page', async () => {
    const { wrapper } = await mountAt(MainNavbar, '/become-partner')

    expect(wrapper.find('.navbar').classes()).toContain('navbar--light-theme')
    expect(wrapper.find('.navbar__logo-link').classes()).not.toContain('desktop-hidden')
  })

  it('marks the bar as scrolled past 40px', async () => {
    const scrollY = vi.spyOn(window, 'scrollY', 'get').mockReturnValue(100)
    const { wrapper } = await mountAt(MainNavbar, '/about')

    window.dispatchEvent(new Event('scroll'))
    await wrapper.vm.$nextTick()
    expect(wrapper.find('.navbar').classes()).toContain('navbar--scrolled')

    scrollY.mockReturnValue(0)
    window.dispatchEvent(new Event('scroll'))
    await wrapper.vm.$nextTick()
    expect(wrapper.find('.navbar').classes()).not.toContain('navbar--scrolled')
  })

  it('opens the mobile menu and closes it from a link or the overlay', async () => {
    const { wrapper } = await mountAt(MainNavbar, '/about')

    await wrapper.find('.navbar__toggle').trigger('click')
    expect(wrapper.find('.navbar__mobile').exists()).toBe(true)

    await wrapper.find('.navbar__mobile-link').trigger('click')
    expect(wrapper.find('.navbar__mobile').exists()).toBe(false)

    await wrapper.find('.navbar__toggle').trigger('click')
    await wrapper.find('.navbar__mobile-overlay').trigger('click')
    expect(wrapper.find('.navbar__mobile').exists()).toBe(false)

    await wrapper.find('.navbar__toggle').trigger('click')
    await wrapper.find('.navbar__toggle').trigger('click')
    expect(wrapper.find('.navbar__mobile').exists()).toBe(false)
  })
})
