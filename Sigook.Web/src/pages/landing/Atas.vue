
<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <sub-menu :solutionType="solutionType"></sub-menu>
    <section class="container-fluid">
      <div class="p-5 sigook-atas-banner">
        <div class="p-sm-5 my-sm-5 sigook-atas-banner-container">
          <div class="sigook-atas-banner-title">
            <label class="m-0">AUTOMATED ATTENDANCE</label>
            <label class="mb-2 font-weight-bold">TRACKING SYSTEM</label>
            <a v-if="solutionType === 'business'"
              class="mt-5 align-self-center button-rounded bg-gray-light color-black cursor-pointer"
              v-scroll-to="'#requestDemoForm'">
              REQUEST A DEMO TODAY
            </a>
          </div>
        </div>
      </div>
    </section>
    <section class="p-5 mx-sm-5 my-3 text-center">
      <div class="sigook-atas-description-title d-sm-flex justify-content-center">
        <label class="color-grey font-weight-bolder">AUTOMATED</label>
        <label class="ml-2 color-red-light font-weight-bolder">ATTENDANCE TRACKING</label>
        <label class="ml-2 color-grey font-weight-bolder">SYSTEM</label>
      </div>
      <p v-if="solutionType === 'jobSeekers'" class="font-weight-bolder pt-4 pb-3">
        Forget about signing check-in/out rosters, avoid errors in your payroll by miscalculated hours,
        and instantly receive your payroll on your phone. With our SIGOOK<label class="superscript">®</label> APP,
        you can punch in/out by scanning your QR code from your mobile and get 24/7 access to your profile and job
        opportunities.
      </p>
      <p v-else class="font-weight-bolder pt-4 pb-3">Forget about tracking hours manually, time-consuming timesheets, and
        long
        payrolls. Our digital system simplifies the monitoring of workers' shifts. It allows you to control and easily
        approve hours. The workers will punch in/out from their mobile, or we can provide a reader device at the
        workplace
        where workers can tap their batch upon arrival and departure from work.</p>
      <p class="font-weight-bolder">
        The Automated Attendance Tracking System is integrated with our SIGOOK<label class="superscript">®</label> APP, to
        grant companies, staffing agencies, and workers the greatness of automation.
      </p>
    </section>
    <section v-if="solutionType === 'business'" class="p-4 bg-gray-light">
      <div class="px-3 sigook-atas-qualities-section">
        <div>
          <div class="text-center">
            <img src="@/assets/images/icon_bgwhite_celphone.png" />
          </div>
          <div class="pt-4">
            <p>Supervisors will be able to review workers' progress and performance with just a few clicks, fix in/out
              times and approve weekly timesheets.</p>
          </div>
        </div>
        <div>
          <div class="text-center">
            <img src="@/assets/images/icon_bgwhite_statistics.png" />
          </div>
          <div class="pt-4">
            <p>Once the hours are approved online, SIGOOK<label class="superscript">®</label> immediately generates the
              weekly timesheet, payroll, and paystubs. All this from the comfort of your home, office or anywhere you are.
            </p>
          </div>
        </div>
        <div>
          <div class="text-center">
            <img src="@/assets/images/icon_bgwhite_monitor.png" />
          </div>
          <div class="pt-4">
            <p>Workers can automatically punch in and out from their mobile or tap their batch on the punching device. Get
              accurate times and save time reviewing hours. Use that extra time to do what you know best, running your
              business!
            </p>
          </div>
        </div>
      </div>
    </section>
    <section class="p-4">
      <div class="pt-4 px-3">
        <h3 class="color-red-light font-weight-bold">HIGHLIGHTS</h3>
      </div>
      <div v-if="solutionType === 'jobSeekers'" class="p-3 sigook-atas-qualities-section">
        <div>
          <p><img src="@/assets/images/icon_check.png" class="mr-2" />Easily clock in/out from your phone with your
            assigned
            QR code</p>
          <p><img src="@/assets/images/icon_check.png" class="mr-2" />Access all you current or past payment information
            anytime on your phone</p>
        </div>
        <div>
          <p><img src="@/assets/images/icon_check.png" class="mr-2" />Access to all our new jobs from the minute they are
            posted
          </p>
          <p><img src="@/assets/images/icon_check.png" class="mr-2" />Apply to any job with one click from your phone</p>
        </div>
      </div>
      <div v-else class="p-3 sigook-atas-qualities-section">
        <div>
          <p><img src="@/assets/images/icon_check.png" class="mr-2" /> Avoid paperwork and manual timesheets</p>
          <p><img src="@/assets/images/icon_check.png" class="mr-2" /> Track your workers’ shifts easily</p>
        </div>
        <div>
          <p><img src="@/assets/images/icon_check.png" class="mr-2" /> Get instant timesheets upon supervisors approval
          </p>
          <p><img src="@/assets/images/icon_check.png" class="mr-2" /> Review, modify, and approve the content anywhere
            and
            anytime</p>
        </div>
        <div>
          <a class="button-rounded bg-linear-red-blue color-white cursor-pointer" v-scroll-to="'#requestDemoForm'">
            REQUEST A DEMO TODAY
          </a>
        </div>
      </div>
    </section>
    <section v-if="solutionType === 'business'" class="bg-gray-light" id="requestDemoForm">
      <div class="px-2 py-4 p-sm-5 sigook-atas-request-demo-section">
        <div class="hide-on-mobile"></div>
        <div>
          <div class="font-weight-bold font-x-large text-center pb-3">
            <label class="color-grey">REQUEST A</label>
            <label class="color-red-light"> DEMO TODAY.</label>
          </div>
          <form class="sigook-atas-request-demo-form" id="requestDemo" @submit.prevent="sendMessage">
            <input type="text" class="control-rounded border-0 sigook-atas-request-demo-large-control"
              v-model="contact.companyName" placeholder="Company Name" v-validate="'required|max:50'" />
            <input type="text" class="control-rounded border-0" placeholder="Address" v-model="contact.address"
              v-validate="'required|max:50'" />
            <input type="text" class="control-rounded border-0" placeholder="Industry" v-model="contact.industry"
              v-validate="'required|max:50'" />
            <input type="text" class="control-rounded border-0" placeholder="Phone" name="phone" v-model="contact.phone"
              v-validate="{ required: true, phoneCustom: '' }" v-cleave="mask" />
            <input type="email" class="control-rounded border-0" placeholder="Email" v-model="contact.email"
              v-validate="'required|max:50|email'" />
            <vue-recaptcha ref="recaptcha" :sitekey="siteKey" @verify="onRecaptchaResponse"></vue-recaptcha>
            <div class="sigook-atas-request-demo-large-control text-right">
              <button class="button-rounded bg-linear-red-blue color-white" type="submit">SEND -></button>
            </div>
          </form>
          <h6 class="text-danger">{{ formError }}</h6>
        </div>
      </div>
    </section>
  </div>
