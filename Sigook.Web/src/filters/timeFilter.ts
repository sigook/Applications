import dayjs from 'dayjs';
export default function(date: string | null): string {
    if(!date) return "";
    return dayjs(date).format('HH:mm');
}