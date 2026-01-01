<template>
  <div class="landing-footer p-2">

    <div class="container">

      <div class="d-sm-flex justify-content-around">

        <div class="d-flex flex-column">
          <div class="p-2">
            <img src="@/assets/images/white_logo.png" width="200" />
          </div>
          <div class="p-2">
            <p><b-icon size="small" icon="phone" class="mr-1"></b-icon>+1 954 9320477</p>
            <p><b-icon size="small" icon="inbox" class="mr-1"></b-icon>appmanager@sigook.com</p>
            <p><b-icon size="small" icon="map-marker" class="mr-1"></b-icon>1451 West Cypress Creek Road, Suite 300 <br /> Fort
              Lauderdale, FL 33309, USA</p>
          </div>
        </div>

        <div class="d-flex flex-column">
          <div class="d-flex p-2 mt-5 mb-4">
            <strong class="color-white">QUICK LINKS</strong>
          </div>
          <div class="p-2 d-flex flex-row">
            <div class="d-flex flex-column">
              <router-link to="/home" class="p-2">HOME</router-link>
              <router-link to="/about-us" class="p-2">ABOUT US</router-link>
              <router-link to="/contact" class="p-2">CONTACT</router-link>
            </div>
            <div class="d-flex flex-column">
              <router-link to="/business" class="p-2">FOR BUSINESS</router-link>
              <router-link to="/jobSeekers" class="p-2">FOR JOB SEEKERS</router-link>
            </div>
          </div>
        </div>

        <div class="d-flex flex-column">
          <div class="d-flex p-2 mt-5 mb-4">
            <strong class="color-white">FOLLOW US!</strong>
          </div>
          <div class="d-flex flex-row">
            <div class="p-2">
              <a href="https://www.facebook.com/people/sigook_work_factory/100085428074968/" target="_blank">
                <img src="@/assets/images/facebook_footer.png" width="30" />
              </a>
            </div>
            <div class="p-2">
              <a href="https://www.linkedin.com/company/sigookworkfactory/" target="_blank">
                <img src="@/assets/images/linked_footer.png" width="30" />
              </a>
            </div>
            <div class="p-2">
              <a href="https://www.instagram.com/sigook_work_factory/" target="_blank">
                <img src="@/assets/images/instagram_footer.png" width="30" />
              </a>
            </div>
          </div>
        </div>

      </div>

    </div>

    <div class="d-flex justify-content-between mt-5">
      <div class="p-2 color-white">
        &copy; {{ year }} All rights reserved. <strong>SIGOOK<label class="superscript">®</label></strong>
      </div>
      <div class="d-flex">
        <router-link to="/landing/privacy-policy" class="p-2">Privacy Policy</router-link>
        <router-link to="/landing/terms-and-conditions" class="p-2">Terms and Conditions</router-link>
        <router-link to="/landing/disclaimer" class="p-2">Disclaimer</router-link>
      </div>
    </div>

  </div>
</template>


<script>

export default {
  data() {
    return {
      year: new Date().getFullYear()
    }
  }
}

</script>

<style lang="scss">
@import "../../assets/scss/variables";

.landing-footer {
  background-color: #343434;
  color: $white;
}

.landing-footer router-link,
.landing-footer a {
  color: $white;
  text-decoration: none !important;
}
</style>