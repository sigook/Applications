export default {
    created(){
        this.$validator.extend('equals', {
            getMessage() {
                return 'The email confirmation does not match';
            },
            validate(value, args) {
                return value === args[0];
            }
        });
    }
};