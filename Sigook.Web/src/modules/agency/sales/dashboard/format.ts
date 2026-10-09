const MINUTE_MS = 60 * 1000;
const HOUR_MS = 60 * MINUTE_MS;
const DAY_MS = 24 * HOUR_MS;

export function compactMoney(amount: number): string {
  if (!Number.isFinite(amount)) {
    return '$0';
  }
  const sign = amount < 0 ? '-' : '';
  const value = Math.abs(amount);

  if (value >= 1_000_000) {
    return `${sign}$${trimZero(value / 1_000_000)}M`;
  }
  if (value >= 1_000) {
    return `${sign}$${trimZero(value / 1_000)}K`;
  }
  return `${sign}$${Math.round(value)}`;
}

export function relativeTime(occurredAt: string, asOf: string): string {
  const then = new Date(occurredAt).getTime();
  const now = new Date(asOf).getTime();
  if (Number.isNaN(then) || Number.isNaN(now)) {
    return '';
  }

  const elapsed = now - then;
  if (elapsed < MINUTE_MS) {
    return 'Now';
  }
  if (elapsed < HOUR_MS) {
    return `${Math.floor(elapsed / MINUTE_MS)}m`;
  }
  if (elapsed < DAY_MS) {
    return `${Math.floor(elapsed / HOUR_MS)}h`;
  }
  if (elapsed < 2 * DAY_MS) {
    return 'Yest.';
  }
  if (elapsed < 7 * DAY_MS) {
    return `${Math.floor(elapsed / DAY_MS)}d`;
  }
  return new Date(occurredAt).toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
}

export function shortDate(iso: string): string {
  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) {
    return '';
  }
  return date.toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
}

export function initialsOf(name: string): string {
  return name
    .split(' ')
    .filter((part) => part.length > 0)
    .slice(0, 2)
    .map((part) => part[0].toUpperCase())
    .join('');
}

export function ratioOf(count: number, max: number): number {
  if (max <= 0) {
    return 0;
  }
  return Math.min(1, count / max);
}

function trimZero(value: number): string {
  const rounded = value >= 100 ? Math.round(value) : Math.round(value * 10) / 10;
  return String(rounded);
}
