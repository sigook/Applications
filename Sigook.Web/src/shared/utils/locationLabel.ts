interface WithLocation {
  city?: string | null;
  provinceCode?: string | null;
}

export function locationLabel(item: WithLocation | null | undefined): string {
  if (!item?.city) return '';
  return item.provinceCode ? `${item.city}, ${item.provinceCode}` : item.city;
}
