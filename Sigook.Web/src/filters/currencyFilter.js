export default function (amount) {
  if (!amount) {
    return amount;
  }
  return "$" + amount.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}
