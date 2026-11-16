import { afterEach, beforeEach, vi } from 'vitest'
import { enableAutoUnmount } from '@vue/test-utils'

vi.mock('vue3-recaptcha2', async () => {
  const { defineComponent, h } = await import('vue')
  return {
    default: defineComponent({
      name: 'Vue3Recaptcha2',
      emits: ['verify', 'expire', 'fail'],
      render: () => h('div', { class: 'recaptcha-stub' }),
    }),
  }
})

enableAutoUnmount(afterEach)

beforeEach(() => {
  vi.stubGlobal('visualViewport', {
    width: 1024,
    height: 768,
    scale: 1,
    offsetLeft: 0,
    offsetTop: 0,
    addEventListener: () => undefined,
    removeEventListener: () => undefined,
  })
})

afterEach(() => {
  document.body.innerHTML = ''
})
