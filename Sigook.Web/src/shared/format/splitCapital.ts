export default function(value: string | null): string {
    return value ? value.split(/(?=[A-Z])/).join(" ") : value;
}