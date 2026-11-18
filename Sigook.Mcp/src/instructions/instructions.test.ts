import assert from 'node:assert/strict';
import { test } from 'node:test';
import { IMPORT_CANDIDATES_TOPIC, Instructions, parseMapping } from './instructions.js';

test('parses front matter', () => {
  const mapping = parseMapping('---\r\nname: x\r\ndetect: A, B\r\n---\r\n\r\n# Body\r\n');
  assert.equal(mapping.title, 'x');
  assert.deepEqual(mapping.detectHeaders, ['A', 'B']);
  assert.equal(mapping.markdown, '# Body');
});

test('packaged instructions load topics and detect ZipRecruiter', () => {
  const instructions = new Instructions();
  assert.ok(instructions.getTopic(IMPORT_CANDIDATES_TOPIC).markdown.length > 0);
  assert.equal(instructions.detect(['candidate overview', 'Deal-Breaker Questions Correct', 'Questions and Answers', 'Name'])?.name, 'ziprecruiter');
  assert.throws(() => instructions.getMapping('nope'), /Known sources: ziprecruiter/);
  assert.throws(() => instructions.getTopic('nope'), /Known topics: import-candidates/);
});

test('server instructions list every topic with its description', () => {
  const instructions = new Instructions();
  const text = instructions.serverInstructions();
  for (const topic of instructions.topics) assert.ok(text.includes(`- ${topic.name}: ${topic.description}`));
  assert.ok(!text.includes(instructions.getTopic(IMPORT_CANDIDATES_TOPIC).markdown));
});
