import { currency, dateMonth } from '@/shared/format';
import type { LocationDetailModel } from '@/shared/types/common';
import type { RequestKpi, RequestStaffingItem } from '@/shared/request-detail/types';

const dayMs = 24 * 60 * 60 * 1000;

export function formatBreak(durationBreak?: string | null, breakIsPaid?: boolean): string {
  if (!durationBreak) return '—';
  const [hours, minutes] = durationBreak.split(':').map((part) => parseInt(part, 10) || 0);
  const total = hours * 60 + minutes;
  if (!total) return 'No break';
  const length = total >= 60 && total % 60 === 0 ? `${total / 60} h` : `${total} min`;
  return breakIsPaid ? `${length} break · paid` : `${length} break · unpaid`;
}

export function rateKpi(rate?: number | null, salary?: number | null, incentive?: number | null): RequestKpi {
  const value = salary ? `${currency(salary)} / year` : rate ? `${currency(rate)} / hr` : '—';
  return {
    key: 'rate',
    label: salary ? 'Salary' : 'Rate',
    value,
    hint: incentive ? `+ ${currency(incentive)} incentive` : undefined,
  };
}

export function datesKpi(startAt?: string | null, finishAt?: string | null, showFinish = true): RequestKpi {
  return {
    key: 'dates',
    label: showFinish ? 'Start / Finish' : 'Start',
    value: startAt ? dateMonth(startAt) : '—',
    hint: showFinish && finishAt ? `→ ${dateMonth(finishAt)}` : undefined,
  };
}

export function shiftKpi(displayShift?: string | null, durationBreak?: string | null, breakIsPaid?: boolean): RequestKpi {
  return {
    key: 'shift',
    label: 'Shift',
    value: displayShift || '—',
    hint: durationBreak ? `· ${formatBreak(durationBreak, breakIsPaid)}` : undefined,
  };
}

export function staffingItems(options: {
  startAt?: string | null;
  openSpots: number;
  applicantsCount?: number;
  invitationSentAt?: string | null;
  isOpen: boolean;
}): RequestStaffingItem[] {
  const items: RequestStaffingItem[] = [];
  if (!options.isOpen) return items;

  if (options.startAt) {
    const days = Math.ceil((new Date(options.startAt).getTime() - Date.now()) / dayMs);
    if (days > 0) {
      items.push({
        key: 'start',
        title: days === 1 ? 'Starts tomorrow' : `Starts in ${days} days`,
        detail: options.openSpots > 0 ? `${options.openSpots} ${options.openSpots === 1 ? 'spot' : 'spots'} still open` : undefined,
        variant: options.openSpots > 0 && days <= 7 ? 'is-warning' : 'is-neutral',
      });
    } else if (options.openSpots > 0) {
      items.push({
        key: 'start',
        title: `${options.openSpots} ${options.openSpots === 1 ? 'spot' : 'spots'} still open`,
        detail: 'Already started',
        variant: 'is-warning',
      });
    }
  }

  if (options.applicantsCount && options.openSpots > 0) {
    items.push({
      key: 'applicants',
      title: `${options.applicantsCount} ${options.applicantsCount === 1 ? 'applicant' : 'applicants'}`,
      detail: 'Waiting for review',
      variant: 'is-neutral',
      actionLabel: 'Review',
    });
  }

  if (options.invitationSentAt) {
    const sent = new Date(options.invitationSentAt);
    const ago = Math.max(0, Math.floor((Date.now() - sent.getTime()) / dayMs));
    const next = new Date(sent.getTime() + 7 * dayMs);
    items.push({
      key: 'invitation',
      title: ago === 0 ? 'Invitation sent today' : `Invitation sent ${ago} ${ago === 1 ? 'day' : 'days'} ago`,
      detail: next.getTime() > Date.now() ? `Next one available ${dateMonth(next.toISOString())}` : 'A new one can be sent',
      variant: 'is-neutral',
    });
  }

  return items;
}

export function formatLocation(location?: LocationDetailModel | null): string {
  if (!location) return '';
  return location.formattedAddress
    || [location.address, location.city?.value, location.city?.province?.code, location.postalCode].filter(Boolean).join(', ');
}

export function mapsUrl(location?: LocationDetailModel | null): string {
  if (!location?.latitude || !location?.longitude) return '';
  return `https://www.google.com/maps/search/?api=1&query=${location.latitude},${location.longitude}`;
}

export function mapsEmbedUrl(location?: LocationDetailModel | null): string {
  if (!location?.latitude || !location?.longitude) return '';
  return `https://www.google.com/maps/embed/v1/place?key=AIzaSyDj0QAxxsRhSUXsZ-pSKlRh62vsK362xqs&q=${location.latitude},${location.longitude}&zoom=13`;
}

export function matchesLocation(location: LocationDetailModel, searchTerm: string): boolean {
  const term = searchTerm.toLowerCase();
  return [location.address, location.city?.value, location.city?.province?.code, location.postalCode]
    .some((value) => value?.toLowerCase().includes(term));
}
