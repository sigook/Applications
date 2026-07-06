import dayjs from 'dayjs';
export default function(date: string | Date | null): string | null {
    if (!date) return null;
    return dayjs(date).format('ddd DD MMMM, YYYY');
}