import { appGlobals } from '@/app/globals';

export default function(value: number | null): string {
  if (!value) return '';
  const type = appGlobals.$agencyTypes.find(t => t.value === value);
  return type ? type.label : '';
}
