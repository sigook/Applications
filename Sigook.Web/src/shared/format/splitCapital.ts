export default function(value: string | null | undefined): string {
    return value ? value.split(/(?=[A-Z])/).join(" ") : '';
}