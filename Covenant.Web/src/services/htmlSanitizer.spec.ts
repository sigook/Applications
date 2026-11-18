import { describe, expect, it } from 'vitest'
import { sanitizeHtml } from '@/services/htmlSanitizer'

describe('sanitizeHtml', () => {
  it('returns an empty string for empty input', () => {
    expect(sanitizeHtml(null)).toBe('')
    expect(sanitizeHtml(undefined)).toBe('')
    expect(sanitizeHtml('')).toBe('')
  })

  it('keeps the markup produced by the rich text editor', () => {
    const html = '<p><strong>Bold</strong></p><ul><li class="ql-indent-1">One</li></ul>'
    expect(sanitizeHtml(html)).toBe(html)
  })

  it('removes scripts and event handlers', () => {
    expect(sanitizeHtml('<p>Hello</p><script>alert(1)</script>')).toBe('<p>Hello</p>')
    expect(sanitizeHtml('<img src="x" onerror="alert(1)">')).not.toContain('onerror')
  })

  it('removes javascript links', () => {
    expect(sanitizeHtml('<a href="javascript:alert(1)">link</a>')).not.toContain('javascript')
  })

  it('protects the opener on links that open in a new tab', () => {
    const result = sanitizeHtml('<a href="https://sigook.com" target="_blank">Site</a>')
    expect(result).toContain('target="_blank"')
    expect(result).toContain('rel="noopener noreferrer"')
  })
})
