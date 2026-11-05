import assert from 'node:assert/strict';
import { test } from 'node:test';
import { toJson } from './sigookApi.js';

test('toJson omits nulls at every depth', () => {
  assert.equal(toJson({ a: null, b: [{ id: null, phoneNumber: '1' }], c: undefined }), '{"b":[{"phoneNumber":"1"}]}');
});
