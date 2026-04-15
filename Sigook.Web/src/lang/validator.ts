import Vue from 'vue';
import VeeValidate, { Validator } from 'vee-validate';
import { PhoneNumberUtil } from 'google-libphonenumber';
import en from './en_error';

Validator.localize({ en });

Validator.extend('cvn-postal-code', {
    getMessage: () => `The field is not a valid postal code`,
    validate: (value: string) => {
        if (/^(?!.*[DFIOQU])[A-VXY][0-9][A-Z] ?[0-9][A-Z][0-9]$/.test(value))
            return true;
        if (/^[0-9]{5}(?:-[0-9]{4})?$/.test(value)) return true;
        return false;
    }
});

const phoneUtil = PhoneNumberUtil.getInstance();
Validator.extend('phoneCustom', {
    getMessage: () => 'The field is not a valid phone number',
    validate: (value: string) => {
        const validRegions = ['CA', 'US', 'PR'];
        if (value && value.length > 12) return true;
        try {
            const instance = phoneUtil.parse(value, 'CA');
            const region = phoneUtil.getRegionCodeForNumber(instance);
            return validRegions.some(vr => vr === region);
        } catch {
            return false;
        }
    }
});

Validator.extend('equals', {
    getMessage: () => 'The email confirmation does not match',
    validate: (value: string, args: any) => value === args[0]
});

const validator = Vue.use(VeeValidate, { locale: 'en' });

export default validator;
