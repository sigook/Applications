export default function(value: string | null): string {
    return value ? value.toLowerCase() : value;
}