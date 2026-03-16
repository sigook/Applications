export default function(value: string | null): string {
    if(!value) return "S";
    return value.substring(0,2).toUpperCase();
}