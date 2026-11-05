import assert from 'node:assert/strict';
import { test } from 'node:test';
import { ImportInstructions, parseMapping } from './importInstructions.js';

test('parses front matter', () => {
  const mapping = parseMapping('---\r\nname: x\r\ndetect: A, B\r\n---\r\n\r\n# Body\r\n');
  assert.equal(mapping.title, 'x');
  assert.deepEqual(mapping.detectHeaders, ['A', 'B']);
  assert.equal(mapping.markdown, '# Body');
});

test('packaged instructions load and detect ZipRecruiter', () => {
  const instructions = new ImportInstructions();
  assert.ok(instructions.workflow.length > 0);
  assert.equal(instructions.detect(['candidate overview', 'Deal-Breaker Questions Correct', 'Questions and Answers', 'Name'])?.name, 'ziprecruiter');
  assert.throws(() => instructions.getMapping('nope'), /Known sources: ziprecruiter/);
});