</template>

<script>
import phoneFormat from "@/mixins/phoneFormatMixin";
import phoneMaskMixin from "@/mixins/phoneMaskMixin";
import recaptchaMixin from "@/mixins/recaptchaMixin";

export default {
  mixins: [phoneFormat, phoneMaskMixin, recaptchaMixin],
  data() {
    return {
      isLoading: false,
      contact: {
        title: 'CONTACT ATAS SYSTEM ~ NOTIFICATION',
        subject: 'Request ATAS Demo',
        emailSetting: process.env.VUE_APP_SIGOOK_NOTIFICATION
      },
      solutionType: null,
      formError: null
    }
  },
  components: {
    SubMenu: () => import("@/components/landing/SubMenu")
  },
  created() {
    this.solutionType = this.$route.query.solutionType;
  },
  methods: {
    async sendMessage() {
      const result = await this.$validator.validateAll();
      if (!result) {
        this.formError = this.errors.has('phone') ? 'Please use a valid phone number in the United States' : 'Please check the information provided and re-submit';
      } else if (!this.contact.captchaResponse) {
        this.formError = 'reCaptcha invalid';
      } else {
        this.isLoading = true;
        this.formError = null;
        this.$store.dispatch('sendForm', this.contact)
          .then(() => {
            this.resetContactForm();
            this.isLoading = false;
          })
          .catch(() => {
            this.resetContactForm();
            this.isLoading = false;
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


<style lang="scss">
@import "../../assets/scss/mixins";
@import "../../assets/scss/variables";

.sigook-atas-banner {
  background: url('../../assets/images/banners/atas.png') no-repeat bottom right;
  background-size: cover;

  .sigook-atas-banner-title {
    color: $white;
    display: flex;
    flex-flow: column;
    font-size: 3.2rem;

    @include only-mobile {
      font-size: 1.4rem;
    }
  }
}


.sigook-atas-description-title {
  text-align: center;
  font-size: 2.3rem;

  @include only-tablet {
    font-size: 1.6rem;
  }
}


.sigook-atas-qualities-section {
  display: grid;
  grid-gap: 3rem;
  grid-template-columns: auto auto auto;

  @include only-mobile {
    grid-template-columns: auto;
    grid-template-rows: auto auto auto;
  }
}


.sigook-atas-request-demo-section {
  display: grid;
  grid-gap: 2rem;
  grid-template-columns: 35% 30% 35%;
  grid-template-rows: auto auto;

  @include only-mobile {
    grid-template-columns: auto;
  }

  @include only-tablet {
    grid-template-columns: auto auto auto;
  }

  @include only-notebook {
    grid-template-columns: auto auto auto;
  }

  .sigook-atas-request-demo-form {
    display: grid;
    grid-gap: 1rem;
    grid-template-rows: auto auto auto;
    grid-template-columns: auto auto;

    .sigook-atas-request-demo-large-control {
      grid-column: 1/3;
    }
  }
}
</style>