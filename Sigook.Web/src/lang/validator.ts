import { defineRule, configure } from 'vee-validate';
import { required, email, min, max, numeric, min_value, max_value, url } from '@vee-validate/rules';
import { PhoneNumberUtil } from 'google-libphonenumber';

const phoneUtil = PhoneNumberUtil.getInstance();

export function registerValidationRules(): void {
  // Built-in rules
  defineRule('required', required);
  defineRule('email', email);
  defineRule('min', min);
  defineRule('max', max);
  defineRule('numeric', numeric);
  defineRule('min_value', min_value);
  defineRule('max_value', max_value);
  defineRule('url', url);

  // Custom rules
  defineRule('cvn-postal-code', (value: string) => {
    if (!value) return true;
    if (/^(?!.*[DFIOQU])[A-VXY][0-9][A-Z] ?[0-9][A-Z][0-9]$/.test(value)) return true;
    if (/^[0-9]{5}(?:-[0-9]{4})?$/.test(value)) return true;
    return 'The field is not a valid postal code';
  });

  defineRule('phoneCustom', (value: string) => {
    if (!value) return true;
    const validRegions = ['CA', 'US', 'PR'];
    if (value.length > 12) return true;
    try {
      const instance = phoneUtil.parse(value, 'CA');
      const region = phoneUtil.getRegionCodeForNumber(instance);
      return validRegions.some(vr => vr === region) || 'The field is not a valid phone number';
    } catch {
      return 'The field is not a valid phone number';
    }
  });

  defineRule('confirmed', (value: string, [target]: [string]) => {
    return value === target || 'The confirmation does not match';
  });

  configure({
    validateOnInput: true,
  });
}
