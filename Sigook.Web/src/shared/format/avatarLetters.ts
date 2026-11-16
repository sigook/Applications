export default function(value: string | null | undefined): string {
    if(!value) return "S";
    return value.substring(0,2).toUpperCase();
}