import { vi } from 'vitest'

export function scrollTarget(id: string) {
  const el = document.createElement('div')
  el.id = id
  document.body.appendChild(el)
  return vi.spyOn(el, 'scrollIntoView').mockImplementation(() => undefined)
}
