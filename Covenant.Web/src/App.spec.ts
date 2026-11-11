import { describe, expect, it } from 'vitest'
import { flushPromises } from '@vue/test-utils'
import { mountAt } from '@/testing/mountWithPlugins'
import { useToast } from '@/composables/useToast'
import App from './App.vue'

describe('App', () => {
  it('renders the global toast message', async () => {
    useToast().showSuccess('Saved!')
    await mountAt(App, '/about/')
    await flushPromises()

    expect(document.body.textContent).toContain('Saved!')
    useToast().toastState.value.show = false
  })

  it('sets the canonical link without a trailing slash', async () => {
    await mountAt(App, '/about/')
    await flushPromises()

    await expect.poll(() => document.head.querySelector('link[rel="canonical"]')?.getAttribute('href'))
      .toBe('https://www.covenantgroupl.com/about')
  })

  it('keeps the root canonical as a slash', async () => {
    await mountAt(App, '/')
    await flushPromises()

    await expect.poll(() => document.head.querySelector('link[rel="canonical"]')?.getAttribute('href'))
      .toBe('https://www.covenantgroupl.com/')
  })
})
