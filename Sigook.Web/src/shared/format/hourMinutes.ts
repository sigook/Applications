export default function(date: string): string {
    const hour = date.split(":");
    return hour[0] + ':' + hour[1]
}