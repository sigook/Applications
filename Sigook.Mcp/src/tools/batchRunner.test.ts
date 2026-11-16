import assert from 'node:assert/strict';
import { test } from 'node:test';
import { runGrouped } from './batchRunner.js';

const tick = () => new Promise(resolve => setImmediate(resolve));

test('keeps results in input order', async () => {
  const results = await runGrouped([3, 1, 2], (_, index) => String(index), 2, async value => {
    for (let i = 0; i < value; i++) await tick();
    return value * 10;
  });
  assert.deepEqual(results, [30, 10, 20]);
});

test('never runs more workers than the concurrency', async () => {
  let running = 0;
  let peak = 0;
  await runGrouped([1, 2, 3, 4, 5, 6], (_, index) => String(index), 3, async () => {
    peak = Math.max(peak, ++running);
    await tick();
    running--;
  });
  assert.equal(peak, 3);
});

test('runs items sharing a key one after another', async () => {
  const order: string[] = [];
  const active = new Set<string>();
  await runGrouped(['a', 'b', 'a', 'a'], item => item, 4, async (item, index) => {
    assert.ok(!active.has(item), `key ${item} ran concurrently`);
    active.add(item);
    await tick();
    order.push(`${item}${index}`);
    active.delete(item);
  });
  assert.deepEqual(order.filter(entry => entry.startsWith('a')), ['a0', 'a2', 'a3']);
});

test('handles an empty list', async () => {
  assert.deepEqual(await runGrouped([], () => '', 4, async () => 1), []);
});
