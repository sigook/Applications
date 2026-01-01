export default {
  methods: {
    onRecaptchaResponse(result) {
      this.contact.captchaResponse = result;
    }
  },
  computed: {
    siteKey() {
      return process.env.VUE_APP_RE_CAPTCHA_SITE_KEY;
    }
  }
}