export default function(date: string | null | undefined): string {
    if (!date) return '';
    const hour = date.split(":");
    return hour[0] + ':' + hour[1]
}