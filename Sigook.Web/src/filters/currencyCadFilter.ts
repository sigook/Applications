export default function (amount: number | null): string {
    if (!amount) {
        return ''
    }
    return `CAD $${amount.toLocaleString(undefined, {maximumFractionDigits: 2})}`;
}