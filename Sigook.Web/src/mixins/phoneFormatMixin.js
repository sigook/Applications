const phoneUtil = require('google-libphonenumber').PhoneNumberUtil.getInstance();

export default {
  created() {
    this.$validator.extend('phoneCustom', {
      getMessage() {
        return 'The field is not a valid phone number'
      },
      validate(value) {
        const validRegions = ['CA', 'US', 'PR'];
        if (value && value.length > 12) {
          return true;
        }
        try {
          const instance = phoneUtil.parse(value, 'CA');
          const region = phoneUtil.getRegionCodeForNumber(instance);
          return validRegions.some(vr => vr === region);
        } catch(ex) {
          return false;
        }
      }
    })
  }
}