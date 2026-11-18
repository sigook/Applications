import { describe, expect, it } from 'vitest';
import { escapeHtml, sanitizeHtml } from '@/shared/utils/html';

describe('escapeHtml', () => {
  it('escapes markup characters', () => {
    expect(escapeHtml(`<img src=x onerror="a('b')">&`)).toBe('&lt;img src=x onerror=&quot;a(&#39;b&#39;)&quot;&gt;&amp;');
  });

  it('leaves plain text unchanged', () => {
    expect(escapeHtml('Nina New')).toBe('Nina New');
  });

  it('turns numbers into text and empty values into an empty string', () => {
    expect(escapeHtml(42)).toBe('42');
    expect(escapeHtml(null)).toBe('');
    expect(escapeHtml(undefined)).toBe('');
  });
});

describe('sanitizeHtml', () => {
  it('returns an empty string for empty input', () => {
    expect(sanitizeHtml(null)).toBe('');
    expect(sanitizeHtml(undefined)).toBe('');
    expect(sanitizeHtml('')).toBe('');
  });

  it('keeps the markup produced by the rich text editor', () => {
    const html = '<p><strong>Bold</strong> <em>italic</em></p><ol><li class="ql-indent-1">One</li></ol>';
    expect(sanitizeHtml(html)).toBe(html);
  });

  it('removes scripts', () => {
    expect(sanitizeHtml('<p>Hello</p><script>alert(1)</script>')).toBe('<p>Hello</p>');
  });

  it('removes event handlers', () => {
    const result = sanitizeHtml('<img src="x" onerror="alert(1)">');
    expect(result).not.toContain('onerror');
  });

  it('removes javascript links', () => {
    const result = sanitizeHtml('<a href="javascript:alert(1)">link</a>');
    expect(result).not.toContain('javascript');
  });

  it('keeps links that open in a new tab and protects the opener', () => {
    const result = sanitizeHtml('<a href="https://sigook.com" target="_blank">Site</a>');
    expect(result).toContain('href="https://sigook.com"');
    expect(result).toContain('target="_blank"');
    expect(result).toContain('rel="noopener noreferrer"');
  });
});
