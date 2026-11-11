import dayjs from 'dayjs';
export default function(date: string | null | undefined): string | null {
    return date ? dayjs(date).format('HH:mm') : null;
}