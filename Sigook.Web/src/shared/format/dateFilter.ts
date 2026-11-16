import dayjs from 'dayjs';
export default function dateFilter(date: string | Date): string;
export default function dateFilter(date: string | Date | null | undefined): string | null;
export default function dateFilter(date: string | Date | null | undefined): string | null {
    if (!date) return null;
    return dayjs(date).format('ddd DD MMMM, YYYY');
}