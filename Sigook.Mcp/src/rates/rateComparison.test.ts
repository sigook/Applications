import assert from 'node:assert/strict';
import { test } from 'node:test';
import { compareRole, missingRoles, parseLocation, roleCategory, roleCity, roleState, roundToCents, type QuoteRow } from './rateComparison.js';

const quote: QuoteRow[] = [
  { row: 10, location: 'CA - Anaheim', state: 'CA', city: 'Anaheim', rates: { 'general-labor': { payRate: 18.5, billRate: 28.31 }, supervisor: { payRate: 25, billRate: 37 } } },
  { row: 11, location: 'CA - San Diego', state: 'CA', city: 'San Diego', rates: { 'general-labor': { payRate: 21.5, billRate: 32.9 } } },
  { row: 12, location: 'CA - San Diego Metro', state: 'CA', city: 'San Diego Metro', rates: { 'general-labor': { payRate: 22, billRate: 33 } } },
  { row: 13, location: 'NY - Syosset', state: 'NY', city: 'Syosset', rates: { 'general-labor': { payRate: 19, billRate: 29 } } },
  { row: 20, location: 'WY - Casper', state: 'WY', city: 'Casper', rates: { 'general-labor': { payRate: 15.5, billRate: 22.79 } } },
  { row: 21, location: 'WY - Cheyenne', state: 'WY', city: 'Cheyenne', rates: { 'general-labor': { payRate: 15.5, billRate: 22.79 } } },
  { row: 14, location: 'CA - LA', state: 'CA', city: 'LA', rates: { 'general-labor': { payRate: 19, billRate: 29.1 } } },
  { row: 30, location: 'PA - Philadelphia', state: 'PA', city: 'Philadelphia', rates: { 'general-labor': { payRate: 19.5, billRate: 30.62 } } }
];

const role = (jobPosition: string, workerRate: number, rate: number) => ({ id: jobPosition, jobPosition, workerRate, rate });

test('parses quote locations with any dash and state-only headers', () => {
  assert.deepEqual(parseLocation('CA – Anaheim'), { state: 'CA', city: 'Anaheim' });
  assert.deepEqual(parseLocation('VA- Norfolk'), { state: 'VA', city: 'Norfolk' });
  assert.deepEqual(parseLocation('IA  '), { state: 'IA', city: null });
  assert.equal(parseLocation('Total'), undefined);
});

test('detects the category and the state code of a role name', () => {
  assert.equal(roleCategory('Forklift Operator/GL - Bellefonte PA'), 'forklift');
  assert.equal(roleCategory('Skilled General Labor - NY Buffalo'), 'skilled-labor');
  assert.equal(roleCategory('Warehouse General Labor - TX San Antonio'), 'general-labor');
  assert.equal(roleCategory('Team Lead - VA Richmond'), undefined);
  assert.equal(roleState('General Labor CA-Ukiah'), 'CA');
  assert.equal(roleState('General Labor - MD, Hagerstown'), 'MD');
  assert.equal(roleState('General Labor Memphis'), undefined);
});

test('equal and different rates by city', () => {
  assert.equal(compareRole(role('General Labor - CA Anaheim', 18.5, 28.31), quote).status, 'equal');
  const different = compareRole(role('General Labor - CA Anaheim', 18, 27.54), quote);
  assert.equal(different.status, 'different');
  assert.equal(different.quoteRow, 10);
  assert.equal(different.payDifference, 0.5);
  assert.equal(different.billDifference, 0.77);
});

test('the longest city wins', () => {
  const result = compareRole(role('General Labor - CA San Diego Metro', 22, 33), quote);
  assert.equal(result.quoteLocation, 'CA - San Diego Metro');
  assert.equal(result.status, 'equal');
});

test('a misspelled city matches fuzzily', () => {
  const result = compareRole(role('General Labor - NY Syosett', 19, 29), quote);
  assert.equal(result.matchedBy, 'fuzzy-city');
  assert.equal(result.quoteRow, 13);
});

test('a state-only role is equal when every city of the state has the same rates', () => {
  const result = compareRole(role('General Laborer - WY', 15.5, 22.79), quote);
  assert.equal(result.status, 'equal');
  assert.equal(result.matchedBy, 'state');
});

test('a state-only role is ambiguous when the cities differ', () => {
  const result = compareRole(role('General Labor - CA', 17.5, 26.43), quote);
  assert.equal(result.status, 'ambiguous');
  assert.equal(result.candidates?.length, 4);
});

test('the state comes from a unique city when the name has no state code', () => {
  const result = compareRole(role('General Labor - Philadelphia', 17.75, 27.16), quote);
  assert.equal(result.state, 'PA');
  assert.equal(result.status, 'different');
});

test('no quote, unparsed and missing roles', () => {
  assert.equal(compareRole(role('Skilled Labor - CA Anaheim', 20, 30), quote).status, 'no-quote');
  assert.equal(compareRole(role('General Labor - TX Irving', 20, 30), quote).status, 'no-quote');
  assert.equal(compareRole(role('Night Supervisor Week', 27, 36), quote).status, 'unparsed');
  const comparisons = [compareRole(role('General Labor - CA Anaheim', 18.5, 28.31), quote)];
  const missing = missingRoles(comparisons, quote);
  assert.ok(missing.some(item => item.location === 'CA - Anaheim' && item.category === 'supervisor'));
  assert.ok(!missing.some(item => item.location === 'CA - Anaheim' && item.category === 'general-labor'));
});

test('a city missing from the quote is no-quote, not the whole state', () => {
  const result = compareRole(role('General Labor - CA Redding', 22, 33.66), quote);
  assert.equal(result.status, 'no-quote');
  assert.match(result.reason!, /redding/);
  assert.equal(roleCity('Warehouse General Labor - NJ Rutherford_Short Assignment', 'NJ'), 'rutherford');
  assert.equal(roleCity('General Labor Night Shift Weekend - VA', 'VA'), undefined);
});

test('city aliases and cents rounding', () => {
  assert.equal(compareRole(role('General Labor - CA Los Angeles', 19, 29.1), quote).status, 'equal');
  assert.equal(roundToCents(32.895), 32.9);
  assert.equal(roundToCents(27.5625), 27.56);
});
