export default {
  created() {
    (this as any).$validator.extend('equals', {
      getMessage() {
        return 'The email confirmation does not match';
      },
      validate(value: string, args: string[]) {
        return value === args[0];
      }
    });
  }
};
