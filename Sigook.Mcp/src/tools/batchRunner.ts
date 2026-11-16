export async function runGrouped<T, R>(
  items: readonly T[],
  keyOf: (item: T, index: number) => string,
  concurrency: number,
  worker: (item: T, index: number) => Promise<R>
): Promise<R[]> {
  const groups = new Map<string, number[]>();
  items.forEach((item, index) => {
    const key = keyOf(item, index);
    const group = groups.get(key);
    if (group) group.push(index);
    else groups.set(key, [index]);
  });

  const queue = [...groups.values()];
  const results = new Array<R>(items.length);
  const lanes = Array.from({ length: Math.max(1, Math.min(concurrency, queue.length)) }, async () => {
    for (let group = queue.shift(); group; group = queue.shift())
      for (const index of group) results[index] = await worker(items[index]!, index);
  });
  await Promise.all(lanes);
  return results;
}
