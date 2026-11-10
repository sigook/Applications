import { describe, expect, it } from 'vitest';
import * as yup from 'yup';
import { identificationNumberSchema, phoneSchema, postalCodeSchema } from '@/shared/utils/validation';
import { IdentificationTypeCode } from '@/shared/types/common';

describe('phoneSchema', () => {
  it.each(['(416) 555-0199', '4165550199', '+1 212 555 0123'])('accepts North American number %s', (phone) => {
    expect(phoneSchema().isValidSync(phone)).toBe(true);
  });

  it.each(['12345', '555-0199', '+44 20 7946 0958'])('rejects invalid number %s', (phone) => {
    expect(phoneSchema().isValidSync(phone)).toBe(false);
  });

  it('requires a value by default', () => {
    expect(phoneSchema().isValidSync('')).toBe(false);
    expect(phoneSchema().isValidSync(null)).toBe(false);
  });

  it('allows empty values when optional', () => {
    expect(phoneSchema(false).isValidSync('')).toBe(true);
    expect(phoneSchema(false).isValidSync(null)).toBe(true);
  });
});

describe('postalCodeSchema', () => {
  it.each(['M5V 3L9', 'M5V3L9', 'K1A 0B1'])('accepts Canadian postal code %s', (code) => {
    expect(postalCodeSchema().isValidSync(code)).toBe(true);
  });

  it.each(['90210', '90210-1234'])('accepts US zip code %s', (code) => {
    expect(postalCodeSchema().isValidSync(code)).toBe(true);
  });

  it.each(['D5V 3L9', 'W1A 0B1', 'm5v 3l9', '1234', 'ABC123'])('rejects invalid code %s', (code) => {
    expect(postalCodeSchema().isValidSync(code)).toBe(false);
  });

  it('allows empty values when optional', () => {
    expect(postalCodeSchema(false).isValidSync(null)).toBe(true);
  });
});

describe('identificationNumberSchema', () => {
  const schema = yup.object({
    type: yup.mixed().nullable(),
    number: identificationNumberSchema('type'),
  });

  const sin = { id: '1', value: 'SIN', code: IdentificationTypeCode.SinSsn };
  const passport = { id: '2', value: 'Passport', code: 1 };

  it('is optional when no type is selected', () => {
    expect(schema.isValidSync({ type: null, number: '' })).toBe(true);
  });

  it('requires a number when a type is selected', () => {
    expect(schema.isValidSync({ type: passport, number: '' })).toBe(false);
    expect(schema.isValidSync({ type: passport, number: 'X1' })).toBe(true);
  });

  it('enforces SIN / SSN length between 9 and 15', () => {
    expect(schema.isValidSync({ type: sin, number: '12345678' })).toBe(false);
    expect(schema.isValidSync({ type: sin, number: '123456789' })).toBe(true);
    expect(schema.isValidSync({ type: sin, number: '123456789012345' })).toBe(true);
    expect(schema.isValidSync({ type: sin, number: '1234567890123456' })).toBe(false);
  });
});
