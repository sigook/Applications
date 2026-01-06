export default function (amount) {
    if (!amount) {
        return amount
    }
    return `CAD $${amount.toLocaleString(undefined, {maximumFractionDigits: 2})}`;
}