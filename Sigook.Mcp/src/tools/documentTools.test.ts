import assert from 'node:assert/strict';
import { test } from 'node:test';
import { normalizeText } from './documentTools.js';

test('collapses spaces and repeated blank lines', () => {
  assert.equal(normalizeText('  a   b \r\n\r\n\r\n c\n'), 'a b\n\nc');
});
