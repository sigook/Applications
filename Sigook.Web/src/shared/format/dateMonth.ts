import dayjs from 'dayjs';
export default function dateMonth(date: string | Date): string;
export default function dateMonth(date: string | Date | null | undefined): string | null;
export default function dateMonth(date: string | Date | null | undefined): string | null {
    return date ? dayjs(date).format('DD-MMM-YYYY') : null;
}
