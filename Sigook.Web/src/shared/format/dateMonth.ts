import dayjs from 'dayjs';
export default function(date: string | Date | null): string | null {
    return date ? dayjs(date).format('DD-MMM-YYYY') : null;
}