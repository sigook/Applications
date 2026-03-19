import Vue from 'vue'
import VueI18n from 'vue-i18n'

import EnStrings from './en.json'
import FrString from './fr.json'
import EsStrings from './es.json'

Vue.use(VueI18n);
const locale = localStorage.getItem('language') || 'en';
const messages = {
  en: EnStrings,
  fr: FrString,
  es: EsStrings
};

const i18n = new VueI18n({
  locale: locale, // set locale
  messages, // set locale messages
});

export default i18n
