export default {
    methods: {
        switchLocale (lang) {
            this.$i18n.locale = lang;
            this.$validator.locale = lang;
            this.lang =  this.$validator.dictionary.locale;

            this.$store.commit('setLanguage', lang);
        },
    }
}