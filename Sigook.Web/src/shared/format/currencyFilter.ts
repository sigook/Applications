export default function (amount: number | null): string {
  if (!amount) {
    return '';
  }
  return "$" + amount.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}
