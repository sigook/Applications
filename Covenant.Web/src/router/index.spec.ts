import { describe, expect, it } from 'vitest'
import router from './index'

function location(path: string) {
  const resolved = router.resolve(path)
  return { ...resolved, name: resolved.name ?? undefined }
}

function scroll(to: string, from: string, saved: { left: number; top: number } | null = null) {
  const behavior = router.options.scrollBehavior
  if (!behavior) throw new Error('scrollBehavior missing')
  return behavior(location(to), location(from), saved)
}

describe('router', () => {
  it.each([
    ['/', 'home'],
    ['/licensed-certified', 'licensed'],
    ['/about', 'about'],
    ['/industries', 'industries'],
    ['/become-partner', 'become_partner'],
    ['/employers', 'employers'],
    ['/talents', 'talents'],
    ['/open-positions', 'open_positions'],
    ['/disclaimer', 'disclaimer'],
    ['/privacy-policy', 'privacy_policy'],
    ['/terms-conditions', 'terms_conditions'],
  ])('resolves %s to %s', (path, name) => {
    expect(router.resolve(path).name).toBe(name)
  })

  it('scrolls smoothly to the hash target', () => {
    expect(scroll('/about#contact-section', '/')).toEqual({ el: '#contact-section', behavior: 'smooth' })
  })

  it('restores the saved position on back navigation', () => {
    expect(scroll('/about', '/', { left: 0, top: 300 })).toEqual({ left: 0, top: 300 })
  })

  it('keeps the position when only the open positions query changes', () => {
    expect(scroll('/open-positions?jobId=1', '/open-positions')).toBe(false)
  })

  it('scrolls to the top on any other navigation', () => {
    expect(scroll('/about', '/')).toEqual({ top: 0, behavior: 'smooth' })
  })
})
