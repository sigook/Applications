
<template>
  <form :class="cssClass" @submit.prevent="sendMessage">
    <input type="text" class="control-borderer" placeholder="Company Name" v-model="contact.companyName"
      v-validate="'required|max:50'" />
    <input type="text" class="control-borderer" placeholder="Address" v-model="contact.address"
      v-validate="'required|max:50'" />
    <input type="text" class="control-borderer" placeholder="Industry" v-model="contact.industry"
      v-validate="'required|max:50'" />
    <input type="text" class="control-borderer" placeholder="Phone" name="phone" v-model="contact.phone"
      v-validate="{ required: true, phoneCustom: '' }" v-cleave="mask" />
    <input type="email" class="control-borderer" placeholder="Email" v-model="contact.email"
      v-validate="'required|max:50|email'" />
    <br />
    <vue-recaptcha ref="recaptcha" :sitekey="siteKey" @verify="onRecaptchaResponse"></vue-recaptcha>
    <div></div>
    <div></div>
    <div></div>
    <button class="bg-red-light color-white button-borderer mt-1" type="submit">Send Message</button>
    <div></div>
    <div></div>
    <h6 class="text-danger">{{ formError }}</h6>
  </form>
</template>
<script>
import phoneFormat from "@/mixins/phoneFormatMixin";
import phoneMaskMixin from "@/mixins/phoneMaskMixin";
import recaptchaMixin from "@/mixins/recaptchaMixin";

export default {
  props: ['cssClass', 'isLoading'],
  mixins: [phoneFormat, phoneMaskMixin, recaptchaMixin],
  data() {
    return {
      contact: {
        title: 'REQUEST PERSONNEL FORM ~ NOTIFICATION',
        subject: 'Contact Request Staff',
        emailSetting: process.env.VUE_APP_SIGOOK_NOTIFICATION
      },
      formError: null
    }
  },
  methods: {
    async sendMessage() {
      const result = await this.$validator.validateAll();
      if (!result) {
        this.formError = this.errors.has('phone') ? 'Please use a valid phone number in the United States' : 'Please check the information provided and re-submit';
      } else if (!this.contact.captchaResponse) {
        this.formError = 'reCaptcha invalid';
      } else {
        this.$emit('onLoading', true);
        this.formError = null;
        this.$store.dispatch('sendForm', this.contact)
          .then(() => {
            this.resetContactForm();
            this.$emit('onLoading', false);
          })
          .catch(() => {
            this.resetContactForm();
            this.$emit('onLoading', false);
          });
      }
    },
    resetContactForm() {
      this.contact.companyName = null;
      this.contact.address = null;
      this.contact.industry = null;
      this.contact.phone = null;
      this.contact.email = null;
      this.contact.captchaResponse = null;
      this.$refs.recaptcha.reset();
    }
  }
}
</script>