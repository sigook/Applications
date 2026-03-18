import { PhoneNumberUtil } from 'google-libphonenumber';

const phoneUtil = PhoneNumberUtil.getInstance();

export default {
  created() {
    (this as any).$validator.extend('phoneCustom', {
      getMessage() {
        return 'The field is not a valid phone number';
      },
      validate(value: string) {
        const validRegions = ['CA', 'US', 'PR'];
        if (value && value.length > 12) {
          return true;
        }
        try {
          const instance = phoneUtil.parse(value, 'CA');
          const region = phoneUtil.getRegionCodeForNumber(instance);
          return validRegions.some(vr => vr === region);
        } catch (ex) {
          return false;
        }
      }
    });
  }
};
