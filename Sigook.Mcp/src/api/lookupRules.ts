export const digitsOnly = (value: string | null | undefined): string => (value ?? '').replace(/\D/g, '');

export function tryNormalizePhone(phone: string | null | undefined): string | null {
  let digits = digitsOnly(phone);
  if (digits.length === 11 && digits[0] === '1') digits = digits.slice(1);
  return digits.length === 10 ? `${digits.slice(0, 3)} ${digits.slice(3, 6)}-${digits.slice(6)}` : null;
}

export const isBlank = (value: string | null | undefined): boolean => !value || value.trim().length === 0;

export const blankToUndefined = (value: string | null | undefined): string | undefined =>
  isBlank(value) ? undefined : (value as string).trim();

export function fold(value: string): string {
  return value.normalize('NFD').replace(/\p{Mn}/gu, '').replace(/[^\p{L}\p{Nd}]/gu, '').toLowerCase();
}

export const same = (a: string | null | undefined, b: string | null | undefined): boolean =>
  !isBlank(a) && !isBlank(b) && fold(a as string) === fold(b as string);

function levenshtein(a: string, b: string): number {
  let previous = Array.from({ length: b.length + 1 }, (_, j) => j);
  let current = new Array<number>(b.length + 1).fill(0);
  for (let i = 1; i <= a.length; i++) {
    current[0] = i;
    for (let j = 1; j <= b.length; j++) {
      const cost = a[i - 1] === b[j - 1] ? 0 : 1;
      current[j] = Math.min(current[j - 1]! + 1, previous[j]! + 1, previous[j - 1]! + cost);
    }
    [previous, current] = [current, previous];
  }
  return previous[b.length]!;
}

export function similarity(a: string, b: string): number {
  if (a.length === 0 || b.length === 0) return 0;
  if (a.includes(b) || b.includes(a)) return 0.9;
  return 1 - levenshtein(a, b) / Math.max(a.length, b.length);
}
