export default {
  methods: {
    onRecaptchaResponse(result: string): void {
      (this as any).contact.captchaResponse = result;
    }
  },
  computed: {
    siteKey(): string | undefined {
      return process.env.VUE_APP_RE_CAPTCHA_SITE_KEY;
    }
  }
};
