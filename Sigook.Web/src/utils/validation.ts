import * as yup from 'yup';
import { PhoneNumberUtil } from 'google-libphonenumber';
import { IdentificationTypeCode, type IdentificationType } from '@/types/common';

const phoneUtil = PhoneNumberUtil.getInstance();

export function isValidPhone(value: string | undefined | null): boolean {
  if (!value) return false;
  const digits = value.replace(/\D/g, '');
  if (digits.length < 10) return false;
  const validRegions = ['CA', 'US', 'PR'];
  try {
    const instance = phoneUtil.parse(digits, 'CA');
    const region = phoneUtil.getRegionCodeForNumber(instance);
    return validRegions.some(vr => vr === region);
  } catch {
    return false;
  }
}

export function phoneSchema(required = true): yup.StringSchema {
  let schema = yup.string().nullable();
  if (required) schema = schema.required('Phone is required');
  return schema.test('phone', 'Invalid phone number', (v) => !v || isValidPhone(v));
}

const SIN_MIN_LENGTH = 9;
const SIN_MAX_LENGTH = 15;

export function identificationNumberSchema(typeField: string): yup.StringSchema {
  return yup.string().when(typeField, ([type]: (IdentificationType | null | undefined)[], schema: yup.StringSchema) => {
    if (!type) return schema;
    const required = schema.required('Identification number is required');
    return type.code === IdentificationTypeCode.SinSsn
      ? required
        .min(SIN_MIN_LENGTH, `SIN / SSN must be between ${SIN_MIN_LENGTH} and ${SIN_MAX_LENGTH} characters`)
        .max(SIN_MAX_LENGTH, `SIN / SSN must be between ${SIN_MIN_LENGTH} and ${SIN_MAX_LENGTH} characters`)
      : required;
  });
}

export function postalCodeSchema(required = true): yup.StringSchema {
  let schema = yup.string().nullable();
  if (required) schema = schema.required('Postal code is required');
  return schema.test('postal', 'Invalid postal code', (v) => {
    if (!v) return true;
    if (/^(?!.*[DFIOQU])[A-VXY][0-9][A-Z] ?[0-9][A-Z][0-9]$/.test(v)) return true;
    if (/^[0-9]{5}(?:-[0-9]{4})?$/.test(v)) return true;
    return false;
  });
}
