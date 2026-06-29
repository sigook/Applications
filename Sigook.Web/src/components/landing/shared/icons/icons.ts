/**
 * Central icon registry for the landing.
 *
 * Every landing icon lives here as plain shape data instead of inline <svg>
 * markup scattered across components. Render with <LandingIcon name="..." />.
 * Icons are single-colour (currentColor) and size to 1em by default; the
 * consuming element can still override size/colour via its own CSS.
 */
export interface IconShape {
  tag: 'path' | 'circle' | 'rect' | 'line' | 'polyline'
  attrs: Record<string, string | number>
}

export interface IconDef {
  /** SVG viewBox. Default '0 0 24 24'. */
  viewBox?: string
  /** 'none' for stroke icons (default), 'currentColor' for filled glyphs/logos. */
  fill?: string
  /** Stroke width for stroke icons (default 1.5). Ignored for filled icons. */
  strokeWidth?: number
  shapes: IconShape[]
}

export const ICONS = {
  // ── UI chrome ───────────────────────────────────────────────────────────
  close: {
    strokeWidth: 2,
    shapes: [{ tag: 'path', attrs: { d: 'M18 6 6 18M6 6l12 12' } }],
  },
  search: {
    strokeWidth: 2,
    shapes: [
      { tag: 'circle', attrs: { cx: 11, cy: 11, r: 7 } },
      { tag: 'path', attrs: { d: 'M21 21l-4.35-4.35' } },
    ],
  },
  'chevron-left': {
    strokeWidth: 2,
    shapes: [{ tag: 'path', attrs: { d: 'm15 18-6-6 6-6' } }],
  },
  'chevron-right': {
    strokeWidth: 2,
    shapes: [{ tag: 'path', attrs: { d: 'm9 18 6-6-6-6' } }],
  },
  location: {
    strokeWidth: 1.8,
    shapes: [
      { tag: 'path', attrs: { d: 'M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0z' } },
      { tag: 'circle', attrs: { cx: 12, cy: 10, r: 3 } },
    ],
  },
  check: {
    strokeWidth: 2.5,
    shapes: [{ tag: 'polyline', attrs: { points: '20 6 9 17 4 12' } }],
  },
  'check-sm': {
    viewBox: '0 0 14 14',
    strokeWidth: 2.5,
    shapes: [{ tag: 'polyline', attrs: { points: '11 4 6 9 3 6' } }],
  },
  'arrow-circle': {
    strokeWidth: 1.5,
    shapes: [
      { tag: 'circle', attrs: { cx: 12, cy: 12, r: 9 } },
      { tag: 'path', attrs: { d: 'M9 12h6M13 9l3 3-3 3' } },
    ],
  },
  upload: {
    strokeWidth: 1.8,
    shapes: [
      { tag: 'path', attrs: { d: 'M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4' } },
      { tag: 'polyline', attrs: { points: '17 8 12 3 7 8' } },
      { tag: 'line', attrs: { x1: 12, y1: 3, x2: 12, y2: 15 } },
    ],
  },
  camera: {
    strokeWidth: 1.5,
    shapes: [
      { tag: 'path', attrs: { d: 'M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z' } },
      { tag: 'circle', attrs: { cx: 12, cy: 13, r: 4 } },
    ],
  },

  // ── Social / brand logos (filled) ───────────────────────────────────────
  instagram: {
    fill: 'currentColor',
    shapes: [{ tag: 'path', attrs: { d: 'M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zM12 0C8.741 0 8.333.014 7.053.072 2.695.272.273 2.69.073 7.052.014 8.333 0 8.741 0 12c0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98C8.333 23.986 8.741 24 12 24c3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.947-.196-4.354-2.617-6.78-6.979-6.98C15.668.014 15.259 0 12 0zm0 5.838a6.162 6.162 0 1 0 0 12.324 6.162 6.162 0 0 0 0-12.324zM12 16a4 4 0 1 1 0-8 4 4 0 0 1 0 8zm6.406-11.845a1.44 1.44 0 1 0 0 2.881 1.44 1.44 0 0 0 0-2.881z' } }],
  },
  facebook: {
    fill: 'currentColor',
    shapes: [{ tag: 'path', attrs: { d: 'M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z' } }],
  },
  linkedin: {
    fill: 'currentColor',
    shapes: [{ tag: 'path', attrs: { d: 'M20.447 20.452h-3.554v-5.569c0-1.328-.027-3.037-1.852-3.037-1.853 0-2.136 1.445-2.136 2.939v5.667H9.351V9h3.414v1.561h.046c.477-.9 1.637-1.85 3.37-1.85 3.601 0 4.267 2.37 4.267 5.455v6.286zM5.337 7.433a2.062 2.062 0 0 1-2.063-2.065 2.064 2.064 0 1 1 2.063 2.065zm1.782 13.019H3.555V9h3.564v11.452zM22.225 0H1.771C.792 0 0 .774 0 1.729v20.542C0 23.227.792 24 1.771 24h20.451C23.2 24 24 23.227 24 22.271V1.729C24 .774 23.2 0 22.222 0h.003z' } }],
  },

  // ── Industry / sector icons ─────────────────────────────────────────────
  automotive: {
    shapes: [{ tag: 'path', attrs: { d: 'M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z' } }],
  },
  aviation: {
    shapes: [
      { tag: 'path', attrs: { d: 'M22 2 11 13' } },
      { tag: 'path', attrs: { d: 'M22 2 15 22l-4-9-9-4z' } },
    ],
  },
  construction: {
    shapes: [
      { tag: 'path', attrs: { d: 'M2 18h20v2H2z' } },
      { tag: 'path', attrs: { d: 'M4 18a8 8 0 0 1 16 0' } },
      { tag: 'path', attrs: { d: 'M10 6h4v6h-4z' } },
    ],
  },
  engineering: {
    shapes: [
      { tag: 'circle', attrs: { cx: 12, cy: 12, r: 3 } },
      { tag: 'path', attrs: { d: 'M12 1v3M12 20v3M4.22 4.22l2.12 2.12M17.66 17.66l2.12 2.12M1 12h3M20 12h3M4.22 19.78l2.12-2.12M17.66 6.34l2.12-2.12' } },
    ],
  },
  technology: {
    shapes: [
      { tag: 'rect', attrs: { x: 6, y: 6, width: 12, height: 12, rx: 1 } },
      { tag: 'path', attrs: { d: 'M4 9h2M4 12h2M4 15h2M18 9h2M18 12h2M18 15h2M9 4v2M12 4v2M15 4v2M9 18v2M12 18v2M15 18v2' } },
    ],
  },
  finance: {
    shapes: [
      { tag: 'circle', attrs: { cx: 12, cy: 12, r: 9 } },
      { tag: 'path', attrs: { d: 'M12 6v12' } },
      { tag: 'path', attrs: { d: 'M15 9.5C15 8.12 13.66 7 12 7s-3 1.12-3 2.5S10.34 12 12 12s3 1.12 3 2.5-1.34 2.5-3 2.5-3-1.12-3-2.5' } },
    ],
  },
  legal: {
    shapes: [
      { tag: 'path', attrs: { d: 'M12 3v18' } },
      { tag: 'path', attrs: { d: 'M5 6h14' } },
      { tag: 'path', attrs: { d: 'm7 6-3 9a3 3 0 0 0 6 0L7 6z' } },
      { tag: 'path', attrs: { d: 'm17 6-3 9a3 3 0 0 0 6 0L17 6z' } },
      { tag: 'path', attrs: { d: 'M8 21h8' } },
    ],
  },
  logistics: {
    shapes: [
      { tag: 'path', attrs: { d: 'M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z' } },
      { tag: 'path', attrs: { d: 'M3.27 6.96 12 12.01l8.73-5.05' } },
      { tag: 'path', attrs: { d: 'M12 22.08V12' } },
    ],
  },
  manufacturing: {
    shapes: [
      { tag: 'path', attrs: { d: 'M2 20V10l6 4V10l6 4V10l6 4v6z' } },
      { tag: 'path', attrs: { d: 'M2 20h20' } },
      { tag: 'path', attrs: { d: 'M19 6V4l3 3v3' } },
    ],
  },
  retail: {
    shapes: [
      { tag: 'path', attrs: { d: 'M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4z' } },
      { tag: 'path', attrs: { d: 'M3 6h18' } },
      { tag: 'path', attrs: { d: 'M16 10a4 4 0 0 1-8 0' } },
    ],
  },
  transportation: {
    shapes: [
      { tag: 'path', attrs: { d: 'M14 18V6a1 1 0 0 0-1-1H2a1 1 0 0 0-1 1v12' } },
      { tag: 'path', attrs: { d: 'M14 8h4l3 5v5h-3' } },
      { tag: 'path', attrs: { d: 'M3 18h2M11 18h-3' } },
      { tag: 'circle', attrs: { cx: 7, cy: 18, r: 2 } },
      { tag: 'circle', attrs: { cx: 17, cy: 18, r: 2 } },
    ],
  },
} satisfies Record<string, IconDef>

export type LandingIconName = keyof typeof ICONS
