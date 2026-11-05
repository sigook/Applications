import assert from 'node:assert/strict';
import { test } from 'node:test';
import { fold, same, similarity, tryNormalizePhone } from './lookupRules.js';

test('normalizes North American phones', () => {
  assert.equal(tryNormalizePhone('+1 4452510680'), '445 251-0680');
  assert.equal(tryNormalizePhone('(445) 251-0680'), '445 251-0680');
  assert.equal(tryNormalizePhone('251-0680'), null);
  assert.equal(tryNormalizePhone(null), null);
});

test('fold removes accents, case and punctuation', () => {
  assert.equal(fold('Québec-City'), 'quebeccity');
  assert.equal(same('Montréal', 'MONTREAL'), true);
  assert.equal(same('', ''), false);
});

test('similarity scores containment and edits', () => {
  assert.equal(similarity('philadelphia', 'phila'), 0.9);
  assert.equal(similarity('abc', ''), 0);
  assert.equal(similarity('abcd', 'abcf'), 0.75);
});
