export default function(date: string): string {
    let hour = date.split(":");
    return hour[0] + ':' + hour[1]
}