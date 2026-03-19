export default {
  methods: {
    switchLocale(lang: string): void {
      (this as any).$i18n.locale = lang;
      (this as any).$validator.locale = lang;
      (this as any).lang = (this as any).$validator.dictionary.locale;

      (this as any).$store.commit('setLanguage', lang);
    },
  }
};
