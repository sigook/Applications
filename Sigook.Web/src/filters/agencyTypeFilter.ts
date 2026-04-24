import { appGlobals } from '@/varaibles';

export default function(value: number | null): string {
  if (!value) return '';
  const type = appGlobals.$agencyTypes.find(t => t.value === value);
  return type ? type.label : '';
}
