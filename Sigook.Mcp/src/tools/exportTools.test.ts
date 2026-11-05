import assert from 'node:assert/strict';
import { test } from 'node:test';
import { detectDelimiter, parseCsv } from './exportTools.js';

test('detects the delimiter ignoring quoted text', () => {
  assert.equal(detectDelimiter('"a,b,c";d;e'), ';');
  assert.equal(detectDelimiter('a,b'), ',');
  assert.equal(detectDelimiter('a\tb'), '\t');
});

test('parses rows with paging, BOM and short rows', () => {
  const result = parseCsv('﻿Name,City\nAna,Toronto\nLuis\n\nEva,Ottawa\n', 1, 1);
  assert.deepEqual(result.headers, ['Name', 'City']);
  assert.equal(result.totalRows, 3);
  assert.deepEqual(result.rows, [{ Name: 'Luis', City: '' }]);
});
