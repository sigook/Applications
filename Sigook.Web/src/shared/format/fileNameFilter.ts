export default function fileName(name: string): string;
export default function fileName(name: string | null | undefined): string | null;
export default function fileName(name: string | null | undefined): string | null {
  if (!name) return null;
  const parts = name.split(/[ ._]+/);
  const joined = parts.length > 1 ? `${parts[0]}.${parts[parts.length - 1]}` : parts[0];
  return joined.toUpperCase();
}
