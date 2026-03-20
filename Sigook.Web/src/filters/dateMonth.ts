import dayjs from 'dayjs';
export default function(date: string | null): string | null {
    return date ? dayjs(date).format('DD-MMM-YYYY') : date;
}