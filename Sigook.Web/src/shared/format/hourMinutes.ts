export default function hourMinutes(time: string | null | undefined): string {
  if (!time) return '';
  const [hours, minutes = '00'] = time.split(':');
  return `${hours}:${minutes}`;
}
