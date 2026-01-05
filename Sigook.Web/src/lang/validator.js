import Vue from 'vue';
import VeeValidate, { Validator } from 'vee-validate';
import es from './es_error';
import en from './en_error';
import fr from './fr_error';

Validator.localize({
    es: es,
    fr: fr,
    en: en
});

Validator.extend('cvn-postal-code', {
    getMessage: () => `The field is not a valid postal code`,
    validate: value => {
        if (/^(?!.*[DFIOQU])[A-VXY][0-9][A-Z] ?[0-9][A-Z][0-9]$/.test(value))
            return true;
        if (/^[0-9]{5}(?:-[0-9]{4})?$/.test(value)) return true;
        return false;
    }
});

const validator = Vue.use(VeeValidate, {
    locale: localStorage.getItem('language') || 'en'
});

export default validator;





